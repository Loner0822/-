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
        private bool isT = false;
        //public string unitname = "";
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;    //当前exe根目录
        private string[] folds = null;

        Dictionary<string, string> GUID_ICON = new Dictionary<string,string>();

        private void MapForm_Shown(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();

            // 清除地图缓存
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            string Read_id = inip.ReadString("Public", "UnitID", "");
            if (Read_id != unitid) 
            {
                HintForm htfm = new HintForm();
                htfm.hinttext = "正在清除地图缓存，请稍后...";
                htfm.Show();
                isT = false;
                Thread t1 = new Thread(new ThreadStart(Delete_Path));
                t1.Start();
                while (isT == true)
                {

                }
                htfm.Close();
            }

            // 导入管理级别
            Load_Unit_Level();

            // 图符对应初始化
            Show_Icon_List();

            if (treeView1.Nodes.Count > 0)
                treeView1.SelectedNode = treeView1.Nodes[0];
            // 地图对应初始化
            button2.Enabled = false;
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            textBox1.Text = inip.ReadString("mapproperties", "map_path", "");
            textBox2.Text = inip.ReadString("mapproperties", "centerlng", "0");
            textBox3.Text = inip.ReadString("mapproperties", "centerlat", "0");
            checkedListBox1.Items.Clear();
            if (textBox1.Text != string.Empty)
                Show_Map_List(textBox1.Text);

            // 刷新地图、图符对应
            if (treeView1.Nodes.Count > 0)
                treeView1.SelectedNode = treeView1.Nodes[0];
        }

        private void Delete_Path()
        {
            string mapPath = WorkPath + "Publish\\googlemap\\map";
            string starPath = WorkPath + "Publish\\googlemap\\satellite";
            while (Directory.Exists(mapPath))
            {
                Tools.DeleteFolder(mapPath, null);
            }
            while (Directory.Exists(starPath))
            {
                Tools.DeleteFolder(starPath, null);
            }
            isT = true;
        }

        Dictionary<string, string> OBJ_NAME;
        Dictionary<string, string> OBJ_JDCODE;
        Dictionary<string, string> OBJ_UPGUID;
        private void Load_Unit_Level()
        {
            OBJ_NAME = new Dictionary<string, string>();
            OBJ_JDCODE = new Dictionary<string, string>();
            OBJ_UPGUID = new Dictionary<string, string>();
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ZSK_H0001Z000K01.mdb");
            string sql = "select PGUID, JDNAME, JDCODE, UPGUID from ZSK_OBJECT_H0001Z000K01 where ISDELETE = 0";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
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
            sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
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
                    sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + pguid + "'";
                    DataTable dt1 = ahp.ExecuteDataTable(sql, null);
                    if (dt1.Rows.Count > 0)
                        ucPB.IconCheck = true;
                    else
                        ucPB.IconCheck = false;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e, string iconguid, bool ischecked)
        {
            //MessageBox.Show("1");
            TreeNode pNode = treeView1.SelectedNode;
            string levelguid = pNode.Tag.ToString();
            ahp = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");
            string sql;
            if (ischecked == false)
            {
                sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "' where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "'";
                ahp.ExecuteSql(sql, null);
            }
            else 
            {
                sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "'";
                DataTable dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    /*sql = "update ICONDUIYING_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                        "' where ISDELETE = 0  and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "'";
                    ahp.ExecuteSql(sql, null);*/
                }
                else 
                {
                    sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 1 and LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "'";
                    dt = ahp.ExecuteDataTable(sql, null);
                    if (dt.Rows.Count > 0)
                    {
                        sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                            "' where LEVELGUID = '" + levelguid + "' and ICONGUID = '" + iconguid + "'";
                        ahp.ExecuteSql(sql, null);
                    }
                    else
                    {
                        sql = "insert into ICONDUIYING_H0001Z000E00 (PGUID, S_UDTIME, LEVELGUID, ICONGUID) values('"
                            + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '"
                            + levelguid + "', '" + iconguid + "')";
                        ahp.ExecuteSql(sql, null);
                    }
                }
            }
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
                MessageBox.Show("请下载混合图(无偏移)，窗口将关闭");
                inip.WriteString("mapproperties", "map_path", "");
                this.Close();
                return;
            }
            gpath = textBox1.Text + "\\roadmap";
            if (!Directory.Exists(gpath))
            {
                MessageBox.Show("请下载街道图，窗口将关闭");
                inip.WriteString("mapproperties", "map_path", "");
                this.Close();
                return;
            }

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
            int index = checkedListBox1.SelectedIndex;
            //List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();//标注list，从数据库获取
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, null, null);
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
            folderBrowserDialog1.SelectedPath = inip.ReadString("mapproperties", "map_path", "");
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // 获取地图文件夹的上级目录
                // 允许用户选择地图文件夹的总目录、其下的：roadmap、satellite或者satellite_en子目录
                string tmp = folderBrowserDialog1.SelectedPath;                
                inip.WriteString("mapproperties", "map_path", tmp);
                Show_Map_List(tmp);
            }
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

            TreeNode pNode = treeView1.SelectedNode;
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
            }


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
            mapHelper1.centerlat = double.Parse(textBox3.Text); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(textBox2.Text); //118.5784; //必须设置的属性,不能为空
            int index = checkedListBox1.SelectedIndex;
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, null, null);
        }

        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
            contextMenuStrip1.Show(x + this.Left + groupBox1.Width + groupBox6.Width + 20, y + this.Top + groupBox4.Height + panel8.Height + 80);
            slat = "" + lat;
            slng = "" + lng;
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text.Equals(""))
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
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, null, null);
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text.Equals(""))
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
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), "", false, null, null);
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            /*List<string> mpLst = new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
            {
                if (i == e.Index)
                {
                    if (e.NewValue == CheckState.Checked)
                        mpLst.Add(checkedListBox1.Items[i].ToString());
                }
                else
                {
                    if (checkedListBox1.GetItemChecked(i))
                    {
                        mpLst.Add(checkedListBox1.Items[i].ToString());
                    }
                }
            }

            TreeNode pNode = treeView1.SelectedNode;
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
        }

        private void checkedListBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            
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
