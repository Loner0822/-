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
using System.Threading;


namespace EnvirInfoSys
{
    /// <summary>
    /// 主窗体类
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 发布单位信息
        /// </summary>
        private string UnitID = "-1";

        /// <summary>
        /// 基础路径变量
        /// </summary>
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory; // 当前exe根目录
        private string AccessPath = AppDomain.CurrentDomain.BaseDirectory + "data\\ENVIR_H0001Z000E00.mdb";
        private string IniFilePath = AppDomain.CurrentDomain.BaseDirectory + "parameter.ini";
 
        /// <summary>
        /// 图符实例变量(标注)
        /// </summary>
        private string Icon_GUID = "";      // 图符GUID
        private bool select_vector = false; // 是否有添加箭头操作 
        private string Operator_GUID = "";
        private int handle;
        private double i_lat, i_lng;        // 当前标注经纬度
        private Dictionary<string, string> GUID_Icon;
        private Dictionary<string, string> FDName_Value;

        /// <summary>
        /// 图符编码/名称数据(数据库获取)
        /// </summary>
        private Dictionary<string, string> Icon_JDCode;
        private Dictionary<string, string> Icon_Name;

        /// <summary>
        /// 组织结构数据(数据库获取)
        /// </summary>
        private string[] GL_PGUID;
        private Dictionary<string, string> GL_NAME;
        private Dictionary<string, string> GL_JDCODE;
        private Dictionary<string, string> GL_UPGUID;
        private Dictionary<string, string> GL_MAP;
        private Dictionary<string, string> GL_NAME_PGUID;

        /// <summary>
        /// 地图数据
        /// </summary>
        private int cur_Level;
        private string levelguid = "";     // 当前组织结构guid
        private string map_type = "g_map";
        private bool Permission = false;
        private bool Before_ShowMap = false;
        private string[] folds = null;
        private List<Dictionary<string, object>> cur_lst;

        // 边界线
        Dictionary<string, object> borderDic = null;
        private List<double[]> borList = null;
        private LineData borData = null;
        private LineData lineData = null;

        /// <summary>
        /// 管辖分类
        /// </summary>
        private string GXguid = "-1";
        private string FLguid = "-1";
        //private Dictionary<string, string> GX_FLNAME;
        //private Dictionary<string, string> GX_UPGUID;
        //private Dictionary<string, List<string>> GX_ICON;
        
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

