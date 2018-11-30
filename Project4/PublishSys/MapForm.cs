using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ucPropertyGrid;
using System.Diagnostics;

namespace PublishSys
{
    public partial class MapForm : Form
    { 
        public MapForm()
        {
            InitializeComponent();
        }

        private IniOperator inip = null;
        private AccessHelper ahp = null;

        public string unitid = "";
        //public string unitname = "";
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;    //当前exe根目录
        private string[] folds = null;
        private string map_type = "g_map";

        private Dictionary<string, string> GUID_ICON = new Dictionary<string,string>();

        // 边界线
        Dictionary<string, object> borderDic = null;
        private List<double[]> borList = null;

        private void MapForm_Shown(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();

            // 导入管理级别
            Load_Unit_Level();
            if (treeView1.Nodes.Count <= 0)
            {
                MessageBox.Show("未导入管理级别数据，即将关闭窗口");
                this.Close();
                return;
            }

            // 图符对应初始化
            Show_Icon_List();

            //if (treeView1.Nodes.Count > 0)
                //treeView1.SelectedNode = treeView1.Nodes[0];

            // 地图对应初始化
            button2.Enabled = false;
            button3.Enabled = false;
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            textBox1.Text = inip.ReadString("mapproperties", unitid, "");
            textBox2.Text = inip.ReadString("mapproperties", "centerlng", "0");
            textBox3.Text = inip.ReadString("mapproperties", "centerlat", "0");
            if (textBox2.Text == "0" || textBox3.Text == "0")
                MessageBox.Show("获取不到当前经纬度");
            checkedListBox1.Items.Clear();
            if (textBox1.Text != string.Empty)
                Show_Map_List(textBox1.Text);

            // 导入边界线
            borList = new List<double[]>();
            borderDic = new Dictionary<string, object>();
            borderDic.Add("type", "实线");
            borderDic.Add("width", 2);
            borderDic.Add("color", "#000000");
            borderDic.Add("opacity", 1);
            ahp = new AccessHelper(WorkPath + "Publish\\data\\经纬度注册.mdb");
            string sql = "select LNG_LAT from BORDERDATA where ISDELETE = 0 and UNITID = '" + unitid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                string alldata = dt.Rows[0]["LNG_LAT"].ToString();
                string[] div_data = alldata.Split(';');
                foreach (string str in div_data)
                {
                    if (str != "")
                    {
                        string[] div_str = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n' });
                        borList.Add(new double[] { double.Parse(div_str[0]), double.Parse(div_str[1]) });
                    }
                }
                borderDic.Add("path", borList);
            }
            else
                borderDic = null;
            ahp.CloseConn();

            // 刷新地图、图符对应
            if (treeView1.Nodes.Count > 0)
            {
                treeView1.SelectedNode = null;
                treeView1.SelectedNode = treeView1.Nodes[0];
            }

            timer1.Enabled = true;
        }

        private void Delete_Path()
        {
            string mapPath = WorkPath + "Publish\\googlemap\\map";
            string starPath = WorkPath + "Publish\\googlemap\\satellite";
            if (Directory.Exists(mapPath))
            {
                Process p = Process.Start(WorkPath + "DeleteDir.exe", mapPath);
                p.WaitForExit();
                //Directory.Delete(mapPath, true);
                //Tools.DeleteFolder(mapPath, null);
            }
            if (Directory.Exists(starPath))
            {
                Process p = Process.Start(WorkPath + "DeleteDir.exe", starPath);
                p.WaitForExit();
                //Tools.DeleteFolder(starPath, null);
            }
            //isT = true;
        }

