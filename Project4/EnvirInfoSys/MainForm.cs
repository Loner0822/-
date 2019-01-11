using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.IO;
using System.Net.NetworkInformation;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;
using System.Diagnostics;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using System.Threading;
using DevExpress.XtraSplashScreen;

namespace EnvirInfoSys
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        private SplashScreenManager _loadForm;

        /// <summary>
        /// 等待窗体管理对象
        /// </summary>
        protected SplashScreenManager LoadForm
        {
            get
            {
                if (_loadForm == null)
                {
                    this._loadForm = new SplashScreenManager(this, typeof(WaitForm1), true, true);
                    this._loadForm.ClosingDelay = 0;
                }
                return _loadForm;
            }
        }

        /// <summary>
        /// 显示等待窗体
        /// </summary>
        public void ShowMessage()
        {
            bool flag = !this.LoadForm.IsSplashFormVisible;
            if (flag)
            {
                this.LoadForm.ShowWaitForm();
            }
        }

        /// <summary>
        /// 关闭等待窗体
        /// </summary>
        public void HideMessage()
        {
            bool isSplashFormVisible = this.LoadForm.IsSplashFormVisible;
            if (isSplashFormVisible)
            {
                this.LoadForm.CloseWaitForm();
            }
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
        private Dictionary<string, string> GUID_Name;
        private Dictionary<string, string> FDName_Value;

        /// <summary>
        /// 图符编码/名称数据(数据库获取)
        /// </summary>
        private Dictionary<string, string> Icon_JDCode;
        private Dictionary<string, string> Icon_Name;

        /// <summary>
        /// 管辖范围数据(数据库获取)
        /// </summary>
        private string[] GL_PGUID;
        List<GL_Node> GL_List;
        private Dictionary<string, string> GL_NAME;
        private Dictionary<string, string> GL_JDCODE;
        private Dictionary<string, string> GL_UPGUID;
        private Dictionary<string, string> GL_MAP;
        private Dictionary<string, string> GL_NAME_PGUID;
        private Dictionary<string, Polygon> GL_POLY;

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
        private string last_marker = "";

        /// <summary>
        /// 边界线数据
        /// </summary>
        Dictionary<string, object> borderDic = null;
        private List<double[]> borList = new List<double[]>();
        private LineData borData = null;
        private LineData lineData = null;
        

        /// <summary>
        /// 管辖分类
        /// </summary>
        private string GXguid = "";
        private string FLguid = "";


        /// <summary>
        /// 注册单位
        /// </summary>
        private List<string> Reg_Guid = null;
        private Dictionary<string, string> Reg_Name = null;
        private Dictionary<string, string> Reg_Down = null;

        /// <summary>
        /// 图符对应
        /// </summary>
        private Dictionary<string, List<string>> Level_Icon;
        private Dictionary<string, List<string>> GX_Icon;

        private InfoForm ifm = new InfoForm();
        private RegForm regfm = new RegForm();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            mapHelper1.wb1.ScriptErrorsSuppressed = true;
            borData = new LineData();
            lineData = new LineData();
            borData.Get_NewLine();
            lineData.Get_NewLine();
            currCtl = pbMove;

            // 加载主界面
            FileReader.inip = new IniOperator(WorkPath + "RegInfo.ini");
            string UnitName = FileReader.inip.ReadString("Public", "UnitName", "");
            UnitName = UnitName.Replace("\0", "");
            string AppName = FileReader.inip.ReadString("Public", "AppName", "");
            AppName = AppName.Replace("\0", "");
            string VerNum = FileReader.inip.ReadString("版本号", "VerNum", "");
            VerNum = VerNum.Substring(0, 4);
            int ListWidth = FileReader.inip.ReadInteger("Individuation", "listwidth", 200);
            dockPanel1.Width = ListWidth;
            double TextNum = dockPanel1.Height / 4.638;
            int dotNum = (int)((TextNum - 8) / 2);
            string Text = "";
            for (int i = 0; i < dotNum; ++i)
                Text += "-";
            Text += "管辖范围";
            for (int i = 0; i < dotNum; ++i)
                Text += "-";
            dockPanel1.TabText = Text;

            Text = "";
            for (int i = 0; i < dotNum; ++i)
                Text += "-";
            Text += "双击设置";
            for (int i = 0; i < dotNum; ++i)
                Text += "-";
            dockPanel2.TabText = Text;
            this.Text = UnitName + AppName + VerNum;
            FileReader.often_ahp = new AccessHelper(AccessPath);
            FileReader.line_ahp = new AccessHelper(WorkPath + "data\\经纬度注册.mdb");
            FileReader.log_ahp = new AccessHelper(WorkPath + "data\\ENVIRLOG_H0001Z000E00.mdb");

            // 读取单位数据
            FileReader.inip = new IniOperator(WorkPath + "RegInfo.ini");
            UnitID = FileReader.inip.ReadString("Public", "UnitID", "-1");

            // 加载管理员权限
            AccessHelper ahp = new AccessHelper(WorkPath + "data\\PASSWORD_H0001Z000E00.mdb");
            string sql = "select AUTHORITY from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWNAME = '管理员密码' and UNITID = '" + UnitID + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
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
                    this.Text += " - [编辑模式]";
                    Permission = true;
                    barButtonItem14.Visibility = BarItemVisibility.Always;
                    barButtonItem15.Visibility = BarItemVisibility.Always;
                    barButtonItem16.Visibility = BarItemVisibility.Always;
                    barButtonItem17.Visibility = BarItemVisibility.Always;
                    barButtonItem18.Visibility = BarItemVisibility.Always;

                    barButtonItem8.Visibility = BarItemVisibility.Always;
                    barButtonItem9.Visibility = BarItemVisibility.Always;
                    barButtonItem10.Visibility = BarItemVisibility.Always;
                    barButtonItem11.Visibility = BarItemVisibility.Always;
                    barButtonItem12.Visibility = BarItemVisibility.Always;

                }
                if (lgf.Mode == 2)
                {
                    this.Text += " - [查看模式]";
                    Permission = false;
                    barButtonItem14.Visibility = BarItemVisibility.Never;
                    barButtonItem15.Visibility = BarItemVisibility.Never;
                    barButtonItem16.Visibility = BarItemVisibility.Never;
                    barButtonItem17.Visibility = BarItemVisibility.Never;
                    barButtonItem18.Visibility = BarItemVisibility.Never;

                    barButtonItem8.Visibility = BarItemVisibility.Never;
                    barButtonItem9.Visibility = BarItemVisibility.Never;
                    barButtonItem10.Visibility = BarItemVisibility.Never;
                    barButtonItem11.Visibility = BarItemVisibility.Never;
                    barButtonItem12.Visibility = BarItemVisibility.Never;
                }
            }
            else
            {
                XtraMessageBox.Show("即将退出界面");
                System.Environment.Exit(0);
            }

            ShowMessage();

            // 获取本机信息
            Get_Computer_Info();

            // 加载管辖范围
            folds = Get_Map_List();
            Load_Unit_Level();

            // 读取注册信息
            Reg_Guid = new List<string>();
            Reg_Name = new Dictionary<string, string>();
            Reg_Down = new Dictionary<string, string>();
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K01.mdb");
            sql = "select PGUID, JDNAME, UPGUID from ZSK_OBJECT_H0001Z000K01 where ISDELETE = 0 order by LEVELNUM";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);

            string toplevel = GL_NAME_PGUID[GL_List[0].level];
            bool usefulnode = false;
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                
                string pguid = dt.Rows[i]["PGUID"].ToString();
                if (pguid == toplevel)
                    usefulnode = true;
                if (!usefulnode)
                    continue;
                Reg_Guid.Add(pguid);
                Reg_Name[pguid] = dt.Rows[i]["JDNAME"].ToString();
                Reg_Down[dt.Rows[i]["UPGUID"].ToString()] = pguid;
            }        
            regfm.Reg_Guid = Reg_Guid;
            regfm.Reg_Name = Reg_Name;
            regfm.Draw_Form();

            // 读入图标对应数据
            Icon_JDCode = new Dictionary<string, string>();
            Icon_Name = new Dictionary<string, string>();
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            sql = "select PGUID, JDNAME, JDCODE from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                Icon_Name.Add(pguid + ".png", dt.Rows[i]["JDNAME"].ToString());
                Icon_JDCode.Add(pguid, dt.Rows[i]["JDCODE"].ToString());
            }
            FileReader.once_ahp.CloseConn();
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000E00.mdb");
            sql = "select PGUID, JDNAME, JDCODE from ZSK_OBJECT_H0001Z000E00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
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
            sql = "select LAT, LNG from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '"
                + UnitID + "'";
            dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                mapHelper1.centerlat = double.Parse(dt.Rows[0]["LAT"].ToString());
                mapHelper1.centerlng = double.Parse(dt.Rows[0]["LNG"].ToString());
            }
            
            mapHelper1.webpath = WorkPath + "googlemap"; //必须设置的属性,不能为空
            mapHelper1.roadmappath = WorkPath + "googlemap\\map"; //必须设置的属性,不能为空
            mapHelper1.satellitemappath = WorkPath + "googlemap\\satellite"; //必须设置的属性,不能为空
            mapHelper1.iconspath = WorkPath + "ICONDER"; //必须设置的属性,不能为空
            mapHelper1.maparr = folds;

            // 边界线导入
            Load_Border(UnitID);

            // 加载管辖类型
            sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '-1' order by SHOWINDEX";
            dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                BarButtonItem bbi = new BarButtonItem();
                bbi.Caption = dt.Rows[i]["FLNAME"].ToString();
                bbi.Tag = dt.Rows[i]["PGUID"].ToString();
                bbi.ItemClick += MenuStripItem_Click;
                bar2.InsertItem(bar2.ItemLinks[0], bbi);
            }

            // 地图设置
            radioButton1.Checked = true;

            // 图符条
            Get_All_Icon();

            // 读取标注
            Get_All_Marker();

            if (levelguid == string.Empty)
            {
                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null, 1, 400);
                HideMessage();
                return;
            }

            if (bar2.ItemLinks[0].Item.Tag != null)
            {
                BarButtonItem now_bbi = (BarButtonItem)bar2.ItemLinks[0].Item;
                if (bar1.ItemLinks.Count == 0)
                {
                    FLguid = "";
                    Get_Marker_From_Access();
                }
                MenuStripItem_Click(barManager1, new ItemClickEventArgs(now_bbi, bar2.ItemLinks[0]));
            }
            else
            {
                FLguid = "";
                Get_Marker_From_Access();
            }
            HideMessage();
        }

        private void Get_All_Marker()
        {
            GUID_Icon = new Dictionary<string, string>();
            GUID_Name = new Dictionary<string, string>();
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();//标注list，从数据库获取
            string sql = "select * from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and UNITEID = '" + UnitID + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                GUID_Icon[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["ICONGUID"].ToString();
                GUID_Name[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["MAKRENAME"].ToString();
                Dictionary<string, object> dic = new Dictionary<string, object>();//添加每个标注
                dic.Add("guid", dt.Rows[i]["PGUID"].ToString());                            //必须加载的标准属性，从数据库查询得到值
                dic.Add("name", dt.Rows[i]["MAKRENAME"].ToString());                        //必须加载的标准属性，从数据库查询得到值
                dic.Add("level", cur_Level.ToString());                                     //必须加载的标准属性，从数据库查询得到值
                dic.Add("canedit", dt.Rows[i]["UNITEID"].ToString() == UnitID.ToString());  //必须加载的标准属性，根据上层单位判断
                dic.Add("type", dt.Rows[i]["MARKETYPE"].ToString());                        //必须加载的标准属性，从数据库查询得到值
                dic.Add("lat", dt.Rows[i]["MARKELAT"].ToString());                          //必须加载的标准属性，从数据库查询得到值
                dic.Add("lng", dt.Rows[i]["MARKELNG"].ToString());                          //必须加载的标准属性，从数据库查询得到值
                string icon_path = WorkPath + "ICONDER\\b_PNGICON\\" + dt.Rows[i]["ICONGUID"].ToString() + ".png";
                icon_path = icon_path.Replace('\\', '/');
                dic.Add("iconpath", icon_path);                                             //必须加载的标准属性
                dic.Add("message", /*sdic*/null);                                           //必须加载，内容随便，此处无用
                dic.Add("topoint", null);
                lst.Add(dic);                       //给list添加一个标注
            }
            cur_lst = lst;
        }

        private void Get_All_Icon()
        {
            flowLayoutPanel1.Controls.Clear();
            string Icon_Path = WorkPath + "ICONDER\\s_PNGICON\\";
            foreach (var item in Icon_Name)
            {
                string tmp = item.Key;
                if (File.Exists(Icon_Path + tmp))
                {
                    PictureBox PB = new PictureBox();
                    ToolTip TT = new ToolTip();
                    TT.SetToolTip(PB, Icon_Name[tmp]);
                    PB.Width = 32;
                    PB.Height = 32;
                    PB.Click += Icon_Click;
                    PB.BorderStyle = BorderStyle.Fixed3D;
                    if (Permission == true)
                    {
                        PB.MouseDown += Icon_MouseDown;
                        PB.MouseMove += Icon_MouseMove;
                        PB.MouseUp += Icon_MouseUp;
                    }
                    PB.SizeMode = PictureBoxSizeMode.CenterImage;
                    PB.Name = tmp;
                    FileStream pFileStream = new FileStream(Icon_Path + tmp, FileMode.Open, FileAccess.Read);
                    PB.Image = Image.FromStream(pFileStream);
                    flowLayoutPanel1.Controls.Add(PB);
                    pFileStream.Close();
                    pFileStream.Dispose();
                }
            }

            PictureBox SelectButton = new PictureBox();
            ToolTip stt = new ToolTip();
            stt.SetToolTip(SelectButton, "全选");
            SelectButton.SizeMode = PictureBoxSizeMode.Zoom;
            SelectButton.BorderStyle = BorderStyle.Fixed3D;
            SelectButton.Width = 32;
            SelectButton.Height = 32;
            SelectButton.Click += SelectAll_Click;
            SelectButton.Name = "全选";
            FileStream pfs = new FileStream(WorkPath + "icon\\全选.png", FileMode.Open, FileAccess.Read);
            SelectButton.Image = Image.FromStream(pfs);
            flowLayoutPanel1.Controls.Add(SelectButton);
            pfs.Close();
            pfs.Dispose();

            SelectButton = new PictureBox();
            stt = new ToolTip();
            stt.SetToolTip(SelectButton, "全不选");
            SelectButton.SizeMode = PictureBoxSizeMode.Zoom;
            SelectButton.Width = 32;
            SelectButton.Height = 32;
            SelectButton.Click += CancelAll_Click;
            SelectButton.Name = "全不选";
            pfs = new FileStream(WorkPath + "icon\\全不选.png", FileMode.Open, FileAccess.Read);
            SelectButton.Image = Image.FromStream(pfs);
            flowLayoutPanel1.Controls.Add(SelectButton);
            pfs.Close();
            pfs.Dispose();
        }

        private void Get_Computer_Info()
        {
            ComputerInfo.UserName = "";
            ComputerInfo.OSName = Environment.UserName;
            Get_Address();
        }

        private void Get_Address()
        {
            string mac = "";
            string ipv4 = "";
            string ipv6 = "";

            //需要引用：System.Net.NetworkInformation
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection allAddress = adapterProperties.UnicastAddresses;
                if (allAddress.Count > 0)
                {
                    if (adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        mac = adapter.GetPhysicalAddress().ToString();
                        foreach (UnicastIPAddressInformation addr in allAddress)
                        {
                            if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ipv4 = addr.Address.ToString();
                            }
                            if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                            {
                                ipv6 = addr.Address.ToString();
                            }
                        }

                        if (string.IsNullOrWhiteSpace(mac) ||
                            (string.IsNullOrWhiteSpace(ipv4) && string.IsNullOrWhiteSpace(ipv6)))
                        {
                            mac = "";
                            ipv4 = "";
                            ipv6 = "";
                            continue;
                        }
                        else
                        {
                            if (mac.Length == 12)
                            {
                                mac = string.Format("{0}-{1}-{2}-{3}-{4}-{5}",
                                    mac.Substring(0, 2), mac.Substring(2, 2), mac.Substring(4, 2),
                                    mac.Substring(6, 2), mac.Substring(8, 2), mac.Substring(10, 2));
                            }
                            break;
                        }
                    }
                }
            }
            ComputerInfo.PhyAddr = mac;
            ComputerInfo.IPv4 = ipv4;
            ComputerInfo.IPv6 = ipv6;
        }

        private string[] Get_Map_List()
        {
            string[] lists = null;
            string mappath = WorkPath + "googlemap\\map";
            if (!Directory.Exists(mappath))
            {
                XtraMessageBox.Show("未导入地图文件!请重新发布软件");
                FileReader.often_ahp.CloseConn();
                FileReader.line_ahp.CloseConn();
                FileReader.log_ahp.CloseConn();
                System.Environment.Exit(0);
            }
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
            GL_NAME = new Dictionary<string, string>();
            GL_JDCODE = new Dictionary<string, string>();
            GL_UPGUID = new Dictionary<string, string>();
            GL_MAP = new Dictionary<string, string>();
            GL_NAME_PGUID = new Dictionary<string, string>();
            GL_POLY = new Dictionary<string, Polygon>();
            Level_Icon = new Dictionary<string, List<string>>();
            GX_Icon = new Dictionary<string, List<string>>();

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
                GL_NAME_PGUID[dt.Rows[i]["JDNAME"].ToString()] = pguid;
            }
            FileReader.once_ahp.CloseConn();

            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb");
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + pguid + "' and UNITEID = '" + UnitID + "'";
                DataTable dt1 = FileReader.once_ahp.ExecuteDataTable(sql, null);
                if (dt1.Rows.Count > 0)
                    GL_MAP.Add(pguid, dt1.Rows[0]["MAPLEVEL"].ToString());
                else
                    GL_MAP.Add(pguid, string.Empty);
            }

            sql = "select LEVELGUID, ICONGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and UNITEID = '" + UnitID + "'";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                if (!Level_Icon.ContainsKey(dt.Rows[i]["LEVELGUID"].ToString()))
                    Level_Icon[dt.Rows[i]["LEVELGUID"].ToString()] = new List<string>();
                Level_Icon[dt.Rows[i]["LEVELGUID"].ToString()].Add(dt.Rows[i]["ICONGUID"].ToString());
            }
            FileReader.once_ahp.CloseConn();

            sql = "select ICONGUID, FLGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and UNITID = '" + UnitID + "'";
            dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                if (!GX_Icon.ContainsKey(dt.Rows[i]["FLGUID"].ToString()))
                    GX_Icon[dt.Rows[i]["FLGUID"].ToString()] = new List<string>();
                GX_Icon[dt.Rows[i]["FLGUID"].ToString()].Add(dt.Rows[i]["ICONGUID"].ToString());
            }

            GL_List = new List<GL_Node>();
            treeList1.Nodes.Clear();
            treeList1.Appearance.FocusedCell.BackColor = System.Drawing.Color.SteelBlue;
            treeList1.KeyFieldName = "pguid";
            treeList1.ParentFieldName = "upguid";
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\PersonMange.mdb");
            sql = "select PGUID, UPPGUID, ORGNAME, ULEVEL from RG_单位注册 where ISDELETE = 0 and PGUID = '" + UnitID + "'";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                GL_Node pNode = new GL_Node();
                pNode.pguid = dt.Rows[i]["PGUID"].ToString();
                pNode.upguid = dt.Rows[i]["UPPGUID"].ToString();
                GL_UPGUID[pNode.pguid] = dt.Rows[i]["UPPGUID"].ToString();
                pNode.Name = dt.Rows[i]["ORGNAME"].ToString();
                pNode.level = dt.Rows[i]["ULEVEL"].ToString();
                pNode.lat = -1;
                pNode.lng = -1;
                GL_List.Add(pNode);
                Add_Unit_Node(pNode);
            }
            treeList1.DataSource = GL_List;
            treeList1.HorzScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Auto;
            treeList1.Columns[1].Visible = false;
            treeList1.Columns[2].Visible = false;
            treeList1.Columns[3].Visible = false;
            treeList1.Columns[4].Visible = false;
            treeList1.ExpandAll();
            FileReader.once_ahp.CloseConn();

            foreach (TreeListNode tln in treeList1.Nodes)
            { 
                string pguid = tln["pguid"].ToString();
                sql = "select LAT, LNG from ORGCENTERDATA where ISDELETE = 0 and PGUID = '" + pguid + "'";
                dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    tln["lat"] = double.Parse(dt.Rows[0]["LAT"].ToString());
                    tln["lng"] = double.Parse(dt.Rows[0]["LNG"].ToString());
                }

                sql = "select MAPLEVEL from ENVIRMAPDY_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + pguid + "'";
                dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    tln["maps"] = dt.Rows[0]["MAPLEVEL"].ToString();
                }
            }
        }

        private void treeList1_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            string sql = "select PGUID from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and MAKRENAME = '" + e.Node["Name"].ToString() + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count <= 0)
            {
                e.Appearance.ForeColor = System.Drawing.Color.DarkRed;
            }
        }
        
        private void Add_Unit_Node(GL_Node pa)
        {
            string sql = "select PGUID, UPPGUID, ORGNAME, ULEVEL from RG_单位注册 where ISDELETE = 0 and UPPGUID = '" + pa.pguid + "'";
            DataTable dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                GL_Node pNode = new GL_Node();
                pNode.pguid = dt.Rows[i]["PGUID"].ToString();
                pNode.upguid = dt.Rows[i]["UPPGUID"].ToString();
                GL_UPGUID[pNode.pguid] = dt.Rows[i]["UPPGUID"].ToString();
                pNode.Name = dt.Rows[i]["ORGNAME"].ToString();
                pNode.level = dt.Rows[i]["ULEVEL"].ToString();
                pNode.lat = -1;
                pNode.lng = -1;
                pNode.maps = "";
                GL_List.Add(pNode);
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
                        borList.Add(new double[] { double.Parse(div_str[1]), double.Parse(div_str[0]) });
                    }
                }
                borderDic.Add("path", borList);
            }
            else
                Load_Border(GL_UPGUID[u_guid]);
        }

        private void MenuStripItem_Click(object sender, ItemClickEventArgs e)
        {
            Operator_GUID = "";
            select_vector = false;
            FLguid = "";
            BarManager tmp = (BarManager)sender;
            foreach (BarItemLink it in tmp.Bars[1].ItemLinks)
            {
                try
                {
                    BarButtonItem barbtn = (BarButtonItem)it.Item;
                    if (barbtn.Border == DevExpress.XtraEditors.Controls.BorderStyles.Style3D)
                    {
                        barbtn.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                        string tmp_text = barbtn.Caption;
                        int index = tmp_text.IndexOf(' ');
                        if (index > 0)
                            tmp_text = tmp_text.Substring(0, index);
                        barbtn.Caption = tmp_text;
                    }
                }
                catch
                {
                    break;
                }
            }
            e.Item.Border = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            GXguid = e.Item.Tag.ToString();
            Load_Guan_Xia();
        }

        private void ToolStripItem_Click(object sender, ItemClickEventArgs e)
        {
            Operator_GUID = "";
            select_vector = false;
            BarManager tmp = (BarManager)sender;
            BarButtonItem Fa_bbi = new BarButtonItem();
            foreach (BarItemLink it in tmp.Bars[1].ItemLinks)
            {
                try
                {
                    BarButtonItem barbtn = (BarButtonItem)it.Item;
                    if (barbtn.Tag != null && barbtn.Tag.ToString() == GXguid)
                    {
                        Fa_bbi = barbtn;
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }
            foreach (BarItemLink it in tmp.Bars[0].ItemLinks)
            {
                BarButtonItem barbtn = (BarButtonItem)it.Item;
                if (barbtn.Border == DevExpress.XtraEditors.Controls.BorderStyles.Style3D)
                    barbtn.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            }
            e.Item.Border = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            FLguid = e.Item.Tag.ToString();
            string tmp_text = Fa_bbi.Caption;
            int index = tmp_text.IndexOf(' ');
            if (index > 0)
                tmp_text = tmp_text.Substring(0, index);
            //Fa_bbi.Caption = tmp_text + " - (" + e.Item.Caption + ")";
            Fa_bbi.Caption = tmp_text;
            if (levelguid == string.Empty)
            {
                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null, 1, 400);
                return;
            }
            FLguid = e.Item.Tag.ToString();

            Get_Marker_From_Access();
        }

        private void Load_Guan_Xia()
        {
            bar1.BeginUpdate();
            bar1.ClearLinks();
            bar1.Offset = 0;
            bar1.ApplyDockRowCol();

            string sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + GXguid + "' order by SHOWINDEX";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                BarButtonItem bbi = new BarButtonItem();
                bbi.Caption = dt.Rows[i]["FLNAME"].ToString();
                bbi.Tag = dt.Rows[i]["PGUID"].ToString();
                bbi.ItemClick += ToolStripItem_Click;
                bar1.AddItem(bbi);
            }
            bar1.EndUpdate();

            if (bar1.ItemLinks.Count > 0)
                ToolStripItem_Click(barManager1, new ItemClickEventArgs(bar1.ItemLinks[0].Item, bar1.ItemLinks[0]));       
        }

        private bool Icon_Reg(string pguid)
        {
            regfm.levelid = GL_NAME_PGUID[GL_List[0].level];
            regfm.unitid = UnitID;
            regfm.nodeid = pguid;
            return regfm.ShowDialog() == System.Windows.Forms.DialogResult.OK;
        }

        private void DrawBorder()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string dlabel = "fb66d40b-50fa-4d88-8156-c590328004cb";
            dic["color"] = borData.Color;
            dic["weight"] = 0;
            dic["fillColor"] = "#C0C0C0";
            dic["fillOpacity"] = 0.5;
            dic["points"] = borList;
            if (borList.Count > 0)
                mapHelper1.DarkOuter(dlabel, dic);

            TreeListNode pNode = treeList1.FocusedNode;
            foreach (TreeListNode tln in pNode.Nodes)
            {
                string pguid = tln.GetValue("pguid").ToString();
                List<double[]> bor_line = Get_Border_Line(pguid);
                Dictionary<string, object> bdic = new Dictionary<string, object>();
                bdic["type"] = "实线";
                bdic["width"] = 1;
                bdic["color"] = "#8B0000";
                bdic["opacity"] = 0.75;
                bdic["path"] = bor_line;
                mapHelper1.DrawBorder(bdic);
            }
        }

        private List<double[]> Get_Border_Line(string pguid)
        {
            string tmp_guid = pguid;
            List<double[]> res = new List<double[]>();
            string sql = "select LNG_LAT from BORDERDATA where ISDELETE = 0 and UNITID = '" + pguid + "'";
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
                        res.Add(new double[] { double.Parse(div_str[1]), double.Parse(div_str[0]) });
                    }
                }
            }
            while (res.Count < 3)
            {
                tmp_guid = GL_UPGUID[tmp_guid];
                res = Get_Border_Line(tmp_guid);
            }
            GL_POLY[pguid] = new Polygon(res);
            return res;
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

            string sql = "insert into ENVIRICONDATA_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, LEVELGUID, MAPLEVEL, MARKELAT, MARKELNG, MAKRENAME, UNITEID, REGINFO, REGGUID) values('"
                         + markerguid + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + iconguid + "', '" + levelguid + "', '"
                         + cur_Level.ToString() + "', '" + lat.ToString() + "', '" + lng.ToString() + "', '" + name + "', '" + UnitID.ToString() + "', '"
                         + regfm.regaddr + "', '" + regfm.regguid + "')";
            FileReader.often_ahp.ExecuteSql(sql, null);
            GUID_Icon[markerguid] = iconguid;
            GUID_Name[markerguid] = name;
            string table_name = Icon_JDCode[iconguid];
            sql = "insert into " + table_name + " (PGUID, S_UDTIME";
            foreach (string key in FDName_Value.Keys)
                sql += ", " + key;
            sql += ") values('" + markerguid + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            foreach (string value in FDName_Value.Values)
                sql += "', '" + value;
            sql += "')";
            FileReader.often_ahp.ExecuteSql(sql, null);
            string Event = "添加" + Icon_Name[iconguid + ".png"] + "标注" + name + "到(" + lng.ToString() + ", " + lat.ToString() + ")";
            ComputerInfo.WriteLog("添加标注", Event);
        }

        private void mapHelper1_ModifyMarkerFinished(string markerguid, double lat, double lng, string name, bool canEdit, string iconpath, string message)
        {
            // 更新完成事件，调用ModifyMarker后触发
            // 数据库  update 
            string sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', MAKRENAME = '" + name + "', MARKELAT = '" + lat.ToString() + "', MARKELNG = '" + lng.ToString()
                + "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            FileReader.often_ahp.ExecuteSql(sql, null);
            string icon = GUID_Icon[markerguid];
            string table_name = Icon_JDCode[icon];
            if (FDName_Value != null)
            {
                sql = "update " + table_name + " set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                foreach (var item in FDName_Value)
                    sql += ", " + item.Key + " = '" + item.Value + "'";
                sql += " where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            }
            FileReader.often_ahp.ExecuteSql(sql, null);

            string Event = "编辑" + Icon_Name[icon + ".png"] + "标注" + name + "的属性";
            ComputerInfo.WriteLog("编辑标注属性", Event);
        }

        private void mapHelper1_RemoveMarkerFinished(string markerguid, bool ok)
        {
            last_marker = "";
        }

        private void UpdateDelete(string markerguid)
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
                if (handle != 2)
                {
                    string icon = GUID_Icon[pguid];
                    string name = GUID_Name[pguid];
                    string Event = "删除" + Icon_Name[icon + ".png"] + "标注" + name + "的指向位置";
                    ComputerInfo.WriteLog("删除标注指向位置", Event);
                }
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
                string name = GUID_Name[markerguid];
                string table_name = Icon_JDCode[icon];
                sql = "update " + table_name + " set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "' where ISDELEtE = 0 and PGUID = '" + markerguid + "'";
                FileReader.often_ahp.ExecuteSql(sql, null);
                string Event = "删除" + Icon_Name[icon + ".png"] + "标注" + name;
                ComputerInfo.WriteLog("删除标注", Event);
            }
            treeList1.Focus();
            mapHelper1.Focus();
        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;

            Operator_GUID = "";
            select_vector = false;
            if (e.Node == null)
                return;

            levelguid = GL_NAME_PGUID[pNode["level"].ToString()];

            // 处理cur_level
            bool flag = false;
            string[] maps = null;
            /*string sql = "select MAPLEVEL from ENVIRMAPDY_H0001Z000E00 where ISDELETE = 0 and UNITID = '"
                    + UnitID + "' and PGUID = '" + pNode["pguid"].ToString() + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
                maps = dt.Rows[0]["MAPLEVEL"].ToString().Split(',');*/
            if (pNode["maps"] == null || pNode["maps"].ToString() == "")
                maps = GL_MAP[levelguid].Split(',');
            else
                maps = pNode["maps"].ToString().Split(',');

            if (!flag)
            {
                if (maps[0] != string.Empty)
                    cur_Level = int.Parse(maps[0]);
                else
                    cur_Level = 0;
            }
            label1.Text = "当前级别：" + GL_NAME[levelguid];
            Load_Border(e.Node.GetValue("pguid").ToString());
            GL_POLY[e.Node.GetValue("pguid").ToString()] = new Polygon(borList);
            flag = false;

            double tmp_lat, tmp_lng;
            tmp_lat = double.Parse(pNode["lat"].ToString());
            tmp_lng = double.Parse(pNode["lng"].ToString());
            if (tmp_lat > 0 && tmp_lng > 0)
            {
                if (Before_ShowMap == true)
                {
                    mapHelper1.centerlat = tmp_lat;
                    mapHelper1.centerlng = tmp_lng;
                }
            }
            else
            {
                string sql = "select MARKELAT, MARKELNG from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and MAKRENAME like '%"
                    + e.Node.GetValue("Name").ToString() + "%'";
                DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    mapHelper1.centerlat = double.Parse(dt.Rows[0]["MARKELAT"].ToString());
                    mapHelper1.centerlng = double.Parse(dt.Rows[0]["MARKELNG"].ToString());
                    flag = true;
                }

                sql = "select LAT, LNG from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '"
                    + e.Node.GetValue("pguid").ToString() + "'";
                dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    mapHelper1.centerlat = double.Parse(dt.Rows[0]["LAT"].ToString());
                    mapHelper1.centerlng = double.Parse(dt.Rows[0]["LNG"].ToString());
                    flag = true;
                }

                if (flag != true)
                {
                    if (Before_ShowMap == true)
                    {
                        double[] tmp_point = mapHelper1.GetMapCenter();
                        mapHelper1.centerlat = tmp_point[0]; //30.067;//必须设置的属性,不能为空
                        mapHelper1.centerlng = tmp_point[1]; //118.5784; //必须设置的属性,不能为空
                    }
                }
            }
            Before_ShowMap = true;

            if (Icon_Name != null)
                Get_Marker_From_Access();
        }

        private void Get_Marker_From_Access()
        {
            string Icon_Path = WorkPath + "ICONDER\\b_PNGICON\\";
            Get_Icon_List();
            
            for (int k = 0; k < cur_lst.Count; ++k)
                cur_lst[k]["level"] = cur_Level.ToString();

            if (folds.Contains(cur_Level.ToString()))
            {
                flowLayoutPanel1.Visible = true;
                mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst, 1, 400);
            }
            else
            {
                mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null, 1, 400);
            }
        }

        private void Get_Icon_List()
        {
            string Icon_Path = WorkPath + "ICONDER\\b_PNGICON\\";
            foreach (PictureBox pb in flowLayoutPanel1.Controls)
            {
                if (pb.Name == "全选" || pb.Name == "全不选" || Check_Icon(pb.Name))
                    pb.Visible = true;
                else
                    pb.Visible = false;
            }
        }

        private bool Check_Icon(string iconguid)
        {
            if (!Level_Icon.ContainsKey(levelguid))
                return false;
            List<string> res_list = Level_Icon[levelguid];
            if (FLguid != "")
            {
                if (!GX_Icon.ContainsKey(FLguid))
                    return false;
                res_list = res_list.Intersect(GX_Icon[FLguid]).ToList();
            }
            if (res_list.Contains(iconguid.Substring(0, 38)))
                return true;
            else
                return false;
        }

        private void SelectAll_Click(object sender, EventArgs e)
        {
            string icon_path = WorkPath + "ICONDER\\b_PNGICON\\";
            foreach (PictureBox item in flowLayoutPanel1.Controls)
            {
                if (item.Visible == false)
                    continue;
                if (item.Name == "全不选")
                    item.BorderStyle = BorderStyle.None;
                else
                {
                    item.BorderStyle = BorderStyle.Fixed3D;
                    if (item.Name != "全选")
                    {
                        string tmppath = icon_path + item.Name;
                        tmppath = tmppath.Replace('\\', '/');
                        mapHelper1.SetMarkerVisibleByIconPath(tmppath, true);
                    }
                }
            }
            //Icon_ShowMap(iconlist);
        }

        private void CancelAll_Click(object sender, EventArgs e)
        {
            string icon_path = WorkPath + "ICONDER\\b_PNGICON\\";
            foreach (PictureBox item in flowLayoutPanel1.Controls)
            {
                if (item.Name == "全不选")
                    item.BorderStyle = BorderStyle.Fixed3D;
                else
                {
                    item.BorderStyle = BorderStyle.None;
                    if (item.Name != "全选")
                    {
                        string tmppath = icon_path + item.Name;
                        tmppath = tmppath.Replace('\\', '/');
                        mapHelper1.SetMarkerVisibleByIconPath(tmppath, false);
                    }
                }
            }
            //Icon_ShowMap("''");
        }
     
        private PictureBox currCtl = null; //被拖动控件
        private Point startPoint = new Point(-100, -100); //被拖动控件的起始位置
        private void Icon_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox theCtl = sender as PictureBox;
            Point mousePos = Control.MousePosition;
            startPoint = theCtl.Parent.PointToClient(mousePos);
            currCtl.Parent = dockPanel1;
            currCtl.Left = theCtl.Left;
            currCtl.Top = theCtl.Top;
            currCtl.Image = theCtl.Image;
            currCtl.ImageLocation = theCtl.ImageLocation;
            currCtl.Visible = true;
        }

        private void Icon_MouseMove(object sender, MouseEventArgs e)
        {
            if (startPoint.X < 0)
                return;
            if (e.Button.ToString().Equals("Left"))
            {
                Point mPos = Control.MousePosition;
                Point currPoint = currCtl.Parent.PointToClient(mPos);
                int dx = currPoint.X - startPoint.X;
                int dy = currPoint.Y - startPoint.Y;
                currCtl.Left += dx;
                currCtl.Top += dy;
                startPoint = currPoint;
                if (currCtl.Top > mapHelper1.Top)
                {
                    currCtl.Parent = mapHelper1.wb1;
                }
                else
                {
                    currCtl.Parent = panel1;
                }
            }
        }

        private void Icon_MouseUp(object sender, MouseEventArgs e)
        {
            if (currCtl.Top < panel1.Bottom)
            {
                return;
            }
            PictureBox PB = (PictureBox)sender;
            string Icon_Path = WorkPath + "ICONDER\\b_PNGICON\\";
            startPoint.X = -100;
            startPoint.Y = -100;
            currCtl.Parent = panel1;
            currCtl.Visible = false;
            Icon_GUID = Icon_Path + PB.Name;
            Icon_GUID = Icon_GUID.Replace('\\', '/');
            mapHelper1.SetBigIconPath(Icon_Path + PB.Name);
        }

        private void Icon_Click(object sender, EventArgs e)
        {
            string icon_path = WorkPath + "ICONDER\\b_PNGICON\\";
            currCtl.Visible = false;
            PictureBox PB = (PictureBox)sender;
            if (PB.BorderStyle == BorderStyle.Fixed3D)
            {
                PB.BorderStyle = BorderStyle.None;
                string tmppath = icon_path + PB.Name;
                tmppath = tmppath.Replace('\\', '/');
                mapHelper1.SetMarkerVisibleByIconPath(tmppath, false);
            }
            else if (PB.BorderStyle == BorderStyle.None)
            {
                PB.BorderStyle = BorderStyle.Fixed3D;
                string tmppath = icon_path + PB.Name;
                tmppath = tmppath.Replace('\\', '/');
                mapHelper1.SetMarkerVisibleByIconPath(tmppath, true);
            }

            bool select_all = true;
            bool cancel_all = true;
            foreach (PictureBox item in flowLayoutPanel1.Controls)
            {
                if (item.Name == "全选")
                {
                    if (select_all)
                        item.BorderStyle = BorderStyle.Fixed3D;
                    else
                        item.BorderStyle = BorderStyle.None;
                    continue;
                }
                if (item.Name == "全不选")
                {
                    if (cancel_all)
                        item.BorderStyle = BorderStyle.Fixed3D;
                    else
                        item.BorderStyle = BorderStyle.None;
                    continue;
                }
                if (item.BorderStyle == BorderStyle.Fixed3D)
                    cancel_all = false;
                else
                    select_all = false;
            }
        }

        private void Icon_ShowMap(string iconlist)
        {
            string extra_sql1 = " and ICONGUID in (select ICONGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and FLGUID = '"
                + FLguid + "' and UNITID = '" + UnitID + "')";
            if (FLguid == "-1")
                extra_sql1 = "";
            string extra_sq12 = " and ICONGUID in (select ICONGUID from [;database=" + WorkPath
                + "data\\ENVIRDYDATA_H0001Z000E00.mdb" + "].ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '"
                + levelguid + "' and UNITEID = '" + UnitID + "')";
            string extra_sql3 = " and ICONGUID in (" + iconlist + ")";

            GUID_Icon = new Dictionary<string, string>();
            GUID_Name = new Dictionary<string, string>();
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();  //标注list，从数据库获取
            string sql = "select * from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 ";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql + extra_sql1 + extra_sq12 + extra_sql3, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                GUID_Icon[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["ICONGUID"].ToString();
                GUID_Name[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["MAKRENAME"].ToString();
                Dictionary<string, object> dic = new Dictionary<string, object>();          //添加每个标注
                dic.Add("guid", dt.Rows[i]["PGUID"].ToString());                            //必须加载的标准属性，从数据库查询得到值
                dic.Add("name", dt.Rows[i]["MAKRENAME"].ToString());                        //必须加载的标准属性，从数据库查询得到值
                dic.Add("level", cur_Level.ToString());                                     //必须加载的标准属性，从数据库查询得到值
                dic.Add("canedit", dt.Rows[i]["UNITEID"].ToString() == UnitID.ToString());  //必须加载的标准属性，根据上层单位判断
                dic.Add("type", dt.Rows[i]["MARKETYPE"].ToString());                        //必须加载的标准属性，从数据库查询得到值
                dic.Add("lat", dt.Rows[i]["MARKELAT"].ToString());                          //必须加载的标准属性，从数据库查询得到值
                dic.Add("lng", dt.Rows[i]["MARKELNG"].ToString());                          //必须加载的标准属性，从数据库查询得到值
                string icon_path = WorkPath + "ICONDER\\b_PNGICON\\" + dt.Rows[i]["ICONGUID"].ToString() + ".png";
                icon_path = icon_path.Replace('\\', '/');
                dic.Add("iconpath", icon_path);                                             //必须加载的标准属性
                dic.Add("message", /*sdic*/null);                                           //必须加载，内容随便，此处无用
                /*Dictionary<string, object> toDic = new Dictionary<string, object>();
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
                }*/
                dic.Add("topoint", null);
                lst.Add(dic);                       //给list添加一个标注
            }
            //cur_lst = lst;
            mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst, 1, 400);
        }

        private void mapHelper1_MapMouseOver(double lat, double lng)
        {
            if (!Icon_GUID.Equals(""))
            {
                ifm.Close();
                TreeListNode pNode = treeList1.FocusedNode;
                Polygon poly = new Polygon(Get_Border_Line(pNode["pguid"].ToString()));
                if (!poly.PointInPolygon(new dPoint(lat, lng)))
                {
                    if (XtraMessageBox.Show("添加图符不在" + pNode["Name"].ToString() + "的范围内!是否继续添加?", "提示", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                    {
                        Icon_GUID = "";         //添加完成后把选择的图标guid清空
                        mapHelper1.SetBigIconPath("");
                        return;
                    }
                }
                regfm.unLock = false;
                regfm.StartPosition = FormStartPosition.Manual;
                regfm.textName = "";
                regfm.Left = MousePosition.X + 20;
                regfm.Top = MousePosition.Y + 20;
                if (regfm.Left + regfm.Width > this.Width)
                    regfm.Left -= regfm.Width + 20;
                if (regfm.Top + regfm.Height > this.Height)
                    regfm.Top -= regfm.Height + 20;
                if (Icon_Reg(pNode["pguid"].ToString()))
                {
                    treeList1.Focus();
                    mapHelper1.Focus();
                    //treeList1.Refresh();
                    dPoint tmp = new dPoint(0, 0);
                    string pa_guid = regfm.regguid;
                    if (GL_POLY.ContainsKey(pa_guid))
                        tmp = GL_POLY[pa_guid].GetAPoint();
                    else
                    {
                        Get_Border_Line(pa_guid);
                        tmp = GL_POLY[pa_guid].GetAPoint();
                    }
                    while (Math.Abs(tmp.x + tmp.y) < 0.01)
                    {
                        GL_Node temp = GL_List.Find(x => x.pguid == pa_guid);
                        pa_guid = temp.upguid;
                        if (GL_POLY.ContainsKey(pa_guid))
                            tmp = GL_POLY[pa_guid].GetAPoint();
                        else
                        {
                            Get_Border_Line(pa_guid);
                            tmp = GL_POLY[pa_guid].GetAPoint();
                        }
                    }
                    string name = regfm.textName;
                    //mapHelper1.addMarker("" + lat, "" + lng, name, true, Icon_GUID, null);
                    string iconguid = Path.GetFileNameWithoutExtension(Icon_GUID);
                    DataForm dtf = new DataForm();
                    dtf.Icon_GUID = iconguid;
                    dtf.Update_Data = false;
                    dtf.Load_Prop();
                    dtf.ReNew();
                    dtf.Close_Conn();
                    FDName_Value = dtf.FDName_Value;
                    Icon_GUID = Icon_GUID.Replace('\\', '/');
                    if (pNode["pguid"].ToString() == regfm.regguid)
                        mapHelper1.addMarker("" + lat, "" + lng, name, true, Icon_GUID, null);
                    else
                    {
                        mapHelper1.SetMapCenter(tmp.x, tmp.y);
                        mapHelper1.addMarker("" + tmp.x, "" + tmp.y, name, true, Icon_GUID, null);
                    }
                }
                Icon_GUID = "";         //添加完成后把选择的图标guid清空
                mapHelper1.SetBigIconPath("");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Permission)
                return;
            FileReader.inip = new IniOperator(WorkPath + "RegInfo.ini");
            FileReader.inip.WriteString("Individuation", "skin", UserLookAndFeel.Default.ActiveSkinName);
            FileReader.inip.WriteInteger("Individuation", "listwidth", dockPanel1.Width);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FileReader.often_ahp.CloseConn();
            FileReader.line_ahp.CloseConn();
            FileReader.log_ahp.CloseConn();
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataBF.exe");
            p.WaitForExit();
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataHF.exe");
            p.WaitForExit();
        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataUP.exe", "EnvirInfoSys.exe 0 2");
            p.WaitForExit();
        }

        private void barButtonItem19_ItemClick(object sender, ItemClickEventArgs e)
        {
            Process p = Process.Start(WorkPath + "OrgDataDown.exe");
            p.WaitForExit();
            treeList1.Nodes.Clear();
            Load_Unit_Level();
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "服务器IP设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            Process p = Process.Start(WorkPath + "SetIP.exe");
            p.WaitForExit();
        }

        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "边界线属性设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show("未能获取管理员权限");
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
                    mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst, 1, 400);
                }
                string Event = "修改边界线属性";
                ComputerInfo.WriteLog("边界线属性设置", Event);
            }
        }

        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "图符管理设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            Classify_1Form clcfm = new Classify_1Form();
            clcfm.unitid = UnitID;
            clcfm.gxguid = GXguid;
            clcfm.ShowDialog();

            string GX_text = "";
            foreach (BarItemLink it in bar2.ItemLinks)
            {
                try
                {
                    BarButtonItem barbtn = (BarButtonItem)it.Item;
                    if (barbtn.Tag != null && barbtn.Tag.ToString() == GXguid)
                    {
                        GX_text = barbtn.Caption;
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }

            FileReader.often_ahp.CloseConn();
            FileReader.often_ahp = new AccessHelper(AccessPath);
            Load_Guan_Xia();
            string Event = "修改" + GX_text + "分类设置";
            ComputerInfo.WriteLog("管辖分类设置", Event);

            Level_Icon = new Dictionary<string, List<string>>();
            GX_Icon = new Dictionary<string, List<string>>();

            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb");
            string sql = "select LEVELGUID, ICONGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and UNITEID = '" + UnitID + "'";
            DataTable dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                if (!Level_Icon.ContainsKey(dt.Rows[i]["LEVELGUID"].ToString()))
                    Level_Icon[dt.Rows[i]["LEVELGUID"].ToString()] = new List<string>();
                Level_Icon[dt.Rows[i]["LEVELGUID"].ToString()].Add(dt.Rows[i]["ICONGUID"].ToString());
            }
            FileReader.once_ahp.CloseConn();


            sql = "select ICONGUID, FLGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and UNITID = '" + UnitID + "'";
            dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                if (!GX_Icon.ContainsKey(dt.Rows[i]["FLGUID"].ToString()))
                    GX_Icon[dt.Rows[i]["FLGUID"].ToString()] = new List<string>();
                GX_Icon[dt.Rows[i]["FLGUID"].ToString()].Add(dt.Rows[i]["ICONGUID"].ToString());
            }

            

            Icon_JDCode = new Dictionary<string, string>();
            Icon_Name = new Dictionary<string, string>();
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            sql = "select PGUID, JDNAME, JDCODE from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                Icon_Name.Add(pguid + ".png", dt.Rows[i]["JDNAME"].ToString());
                Icon_JDCode.Add(pguid, dt.Rows[i]["JDCODE"].ToString());
            }
            FileReader.once_ahp.CloseConn();
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000E00.mdb");
            sql = "select PGUID, JDNAME, JDCODE from ZSK_OBJECT_H0001Z000E00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                Icon_Name.Add(pguid + ".png", dt.Rows[i]["JDNAME"].ToString());
                Icon_JDCode.Add(pguid, dt.Rows[i]["JDCODE"].ToString());
            }
            FileReader.once_ahp.CloseConn();

            Get_All_Icon();
            Get_Icon_List();
            Get_All_Marker();
            Get_Marker_From_Access();
        }

        private void barButtonItem10_ItemClick(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "图符对应设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            Classify_2Form clcfm = new Classify_2Form();
            clcfm.unitid = UnitID;
            clcfm.ShowDialog();

            Get_Marker_From_Access();

            string Event = "修改图符对应设置";
            ComputerInfo.WriteLog("图符对应设置", Event);            
        }

        private void barButtonItem11_ItemClick(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "图符扩展设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            Process p = Process.Start(WorkPath + "tfkzdy.exe");
            p.WaitForExit();

            Get_Marker_From_Access();

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
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000E00.mdb");
            sql = "select PGUID, JDNAME, JDCODE from ZSK_OBJECT_H0001Z000E00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                Icon_Name.Add(pguid + ".png", dt.Rows[i]["JDNAME"].ToString());
                Icon_JDCode.Add(pguid, dt.Rows[i]["JDCODE"].ToString());
            }
            FileReader.once_ahp.CloseConn();
        }

        private void barButtonItem12_ItemClick(object sender, ItemClickEventArgs e)
        {
            CheckPwForm ckpwf = new CheckPwForm();
            ckpwf.unitid = UnitID;
            if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                XtraMessageBox.Show("未能获取管理员权限");
                return;
            }
            PasswordForm psfm = new PasswordForm();
            psfm.ShowDialog();
            string Event = "修改密码管理设置";
            ComputerInfo.WriteLog("密码管理", Event);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "查看日志权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            LogForm lf = new LogForm();
            lf.ShowDialog();
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            HelpForm hpfm = new HelpForm();
            hpfm.ShowDialog();
        }

        private void mapHelper1_IconSelected(string level, string iconPath)
        {
            Icon_GUID = iconPath;//小图标选择事件
        }

        private void StopMarkerShine(string pguid)
        {
            mapHelper1.deleteMarker(pguid + "_line");
            mapHelper1.SetMarkerShine(pguid, false);
        }

        private void DrawLine(string markerguid)
        {
            double tmp_lat = 0, tmp_lng = 0;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string sql = "select LINETYPE, LINEWIDTH, LINECOLOR, LINEOPACITY from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + markerguid + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                dic["type"] = dt.Rows[0]["LINETYPE"].ToString();
                dic["width"] = int.Parse(dt.Rows[0]["LINEWIDTH"].ToString());
                dic["color"] = dt.Rows[0]["LINECOLOR"].ToString();
                dic["opacity"] = double.Parse(dt.Rows[0]["LINEOPACITY"].ToString());
                dic["arrow"] = false;
            }
            else
                return;

            sql = "select MARKELAT, MARKELNG, POINTLAT, POINTLNG from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0 && dt.Rows[0]["POINTLAT"].ToString() != "")
            {
                dic["lat"] = double.Parse(dt.Rows[0]["POINTLAT"].ToString());
                dic["lng"] = double.Parse(dt.Rows[0]["POINTLNG"].ToString());
                tmp_lat = double.Parse(dt.Rows[0]["MARKELAT"].ToString());
                tmp_lng = double.Parse(dt.Rows[0]["MARKELNG"].ToString());
            }
            else
                return;

            mapHelper1.DrawPointLine(markerguid, tmp_lat, tmp_lng, dic);
        }

        private void mapHelper1_MapMouseup(string Mousebutton, bool canedit, double lat, double lng, int x, int y, string markerguid)
        {
            mapHelper1.deleteMarker(last_marker + "_line");
            if (markerguid == "fb66d40b-50fa-4d88-8156-c590328004cb")
                return;
            if (markerguid != string.Empty)
            {
                //mapHelper1.deleteMarker(last_marker + "_line");
                ifm.Close();
                // DrawLine
                DrawLine(markerguid);
                mapHelper1.SetMarkerShine(markerguid, true);
                ifm = new InfoForm();
                ifm.Owner = this;
                ifm.CanEdit = false;
                ifm.Update_Data = true;
                ifm.Node_GUID = markerguid;
                ifm.Icon_GUID = GUID_Icon[markerguid];
                ifm.JdCode = Icon_JDCode[ifm.Icon_GUID];
                ifm.Text = GUID_Name[markerguid];
                ifm.unitid = UnitID;
                ifm.StartPosition = FormStartPosition.Manual;
                ifm.Left = groupControl1.Right - ifm.Width;
                ifm.Top = groupControl1.Bottom - ifm.Height + 20;
                ifm.stopshine += new InfoForm.StopShine(StopMarkerShine);
                ifm.Show();
            }

            if (markerguid.Equals("") && Icon_GUID.Equals("") && select_vector == true)
            {
                last_marker = markerguid;
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
                    {
                        mapHelper1.deleteMarker(Operator_GUID + "_line");
                        last_marker = Operator_GUID;
                        UpdateDelete(Operator_GUID + "_line");
                    }
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
                    if (handle == 1)
                    {
                        string icon = GUID_Icon[Operator_GUID];
                        string name = GUID_Name[Operator_GUID];
                        string Event = "添加" + Icon_Name[icon + ".png"] + "标注" + name + "的指向位置到(" + lng.ToString() + ", " + lat.ToString() + ")";
                        ComputerInfo.WriteLog("添加指向位置", Event);
                    }
                    else
                    {
                        string icon = GUID_Icon[Operator_GUID];
                        string name = GUID_Name[Operator_GUID];
                        string Event = "修改" + Icon_Name[icon + ".png"] + "标注" + name + "的指向位置到(" + lng.ToString() + ", " + lat.ToString() + ")";
                        ComputerInfo.WriteLog("修改指向位置", Event);
                    }
                }
                select_vector = false;
            }
        }

        private void mapHelper1_MarkerDragBegin(string markerguid, bool candrag)
        {
            mapHelper1.deleteMarker(last_marker + "_line");
            DrawLine(markerguid);
            last_marker = markerguid;
            Operator_GUID = "";
            select_vector = false;

            if (!Permission)
                candrag = false;
            else
                candrag = true;
        }

        private void mapHelper1_MarkerDragEnd(string markerguid, bool canedit, double lat, double lng)
        {
            if (markerguid == "")
                return;
            //  MessageBox.Show("移动：" + markerguid);
            //  数据库 update 坐标

            // 判断是否在范围
            bool isIn = false;
            dPoint pnt = new dPoint(lat, lng);
            dPoint origin_pnt = pnt;
            string sql = "select MARKELAT, MARKELNG, MAKRENAME, REGGUID from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                origin_pnt = new dPoint(double.Parse(dt.Rows[0]["MARKELAT"].ToString()), double.Parse(dt.Rows[0]["MARKELNG"].ToString()));
                string guid = dt.Rows[0]["REGGUID"].ToString();
                if (GL_POLY.ContainsKey(guid))
                    isIn = GL_POLY[guid].PointInPolygon(pnt);
                else
                {
                    Get_Border_Line(guid);
                    isIn = GL_POLY[guid].PointInPolygon(pnt);
                }
            }
            if (!isIn)
            {
                if (XtraMessageBox.Show("移动超出注册范围!是否要完成移动?", "提示", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                {
                    if (dt.Rows.Count > 0)
                        mapHelper1.modifyMarker(markerguid, dt.Rows[0]["MAKRENAME"].ToString(), true, origin_pnt.x, origin_pnt.y, null);
                    return; 
                }
            }

            for (int i = 0; i < cur_lst.Count; ++i)
            {
                if (cur_lst[i]["guid"].ToString() == markerguid)
                {
                    cur_lst[i]["lat"] = lat;
                    cur_lst[i]["lng"] = lng;
                    break;
                }
            }

            sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', MARKELAT = '" + lat.ToString() + "', MARKELNG = '" + lng.ToString() + "' where ISDELETE = 0 and PGUID = '"
                + markerguid + "'";
            FileReader.often_ahp.ExecuteSql(sql, null);
            string icon = GUID_Icon[markerguid];
            string name = GUID_Name[markerguid];
            string Event = "移动" + Icon_Name[icon + ".png"] + "标注" + name + "到(" + lng.ToString() + ", " + lat.ToString() + ")";
            ComputerInfo.WriteLog("移动标注", Event);
        }

        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
            Icon_GUID = "";
        }

        private int MapX, MapY;
        private void mapHelper1_MarkerRightClick(int sx, int sy, double lat, double lng, string level, string sguid, string name, bool canedit, string message)
        {
            mapHelper1.deleteMarker(last_marker + "_line");
            last_marker = sguid;
            i_lat = lat;
            i_lng = lng;
            for (int i = 0; i < cur_lst.Count; ++i)
            {
                if (cur_lst[i]["guid"].ToString() == sguid)
                {
                    string sql = "select PGUID from ENVIRLINE_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + sguid + "'";
                    DataTable dt1 = FileReader.often_ahp.ExecuteDataTable(sql, null);
                    sql = "select POINTLAT from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + sguid + "'";
                    DataTable dt2 = FileReader.often_ahp.ExecuteDataTable(sql, null);

                    if (dt1.Rows.Count <= 0 || dt2.Rows.Count <= 0 || dt2.Rows[0]["POINTLAT"].ToString() == "")
                    {
                        barButtonItem15.Enabled = true;
                        barButtonItem16.Enabled = false;
                        barButtonItem17.Enabled = false;
                    }
                    else
                    {
                        barButtonItem15.Enabled = false;
                        barButtonItem16.Enabled = true;
                        barButtonItem17.Enabled = true;
                    }
                    break;
                }
            }
            MapX = MousePosition.X;
            MapY = MousePosition.Y;
            popupMenu1.ShowPopup(barManager1, MousePosition);
            Operator_GUID = sguid;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="now_gl"></param> 管辖级别编号
        /// <param name="now_map"></param> 地图级别编号
        private void Map_Size_Change(int now_gl, int now_map, string szchange)
        {
            if (now_gl > GL_PGUID.Length)
                return;
            TreeListNode pNode = treeList1.FocusedNode;
            string GL_guid = GL_PGUID[now_gl];
            string[] maps = GL_MAP[GL_guid].Split(',');
            string sql = "select MAPLEVEL from ENVIRMAPDY_H0001Z000E00 where ISDELETE = 0 and UNITID = '"
                    + UnitID + "' and PGUID = '" + pNode["pguid"].ToString() + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
                maps = dt.Rows[0]["MAPLEVEL"].ToString().Split(',');

            bool NeedChange = true;
            for (int i = 0; i < maps.Length; ++i)
            {
                if (maps[i] == folds[now_map])
                {
                    NeedChange = false;
                    cur_Level = int.Parse(maps[i]);
                    for (int k = 0; k < cur_lst.Count; ++k)
                        cur_lst[k]["level"] = cur_Level.ToString();

                    if (folds.Contains(cur_Level.ToString()))
                    {
                        flowLayoutPanel1.Visible = true;
                        mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst, 1, 400);
                    }
                    else
                    {
                        mapHelper1.ShowMap(cur_Level, cur_Level.ToString(), false, map_type, null, borderDic, null, 1, 400);
                    }
                }
            }

            if (NeedChange)
            {
                TreeListNode resNode = null;
                foreach (TreeListNode tln in treeList1.Nodes)
                {
                    if (szchange == "up")
                        resNode = Select_Node(GL_PGUID[now_gl + 1], tln);
                    else
                        resNode = Select_Node(GL_PGUID[now_gl - 1], tln);
                    if (resNode != null)
                    {
                        treeList1.FocusedNode = resNode;
                        return;
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
            for (int i = 0; i < len_map; ++i)
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
                    TreeListNode resNode = null;
                    foreach (TreeListNode tln in treeList1.Nodes)
                    {
                        if (now_gl == len_gl)
                            return;
                        resNode = Select_Node(GL_PGUID[now_gl + 1], tln);

                        if (resNode != null)
                        {
                            treeList1.FocusedNode = resNode;
                            return;
                        }
                    }

                    //MessageBox.Show("地图已达最大级别!");
                    return;
                }
                Map_Size_Change(now_gl, now_map, "up");
            }
            else
            {
                --now_map;
                if (now_map < 0)
                {
                    TreeListNode resNode = null;
                    foreach (TreeListNode tln in treeList1.Nodes)
                    {
                        if (now_gl == 0)
                            return;
                        resNode = Select_Node(GL_PGUID[now_gl - 1], tln);
                        if (resNode != null)
                        {
                            treeList1.FocusedNode = resNode;
                            return;
                        }
                    }

                    //MessageBox.Show("地图已达最小级别!");
                    return;
                }
                Map_Size_Change(now_gl, now_map, "down");
            }
        }

        private double center_lat = 0;
        private double center_lng = 0;
        private void mapHelper1_MapDblClick(string button, bool canedit, double lat, double lng, int x, int y, string markerguid)
        {
            center_lat = lat;
            center_lng = lng;
            if (radioButton1.Checked && button == "left")
            {
                Map_Resize(true);
            }
            else if (radioButton2.Checked && button == "left")
            {
                Map_Resize(false);
            }
        }

        private TreeListNode Select_Node(string levelguid, TreeListNode pNode)
        {
            if (pNode == null)
                return null;
            if (GL_NAME_PGUID[pNode["level"].ToString()] == levelguid && Check_relative(pNode))
                return pNode;
            TreeListNode resNode = null;
            foreach (TreeListNode tln in pNode.Nodes)
            {
                resNode = Select_Node(levelguid, tln);
                if (resNode != null)
                    break;
            }
            return resNode;
        }

        private bool Check_relative(TreeListNode pNode)
        {
            dPoint point = new dPoint(center_lat, center_lng);
            string pguid = pNode["pguid"].ToString();
            if (GL_POLY.ContainsKey(pguid))
                return GL_POLY[pguid].PointInPolygon(point);
            else
            {
                Polygon tmp = new Polygon(Get_Border_Line(pguid));
                return tmp.PointInPolygon(point);
            }
        }

        private void mapHelper1_MapTypeChanged(string mapType)
        {
            map_type = mapType;
        }

        private void mapHelper1_MapMouseWheel(string direction)
        {
            double[] tmp = mapHelper1.GetMapCenter();
            center_lat = tmp[0];
            center_lng = tmp[1];
            if (direction == "up")
                Map_Resize(true);
            else
                Map_Resize(false);
        }

        // 属性编辑
        private void barButtonItem13_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Operator_GUID != "")
            {
                DrawLine(Operator_GUID);
                mapHelper1.SetMarkerShine(Operator_GUID, true);

                string iconguid = "";
                string sql = "select MAKRENAME, ICONGUID, MARKELAT, MARKELNG from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + Operator_GUID + "'";
                DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count != 0)
                    iconguid = dt.Rows[0]["ICONGUID"].ToString();
                else
                    return;
                DataForm dtf = new DataForm();
                dtf.CanEdit = Permission;
                dtf.Update_Data = true;
                dtf.Node_GUID = Operator_GUID;
                dtf.Icon_GUID = iconguid;
                dtf.JdCode = Icon_JDCode[iconguid];
                dtf.Text = "编辑标注";
                dtf.StartPosition = FormStartPosition.Manual;
                dtf.Left = MapX;
                dtf.Top = MapY;
                if (dtf.Left + dtf.Width > this.Width)
                    dtf.Left -= dtf.Width;
                if (dtf.Top + dtf.Height > this.Height)
                    dtf.Top -= dtf.Height;
                if (dtf.ShowDialog() == DialogResult.OK)
                {
                    string name = dt.Rows[0]["MAKRENAME"].ToString();
                    FDName_Value = dtf.FDName_Value;
                    mapHelper1.modifyMarker(Operator_GUID, name, true, double.Parse(dt.Rows[0]["MARKELAT"].ToString()), double.Parse(dt.Rows[0]["MARKELNG"].ToString()), null);
                }
                StopMarkerShine(Operator_GUID);
                Operator_GUID = "";
            }
        }

        private void barButtonItem14_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!Permission)
            {
                XtraMessageBox.Show("您没有删除权限!");
                return;
            }
            if (Operator_GUID != "")
            {
                mapHelper1.deleteMarker(Operator_GUID);
                UpdateDelete(Operator_GUID);
                Operator_GUID = "";
            }
        }

        private void barButtonItem15_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Operator_GUID == "")
                return;
            // 显示添加
            select_vector = true;
            handle = 1;
        }

        private void barButtonItem16_ItemClick(object sender, ItemClickEventArgs e)
        {
            select_vector = true;
            handle = 2;
        }


        private void barButtonItem17_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Operator_GUID != "")
            {
                mapHelper1.deleteMarker(Operator_GUID + "_line");
                UpdateDelete(Operator_GUID + "_line");
                Operator_GUID = "";
            }
        }

        /// <summary>
        /// 导入边界线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem18_ItemClick(object sender, ItemClickEventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            if (xtraOpenFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string file = xtraOpenFileDialog1.FileName;
            string[] strAll = File.ReadAllLines(file);
            string ds_lng_lat = "";
            borList = new List<double[]>();
            foreach (string str in strAll)
            {
                string[] split = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n' });
                borList.Add(new double[] { double.Parse(split[1]), double.Parse(split[0]) });
                ds_lng_lat += split[0] + "," + split[1] + ";";
            }
            borderDic["path"] = borList;
            string sql = "select PGUID from BORDERDATA where ISDELETE = 0 and UNITID = '" + pNode["pguid"].ToString() + "'";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update BORDERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LNG_LAT = '"
                    + ds_lng_lat + "' where ISDELETE = 0 and UNITID = '" + pNode["pguid"].ToString() + "'";
                FileReader.line_ahp.ExecuteSql(sql, null);
                string Event = "修改" + pNode["Name"].ToString() + "的边界线";
                ComputerInfo.WriteLog("导入边界线", Event);
            }
            else
            {
                sql = "insert into BORDERDATA (PGUID, S_UDTIME, UNITID, LNG_LAT) values ('" + Guid.NewGuid().ToString("B")
                    + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode["pguid"].ToString() + "', '" + ds_lng_lat + "')";
                FileReader.line_ahp.ExecuteSql(sql, null);
                string Event = "添加" + pNode["Name"].ToString() + "的边界线";
                ComputerInfo.WriteLog("导入边界线", Event);
            }
            mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst, 1, 400);
        }

        /// <summary>
        /// 修改注册信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem20_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Operator_GUID != "")
            {
                DrawLine(Operator_GUID);
                mapHelper1.SetMarkerShine(Operator_GUID, true);
                string sql = "select MAKRENAME, MARKELAT, MARKELNG, REGGUID from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + Operator_GUID + "'";
                DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    regfm.unLock = true;
                    regfm.StartPosition = FormStartPosition.Manual;
                    regfm.textName = dt.Rows[0]["MAKRENAME"].ToString();
                    regfm.markerguid = Operator_GUID;
                    regfm.Left = MousePosition.X + 20;
                    regfm.Top = MousePosition.Y + 20;
                    if (regfm.Left + regfm.Width > this.Width)
                        regfm.Left -= regfm.Width + 20;
                    if (regfm.Top + regfm.Height > this.Height)
                        regfm.Top -= regfm.Height + 20;
                    string pguid = dt.Rows[0]["REGGUID"].ToString();
                    if (Icon_Reg(pguid))
                    {
                        dPoint tmp = new dPoint(0, 0);
                        string pa_guid = regfm.regguid;
                        if (GL_POLY.ContainsKey(regfm.regguid))
                            tmp = GL_POLY[regfm.regguid].GetAPoint();
                        else
                        {
                            Get_Border_Line(regfm.regguid);
                            tmp = GL_POLY[regfm.regguid].GetAPoint();
                        }
                        while (Math.Abs(tmp.x + tmp.y) < 0.01)
                        {
                            GL_Node temp = GL_List.Find(x => x.pguid == pa_guid);
                            pa_guid = temp.upguid;
                            if (GL_POLY.ContainsKey(pa_guid))
                                tmp = GL_POLY[pa_guid].GetAPoint();
                            else
                            {
                                Get_Border_Line(pa_guid);
                                tmp = GL_POLY[pa_guid].GetAPoint();
                            }
                        }
                        if (pguid != regfm.regguid)
                        {
                            mapHelper1.deleteMarker(Operator_GUID + "_line");
                            UpdateDelete(Operator_GUID + "_line");
                            mapHelper1.SetMapCenter(tmp.x, tmp.y);
                            mapHelper1.modifyMarker(Operator_GUID, regfm.textName, true, tmp.x, tmp.y, null);
                        }
                        else
                        {
                            mapHelper1.modifyMarker(Operator_GUID, regfm.textName, true, double.Parse(dt.Rows[0]["MARKELAT"].ToString()), double.Parse(dt.Rows[0]["MARKELNG"].ToString()), null);
                        }
                        for (int i = 0; i < cur_lst.Count; ++i)
                        {
                            if (cur_lst[i]["guid"].ToString() == Operator_GUID)
                            {
                                cur_lst[i]["name"] = regfm.textName;
                                break;
                            }
                        }
                        sql = "update ENVIRICONDATA_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            + "', MAKRENAME = '" + regfm.textName + "', REGINFO = '" + regfm.regaddr + "', REGGUID = '" + regfm.regguid
                            + "' where ISDELETE = 0 and PGUID = '" + Operator_GUID + "'";
                        FileReader.often_ahp.ExecuteSql(sql, null);
                    }
                }
                StopMarkerShine(Operator_GUID);
                Operator_GUID = "";
                treeList1.Focus();
                mapHelper1.Focus();
            }
        }

        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!Permission)
            {
                System.Environment.Exit(0);
                return;
            }

            FileReader.inip = new IniOperator(WorkPath + "RegInfo.ini");
            FileReader.inip.WriteString("Individuation", "skin", UserLookAndFeel.Default.ActiveSkinName);
            FileReader.inip.WriteInteger("Individuation", "listwidth", dockPanel1.Width);
            FileReader.often_ahp.CloseConn();
            FileReader.line_ahp.CloseConn();
            FileReader.log_ahp.CloseConn();
            System.Environment.Exit(0);
        }

        
        private void mapHelper1_LevelChanged(int lastLevel, int currLevel, string showLevel)
        {
            string Icon_Path = WorkPath + "ICONDER\\b_PNGICON\\";
            if (ifm != null)
                ifm.Close();
            DrawBorder();
            
            foreach (PictureBox pb in flowLayoutPanel1.Controls)
            {
                
                if (pb.Name == "全选" || pb.Name == "全不选")
                    continue;
                string tmppath = Icon_Path + pb.Name;
                tmppath = tmppath.Replace('\\', '/');
                if (pb.BorderStyle == BorderStyle.Fixed3D && pb.Visible == true)
                    mapHelper1.SetMarkerVisibleByIconPath(tmppath, true);
                else
                    mapHelper1.SetMarkerVisibleByIconPath(tmppath, false);
                Application.DoEvents();
            }

            HideMessage();
        }

        private void barButtonItem22_ItemClick(object sender, ItemClickEventArgs e)
        {
            MapLevelForm mplvf = new MapLevelForm();
            TreeListNode pNode = treeList1.FocusedNode;
            mplvf.StartPosition = FormStartPosition.Manual;
            mplvf.Left = MapX;
            mplvf.Top = MapY;
            if (mplvf.Top + mplvf.Height > this.Height)
                mplvf.Top -= mplvf.Height;
            mplvf.nodeid = pNode["pguid"].ToString();
            mplvf.unitid = UnitID;
            mplvf.ShowDialog();
            if (pNode.ParentNode != null)
                treeList1.FocusedNode = pNode.ParentNode;
            else if (pNode.Nodes[0] != null)
                treeList1.FocusedNode = pNode.Nodes[0];
            treeList1.FocusedNode = pNode;
        }

        private void barButtonItem23_ItemClick(object sender, ItemClickEventArgs e)
        {
            CheckPwForm ckpwf = new CheckPwForm();
            ckpwf.unitid = UnitID;
            if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                XtraMessageBox.Show("未能获取管理员权限");
                return;
            }

            Process p = Process.Start(WorkPath + "DataUP.exe", "EnvirInfoSys.exe 1 1");
            p.WaitForExit();
        }

        private void barButtonItem24_ItemClick(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "地图设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = UnitID;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            MapSetForm mpstf = new MapSetForm();
            mpstf.unitid = UnitID;
            mpstf.ShowDialog();

            FileReader.line_ahp.CloseConn();
            FileReader.line_ahp = new AccessHelper(WorkPath + "data\\经纬度注册.mdb");

            foreach (TreeListNode tln in treeList1.Nodes)
            {
                string pguid = tln["pguid"].ToString();
                string sql = "select LAT, LNG from ORGCENTERDATA where ISDELETE = 0 and PGUID = '" + pguid + "'";
                DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    tln["lat"] = double.Parse(dt.Rows[0]["LAT"].ToString());
                    tln["lng"] = double.Parse(dt.Rows[0]["LNG"].ToString());
                }

                sql = "select MAPLEVEL from ENVIRMAPDY_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + pguid + "'";
                dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    tln["maps"] = dt.Rows[0]["MAPLEVEL"].ToString();
                }
            }

            /*string sql = "select LAT, LNG from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '"
                + UnitID + "'";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                mapHelper1.centerlat = double.Parse(dt.Rows[0]["LAT"].ToString());
                mapHelper1.centerlng = double.Parse(dt.Rows[0]["LNG"].ToString());
            }*/
            mapHelper1.ShowMap(cur_Level, GL_NAME[levelguid], Permission, map_type, Icon_Name, borderDic, cur_lst, 1, 400);
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
           
        }
    }

    /// <summary>
    /// MessageBox 中文
    /// </summary>
    public class MessageboxClass : Localizer
    {
        public override string GetLocalizedString(StringId id)
        {
            switch (id)
            {
                case StringId.XtraMessageBoxCancelButtonText:
                    return "取消";
                case StringId.XtraMessageBoxOkButtonText:
                    return "确定";
                case StringId.XtraMessageBoxYesButtonText:
                    return "是";
                case StringId.XtraMessageBoxNoButtonText:
                    return "否";
                case StringId.XtraMessageBoxIgnoreButtonText:
                    return "忽略";
                case StringId.XtraMessageBoxAbortButtonText:
                    return "中止";
                case StringId.XtraMessageBoxRetryButtonText:
                    return "重试";
                default:
                    return "";
            }
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
        public static AccessHelper log_ahp = null;
        public static IniOperator inip = null;
        public static string[] Authority = null;
    }

    /// <summary>
    /// 本机信息类
    /// </summary>
    public class ComputerInfo
    {
        public static string UserName = null;
        public static string OSName = null;
        public static string PhyAddr = null;
        public static string IPv4 = null;
        public static string IPv6 = null;

        public static void WriteLog(string Event, string Remark)
        {
            string sql = "insert into LOG_H0001Z000E00 (PGUID, S_UDTIME, USERNAME, OSNAME, PHYADDRESS, IPV4ADDRESS, IPV6ADDRESS, RUNTIME, EVENT, REMARK) values ('"
                + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + UserName + "', '" + OSName + "', '"
                + PhyAddr + "', '" + IPv4 + "', '" + IPv6 + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + Event + "', '" + Remark + "')";
            FileReader.log_ahp.ExecuteSql(sql, null);
        }
    }

    /// <summary>
    /// 管辖范围节点类
    /// </summary>
    public class GL_Node
    {
        public string pguid { set; get; }
        public string upguid { set; get; }
        public string Name { set; get; }
        public string level { set; get; }
        public string maps { set; get; }
        public double lat { set; get; }
        public double lng { set; get; }
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

    public class dPoint
    {
        public double x { set; get; }
        public double y { set; get; }

        public dPoint(double p1, double p2)
        {
            this.x = p1;
            this.y = p2;
        }
    }

    public class dLine
    {
        public dPoint pa;
        public dPoint pb;

        public dLine(dPoint a, dPoint b)
        {
            this.pa = a;
            this.pb = b;
        }

        public dPoint GetCross(double Y)
        {
            double X = pa.x - (pa.y - Y) * (pa.x - pb.x) / (pa.y - pb.y);
            dPoint res = new dPoint(X, Y);
            return res;
        }
    }

    public class Polygon
    {
        public int len { set; get; }
        public List<dPoint> ploygon { set; get; }

        public Polygon()
        {
            len = 0;
            ploygon = new List<dPoint>();
        }

        public Polygon(List<double[]> list)
        {
            len = list.Count;
            ploygon = new List<dPoint>();
            for (int i = 0; i < len; ++i)
                ploygon.Add(new dPoint(list[i][0], list[i][1]));
        }

        public bool PointInPolygon(dPoint p)
        {
            if (len < 3)
                return true;
            int i, j = len - 1;
            bool res = false;
            for (i = 0; i < len; ++i)
            {
                if (((ploygon[i].y < p.y && ploygon[j].y >= p.y) ||
                     (ploygon[j].y < p.y && ploygon[i].y >= p.y)) &&
                     (ploygon[i].x <= p.x || ploygon[j].x <= p.x))
                    res ^= (ploygon[i].x + (p.y - ploygon[i].y) / (ploygon[j].y - ploygon[i].y) * (ploygon[j].x - ploygon[i].x) < p.x);
                j = i;
            }
            return res;
        }

        public dPoint GetAPoint()
        {
            List<dPoint> crosspoint = new List<dPoint>();
            double avgY = 0;
            for (int i = 0; i < len; ++i)
                avgY += ploygon[i].y;
            avgY /= len;
            for (int i = 0; i < len; ++i)
            {
                dLine line = new dLine(ploygon[i], ploygon[(i + 1) % len]);
                if (ploygon[i].y > avgY ^ ploygon[(i + 1) % len].y > avgY)
                    crosspoint.Add(line.GetCross(avgY));
            }
            if (crosspoint.Count < 2)
                return new dPoint(0, 0);
            crosspoint.Sort((x, y) => y.x.CompareTo(x.x));
            dPoint res = new dPoint((crosspoint[0].x + crosspoint[1].x) / 2, (crosspoint[0].y + crosspoint[1].y) / 2);
            return res;
        }
    }
}