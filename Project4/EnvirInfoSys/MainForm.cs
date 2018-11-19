using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;


namespace EnvirInfoSys
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private AccessHelper ahp = null;
        private IniOperator inip = null;   

        private string Icon_GUID = "";//当前选择的图标guid
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;//当前exe根目录
        private string AccessPath = AppDomain.CurrentDomain.BaseDirectory + "data\\ENVIR_H0001Z000E00.mdb";
        private string IniFilePath = AppDomain.CurrentDomain.BaseDirectory + "parameter.ini";
        private string UnitID = "-1";
        private int cur_Level;
        private Dictionary<string, string> GUID_Icon;
        private Dictionary<string, string> FDName_Value;
        private string[] folds = null;
        private Dictionary<string, string> Icon_JDCode;
        private Dictionary<string, string> Icon_Name;
        private List<Dictionary<string, object>> cur_lst;

        // 管理级别数据
        private Dictionary<string, string> OBJ_NAME;
        private Dictionary<string, string> OBJ_JDCODE;
        private Dictionary<string, string> OBJ_UPGUID;
        private Dictionary<string, string> OBJ_MAP;
        private string[] OBJ_PGUID;

        private void 数据备份ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataBF.exe");
            p.WaitForExit();
        }

        private void 数据恢复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataHF.exe");
            p.WaitForExit();
        }

        private void 数据同步ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataUP.exe", "EnvirInfoSys.exe 0");
            p.WaitForExit();
        }

        private void iP设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "SetIP.exe");
            p.WaitForExit();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 加载主界面
            inip = new IniOperator(WorkPath + "RegInfo.ini");
            string UnitName = inip.ReadString("Public", "UnitName", "");
            UnitName = UnitName.Replace("\0", "");
            string AppName = inip.ReadString("Public", "AppName", "");
            AppName = AppName.Replace("\0", "");
            this.Text = UnitName + AppName;

            // 读取单位数据
            inip = new IniOperator(WorkPath + "RegInfo.ini");
            UnitID = inip.ReadString("Public", "UnitID", "-1");

            // 加载管理级别
            folds = get_map_list();
            Load_Unit_Level();

            // 读入图标对应数据
            Icon_JDCode = new Dictionary<string, string>();
            Icon_Name = new Dictionary<string, string>();
            ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            string sql = "select PGUID, JDNAME, JDCODE from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                Icon_Name.Add(pguid + ".png", dt.Rows[i]["JDNAME"].ToString());
                Icon_JDCode.Add(pguid, dt.Rows[i]["JDCODE"].ToString());
            }

            // 地图初始化
            inip = new IniOperator(IniFilePath);
            string slat = inip.ReadString("mapproperties", "centerlat", "");
            string slng = inip.ReadString("mapproperties", "centerlng", "");
            mapHelper1.centerlat = double.Parse(slat); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(slng); //118.5784; //必须设置的属性,不能为空
            mapHelper1.webpath = WorkPath + "googlemap"; //必须设置的属性,不能为空
            mapHelper1.roadmappath = WorkPath + "googlemap\\map"; //必须设置的属性,不能为空
            mapHelper1.satellitemappath = WorkPath + "googlemap\\satellite"; //必须设置的属性,不能为空
            mapHelper1.iconspath = WorkPath + "PNGICONFOLDER"; //必须设置的属性,不能为空
            mapHelper1.maparr = folds;

            radioButton1.Checked = true;
        }

        private string[] get_map_list() 
        {
            string[] lists = null;
            string mappath = WorkPath + "googlemap\\map";
            lists = Directory.GetDirectories(mappath);

            for (int i = 0; i < lists.Length; i++)
            {
                int tmp = lists[i].LastIndexOf("\\");
                lists[i] = lists[i].Substring(tmp + 1);
            }

            return lists;
        }

        private void Load_Unit_Level()
        {
            OBJ_NAME = new Dictionary<string, string>();
            OBJ_JDCODE = new Dictionary<string, string>();
            OBJ_UPGUID = new Dictionary<string, string>();
            OBJ_MAP = new Dictionary<string, string>();
            
            ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K01.mdb");
            string sql = "select PGUID, JDNAME, JDCODE, UPGUID from ZSK_OBJECT_H0001Z000K01 where ISDELETE = 0";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            OBJ_PGUID = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                OBJ_PGUID[i] = pguid;
                OBJ_NAME[pguid] = dt.Rows[i]["JDNAME"].ToString();
                OBJ_JDCODE[pguid] = dt.Rows[i]["JDCODE"].ToString();
                OBJ_UPGUID[pguid] = dt.Rows[i]["UPGUID"].ToString();
            }

            treeView1.Nodes.Clear();
            treeView1.HideSelection = false;
            treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);

            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                ahp = new AccessHelper(WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb");
                sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + pguid + "'";
                DataTable dt1 = ahp.ExecuteDataTable(sql, null);
                if (dt1.Rows.Count > 0)
                    OBJ_MAP.Add(pguid, dt1.Rows[0]["MAPLEVEL"].ToString());
                else
                    OBJ_MAP.Add(pguid, string.Empty);

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            Point screenPoint = Control.MousePosition;
            if (screenPoint.X > groupBox1.Width && screenPoint.Y > 200)
            {
                this.groupBox1.Visible = false;
            }
            else if (screenPoint.X < groupBox1.Width && screenPoint.Y > 200)
            {
                this.groupBox1.Visible = true;
            }

            if (screenPoint.X > this.Width - groupBox2.Width && screenPoint.Y > 200)
            {
                this.groupBox2.Visible = true;
            }
            else if (screenPoint.X < this.Width - groupBox2.Width && screenPoint.Y > 200)
            {
                this.groupBox2.Visible = false;
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

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色，只需要在TreeView失去焦点时选中节点仍然突显  
            return;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            string levelguid = pNode.Tag.ToString();

            // 处理cur_level
            bool flag = false;
            string[] maps = OBJ_MAP[levelguid].Split(',');
            for (int i = 0; i < maps.Length; ++i)
            {
                if (maps[i] == cur_Level.ToString())
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                if (maps[0] != string.Empty)
                    cur_Level = int.Parse(maps[0]);
                else
                    cur_Level = 0;
            }
            GUID_Icon = new Dictionary<string, string>();
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();//标注list，从数据库获取
            
            ahp = new AccessHelper(AccessPath);
            string sql = "select * from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++ i)
            {
                GUID_Icon[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["ICONGUID"].ToString();
                Dictionary<string, object> dic = new Dictionary<string, object>();//添加每个标注
                dic.Add("guid", dt.Rows[i]["PGUID"].ToString());                            //必须加载的标准属性，从数据库查询得到值
                dic.Add("name", dt.Rows[i]["MAKRENAME"].ToString());                        //必须加载的标准属性，从数据库查询得到值
                dic.Add("level", cur_Level.ToString());                                     //必须加载的标准属性，从数据库查询得到值
                dic.Add("canedit", dt.Rows[i]["UNITEID"].ToString() == UnitID.ToString());  //必须加载的标准属性，根据上层单位判断
                dic.Add("type", dt.Rows[i]["MARKETYPE"].ToString());                        //必须加载的标准属性，从数据库查询得到值
                dic.Add("lat", dt.Rows[i]["MARKELAT"].ToString());                          //必须加载的标准属性，从数据库查询得到值
                dic.Add("lng", dt.Rows[i]["MARKELNG"].ToString());                          //必须加载的标准属性，从数据库查询得到值
                string icon_path = WorkPath + "googlemap\\mapfiles\\icons\\bicon" + cur_Level + "\\" + dt.Rows[i]["ICONGUID"].ToString() + ".png";
                dic.Add("iconpath", icon_path);                                             //必须加载的标准属性
                dic.Add("message", /*sdic*/null);                                           //必须加载，内容随便，此处无用
                lst.Add(dic);                                                               //给list添加一个标注
            }
            cur_lst = lst;
            mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], true, Icon_Name, lst);
        }

        private void mapHelper1_IconSelected(string level, string iconPath)
        {
            Icon_GUID = iconPath;//小图标选择事件
        }

        private void mapHelper1_MapMouseup(string Mousebutton, bool canedit, double lat, double lng, int x, int y, string markerguid)
        {
            if (markerguid.Equals("") && !Icon_GUID.Equals(""))
            {
                string iconguid = Path.GetFileNameWithoutExtension(Icon_GUID);
                DataForm dtf = new DataForm();
                dtf.Icon_GUID = iconguid;
                dtf.Update_Data = false;
                dtf.Text = "添加标注";
                //dtf.Left = x;
                //dtf.Top = y;
                if (dtf.ShowDialog() == DialogResult.OK)
                {
                    string name = dtf.Node_Name;
                    FDName_Value = dtf.FDName_Value;
                    mapHelper1.addMarker("" + lat, "" + lng, name, true, Icon_GUID, null);//在up事件中添加新标注
                }    
                Icon_GUID = "";//添加完成后把选择的图标guid清空
            }
        }

        private void mapHelper1_MouseDown(object sender, MouseEventArgs e)
        {
            //
        }

        private void mapHelper1_MarkerDragEnd(string markerguid, bool canedit, double lat, double lng)
        {

            //MessageBox.Show("移动：" + markerguid);
            // 数据库 update 坐标
            ahp = new AccessHelper(AccessPath);
            string sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', MARKELAT = '" + lat.ToString() + "', MARKELNG = '" + lng.ToString() + "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);
        }

        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
           // contextMenuStrip1.Show(x, y);
        }

        private string Operator_GUID = "";
        private void mapHelper1_MarkerRightClick(int sx, int sy, string level, string sguid, string name, bool canedit, string message)
        {
            contextMenuStrip1.Show(sx + groupBox3.Left, sy + groupBox3.Top + 80);
            Operator_GUID = sguid;
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Operator_GUID != "")
            {
                string iconguid = "";
                ahp = new AccessHelper(AccessPath);
                string sql = "select ICONGUID from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + Operator_GUID + "'";
                DataTable dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count != 0)
                    iconguid = dt.Rows[0]["ICONGUID"].ToString();
                else
                    return;
                DataForm dtf = new DataForm();
            
                dtf.Update_Data = true;
                dtf.Node_GUID = Operator_GUID;
                dtf.Icon_GUID = iconguid;
                dtf.JdCode = Icon_JDCode[iconguid];
                dtf.Text = "编辑标注";
                if (dtf.ShowDialog() == DialogResult.OK)
                {
                    string name = dtf.Node_Name;
                    FDName_Value = dtf.FDName_Value;
                    mapHelper1.modifyMarker(Operator_GUID, name, true, null);
                }
                Operator_GUID = "";
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Operator_GUID != "")
            {
                mapHelper1.deleteMarker(Operator_GUID);
                Operator_GUID = "";
            }
        }

        private void mapHelper1_AddMarkerFinished(string markerguid, double lat, double lng, string name, bool canEdit, string iconpath, string message)
        {
            // 添加完成事件，调用addMarker后触发
            // 数据库  insert
            ahp = new AccessHelper(AccessPath);
            string iconguid = Path.GetFileNameWithoutExtension(iconpath);

            TreeNode pNode = treeView1.SelectedNode;
            string levelguid = pNode.Tag.ToString();

            string sql = "insert into ENVIRICONDATA_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, LEVELGUID, MAPLEVEL, MARKELAT, MARKELNG, MAKRENAME, UNITEID) values('" 
                         + markerguid + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + iconguid + "', '" + levelguid + "', '" 
                         + cur_Level.ToString() + "', '" + lat.ToString() + "', '" + lng.ToString() + "', '" + name + "', '" + UnitID.ToString() + "')";
            ahp.ExecuteSql(sql, null);
            GUID_Icon[markerguid] = iconguid;
            string table_name = Icon_JDCode[iconguid];
            sql = "insert into " + table_name + " (PGUID, S_UDTIME";
            //bool flag = false;
            foreach (string key in FDName_Value.Keys) 
                sql += ", " + key;
            sql += ") values('" + markerguid + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            foreach (string value in FDName_Value.Values)
                sql += "', '" + value;
            sql += "')";
            ahp.ExecuteSql(sql, null);
        }

        private void mapHelper1_RemoveMarkerFinished(string markerguid, bool ok)
        {
            // 删除完成事件，调用deleteMarker后触发
            // 数据库  update isdelete = 1
            
            ahp = new AccessHelper(AccessPath);
            string sql = "update ENVIRICONDATA_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);

            string icon = GUID_Icon[markerguid];
            string table_name = Icon_JDCode[icon];
            sql = "update " + table_name + " set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ISDELEtE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);
        }

        private void mapHelper1_ModifyMarkerFinished(string markerguid, double lat, double lng, string name, bool canEdit, string iconpath, string message)
        {
            // 更新完成事件，调用ModifyMarker后触发
            // 数据库  update 

            ahp = new AccessHelper(AccessPath);
            string sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', MAKRENAME = '" + name + "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);

            ahp = new AccessHelper(AccessPath);
            string icon = GUID_Icon[markerguid];
            string table_name = Icon_JDCode[icon];
            sql = "update " + table_name + " set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            foreach (var item in FDName_Value)
            {
                sql += ", " + item.Key + " = '" + item.Value + "'";
            }
            sql += " where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);
        }

        private void mapHelper1_MapDblClick(string button, bool canedit, double lat, double lng, int x, int y, string markerguid)
        {
            TreeNode pNode = treeView1.SelectedNode;
            string levelguid = pNode.Tag.ToString();
            int len_map = folds.Length;
            int len_obj = OBJ_PGUID.Length;
            int now_map = 0, now_obj = 0;
            for (int i = 0; i < len_map; ++ i)
                if (folds[i] == cur_Level.ToString()) 
                {
                    now_map = i;
                    break;
                }
            for (int i = 0; i < len_obj; ++i)
                if (OBJ_PGUID[i] == levelguid)
                {
                    now_obj = i;
                    break;
                }

            if (radioButton1.Checked)
            {
                ++now_map;
                if (now_map >= len_map)
                {
                    //MessageBox.Show("地图已达最大级别!");
                    return;
                }
                for (int i = 0; i < len_obj; ++i)
                {
                    string objguid = OBJ_PGUID[(now_obj + i) % len_obj];
                    string[] maps = OBJ_MAP[objguid].Split(',');
                    for (int j = 0; j < maps.Length; ++j)
                        if (maps[j] == folds[now_map])
                        {
                            cur_Level = int.Parse(maps[j]);
                            for (int k = 0; k < cur_lst.Count; ++k)
                            {
                                cur_lst[k]["level"] = cur_Level.ToString();
                            }

                            mapHelper1.ShowMap(cur_Level, OBJ_NAME[objguid], true, Icon_Name, cur_lst);
                            TreeNode resNode = null;
                            foreach (TreeNode tn in treeView1.Nodes)
                            {
                                resNode = Select_Node(objguid, tn);
                                if (resNode != null)
                                {
                                    treeView1.SelectedNode = resNode;
                                    return;
                                }
                            }
                        }
                }
            }
            else
            {
                --now_map;
                if (now_map < 0)
                {
                    //MessageBox.Show("地图已达最小级别!");
                    return;
                }
                for (int i = 0; i < len_obj; ++i)
                {
                    string objguid = OBJ_PGUID[(now_obj - i + len_obj) % len_obj];
                    string[] maps = OBJ_MAP[objguid].Split(',');
                    for (int j = 0; j < maps.Length; ++j)
                        if (maps[j] == folds[now_map])
                        {
                            cur_Level = int.Parse(maps[j]);
                            for (int k = 0; k < cur_lst.Count; ++k)
                            {
                                cur_lst[k]["level"] = cur_Level.ToString();
                            }

                            mapHelper1.ShowMap(cur_Level, OBJ_NAME[objguid], true, Icon_Name, cur_lst);
                            TreeNode resNode = null;
                            foreach (TreeNode tn in treeView1.Nodes)
                            {
                                resNode = Select_Node(objguid, tn);
                                if (resNode != null)
                                {
                                    treeView1.SelectedNode = resNode;
                                    return;
                                }
                            }
                        }
                }
            }
        }

        private TreeNode Select_Node(string levelguid, TreeNode pNode)
        {
            if (pNode == null)
                return null;
            if (pNode.Tag.ToString() == levelguid)
                return pNode;
            TreeNode resNode = null;
            foreach (TreeNode tn in pNode.Nodes)
            {
                resNode = Select_Node(levelguid, tn);
                if (resNode != null)
                    break;
            }
            return resNode;
        }

    }

    //标注实体类
    public class MarkerData
    {
        public string id { set; get; }
        public string mlevel { set; get; }
        public string type { set; get; }
        public string lat { set; get; }
        public string lng { set; get; }
        public string icon { set; get; }
        public string title { set; get; }
        public string message { set; get; }
    }
}