        private Dictionary<string, string> OBJ_NAME;
        private Dictionary<string, string> OBJ_JDCODE;
        private Dictionary<string, string> OBJ_UPGUID;
        private Dictionary<string, string> OBJ_MAP;
        private void Load_Unit_Level()
        {
            OBJ_NAME = new Dictionary<string, string>();
            OBJ_JDCODE = new Dictionary<string, string>();
            OBJ_UPGUID = new Dictionary<string, string>();
            OBJ_MAP = new Dictionary<string, string>();
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ZSK_H0001Z000K01.mdb");
            string sql = "select PGUID, JDNAME, JDCODE, UPGUID from ZSK_OBJECT_H0001Z000K01 where ISDELETE = 0";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            ahp.CloseConn();
            for (int i = 0; i < dt.Rows.Count; ++i) 
            {
                OBJ_NAME[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["JDNAME"].ToString();
                OBJ_JDCODE[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["JDCODE"].ToString();
                OBJ_UPGUID[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["UPGUID"].ToString();
            }

            treeView1.Nodes.Clear();
            treeView1.HideSelection = false;
            treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);

            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                if (dt.Rows[i]["UPGUID"].ToString() == string.Empty)
                {
                    TreeNode pNode = new TreeNode();
                    pNode.Text = OBJ_NAME[dt.Rows[i]["PGUID"].ToString()];
                    pNode.Tag = dt.Rows[i]["PGUID"].ToString();
                    treeView1.Nodes.Add(pNode);
                    Add_Unit_Node(pNode);
                }
            }
            treeView1.ExpandAll();
            
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色，只需要在TreeView失去焦点时选中节点仍然突显  
            return;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");
            string sql;
            DataTable dt;
            // 图符对应
            /*sql = "select ICONGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            foreach (Control ctrl in flowLayoutPanel1.Controls)
            {
                ucPictureBox ucPB = (ucPictureBox)ctrl;
                ucPB.IconCheck = false;
            }
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string iconguid = dt.Rows[i]["ICONGUID"].ToString();
                foreach (Control ctrl in flowLayoutPanel1.Controls)
                {
                    if (ctrl.Name == iconguid)
                    {
                        ucPictureBox ucPB = (ucPictureBox)ctrl;
                        ucPB.IconCheck = true;
                    }
                }
            }*/
            Show_Icon_List(levelguid);


            // 地图对应
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
                if (checkedListBox1.GetItemChecked(i))
                    checkedListBox1.SetItemChecked(i, false);
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");
            sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            ahp.CloseConn();
            if (dt.Rows.Count > 0)
            {
                string maplevel = dt.Rows[0]["MAPLEVEL"].ToString();
                string[] level = maplevel.Split(',');
                for (int i = 0; i < level.Length; ++i)
                {
                    for (int j = 0; j < checkedListBox1.Items.Count; ++j)
                        if (checkedListBox1.Items[j].ToString() == level[i])
                        {
                            checkedListBox1.SetItemChecked(j, true);
                            break;
                        }
                }
            }
            int index = checkedListBox1.SelectedIndex;
            if (index >= 0)
                mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, map_type, null, borderDic, null);
        }

        private void Add_Unit_Node(TreeNode pa) 
        {
            foreach (var item in OBJ_UPGUID)
            {
                if (item.Value == pa.Tag.ToString())
                {
                    TreeNode pNode = new TreeNode();
                    pNode.Text = OBJ_NAME[item.Key];
                    pNode.Tag = item.Key;
                    pa.Nodes.Add(pNode);
                    Add_Unit_Node(pNode);
                }
            }
        }

