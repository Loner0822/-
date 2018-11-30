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
        private bool select_point = false;
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;//当前exe根目录
        private string AccessPath = AppDomain.CurrentDomain.BaseDirectory + "data\\ENVIR_H0001Z000E00.mdb";
        private string IniFilePath = AppDomain.CurrentDomain.BaseDirectory + "parameter.ini";
        private string UnitID = "-1";
        private string map_type = "g_map";
        private int cur_Level;
        private Dictionary<string, string> GUID_Icon;
        private Dictionary<string, string> FDName_Value;
        private string[] folds = null;
        private Dictionary<string, string> Icon_JDCode;
        private Dictionary<string, string> Icon_Name;
        private List<Dictionary<string, object>> cur_lst;
        private bool Permission = false;
        private bool Before_ShowMap = false;

        // 管理级别数据
        private Dictionary<string, string> OBJ_NAME;
        private Dictionary<string, string> OBJ_JDCODE;
        private Dictionary<string, string> OBJ_UPGUID;
        private Dictionary<string, string> OBJ_MAP;
        private string[] OBJ_PGUID;

        // 边界线
        Dictionary<string, object> borderDic = null;
        private List<double[]> borList = null;
        private LineData borData = null;

        // 管辖分类
        private string GXguid = "-1";
        private string FLguid = "-1";
        
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

        private void IP设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "SetIP.exe");
            p.WaitForExit();
        }

        private void 边界线属性设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BorderForm bdfm = new BorderForm();
            bdfm.IsPoint = false;
            bdfm.IsLine = false;
            bdfm.borData = Load_Line("边界线");
            if (bdfm.ShowDialog() == DialogResult.OK)
            {
                borData = bdfm.borData;
                TreeNode pNode = treeView1.SelectedNode;
                if (treeView1.Nodes.Count > 0)
                {
                    string levelguid = pNode.Tag.ToString();
                    borderDic["type"] = borData.Type;
                    borderDic["width"] = borData.Width;
                    borderDic["color"] = borData.Color;
                    borderDic["opacity"] = borData.Opacity;
                    Save_Line(borData, "边界线");
                    mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
                }
            }
        }

        private void 管辖设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClassifyForm clcfm = new ClassifyForm();
            clcfm.unitid = UnitID;
            clcfm.gxguid = GXguid;
            clcfm.ShowDialog();
            panel2.Controls.Clear();
            Load_Guan_Xia();
            foreach (Control ctrl in panel2.Controls)
                ctrl.Width = 80;
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
            string sql = "select PGUID, JDNAME, JDCODE from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
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

            // 边界线导入
            borList = new List<double[]>();
            borderDic = new Dictionary<string, object>();
            borData = new LineData();
            borData = Load_Line("边界线");
            borderDic.Add("type", borData.Type);
            borderDic.Add("width", borData.Width);
            borderDic.Add("color", borData.Color);
            borderDic.Add("opacity", borData.Opacity);
            ahp = new AccessHelper(WorkPath + "data\\经纬度注册.mdb");
            sql = "select LNG_LAT from BORDERDATA where ISDELETE = 0 and UNITID = '" + UnitID + "'";
            dt = ahp.ExecuteDataTable(sql, null);
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

            // 加载管辖类型
            ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '-1' order by SHOWINDEX";
            dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                RadioButton new_RB = new RadioButton();
                new_RB.Parent = panel1;
                new_RB.Dock = DockStyle.Left;
                new_RB.Appearance = Appearance.Button;
                new_RB.FlatStyle = FlatStyle.Popup;
                new_RB.Text = dt.Rows[i]["FLNAME"].ToString();
                new_RB.Tag = dt.Rows[i]["PGUID"].ToString();
                new_RB.TextAlign = ContentAlignment.MiddleCenter;
                new_RB.CheckedChanged += RadioButton_CheckedChanged;
            }
            foreach (Control ctrl in panel1.Controls)
                ctrl.Width = 80;
            int cnt = panel1.Controls.Count;
            if (cnt > 0)
            {
                RadioButton First_Button = (RadioButton)panel1.Controls[cnt - 1];
                First_Button.Checked = true;
            }

            // 地图设置
            radioButton1.Checked = true;
            groupBox1.Visible = false;
            groupBox2.Visible = false;

            // 显示登陆界面
            LoginForm lgf = new LoginForm();
            lgf.Text += " " + this.Text;
            if (lgf.ShowDialog() == DialogResult.OK)
            {
                if (lgf.Mode == 1)
                {
                    //MessageBox.Show("登录成功，即将进入管理员模式");
                    this.Text += " - [编辑模式]";
                    Permission = true;
                    删除ToolStripMenuItem.Visible = true;
                    添加指向位置ToolStripMenuItem.Visible = true;
                    修改指向位置ToolStripMenuItem.Visible = true;
                    删除指向位置ToolStripMenuItem.Visible = true;
                    边界线属性设置ToolStripMenuItem1.Visible = true;
                    管辖设置ToolStripMenuItem.Visible = true;
                    修改箭头样式ToolStripMenuItem.Visible = true;
                }
                if (lgf.Mode == 2)
                {
                    //MessageBox.Show("登录失败，即将进入游客模式");
                    this.Text += " - [查看模式]";
                    Permission = false;
                    删除ToolStripMenuItem.Visible = false;
                    添加指向位置ToolStripMenuItem.Visible = false;
                    修改指向位置ToolStripMenuItem.Visible = false;
                    删除指向位置ToolStripMenuItem.Visible = false;
                    边界线属性设置ToolStripMenuItem1.Visible = false;
                    管辖设置ToolStripMenuItem.Visible = false;
                    修改箭头样式ToolStripMenuItem.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("即将退出界面");
                System.Environment.Exit(0);
            }

            if (treeView1.Nodes.Count > 0)
                treeView1.SelectedNode = treeView1.Nodes[0];
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Focus();
            contextMenuStrip2.Show(button1.Left, button1.Top + 45);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel2.Focus();
            contextMenuStrip3.Show(button2.Left, button2.Top + 45);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
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

            string sql = "select PGUID, JDNAME, JDCODE, UPGUID from ZSK_OBJECT_H0001Z000K01 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
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

        private void Load_Guan_Xia()
        {
            ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            string sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + GXguid +  "' order by SHOWINDEX desc";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                RadioButton new_RB = new RadioButton();
                new_RB.Parent = panel2;
                new_RB.Dock = DockStyle.Left;
                new_RB.Appearance = Appearance.Button;
                new_RB.FlatStyle = FlatStyle.Popup;
                new_RB.Text = dt.Rows[i]["FLNAME"].ToString();
                new_RB.Tag = dt.Rows[i]["PGUID"].ToString();
                new_RB.TextAlign = ContentAlignment.MiddleCenter;
                new_RB.CheckedChanged += RadioButton_CheckedChanged;
                //new_RB.Checked = true;
            }
            int cnt = panel2.Controls.Count;
            if (cnt > 0)
            {
                RadioButton First_Button = (RadioButton)panel2.Controls[cnt - 1];
                First_Button.Checked = true;
            }
            else
            {
                TreeNode pNode = treeView1.SelectedNode;
                if (pNode != null)
                {
                    if (FLguid == "")
                        FLguid = "-1";
                    string levelguid = pNode.Tag.ToString();
                    Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                    p.WaitForExit();
                    if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                        mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                    else
                        mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
                }
                
            }
            foreach (Control ctrl in panel2.Controls)
                ctrl.Width = 80;
        }

        private void Get_Marker_From_Access()
        {
 
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            Operator_GUID = "";
            select_point = false;
            RadioButton tmp = (RadioButton)sender;
            if (!tmp.Checked)
                tmp.BackColor = SystemColors.Control;
            else
                tmp.BackColor = SystemColors.ActiveCaption;
            if (tmp.Parent == panel1 && tmp.Checked == true)
            {
                // 刷新panel2
                GXguid = tmp.Tag.ToString();
                panel2.Controls.Clear();
                Load_Guan_Xia();
            }
            if (tmp.Parent == panel2 && tmp.Checked == true)
            {
                
                TreeNode pNode = treeView1.SelectedNode;
                if (pNode == null)
                {
                    mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                    return;
                }
                string levelguid = pNode.Tag.ToString();

                FLguid = tmp.Tag.ToString();
                // 数据库操作 刷新图符列表, 刷新地图

                GUID_Icon = new Dictionary<string, string>();
                List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();//标注list，从数据库获取

                // 获取显示图符列表
                /*string extra_sql = "";
                string sql = "select ICONGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and FLGUID = '" + FLguid + "' and UNITID = '" + UnitID + "'";
                DataTable dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    extra_sql = " and (ICONGUID = '" + dt.Rows[0]["ICONGUID"].ToString() + "'";
                    for (int i = 1; i < dt.Rows.Count; ++i)
                        extra_sql += " or ICONGUID = '" + dt.Rows[i]["ICONGUID"].ToString() + "'";
                    extra_sql += ")";
                }
                */
                ahp = new AccessHelper(AccessPath);
                string extra_sql1 = "and ICONGUID in (select ICONGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and FLGUID = '" + FLguid + "' and UNITID = '" + UnitID + "')";
                string extra_sq12 = "and ICONGUID in (select ICONGUID from [;database=" + WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb" + "].ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and UNITEID = '" + UnitID + "')";
                string sql = "select * from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 ";
                DataTable dt = ahp.ExecuteDataTable(sql + extra_sql1 + extra_sq12, null);
                
                for (int i = 0; i < dt.Rows.Count; ++i)
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
                    Dictionary<string, object> toDic = new Dictionary<string, object>();
                    if (dt.Rows[i]["POINTLINE"].ToString() != "0")
                    {
                        sql = "select LINETYPE, LINEWIDTH, LINECOLOR, LINEOPACITY from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + dt.Rows[i]["PGUID"].ToString() + "'";
                        DataTable dt1 = ahp.ExecuteDataTable(sql, null);
                        if (dt1.Rows.Count > 0)
                        {
                            toDic.Add("lat", double.Parse(dt.Rows[i]["POINTLAT"].ToString()));
                            toDic.Add("lng", double.Parse(dt.Rows[i]["POINTLNG"].ToString()));
                            toDic.Add("type", dt1.Rows[0]["LINETYPE"].ToString());
                            toDic.Add("width", int.Parse(dt1.Rows[0]["LINEWIDTH"].ToString()));
                            toDic.Add("color", dt1.Rows[0]["LINECOLOR"].ToString());
                            toDic.Add("opacity", double.Parse(dt1.Rows[0]["LINEOPACITY"].ToString()));
                            toDic.Add("arrow", dt.Rows[i]["POINTARROW"].ToString() == "1");
                        }
                    }
                    dic.Add("topoint", toDic);
                    lst.Add(dic);                                                               //给list添加一个标注
                }
                cur_lst = lst;
                double[] tmp_point = mapHelper1.GetMapCenter();
                mapHelper1.centerlat = tmp_point[0]; //30.067;//必须设置的属性,不能为空
                mapHelper1.centerlng = tmp_point[1]; //118.5784; //必须设置的属性,不能为空
                if (folds.Contains(cur_Level.ToString()))
                {
                    if (FLguid == "")
                        FLguid = "-1";
                    Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                    p.WaitForExit();
                    if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                        mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                    else
                        mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, lst);
                }
                else
                    mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Point screenPoint = Control.MousePosition;
            if (screenPoint.X > groupBox1.Width + 10 && screenPoint.Y > 200)
            {
                this.groupBox1.Visible = false;
            }
            else if (screenPoint.X < groupBox1.Width / 2 && screenPoint.Y > 200)
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
            Operator_GUID = "";
            select_point = false;

            if (Before_ShowMap == true)
            {
                double[] tmp_point = mapHelper1.GetMapCenter();
                mapHelper1.centerlat = tmp_point[0]; //30.067;//必须设置的属性,不能为空
                mapHelper1.centerlng = tmp_point[1]; //118.5784; //必须设置的属性,不能为空
            }
            else
                Before_ShowMap = true;
            foreach (RadioButton rdobt in panel2.Controls)
            {
                if (rdobt.Checked == true)
                {
                    FLguid = rdobt.Tag.ToString();
                    break;
                }
            }

            string extra_sql1 = "and ICONGUID in (select ICONGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and FLGUID = '" + FLguid + "' and UNITID = '" + UnitID + "')";
            
            
            if (panel2.Controls.Count <= 0)
            {
                FLguid = "";
                extra_sql1 = "";
            }

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

            // 获取显示图符列表
            /*string extra_sql = "";
            string sql = "select ICONGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and FLGUID = '" + FLguid + "' and UNITID = '" + UnitID + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                extra_sql = " and (ICONGUID = '" + dt.Rows[0]["ICONGUID"].ToString() + "'";
                for (int i = 1; i < dt.Rows.Count; ++i)
                    extra_sql += " or ICONGUID = '" + dt.Rows[i]["ICONGUID"].ToString() + "'";
                extra_sql += ")";
            }*/

            
            string sql = "select * from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 ";
            string extra_sq12 = "and ICONGUID in (select ICONGUID from [;database=" + WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb" + "].ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and UNITEID = '" + UnitID + "')";
            DataTable dt = ahp.ExecuteDataTable(sql + extra_sql1 + extra_sq12, null);
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
                Dictionary<string, object> toDic = new Dictionary<string, object>();
                if (dt.Rows[i]["POINTLINE"].ToString() != "0")
                {
                    sql = "select LINETYPE, LINEWIDTH, LINECOLOR, LINEOPACITY from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + dt.Rows[i]["PGUID"].ToString() + "'";
                    DataTable dt1 = ahp.ExecuteDataTable(sql, null);
                    if (dt1.Rows.Count > 0)
                    {
                        toDic.Add("lat", double.Parse(dt.Rows[i]["POINTLAT"].ToString()));
                        toDic.Add("lng", double.Parse(dt.Rows[i]["POINTLNG"].ToString()));
                        toDic.Add("type", dt1.Rows[0]["LINETYPE"].ToString());
                        toDic.Add("width", int.Parse(dt1.Rows[0]["LINEWIDTH"].ToString()));
                        toDic.Add("color", dt1.Rows[0]["LINECOLOR"].ToString());
                        toDic.Add("opacity", double.Parse(dt1.Rows[0]["LINEOPACITY"].ToString()));
                        toDic.Add("arrow", dt.Rows[i]["POINTARROW"].ToString() == "1");
                    }
                }
                dic.Add("topoint", toDic);
                lst.Add(dic);                                                               //给list添加一个标注
            }
            cur_lst = lst;
            
            if (folds.Contains(cur_Level.ToString()))
            {
                if (FLguid == "")
                    FLguid = "-1";
                Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                p.WaitForExit();
                if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                    mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                else
                    mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, lst);
            }
            else
                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
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
                    mapHelper1.addMarker("" + lat, "" + lng, name, true, Icon_GUID, null);      //在up事件中添加新标注
                }
                Icon_GUID = "";//添加完成后把选择的图标guid清空
            }
            if (Icon_GUID.Equals("") && select_point == true)
            {
                
                BorderForm bdfm = new BorderForm();
                bdfm.IsPoint = true;
                bdfm.IsLine = false;
                bdfm.borData = Load_Line(Operator_GUID);
                bdfm.borData.lat = lat;
                bdfm.borData.lng = lng;
                if (bdfm.ShowDialog() == DialogResult.OK)
                {
                    if (handle == 2)
                        mapHelper1.deleteMarker(Operator_GUID + "_line");
                    borData = bdfm.borData;
                    Dictionary<string, object> dic = new Dictionary<string, object>();//添加每个标注
                    dic.Add("lat", borData.lat);
                    dic.Add("lng", borData.lng);
                    dic.Add("type", borData.Type);
                    dic.Add("width", borData.Width);
                    dic.Add("color", borData.Color);
                    dic.Add("opacity", borData.Opacity);
                    dic.Add("arrow", false);
                    mapHelper1.DrawPointLine(Operator_GUID, i_lat, i_lng, dic);
                    Save_Line(borData, Operator_GUID, lat, lng, true);
                    for (int i = 0; i < cur_lst.Count; ++i)
                    {
                        if (cur_lst[i]["guid"].ToString() == Operator_GUID)
                        {
                            cur_lst[i]["topoint"] = dic;
                            break;
                        }
                    }
                }
                select_point = false;
            }
        }

        private void mapHelper1_MouseDown(object sender, MouseEventArgs e)
        {
            //
        }

        private void mapHelper1_MarkerDragEnd(string markerguid, bool canedit, double lat, double lng)
        {
            //  MessageBox.Show("移动：" + markerguid);
            //  数据库 update 坐标
            for (int i = 0; i < cur_lst.Count; ++i)
            {
                if (cur_lst[i]["guid"].ToString() == markerguid)
                {
                    cur_lst[i]["lat"] = lat;
                    cur_lst[i]["lng"] = lng;
                    break;
                }
            }
            ahp = new AccessHelper(AccessPath);
            string sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', MARKELAT = '" + lat.ToString() + "', MARKELNG = '" + lng.ToString() + "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);
        }

        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
           // contextMenuStrip1.Show(x, y);
        }

        private string Operator_GUID = "";
        private int handle;
        private double i_lat, i_lng;
        private void mapHelper1_MarkerRightClick(int sx, int sy, double lat, double lng, string level, string sguid, string name, bool canedit, string message)
        {
            int delta = 120;
            if (Permission)
                delta = 120;
            else
                delta = 80;
            i_lat = lat;
            i_lng = lng;
            for (int i = 0; i < cur_lst.Count; ++i)
            {
                if (cur_lst[i]["guid"].ToString() == sguid)
                {
                    Dictionary<string, object> tmp_todic = (Dictionary<string, object>)cur_lst[i]["topoint"];
                    if (tmp_todic == null || tmp_todic.Count == 0)
                    {
                        添加指向位置ToolStripMenuItem.Enabled = true;
                        删除指向位置ToolStripMenuItem.Enabled = false;
                        修改指向位置ToolStripMenuItem.Enabled = false;
                        修改箭头样式ToolStripMenuItem.Enabled = false;
                    }
                    else
                    {
                        添加指向位置ToolStripMenuItem.Enabled = false;
                        删除指向位置ToolStripMenuItem.Enabled = true;
                        修改指向位置ToolStripMenuItem.Enabled = true;
                        修改箭头样式ToolStripMenuItem.Enabled = true;
                    }
                    break;
                }
            }
            contextMenuStrip1.Show(sx + groupBox3.Left, sy + groupBox3.Top + delta);
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
                if (Permission)
                    dtf.CanEdit = true;
                else
                    dtf.CanEdit = false;

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
            if (!Permission)
            {
                MessageBox.Show("您没有删除权限!");
                return;
            }
            if (Operator_GUID != "")
            {
                mapHelper1.deleteMarker(Operator_GUID);
                Operator_GUID = "";
            }
        }

        private void 添加指向位置ToolStripMenuItem_Click(object sender, EventArgs e) // 添加&修改
        {
            if (Operator_GUID == "")
                return;
            // 显示添加
            select_point = true;
            handle = 1;
        }

        private void 修改指向位置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mapHelper1.deleteMarker(Operator_GUID + "_line");
            select_point = true;
            handle = 2;
        }

        private void 删除指向位置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Operator_GUID != "")
            {
                mapHelper1.deleteMarker(Operator_GUID + "_line");
                Operator_GUID = "";
            }
        }

        private void 修改箭头样式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BorderForm bdfm = new BorderForm();
            bdfm.IsPoint = true;
            bdfm.IsLine = true;
            bdfm.borData = Load_Line(Operator_GUID);
            ahp = new AccessHelper (WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            string sql = "select POINTLAT, POINTLNG from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + Operator_GUID + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0) 
            {
                bdfm.borData.lat = double.Parse(dt.Rows[0]["POINTLAT"].ToString());
                bdfm.borData.lng = double.Parse(dt.Rows[0]["POINTLNG"].ToString());
            }
            else
            {
                Operator_GUID = "";
                return;
            }
            
            if (bdfm.ShowDialog() == DialogResult.OK)
            {
                mapHelper1.deleteMarker(Operator_GUID + "_line");
                borData = bdfm.borData;
                Dictionary<string, object> dic = new Dictionary<string, object>();//添加每个标注
                dic.Add("lat", borData.lat);
                dic.Add("lng", borData.lng);
                dic.Add("type", borData.Type);
                dic.Add("width", borData.Width);
                dic.Add("color", borData.Color);
                dic.Add("opacity", borData.Opacity);
                dic.Add("arrow", false);
                mapHelper1.DrawPointLine(Operator_GUID, i_lat, i_lng, dic);
                Save_Line(borData, Operator_GUID, borData.lat, borData.lng, true);
                for (int i = 0; i < cur_lst.Count; ++i)
                {
                    if (cur_lst[i]["guid"].ToString() == Operator_GUID)
                    {
                        cur_lst[i]["topoint"] = dic;
                        break;
                    }
                }
            }
        }

        private void mapHelper1_AddMarkerFinished(string markerguid, double lat, double lng, string name, bool canEdit, string iconpath, string message)
        {
            // 添加完成事件，调用addMarker后触发
            // 数据库  insert

            Dictionary<string, object> dic = new Dictionary<string, object>();//添加每个标注
            dic.Add("guid", markerguid);                    //必须加载的标准属性，从数据库查询得到值
            dic.Add("name", name);                          //必须加载的标准属性，从数据库查询得到值
            dic.Add("level", cur_Level.ToString());         //必须加载的标准属性，从数据库查询得到值
            dic.Add("canedit", canEdit);                    //必须加载的标准属性，根据上层单位判断
            dic.Add("type", "标注");                        //必须加载的标准属性，从数据库查询得到值
            dic.Add("lat", lat.ToString());                 //必须加载的标准属性，从数据库查询得到值
            dic.Add("lng", lng.ToString());                 //必须加载的标准属性，从数据库查询得到值
            dic.Add("iconpath", iconpath);                  //必须加载的标准属性
            dic.Add("message", /*sdic*/null);
            dic.Add("topoint", null);
            cur_lst.Add(dic);

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
            
            if (markerguid.IndexOf("_arrow") > 0)
            {
 
            }
            else if (markerguid.IndexOf("_line") > 0)
            {
                string pguid = markerguid.Substring(0, 32);
                for (int i = 0; i < cur_lst.Count; ++i)
                {
                    if (cur_lst[i]["guid"].ToString() == pguid)
                    {
                        cur_lst[i]["topoint"] = null;
                        break;
                    }
                }
                ahp = new AccessHelper(AccessPath);
                string sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', POINTLNG = '', POINTLAT = '', POINTLINE = 0, POINTARROW = 0 where ISDELETE = 0 and PGUID = '" + pguid + "'";
                ahp.ExecuteSql(sql, null);

                sql = "update ENVIRLINE_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ISDELETE = 0 and PGUID = '" + pguid + "'";
                ahp.ExecuteSql(sql, null);
            }
            else
            {
                for (int i = 0; i < cur_lst.Count; ++i)
                {
                    if (cur_lst[i]["guid"].ToString() == markerguid)
                    {
                        cur_lst.RemoveAt(i);
                        break;
                    }
                }
                ahp = new AccessHelper(AccessPath);
                string sql = "update ENVIRICONDATA_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
                ahp.ExecuteSql(sql, null);

                string icon = GUID_Icon[markerguid];
                string table_name = Icon_JDCode[icon];
                sql = "update " + table_name + " set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ISDELEtE = 0 and PGUID = '" + markerguid + "'";
                ahp.ExecuteSql(sql, null);
            }
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
                sql += ", " + item.Key + " = '" + item.Value + "'";
            sql += " where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);
        }
        
        private void mapHelper1_PointerDone(string mkguid)
        {

        }

        private void mapHelper1_MapDblClick(string button, bool canedit, double lat, double lng, int x, int y, string markerguid)
        {
            Operator_GUID = "";
            select_point = false;
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
            double[] tmp_point = mapHelper1.GetMapCenter();
            mapHelper1.centerlat = tmp_point[0]; //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = tmp_point[1]; //118.5784; //必须设置的属性,不能为空
            if (radioButton1.Checked && button == "left")
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

                            if (folds.Contains(cur_Level.ToString()))
                            {
                                if (FLguid == "")
                                    FLguid = "-1";
                                Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                                p.WaitForExit();
                                if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                                    mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                                else
                                    mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
                            }
                            else
                                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
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
            else if (button == "right" || (radioButton2.Checked && button == "left"))
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

                            if (folds.Contains(cur_Level.ToString()))
                            {
                                if (FLguid == "")
                                    FLguid = "-1";
                                Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                                p.WaitForExit();
                                if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                                    mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                                else
                                    mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
                            }
                            else
                                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
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

        private void mapHelper1_MapTypeChanged(string mapType)
        {
            map_type = mapType;
        }

        private void mapHelper1_MapMouseWheel(string direction)
        {
            Operator_GUID = "";
            select_point = false;
            TreeNode pNode = treeView1.SelectedNode;
            string levelguid = pNode.Tag.ToString();
            int len_map = folds.Length;
            int len_obj = OBJ_PGUID.Length;
            int now_map = 0, now_obj = 0;
            for (int i = 0; i < len_map; ++i)
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
            double[] tmp_point = mapHelper1.GetMapCenter();
            mapHelper1.centerlat = tmp_point[0]; //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = tmp_point[1]; //118.5784; //必须设置的属性,不能为空
            if (direction == "up")
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

                            if (folds.Contains(cur_Level.ToString()))
                            {
                                if (FLguid == "")
                                    FLguid = "-1";
                                Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                                p.WaitForExit();
                                if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                                    mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                                else
                                    mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
                            }
                            else
                                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
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
                    {
                        if (maps[j] == folds[now_map])
                        {
                            cur_Level = int.Parse(maps[j]);
                            for (int k = 0; k < cur_lst.Count; ++k)
                            {
                                cur_lst[k]["level"] = cur_Level.ToString();
                            }

                            if (folds.Contains(cur_Level.ToString()))
                            {
                                if (FLguid == "")
                                    FLguid = "-1";
                                Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                                p.WaitForExit();
                                if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                                    mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                                else
                                    mapHelper1.ShowMap(cur_Level, OBJ_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
                            }
                            else
                                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
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
        }

        private void mapHelper1_MarkerDragBegin(string markerguid, bool candrag)
        {
            Operator_GUID = "";
            select_point = false;
            if (!Permission)
                candrag = false;
            else
                candrag = true;
        }

        private void Save_Line(LineData bdData, string markerguid)
        {
            ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            string sql = "select PGUID from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ENVIRLINE_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LINETYPE = '"
                    + bdData.Type + "', LINEWIDTH = " + bdData.Width + ", LINECOLOR = '" + bdData.Color + "', LINEOPACITY = '" 
                    + bdData.Opacity.ToString() + "' where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
                ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ENVIRLINE_H0001Z000E00 (PGUID, S_UDTIME, UPGUID, LINETYPE, LINEWIDTH, LINECOLOR, LINEOPACITY) values ('"
                    + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + markerguid + "', '"
                    + bdData.Type + "', " + bdData.Width + ", '" + bdData.Color + "', '" + bdData.Opacity.ToString() + "')";
                ahp.ExecuteSql(sql, null);
            }
        }

        private void Save_Line(LineData bdData, string markerguid, double lat, double lng , bool isAdd)
        {
            ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            string sql = "select PGUID from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ENVIRLINE_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LINETYPE = '"
                    + bdData.Type + "', LINEWIDTH = " + bdData.Width + ", LINECOLOR = '" + bdData.Color + "', LINEOPACITY = '"
                    + bdData.Opacity.ToString() + "' where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
                ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ENVIRLINE_H0001Z000E00 (PGUID, S_UDTIME, UPGUID, LINETYPE, LINEWIDTH, LINECOLOR, LINEOPACITY) values ('"
                    + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + markerguid + "', '"
                    + bdData.Type + "', " + bdData.Width + ", '" + bdData.Color + "', '" + bdData.Opacity.ToString() + "')";
                ahp.ExecuteSql(sql, null);
            }
            string tmp = "0";
            if (isAdd)
                tmp = "1";
            sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', POINTLAT = '"
                + lat.ToString() + "', POINTLNG = '" + lng + "', POINTARROW = " + tmp + ", POINTLINE = " + tmp + " where ISDELETE = 0 and PGUID = '"
                + markerguid + "'";
            ahp.ExecuteSql(sql, null);
        }

        private LineData Load_Line(string markerguid)
        {
            LineData res_data = new LineData();
            ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            string sql = "select * from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                res_data.Type = dt.Rows[0]["LINETYPE"].ToString();
                res_data.Width = int.Parse(dt.Rows[0]["LINEWIDTH"].ToString());
                res_data.Color = dt.Rows[0]["LINECOLOR"].ToString();
                res_data.Opacity = double.Parse(dt.Rows[0]["LINEOPACITY"].ToString());
            }
            else
            {
                res_data.Type = "实线";
                res_data.Width = 2;
                res_data.Color = "#000000";
                res_data.Opacity = 1;
            }
            return res_data;
        }

        private void mapHelper1_MapMouseMove(double lat, double lng)
        {
            
        }

        
    }

    // 标注实体类
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

    // 边界线类
    public class LineData
    {
        public string Type { set; get; }
        public int Width { set; get; }
        public string Color { set; get; }
        public double Opacity { set; get; }
        public double lat { set; get; }
        public double lng { set; get; }
    }
}