        private void 下载单位注册数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "OrgDataDown.exe");
            p.WaitForExit();
            treeView1.Nodes.Clear();
            Load_Unit_Level();
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        private void IP设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "服务器IP设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        MessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            Process p = Process.Start(WorkPath + "SetIP.exe");
            p.WaitForExit();
        }

        private void 边界线属性设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "边界线属性设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        MessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            BorderForm bdfm = new BorderForm();
            bdfm.IsPoint = false;
            bdfm.IsLine = false;
            bdfm.borData.Load_Line("边界线");
            if (bdfm.borData.line_data == null)
                bdfm.borData.line_data = borData;
            
            if (bdfm.ShowDialog() == DialogResult.OK)
            {
                borData = bdfm.borData.line_data;
                if (levelguid != string.Empty)
                {
                    borderDic["type"] = borData.Type;
                    borderDic["width"] = borData.Width;
                    borderDic["color"] = borData.Color;
                    borderDic["opacity"] = borData.Opacity;
                    borData.Save_Line("边界线");
                    mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
                }
            }
        }

        private void 管辖分类设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "管辖分类设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        MessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            Classify_1Form clcfm = new Classify_1Form();
            clcfm.unitid = UnitID;
            clcfm.gxguid = GXguid;
            clcfm.ShowDialog();
            ToolStripMenuItem Fa_TSMI = new ToolStripMenuItem();
            foreach (ToolStripMenuItem tsmi in menuStrip1.Items)
                if (tsmi.Tag.ToString() == GXguid)
                {
                    Fa_TSMI = tsmi;
                    break;
                }
            Fa_TSMI.DropDownItems.Clear();
            Load_Guan_Xia();
        }

        private void 图符对应设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "图符对应设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        MessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            Classify_2Form clcfm = new Classify_2Form();
            clcfm.unitid = UnitID;
            clcfm.ShowDialog();
            TreeNode pNode = treeView1.SelectedNode;
            treeView1.SelectedNode = null;
            treeView1.SelectedNode = pNode;
        }

        private void 图符管理设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "图符扩展设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        MessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            Process p = Process.Start(WorkPath + "tfkzdy.exe");
            p.WaitForExit();
        }

        private void 密码管理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckPwForm ckpwf = new CheckPwForm();
            ckpwf.unitid = UnitID;
            if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("未能获取管理员权限");
                return;
            }
            PasswordForm psfm = new PasswordForm();
            psfm.ShowDialog();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show("是否将本次数据上传服务器?", "提示", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Asterisk);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                Process p = Process.Start(WorkPath + "DataUP.exe", "EnvirInfoSys.exe 0");
                p.WaitForExit();
            }
            else if (dr == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            FileReader.often_ahp.CloseConn();
            FileReader.line_ahp.CloseConn();
            System.Environment.Exit(0);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            borData = new LineData();
            lineData = new LineData();
            borData.Get_NewLine();
            lineData.Get_NewLine();

            // 加载主界面
            FileReader.inip = new IniOperator(WorkPath + "RegInfo.ini");
            string UnitName = FileReader.inip.ReadString("Public", "UnitName", "");
            UnitName = UnitName.Replace("\0", "");
            string AppName = FileReader.inip.ReadString("Public", "AppName", "");
            AppName = AppName.Replace("\0", "");
            string VerNum = FileReader.inip.ReadString("版本号", "VerNum", "");
            this.Text = UnitName + AppName + VerNum;
            FileReader.often_ahp = new AccessHelper(AccessPath);
            FileReader.line_ahp = new AccessHelper(WorkPath + "data\\经纬度注册.mdb");

            // 读取单位数据
            FileReader.inip = new IniOperator(WorkPath + "RegInfo.ini");
            UnitID = FileReader.inip.ReadString("Public", "UnitID", "-1");

            // 加载组织结构
            folds = Get_Map_List();
            Load_Unit_Level();

            // 读入图标对应数据
            Icon_JDCode = new Dictionary<string, string>();
            Icon_Name = new Dictionary<string, string>();
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            string sql = "select PGUID, JDNAME, JDCODE from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            DataTable dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                Icon_Name.Add(pguid + ".png", dt.Rows[i]["JDNAME"].ToString());
                Icon_JDCode.Add(pguid, dt.Rows[i]["JDCODE"].ToString());
            }
            FileReader.once_ahp.CloseConn();

            // 地图初始化
            FileReader.inip = new IniOperator(IniFilePath);
            string slat = FileReader.inip.ReadString("mapproperties", "centerlat", "");
            string slng = FileReader.inip.ReadString("mapproperties", "centerlng", "");
            mapHelper1.centerlat = double.Parse(slat); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(slng); //118.5784; //必须设置的属性,不能为空
            mapHelper1.webpath = WorkPath + "googlemap"; //必须设置的属性,不能为空
            mapHelper1.roadmappath = WorkPath + "googlemap\\map"; //必须设置的属性,不能为空
            mapHelper1.satellitemappath = WorkPath + "googlemap\\satellite"; //必须设置的属性,不能为空
            mapHelper1.iconspath = WorkPath + "PNGICONFOLDER"; //必须设置的属性,不能为空
            mapHelper1.maparr = folds;

            // 边界线导入
            
            Load_Border(UnitID);

            // 加载管辖类型
            sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '-1' order by SHOWINDEX";
            dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                ToolStripMenuItem new_TSMI = new ToolStripMenuItem();
                new_TSMI.Text = dt.Rows[i]["FLNAME"].ToString();
                new_TSMI.Tag = dt.Rows[i]["PGUID"].ToString();
                GXguid = dt.Rows[i]["PGUID"].ToString();
                new_TSMI.Click += MenuStripItem_CheckedChanged;
                menuStrip1.Items.Insert(i, new_TSMI);
                Load_Guan_Xia();
            }

            // 地图设置
            radioButton1.Checked = true;
            groupBox1.Visible = false;
            groupBox2.Visible = false;

            // 加载管理员权限
            AccessHelper ahp = new AccessHelper(WorkPath + "data\\PASSWORD_H0001Z000E00.mdb");
            sql = "select AUTHORITY from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWNAME = '管理员密码' and UNITID = '" + UnitID + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            string author_list = "";
            if (dt.Rows.Count > 0)
                author_list = dt.Rows[0]["AUTHORITY"].ToString();
            FileReader.Authority = author_list.Split(';');
            ahp.CloseConn();

            // 显示登陆界面
            LoginForm lgf = new LoginForm();
            lgf.Text += " " + this.Text;
            lgf.unitid = UnitID;
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
                    边界线属性设置ToolStripMenuItem.Visible = true;
                    管辖分类设置ToolStripMenuItem.Visible = true;
                    修改箭头样式ToolStripMenuItem.Visible = true;

                    设置当前点为中心点ToolStripMenuItem.Visible = true;
                    导入当前单位边界线ToolStripMenuItem.Visible = true;
                    图符对应设置ToolStripMenuItem.Visible = true;
                    图符管理设置ToolStripMenuItem.Visible = true;
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
                    边界线属性设置ToolStripMenuItem.Visible = false;
                    管辖分类设置ToolStripMenuItem.Visible = false;
                    修改箭头样式ToolStripMenuItem.Visible = false;

                    设置当前点为中心点ToolStripMenuItem.Visible = false;
                    导入当前单位边界线ToolStripMenuItem.Visible = false;
                    图符对应设置ToolStripMenuItem.Visible = false;
                    图符管理设置ToolStripMenuItem.Visible = false;
                }
            }
            else
            {
                MessageBox.Show("即将退出界面");
                System.Environment.Exit(0);
            }

            if (treeView1.Nodes.Count > 0)
                treeView1.SelectedNode = treeView1.Nodes[0];
            if (menuStrip1.Items[0].Tag != null) 
            {
                ToolStripMenuItem now_TSMI = (ToolStripMenuItem)menuStrip1.Items[0];
                GXguid = now_TSMI.Tag.ToString();
                now_TSMI.BackColor = SystemColors.ActiveCaption;
            }
        }

        private string[] Get_Map_List() 
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

        private void MenuStripItem_CheckedChanged(object sender, EventArgs e)
        {
            Operator_GUID = "";
            select_vector = false;
            FLguid = "-1";
            ToolStripMenuItem tmp = (ToolStripMenuItem)sender;
            if (GXguid == tmp.Tag.ToString())
                return;
            
            foreach (ToolStripMenuItem it in menuStrip1.Items)
                if (it.BackColor == SystemColors.ActiveCaption)
                {
                    it.BackColor = SystemColors.Control;
                    string tmp_text = it.Text;
                    int index = tmp_text.IndexOf(' ');
                    if (index > 0)
                        tmp_text = tmp_text.Substring(0, index);
                    it.Text = tmp_text;
                    foreach (ToolStripMenuItem tsmi in it.DropDownItems)
                    {
                        tsmi.Checked = false;
                        tsmi.BackColor = SystemColors.Control;
                    }
                }
            tmp.BackColor = SystemColors.ActiveCaption;
            GXguid = tmp.Tag.ToString();
            tmp.DropDownItems.Clear();
            Load_Guan_Xia();
        }

        private void ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Operator_GUID = "";
            select_vector = false;
            ToolStripMenuItem tmp = (ToolStripMenuItem)sender;
            ToolStripMenuItem Fa_TSMI = new ToolStripMenuItem();
            foreach (ToolStripMenuItem tsmi in menuStrip1.Items)
                if (tsmi.Tag != null && tsmi.Tag.ToString() == GXguid)
                {
                    Fa_TSMI = tsmi;
                    break;
                }
            foreach (ToolStripMenuItem tsmi in Fa_TSMI.DropDownItems)
                if (tsmi.Checked == true)
                {
                    tsmi.Checked = false;
                    tsmi.BackColor = SystemColors.Control;
                }
            tmp.Checked = true;
            tmp.BackColor = SystemColors.ActiveCaption;
                     
            string tmp_text = Fa_TSMI.Text;
            int index = tmp_text.IndexOf(' ');
            if (index > 0)
                tmp_text = tmp_text.Substring(0, index);
            Fa_TSMI.Text = tmp_text + " - (" + tmp.Text + ")";

            if (levelguid == string.Empty)
            {
                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                return;
            }
            FLguid = tmp.Tag.ToString();
            string extra_sql1 = "and ICONGUID in (select ICONGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and FLGUID = '"
                + FLguid + "' and UNITID = '" + UnitID + "')";
            string extra_sq12 = "and ICONGUID in (select ICONGUID from [;database=" + WorkPath
                + "data\\ENVIRDYDATA_H0001Z000E00.mdb" + "].ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '"
                + levelguid + "' and UNITEID = '" + UnitID + "')";
            Get_Marker_From_Access(extra_sql1 + extra_sq12);
        }

        private void Load_Unit_Level()
        {
            GL_NAME = new Dictionary<string, string>();
            GL_JDCODE = new Dictionary<string, string>();
            GL_UPGUID = new Dictionary<string, string>();
            GL_MAP = new Dictionary<string, string>();
            GL_NAME_PGUID = new Dictionary<string, string>();

            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K01.mdb");
            string sql = "select PGUID, JDNAME, JDCODE, UPGUID from ZSK_OBJECT_H0001Z000K01 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            DataTable dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            GL_PGUID = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                GL_PGUID[i] = pguid;
                GL_NAME[pguid] = dt.Rows[i]["JDNAME"].ToString();
                GL_JDCODE[pguid] = dt.Rows[i]["JDCODE"].ToString();
                GL_UPGUID[pguid] = dt.Rows[i]["UPGUID"].ToString();
                GL_NAME_PGUID[dt.Rows[i]["JDNAME"].ToString()] = pguid;
            }
            FileReader.once_ahp.CloseConn();

            treeView1.Nodes.Clear();
            treeView1.HideSelection = false;
            treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);

            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb");
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + pguid + "'";
                DataTable dt1 = FileReader.once_ahp.ExecuteDataTable(sql, null);
                if (dt1.Rows.Count > 0)
                    GL_MAP.Add(pguid, dt1.Rows[0]["MAPLEVEL"].ToString());
                else
                    GL_MAP.Add(pguid, string.Empty);
            }
            FileReader.once_ahp.CloseConn();

            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\PersonMange.mdb");
            sql = "select PGUID, ORGNAME, ULEVEL from RG_单位注册 where ISDELETE = 0 and PGUID = '" + UnitID + "'";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                TreeNode pNode = new TreeNode();
                pNode.Name = UnitID;
                pNode.Text = dt.Rows[0]["ORGNAME"].ToString();
                pNode.Tag = GL_NAME_PGUID[dt.Rows[0]["ULEVEL"].ToString()];
                treeView1.Nodes.Add(pNode);
                Add_Unit_Node(pNode);
                treeView1.ExpandAll();
            }
            FileReader.once_ahp.CloseConn();
        }

        private void Load_Guan_Xia()
        {
            ToolStripMenuItem Fa_TSMI = new ToolStripMenuItem();
            foreach (ToolStripMenuItem tsmi in menuStrip1.Items)
                if (tsmi.Tag != null && tsmi.Tag.ToString() == GXguid)
                {
                    Fa_TSMI = tsmi;
                    break;
                }
            string sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + GXguid +  "' order by SHOWINDEX";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                ToolStripMenuItem new_TSMI = new ToolStripMenuItem();
                new_TSMI.Text = dt.Rows[i]["FLNAME"].ToString();
                new_TSMI.Tag = dt.Rows[i]["PGUID"].ToString();
                new_TSMI.Click += ToolStripMenuItem_CheckedChanged;
                new_TSMI.CheckOnClick = true;
                Fa_TSMI.DropDownItems.Add(new_TSMI);
            }

            if (levelguid != string.Empty)
            {
                if (FLguid == "")
                    FLguid = "-1";
                Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                p.WaitForExit();
                if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                    mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                else
                    mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
            }
            FLguid = "-1";
        }

        private void Get_Marker_From_Access(string extra_sql)
        {
            GUID_Icon = new Dictionary<string, string>();
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();//标注list，从数据库获取
            string sql = "select * from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 ";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql + extra_sql, null);
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
                    sql = "select LINETYPE, LINEWIDTH, LINECOLOR, LINEOPACITY from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '"
                        + dt.Rows[i]["PGUID"].ToString() + "'";
                    DataTable dt1 = FileReader.often_ahp.ExecuteDataTable(sql, null);
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
                lst.Add(dic);                       //给list添加一个标注
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
                    mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, lst);
            }
            else
                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Point screenPoint = Control.MousePosition;
            if (screenPoint.X > groupBox1.Width + 50 && screenPoint.Y > 200)
            {
                this.groupBox1.Visible = false;
            }
            else if (screenPoint.X < groupBox1.Width / 2 && screenPoint.Y > 200)
            {
                this.groupBox1.Visible = true;
            }

            if (screenPoint.X > this.Width - groupBox2.Width && screenPoint.Y > 250)
            {
                this.groupBox2.Visible = true;
            }
            else if (screenPoint.X < this.Width - groupBox2.Width && screenPoint.Y > 250)
            {
                this.groupBox2.Visible = false;
            }
        }

        private void Add_Unit_Node(TreeNode pa)
        {
            string sql = "select PGUID, ORGNAME, ULEVEL from RG_单位注册 where ISDELETE = 0 and UPPGUID = '" + pa.Name + "'";
            DataTable dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                TreeNode pNode = new TreeNode();
                pNode.Name = dt.Rows[i]["PGUID"].ToString();
                pNode.Text = dt.Rows[i]["ORGNAME"].ToString();
                pNode.Tag = GL_NAME_PGUID[dt.Rows[i]["ULEVEL"].ToString()];
                pa.Nodes.Add(pNode);
                Add_Unit_Node(pNode);
            }
        }

        private void Load_Border(string u_guid)
        {
            borList = new List<double[]>();
            borderDic = new Dictionary<string, object>();
            LineData new_borData = new LineData();
            new_borData.Load_Line("边界线");
            if (new_borData.Type != null)
                borData = new_borData;
            borderDic.Add("type", borData.Type);
            borderDic.Add("width", borData.Width);
            borderDic.Add("color", borData.Color);
            borderDic.Add("opacity", borData.Opacity);
            string sql = "select LNG_LAT from BORDERDATA where ISDELETE = 0 and UNITID = '" + u_guid + "'";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                string alldata = dt.Rows[0]["LNG_LAT"].ToString();
                string[] div_data = alldata.Split(';');
                foreach (string str in div_data)
                {
                    if (str != "")
                    {
                        string[] div_str = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n' });
                        borList.Add(new double[] { double.Parse(div_str[1]), double.Parse(div_str[0])});
                    }
                }
                borderDic.Add("path", borList);
            }
            else
                borderDic = null;
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色，只需要在TreeView失去焦点时选中节点仍然突显  
            return;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Operator_GUID = "";
            select_vector = false;

            TreeNode pNode = treeView1.SelectedNode;
            levelguid = pNode.Tag.ToString();
            Load_Border(pNode.Name);

            bool flag = false;

            string sql = "select MARKELAT, MARKELNG from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and MAKRENAME like '%" + pNode.Text + "%'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                mapHelper1.centerlat = double.Parse(dt.Rows[0]["MARKELAT"].ToString());
                mapHelper1.centerlng = double.Parse(dt.Rows[0]["MARKELNG"].ToString());
                flag = true;
            }

            sql = "select LAT, LNG from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '" + pNode.Name + "'";
            dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                mapHelper1.centerlat = double.Parse(dt.Rows[0]["LAT"].ToString());
                mapHelper1.centerlng = double.Parse(dt.Rows[0]["LNG"].ToString());
                flag = true;
            }
            else if (flag != true)
            {
                if (Before_ShowMap == true)
                {
                    double[] tmp_point = mapHelper1.GetMapCenter();
                    mapHelper1.centerlat = tmp_point[0]; //30.067;//必须设置的属性,不能为空
                    mapHelper1.centerlng = tmp_point[1]; //118.5784; //必须设置的属性,不能为空
                }
            }
            Before_ShowMap = true;

            if (flag == false)
            {
                MessageBox.Show("无对应中心点经纬度数据，无法定位到" + pNode.Text + "，请在地图上相应位置右键进行设置");
            }

            ToolStripMenuItem Fa_TSMI = new ToolStripMenuItem();
            foreach (ToolStripMenuItem tsmi in menuStrip1.Items)
                if (tsmi.Tag != null && tsmi.Tag.ToString() == GXguid)
                {
                    Fa_TSMI = tsmi;
                    break;
                }
            foreach (ToolStripMenuItem tsmi in Fa_TSMI.DropDownItems)
            {
                if (tsmi.Checked == true)
                {
                    FLguid = tsmi.Tag.ToString();
                    break;
                }
            }
            string extra_sql1 = "and ICONGUID in (select ICONGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and FLGUID = '"
                + FLguid + "' and UNITID = '" + UnitID + "')";
            string extra_sq12 = "and ICONGUID in (select ICONGUID from [;database=" + WorkPath
                + "data\\ENVIRDYDATA_H0001Z000E00.mdb" + "].ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '"
                + levelguid + "' and UNITEID = '" + UnitID + "')";
            if (FLguid == "-1")
            {
                FLguid = "";
                extra_sql1 = "";
            }
            
            // 处理cur_level
            flag = false;
            string[] maps = GL_MAP[levelguid].Split(',');
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
            Get_Marker_From_Access(extra_sql1 + extra_sq12);
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
            if (Icon_GUID.Equals("") && select_vector == true)
            {
                BorderForm bdfm = new BorderForm();
                bdfm.IsPoint = true;
                bdfm.IsLine = false;
                bdfm.borData.Load_Line(Operator_GUID);
                if (bdfm.borData.line_data == null)
                    bdfm.borData.line_data = lineData;
                bdfm.borData.lat = lat;
                bdfm.borData.lng = lng;
                if (bdfm.ShowDialog() == DialogResult.OK)
                {
                    lineData = bdfm.borData.line_data;
                    if (handle == 2)
                        mapHelper1.deleteMarker(Operator_GUID + "_line");
                    Dictionary<string, object> dic = bdfm.borData.ToDic();//添加每个标注
                    mapHelper1.DrawPointLine(Operator_GUID, i_lat, i_lng, dic);
                    bdfm.borData.Save_Line(Operator_GUID, lat, lng, true);
                    for (int i = 0; i < cur_lst.Count; ++i)
                    {
                        if (cur_lst[i]["guid"].ToString() == Operator_GUID)
                        {
                            cur_lst[i]["topoint"] = dic;
                            break;
                        }
                    }
                }
                select_vector = false;
            }
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
            string sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', MARKELAT = '" + lat.ToString() + "', MARKELNG = '" + lng.ToString() + "' where ISDELETE = 0 and PGUID = '"
                + markerguid + "'";
            FileReader.often_ahp.ExecuteSql(sql, null);
        }

        private double center_lat;
        private double center_lng;
        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
            center_lat = lat;
            center_lng = lng;
            contextMenuStrip2.Show(MousePosition.X, MousePosition.Y);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                TreeNode pNode = treeView1.GetNodeAt(e.X, e.Y);
                if (pNode != null)
                {
                    treeView1.SelectedNode = pNode;
                    contextMenuStrip3.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void 设置当前点为中心点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            DialogResult dr = MessageBox.Show("是否设置当前位置为" + pNode.Text + "中心点经纬度", "提示", MessageBoxButtons.OKCancel);
            if (dr != System.Windows.Forms.DialogResult.OK)
                return;
            
            string sql = "select PGUID from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '" + pNode.Name + "'";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ORGCENTERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "', LAT = '" + center_lat.ToString() + "', LNG = '" + center_lng.ToString()
                    + "' where ISDELETE = 0 and UNITEID = '" + pNode.Name + "'";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ORGCENTERDATA (PGUID, S_UDTIME, UNITEID, LAT, LNG) values('" + pNode.Name + "', '" + 
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode.Name + "', '" + center_lat.ToString() + 
                    "', '" + center_lng.ToString() + "')";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }
        }

        private void 导入当前单位边界线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string file = openFileDialog1.FileName;
            string[] strAll = File.ReadAllLines(file);
            string ds_lng_lat = "";
            foreach (string str in strAll)
            {
                string[] split = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n' });
                borList.Add(new double[] { double.Parse(split[0]), double.Parse(split[1]) });
                ds_lng_lat += split[0] + "," + split[1] + ";";
            }
            string sql = "select PGUID from BORDERDATA where ISDELETE = 0 and UNITID = '" + pNode.Name + "'";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update BORDERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LNG_LAT = '"
                    + ds_lng_lat + "' where ISDELETE = 0 and UNITID = '" + pNode.Name + "'";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into BORDERDATA (PGUID, S_UDTIME, UNITID, LNG_LAT) values ('" + Guid.NewGuid().ToString("B")
                    + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode.Name + "', '" + ds_lng_lat + "')";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }
            treeView1.SelectedNode = null;
            treeView1.SelectedNode = pNode;
        }

        private void mapHelper1_MarkerRightClick(int sx, int sy, double lat, double lng, string level, string sguid, string name, bool canedit, string message)
        {
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
            contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            Operator_GUID = sguid;
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Operator_GUID != "")
            {
                string iconguid = "";
                string sql = "select ICONGUID from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + Operator_GUID + "'";
                DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
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
            select_vector = true;
            handle = 1;
        }

        private void 修改指向位置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //mapHelper1.deleteMarker(Operator_GUID + "_line");
            select_vector = true;
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
            bdfm.borData.Load_Line(Operator_GUID);
            if (bdfm.borData.line_data == null)
                bdfm.borData.line_data = lineData;
            string sql = "select POINTLAT, POINTLNG from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + Operator_GUID + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
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
                lineData = bdfm.borData.line_data;
                mapHelper1.deleteMarker(Operator_GUID + "_line");
                Dictionary<string, object> dic = bdfm.borData.ToDic();//添加每个标注
                mapHelper1.DrawPointLine(Operator_GUID, i_lat, i_lng, dic);
                bdfm.borData.Save_Line(Operator_GUID, bdfm.borData.lat, bdfm.borData.lng, true);
                for (int i = 0; i < cur_lst.Count; ++i)
                {
                    if (cur_lst[i]["guid"].ToString() == Operator_GUID)
                    {
                        cur_lst[i]["topoint"] = dic;
                        break;
                    }
                }
                borData = bdfm.borData.line_data;
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

            string iconguid = Path.GetFileNameWithoutExtension(iconpath);

            string sql = "insert into ENVIRICONDATA_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, LEVELGUID, MAPLEVEL, MARKELAT, MARKELNG, MAKRENAME, UNITEID) values('" 
                         + markerguid + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + iconguid + "', '" + levelguid + "', '" 
                         + cur_Level.ToString() + "', '" + lat.ToString() + "', '" + lng.ToString() + "', '" + name + "', '" + UnitID.ToString() + "')";
            FileReader.often_ahp.ExecuteSql(sql, null);
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
            FileReader.often_ahp.ExecuteSql(sql, null);
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
                string sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + 
                    "', POINTLNG = '', POINTLAT = '', POINTLINE = 0, POINTARROW = 0 where ISDELETE = 0 and PGUID = '" + pguid + "'";
                FileReader.often_ahp.ExecuteSql(sql, null);

                sql = "update ENVIRLINE_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + 
                    "' where ISDELETE = 0 and PGUID = '" + pguid + "'";
                FileReader.often_ahp.ExecuteSql(sql, null);
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
                string sql = "update ENVIRICONDATA_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + 
                    "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
                FileReader.often_ahp.ExecuteSql(sql, null);

                string icon = GUID_Icon[markerguid];
                string table_name = Icon_JDCode[icon];
                sql = "update " + table_name + " set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "' where ISDELEtE = 0 and PGUID = '" + markerguid + "'";
                FileReader.often_ahp.ExecuteSql(sql, null);
            }
        }

        private void mapHelper1_ModifyMarkerFinished(string markerguid, double lat, double lng, string name, bool canEdit, string iconpath, string message)
        {
            // 更新完成事件，调用ModifyMarker后触发
            // 数据库  update 
            string sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', MAKRENAME = '" + name + "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            FileReader.often_ahp.ExecuteSql(sql, null);
            string icon = GUID_Icon[markerguid];
            string table_name = Icon_JDCode[icon];
            sql = "update " + table_name + " set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            foreach (var item in FDName_Value)
                sql += ", " + item.Key + " = '" + item.Value + "'";
            sql += " where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            FileReader.often_ahp.ExecuteSql(sql, null);
        }
        
        private void mapHelper1_PointerDone(string mkguid)
        {

        }

        private void Map_Size_Change(int len_gl, int now_gl, int now_map)
        {
            for (int i = 0; i < len_gl; ++i)
            {
                string objguid = GL_PGUID[(now_gl + i) % len_gl];
                string[] maps = GL_MAP[objguid].Split(',');
                for (int j = 0; j < maps.Length; ++j)
                    if (maps[j] == folds[now_map])
                    {
                        cur_Level = int.Parse(maps[j]);
                        levelguid = objguid;
                        for (int k = 0; k < cur_lst.Count; ++k)
                            cur_lst[k]["level"] = cur_Level.ToString();

                        if (folds.Contains(cur_Level.ToString()))
                        {
                            if (FLguid == "")
                                FLguid = "-1";
                            Process p = Process.Start(WorkPath + "CreatePng.exe", "0 " + cur_Level + " " + levelguid + " " + FLguid);
                            p.WaitForExit();
                            if (Directory.GetFiles(WorkPath + "PNGICONFOLDER\\b_" + cur_Level.ToString()).Length <= 0)
                                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null);
                            else
                                mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst);
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

        private void Map_Resize(bool IsEnlarge)
        {
            Operator_GUID = "";
            select_vector = false;
            int len_map = folds.Length;
            int len_gl = GL_PGUID.Length;
            int now_map = 0, now_gl = 0;
            for (int i = 0; i < len_map; ++ i)
                if (folds[i] == cur_Level.ToString()) 
                {
                    now_map = i;
                    break;
                }
            for (int i = 0; i < len_gl; ++i)
                if (GL_PGUID[i] == levelguid)
                {
                    now_gl = i;
                    break;
                }
            double[] tmp_point = mapHelper1.GetMapCenter();
            mapHelper1.centerlat = tmp_point[0]; //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = tmp_point[1]; //118.5784; //必须设置的属性,不能为空
            if (IsEnlarge == true)
            {
                ++now_map;
                if (now_map >= len_map)
                {
                    //MessageBox.Show("地图已达最大级别!");
                    return;
                }
                Map_Size_Change(len_gl, now_gl, now_map);
            }
            else
            {
                --now_map;
                if (now_map < 0)
                {
                    //MessageBox.Show("地图已达最小级别!");
                    return;
                }
                Map_Size_Change(len_gl, now_gl, now_map);
            }
        }

        private void mapHelper1_MapDblClick(string button, bool canedit, double lat, double lng, int x, int y, string markerguid)
        {
            //Thread.Sleep(100);
            if (radioButton1.Checked && button == "left")
            {
                Map_Resize(true);
            }
            else if (button == "right" || (radioButton2.Checked && button == "left"))
            {
                Map_Resize(false);
            }
        }

        private TreeNode Select_Node(string levelguid, TreeNode pNode)
        {
            if (pNode == null)
                return null;
            if (pNode.Tag.ToString() == levelguid && Check_relative(pNode))
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

        bool Check_relative(TreeNode pNode)
        {
            TreeNode now_Node = treeView1.SelectedNode;
            if (pNode == now_Node)
                return true;
            if (pNode == now_Node.Parent)
                return true;
            if (pNode.Parent == now_Node)
                return true;
            return false;
        }

        private void mapHelper1_MapTypeChanged(string mapType)
        {
            map_type = mapType;
        }

        private void mapHelper1_MapMouseWheel(string direction)
        {
            if (direction == "up")
                Map_Resize(true);
            else
                Map_Resize(false);
        }

        private void mapHelper1_MarkerDragBegin(string markerguid, bool candrag)
        {
            Operator_GUID = "";
            select_vector = false;
            if (!Permission)
                candrag = false;
            else
                candrag = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr;
            dr = MessageBox.Show("是否将本次数据上传服务器?", "提示", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Asterisk);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                Process p = Process.Start(WorkPath + "DataUP.exe", "EnvirInfoSys.exe 1");
                p.WaitForExit();
            }
            else if (dr == System.Windows.Forms.DialogResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FileReader.often_ahp.CloseConn();
            FileReader.line_ahp.CloseConn();
        }

    }

    /// <summary>
    /// 数据读入类
    /// </summary>
    public class FileReader
    {
        public static AccessHelper once_ahp = null;
        public static AccessHelper often_ahp = null;
        public static AccessHelper line_ahp = null;
        public static IniOperator inip = null;
        public static string[] Authority = null;
    }

    /// <summary>
    /// 标注实体类
    /// </summary>
    public class MarkerData
    {
        public string guid { set; get; }
        public string name { set; get; }
        public int level { set; get; }
        public bool canedit { set; get; }
        public string type { set; get; }
        public double lat { set; get; }
        public double lng { set; get; }
        public string iconpath { set; get; }
        public Dictionary<string, string> message { set; get; }
        public Dictionary<string, object> topoint { set; get; }
    }

    /// <summary>
    /// 线条类
    /// </summary>
    public class LineData
    {
        public string Type { set; get; }
        public int Width { set; get; }
        public string Color { set; get; }
        public double Opacity { set; get; }

        public void Get_NewLine()
        {
            this.Type = "实线";
            this.Width = 1;
            this.Color = "#000000";
            this.Opacity = 1;
        }

        public void Load_Line(string markerguid)
        {
            //LineData res_data = new LineData();
            string sql = "select * from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                this.Type = dt.Rows[0]["LINETYPE"].ToString();
                this.Width = int.Parse(dt.Rows[0]["LINEWIDTH"].ToString());
                this.Color = dt.Rows[0]["LINECOLOR"].ToString();
                this.Opacity = double.Parse(dt.Rows[0]["LINEOPACITY"].ToString());
            }
            else
            {
                this.Type = null;
                this.Width = 0;
                this.Color = null;
                this.Opacity = 0;
            }
        }

        public void Save_Line(string markerguid)
        {
            string sql = "select PGUID from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ENVIRLINE_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LINETYPE = '"
                    + this.Type + "', LINEWIDTH = " + this.Width + ", LINECOLOR = '" + this.Color + "', LINEOPACITY = '"
                    + this.Opacity.ToString() + "' where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
                FileReader.often_ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ENVIRLINE_H0001Z000E00 (PGUID, S_UDTIME, UPGUID, LINETYPE, LINEWIDTH, LINECOLOR, LINEOPACITY) values ('"
                    + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + markerguid + "', '"
                    + this.Type + "', " + this.Width + ", '" + this.Color + "', '" + this.Opacity.ToString() + "')";
                FileReader.often_ahp.ExecuteSql(sql, null);
            }
        }
    }

    /// <summary>
    /// 指向类 
    /// </summary>
    public class ToPointData
    {
        public double lat { set; get; }
        public double lng { set; get; }
        public LineData line_data { set; get; }
        public bool arrow { set; get; }

        public Dictionary<string, object> ToDic()
        {
            Dictionary<string, object> res_dic = new Dictionary<string, object>();//添加每个标注
            res_dic.Add("lat", this.lat);
            res_dic.Add("lng", this.lng);
            res_dic.Add("type", this.line_data.Type);
            res_dic.Add("width", this.line_data.Width);
            res_dic.Add("color", this.line_data.Color);
            res_dic.Add("opacity", this.line_data.Opacity);
            res_dic.Add("arrow", false);
            return res_dic;
        }

        public void Load_Line(string markerguid)
        {
            LineData res_data = new LineData();
            string sql = "select * from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                res_data.Type = dt.Rows[0]["LINETYPE"].ToString();
                res_data.Width = int.Parse(dt.Rows[0]["LINEWIDTH"].ToString());
                res_data.Color = dt.Rows[0]["LINECOLOR"].ToString();
                res_data.Opacity = double.Parse(dt.Rows[0]["LINEOPACITY"].ToString());
            }
            else
            {
                res_data = null;
            }
            this.line_data = res_data;
        }

        public void Save_Line(string markerguid, double lat, double lng, bool isAdd)
        {
            string sql = "select PGUID from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ENVIRLINE_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LINETYPE = '"
                    + line_data.Type + "', LINEWIDTH = " + line_data.Width + ", LINECOLOR = '" + line_data.Color + "', LINEOPACITY = '"
                    + line_data.Opacity.ToString() + "' where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
                FileReader.often_ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ENVIRLINE_H0001Z000E00 (PGUID, S_UDTIME, UPGUID, LINETYPE, LINEWIDTH, LINECOLOR, LINEOPACITY) values ('"
                    + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + markerguid + "', '"
                    + line_data.Type + "', " + line_data.Width + ", '" + line_data.Color + "', '" + line_data.Opacity.ToString() + "')";
                FileReader.often_ahp.ExecuteSql(sql, null);
            }
            string tmp = "0";
            if (isAdd)
                tmp = "1";
            sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', POINTLAT = '"
                + lat.ToString() + "', POINTLNG = '" + lng + "', POINTARROW = 0, POINTLINE = " + tmp + " where ISDELETE = 0 and PGUID = '"
                + markerguid + "'";
            FileReader.often_ahp.ExecuteSql(sql, null);
        }
    }

}