        private void Show_Icon_List()
        {
            string icon_path = WorkPath + "Publish\\ICONDER\\b_PNGICON\\";
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ZSK_H0001Z000K00.mdb");
            string sql = "select PGUID, JDNAME from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            ahp.CloseConn();
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                string name = dt.Rows[i]["JDNAME"].ToString();
                if (File.Exists(icon_path + pguid + ".png"))
                {
                    ucPictureBox ucPB = new ucPictureBox();
                    ucPB.Parent = this.flowLayoutPanel1;
                    ucPB.Name = pguid;
                    ucPB.IconName = name;
                    ucPB.IconPguid = pguid;
                    ucPB.IconPath = icon_path + pguid + ".png";
                    ucPB.CheckBoxChanged += checkBox1_CheckedChanged;
                    //ucPB.IconCheck = true;
                }
            }
        }

        private void Show_Icon_List(string levelguid)
        {
            string icon_path = WorkPath + "Publish\\ICONDER\\b_PNGICON\\";
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ZSK_H0001Z000K00.mdb");
            string sql = "select PGUID, JDNAME from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            ahp.CloseConn();
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                string name = dt.Rows[i]["JDNAME"].ToString();
                if (File.Exists(icon_path + pguid + ".png"))
                {
                    ucPictureBox ucPB = new ucPictureBox();
                    ucPB.Parent = this.flowLayoutPanel1;
                    ucPB.Name = pguid;
                    ucPB.IconName = name;
                    ucPB.IconPguid = pguid;
                    ucPB.IconPath = icon_path + pguid + ".png";
                    ucPB.CheckBoxChanged += checkBox1_CheckedChanged;
                    ahp = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");
                    sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + pguid + "' and UNITEID = '" + unitid + "'";
                    DataTable dt1 = ahp.ExecuteDataTable(sql, null);
                    ahp.CloseConn();
                    if (dt1.Rows.Count > 0)
                        ucPB.IconCheck = true;
                    else
                        ucPB.IconCheck = false;
                    ahp.CloseConn();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e, string iconguid, bool ischecked)
        {
            //MessageBox.Show("1");
            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");
            string sql;
            if (ischecked == false)
            {
                sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "' where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "' and UNITEID = '" + unitid + "'";
                ahp.ExecuteSql(sql, null);
                //ahp.CloseConn();
            }
            else 
            {
                sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "' and UNITEID = '" + unitid + "'";
                DataTable dt = ahp.ExecuteDataTable(sql, null);
                //ahp.CloseConn();
                if (dt.Rows.Count > 0)
                {
                    /*sql = "update ICONDUIYING_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                        "' where ISDELETE = 0  and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "'";
                    ahp.ExecuteSql(sql, null);*/
                }
                else 
                {
                    sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 1 and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "' and UNITEID = '" + unitid + "'";
                    dt = ahp.ExecuteDataTable(sql, null);
                    //ahp.CloseConn();
                    if (dt.Rows.Count > 0)
                    {
                        sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                            "' where LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "' and UNITEID = '" + unitid + "'";
                        ahp.ExecuteSql(sql, null);
                        //ahp.CloseConn();
                    }
                    else
                    {
                        sql = "insert into ICONDUIYING_H0001Z000E00 (PGUID, S_UDTIME, LEVELGUID, ICONGUID, UNITEID) values('"
                            + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '"
                            + levelguid + "', '" + iconguid + "', '" + unitid + "')";
                        ahp.ExecuteSql(sql, null);
                        //ahp.CloseConn();
                    }
                }
            }
            ahp.CloseConn();
        }

        private void Show_Map_List(string tmp) 
        {
            int k = tmp.LastIndexOf("roadmap");
            if (k < 0)
            {
                k = tmp.LastIndexOf("satellite");
            }
            if (k < 0)
            {
                k = tmp.LastIndexOf("satellite_en");
            }
            if (k > 0)
            {
                tmp = tmp.Substring(0, k);
            }
            textBox1.Text = tmp;

            // 清除checkedListBox控件
            checkedListBox1.Items.Clear();
 
            // 检查地图子文件夹是否存在
            string gpath = textBox1.Text + "\\satellite_en";
            if (!Directory.Exists(gpath))
            {
                MessageBox.Show("请下载或导入混合图(无偏移)");
                //this.Close();
                textBox1.Text = "";
                return;
            }
            gpath = textBox1.Text + "\\roadmap";
            if (!Directory.Exists(gpath))
            {
                MessageBox.Show("请下载或导入街道图");
                //this.Close();
                textBox1.Text = "";
                return;
            }

            // 检查所下载地图是否对应当前经纬度
            if (!Check_Map_LngLat())
            {
                MessageBox.Show("当前下载地图与经纬度不对应");
                //this.Close();
                textBox1.Text = "";
                return;
            }
            inip.WriteString("mapproperties", unitid, textBox1.Text);

            // 获取纯文件夹名
            folds = Directory.GetDirectories(gpath);
            for (int i = 0; i < folds.Length; i++)
            {
                k = folds[i].LastIndexOf("\\");
                folds[i] = folds[i].Substring(k + 1);
            }

            // 按文件序号排序
            Array.Sort(folds);

            // 添加入checkedListBox控件
            for (int i = 0; i < folds.Length; ++i)
                checkedListBox1.Items.Add(folds[i]);

            // 初始化MapHelper
            mapHelper1.centerlat = double.Parse(textBox3.Text); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(textBox2.Text); //118.5784; //必须设置的属性,不能为空
            mapHelper1.webpath = WorkPath + "Publish\\googlemap"; //必须设置的属性,不能为空
            mapHelper1.roadmappath = tmp + "\\roadmap"; //必须设置的属性,不能为空
            mapHelper1.satellitemappath = tmp + "\\satellite_en"; //必须设置的属性,不能为空
            mapHelper1.iconspath = WorkPath + "Publish\\PNGICONFOLDER"; //必须设置的属性,不能为空
            mapHelper1.maparr = folds; //必须设置的属性,不能为空
            
            // 选择checkedListBox第一项
            if (checkedListBox1.Items.Count > 0)
                checkedListBox1.SelectedIndex = 0;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button3.Enabled = true;
            int index = checkedListBox1.SelectedIndex;
            //List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();//标注list，从数据库获取
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, map_type, null, borderDic, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 显示地图需要经纬度
            if (textBox2.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入经度");
                textBox2.Focus();
                return;
            }
            if (textBox3.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入纬度");
                textBox3.Focus();
                return;
            }

            // 载入已下载地图
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            folderBrowserDialog1.SelectedPath = inip.ReadString("mapproperties", unitid, "");
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // 获取地图文件夹的上级目录
                // 允许用户选择地图文件夹的总目录、其下的：roadmap、satellite或者satellite_en子目录
                string tmp = folderBrowserDialog1.SelectedPath;
                //inip.WriteString("mapproperties", unitid, tmp);
                Show_Map_List(tmp);
            }
        }

        private bool Check_Map_LngLat()
        {
            string gpath = textBox1.Text + "\\satellite_en";
            folds = Directory.GetDirectories(gpath);
            for (int i = 0; i < folds.Length; i++)
            {
                int k = folds[i].LastIndexOf("\\");
                folds[i] = folds[i].Substring(k + 1);
            }

            double Lng = double.Parse(textBox2.Text);
            double Lat = double.Parse(textBox3.Text);
            Lng = Math.Pow(2, int.Parse(folds[0]) - 1) * (1 + Lng / 180.0);
            Lat = Math.Pow(2, int.Parse(folds[0]) - 1) * (1 - Math.Log(Math.Tan(Math.PI * Lat / 180.0) + 1 / Math.Cos(Math.PI * Lat / 180.0)) / Math.PI);
            string search_path = gpath + "\\" + folds[0];
            int c_Lng = (int)Math.Floor(Lng);
            int c_Lat = (int)Lat + 1;
            search_path += "\\" + c_Lng.ToString();
            if (Directory.Exists(search_path))
            {
                search_path += "\\" + c_Lat.ToString() + ".jpg";
                if (File.Exists(search_path))
                    return true;
            }
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string lat = textBox3.Text.Trim();
            string lng = textBox2.Text.Trim();
            if (lat.Equals(""))
            {
                MessageBox.Show("请输入纬度");
                textBox3.Focus();
                return;
            }
            if (lng.Equals(""))
            {
                MessageBox.Show("请输入经度");
                textBox2.Focus();
                return;
            }
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlat", lat);
            inip.WriteString("mapproperties", "centerlng", lng);

            string rootPath = textBox1.Text.Trim();
            string mapPath = WorkPath + "Publish\\googlemap\\map";
            string starPath = WorkPath + "Publish\\googlemap\\satellite";

            HintForm htfm = new HintForm();
            htfm.hinttext = "正在拷贝地图...";
            htfm.Show();
            htfm.Refresh();

            // 创建目录
            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }
            if (!Directory.Exists(starPath))
            {
                Directory.CreateDirectory(starPath);
            }

            List<string> mpLst = new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    mpLst.Add(checkedListBox1.Items[i].ToString());
                }
            }

            string smapPath = rootPath + "\\roadmap";
            for (int i = 0; i < mpLst.Count; i++)
            {
                htfm.hinttext = "正在拷贝街道图...层级" + mpLst[i];
                htfm.Get_New_Text();
                string destPath = mapPath + "\\" + mpLst[i];
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string sourPath = smapPath + "\\" + mpLst[i];
                Tools.CopyDirectory(sourPath, destPath, panel3, 0);
            }

            smapPath = rootPath + "\\satellite_en";
            for (int i = 0; i < mpLst.Count; i++)
            {
                htfm.hinttext = "正在拷贝卫星图...层级" + mpLst[i];
                htfm.Get_New_Text();
                string destPath = starPath + "\\" + mpLst[i];
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string sourPath = smapPath + "\\" + mpLst[i];
                Tools.CopyDirectory(sourPath, destPath, panel3, 0);
            }
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            inip.WriteString("Public", "UnitID", unitid);            
            htfm.Close();

            /*TreeNode pNode = treeView1.SelectedNode;
            string levelguid = pNode.Tag.ToString();
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");
            string sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            string maplevel = string.Join(",", mpLst);
            if (dt.Rows.Count > 0)
            {
                sql = "update MAPDUIYING_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', MAPLEVEL = '" + maplevel + "' where ISDELETE = 0 and LEVELGUID = '" + levelguid + "'";
                ahp.ExecuteSql(sql, null);
            }
            else 
            {
                sql = "insert into MAPDUIYING_H0001Z000E00 (PGUID, S_UDTIME, LEVELGUID, MAPLEVEL) values ('"
                    + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '"
                    + levelguid + "', '" + maplevel + "')";
                ahp.ExecuteSql(sql, null);
            }*/
            MessageBox.Show("地图导入成功!");
        }


        private string slat, slng;
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            textBox2.Text = slng;
            textBox3.Text = slat;
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlat", slat);
            inip.WriteString("mapproperties", "centerlng", slng);
            ahp = new AccessHelper(WorkPath + "Publish\\data\\经纬度注册.mdb");
            string sql = "update ORGCENTERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LNG = '" + slng + "', LAT = '" + slat + "' where ISDELETE = 0 and UNITEID = '" + unitid + "'";
            ahp.ExecuteSql(sql, null);

            mapHelper1.centerlat = double.Parse(textBox3.Text); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(textBox2.Text); //118.5784; //必须设置的属性,不能为空
            int index = checkedListBox1.SelectedIndex;
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, map_type, null, borderDic, null);
        }

        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
            contextMenuStrip1.Show(x + this.Left + groupBox1.Width + groupBox6.Width + 20, y + this.Top + groupBox4.Height + panel8.Height + 80);
            slat = "" + lat;
            slng = "" + lng;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            /*if (textBox3.Text.Equals(""))
            {
                MessageBox.Show("请输入纬度");
                textBox3.Focus();
                return;
            }
            if (textBox2.Text.Equals(""))
            {
                MessageBox.Show("请输入经度");
                textBox2.Focus();
                return;
            }
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlat", textBox3.Text);
            inip.WriteString("mapproperties", "centerlng", textBox2.Text);
            mapHelper1.centerlat = double.Parse(textBox3.Text); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(textBox2.Text); //118.5784; //必须设置的属性,不能为空
            int index = checkedListBox1.SelectedIndex;
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, null, null);*/
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            /*if (textBox3.Text.Equals(""))
            {
                MessageBox.Show("请输入纬度");
                textBox3.Focus();
                return;
            }
            if (textBox2.Text.Equals(""))
            {
                MessageBox.Show("请输入经度");
                textBox2.Focus();
                return;
            }
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlat", textBox3.Text);
            inip.WriteString("mapproperties", "centerlng", textBox2.Text);
            mapHelper1.centerlat = double.Parse(textBox3.Text); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(textBox2.Text); //118.5784; //必须设置的属性,不能为空
            int index = checkedListBox1.SelectedIndex;
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, null, null);*/
        }

        private void checkedListBox1_Leave(object sender, EventArgs e)
        {
            List<string> mpLst = new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    mpLst.Add(checkedListBox1.Items[i].ToString());
                }
            }

            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");
            string sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            //ahp.CloseConn();
            string maplevel = string.Join(",", mpLst);
            OBJ_MAP[levelguid] = maplevel;
            if (dt.Rows.Count > 0)
            {
                sql = "update MAPDUIYING_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', MAPLEVEL = '" + maplevel + "' where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                ahp.ExecuteSql(sql, null);
                //ahp.CloseConn();
            }
            else
            {
                sql = "insert into MAPDUIYING_H0001Z000E00 (PGUID, S_UDTIME, LEVELGUID, MAPLEVEL, UNITEID) values ('"
                    + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '"
                    + levelguid + "', '" + maplevel + "', '" + unitid + "')";
                ahp.ExecuteSql(sql, null);
                //ahp.CloseConn();
            }
            ahp.CloseConn();
        }

        private void MapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            groupBox1.Focus();

            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();

            List<string> cur_mplst = new List<string>();
            List<string> need_mplst = new List<string>();

            // 读取当前地图列表
            string mappath = WorkPath + "Publish\\googlemap\\map";
            if (Directory.Exists(mappath))
                cur_mplst = Directory.GetDirectories(mappath).ToList();
            for (int i = 0; i < cur_mplst.Count; ++i)
            {
                cur_mplst[i] = Path.GetFileNameWithoutExtension(cur_mplst[i]);
            }

            // 获取当前需导入地图列表
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");
            string sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and UNITEID = '" + unitid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            ahp.CloseConn();
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string tmplevel = dt.Rows[i]["MAPLEVEL"].ToString();
                if (tmplevel != string.Empty)
                {
                    need_mplst.AddRange(tmplevel.Split(','));
                }
            }
            need_mplst = need_mplst.Distinct().ToList();
            for (int i = need_mplst.Count - 1; i >= 0; --i)
            {
                if (need_mplst[i] == string.Empty)
                    need_mplst.Remove(need_mplst[i]);
            }

            List<string> del_mplst = cur_mplst.Except(need_mplst).ToList();
            List<string> add_mplst = need_mplst.Except(cur_mplst).ToList();

            if (del_mplst.Count + add_mplst.Count == 0)
                return;

            if (MessageBox.Show("是否更新对应地图文件?", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            string lat = textBox3.Text.Trim();
            string lng = textBox2.Text.Trim();
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlat", lat);
            inip.WriteString("mapproperties", "centerlng", lng);

            string rootPath = textBox1.Text.Trim();
            string mapPath = WorkPath + "Publish\\googlemap\\map";
            string starPath = WorkPath + "Publish\\googlemap\\satellite";

            //HintForm htfm = new HintForm();
            //htfm.hinttext = "正在更新地图...";
            //htfm.Show();
            //htfm.Refresh();

            string Copy_File = string.Empty;
            string Delete_File = string.Empty;

            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }
            if (!Directory.Exists(starPath))
            {
                Directory.CreateDirectory(starPath);
            }

            string smapPath = rootPath + "\\roadmap";
            for (int i = 0; i < add_mplst.Count; i++)
            {
                //htfm.hinttext = "正在拷贝街道图...层级" + add_mplst[i];
                //htfm.Get_New_Text();
                string destPath = mapPath + "\\" + add_mplst[i];
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string sourPath = smapPath + "\\" + add_mplst[i];
                //Tools.CopyDirectory(sourPath, destPath, panel3, 0);
                //Process p = Process.Start(WorkPath + "CopyDir.exe", sourPath + " " + destPath + " 1");
                //p.WaitForExit();
                Copy_File += sourPath + "：" + destPath + ",";
            }

            smapPath = rootPath + "\\satellite_en";
            for (int i = 0; i < add_mplst.Count; i++)
            {
                //htfm.hinttext = "正在拷贝卫星图...层级" + add_mplst[i];
                //htfm.Get_New_Text();
                string destPath = starPath + "\\" + add_mplst[i];
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string sourPath = smapPath + "\\" + add_mplst[i];
                //Tools.CopyDirectory(sourPath, destPath, panel3, 0);
                //Process p = Process.Start(WorkPath + "CopyDir.exe", sourPath + " " + destPath + " 1");
                //p.WaitForExit();
                Copy_File += sourPath + "：" + destPath + ",";
            }
            Process p;
            if (Copy_File != string.Empty)
            {
                Copy_File = Copy_File.Substring(0, Copy_File.Length - 1);
                p = Process.Start(WorkPath + "CopyDir.exe", Copy_File + " 1");
                p.WaitForExit();
            }

            for (int i = 0; i < del_mplst.Count; ++i)
            {
                //htfm.hinttext = "正在删除街道图...层级" + del_mplst[i];
                //htfm.Get_New_Text();
                string destPath = mapPath + "\\" + del_mplst[i];
                while (Directory.Exists(destPath))
                {
                    //Directory.Delete(destPath, true);
                    //Process p = Process.Start(WorkPath + "DeleteDir.exe", destPath);
                    //p.WaitForExit();
                    Delete_File += destPath + ",";
                }
            }

            for (int i = 0; i < del_mplst.Count; ++i)
            {
                //htfm.hinttext = "正在删除卫星图...层级" + del_mplst[i];
                //htfm.Get_New_Text();
                string destPath = starPath + "\\" + del_mplst[i];
                while (Directory.Exists(destPath))
                {
                    //Directory.Delete(destPath, true);
                    //Process p = Process.Start(WorkPath + "DeleteDir.exe", destPath);
                    //p.WaitForExit();
                    Delete_File += destPath + ",";
                }
            }
            p = Process.Start(WorkPath + "DeleteDir.exe", Delete_File);
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            inip.WriteString("Public", "UnitID", unitid);
            //htfm.Close();
            MessageBox.Show("地图更新成功!");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            // 清除地图缓存
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            string Read_id = inip.ReadString("Public", "UnitID", "");
            if (Read_id != unitid)
            {
                //HintForm htfm = new HintForm();
                //htfm.hinttext = "正在清除地图缓存，请稍后...";
                //htfm.Show();
                //Thread t1 = new Thread(new ThreadStart(Delete_Path));
                //t1.Start();
                Delete_Path();
                //htfm.Close();
            }
        }

        private void mapHelper1_MapTypeChanged(string mapType)
        {
            map_type = mapType;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string file = openFileDialog1.FileName;

            borList = new List<double[]>();
            borderDic = new Dictionary<string, object>();
            borderDic.Add("type", "实线");
            borderDic.Add("width", 2);
            borderDic.Add("color", "#000000");
            borderDic.Add("opacity", 1);
            string[] strAll = File.ReadAllLines(file);
            string ds_lng_lat = "";
            foreach (string str in strAll)
            {
                string[] split = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n' });
                borList.Add(new double[] { double.Parse(split[1]), double.Parse(split[0]) });
                ds_lng_lat += split[1] + "," + split[0] + ";";
            }
            borderDic.Add("path", borList);
            int index = checkedListBox1.SelectedIndex;
            ahp = new AccessHelper(WorkPath + "Publish\\data\\经纬度注册.mdb");
            string sql = "select PGUID from BORDERDATA where ISDELETE = 0 and UNITID = '" + unitid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update BORDERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LNG_LAT = '" 
                    + ds_lng_lat + "' where ISDELETE = 0 and UNITID = '" + unitid + "'";
                ahp.ExecuteSql(sql, null);
            }
            else 
            {
                sql = "insert into BORDERDATA (PGUID, S_UDTIME, UNITID, LNG_LAT) values ('" + Guid.NewGuid().ToString("B") 
                    + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + unitid + "', '" + ds_lng_lat + "')";
                ahp.ExecuteSql(sql, null);
            }
            ahp.CloseConn();
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, map_type, null, borderDic, null);
        }
    }

    public class CompStr : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int tx, ty;
            tx = int.Parse(x);
            ty = int.Parse(y);
            if (tx > ty)
                return 1;
            if (tx < ty)
                return -1;
            return 0;
        }
    }
}
