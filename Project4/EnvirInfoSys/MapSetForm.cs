using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using DevExpress.XtraTreeList.Nodes;

namespace EnvirInfoSys
{
    public partial class MapSetForm : DevExpress.XtraEditors.XtraForm
    {
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;
        private string AccessPath = AppDomain.CurrentDomain.BaseDirectory + "data\\ENVIR_H0001Z000E00.mdb";
        private string IniFilePath = AppDomain.CurrentDomain.BaseDirectory + "parameter.ini";
        private string[] folds = null;
        public string unitid = "";
        public string MapPath = "";


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
        private Dictionary<string, Dictionary<string, Polygon>> GL_POLY;

        /// <summary>
        /// 边界线数据
        /// </summary>
        Dictionary<string, object> borderDic = null;
        private Dictionary<string, List<double[]>> borList = new Dictionary<string, List<double[]>>();

        private LineData borData = null;
        //private LineData lineData = null;


        /// <summary>
        /// 地图数据
        /// </summary>
        private bool Before_ShowMap = false;
        private int cur_level = 0;
        private int now_level = 0;
        private string levelguid = "";
        private string map_type = "";

        private int MapX = 0;
        private int MapY = 0;

        public MapSetForm()
        {
            InitializeComponent();
        }

        private void MapSetForm_Load(object sender, EventArgs e)
        {
            borData = new LineData();
            borData.Get_NewLine();

            // 添加管辖级别
            folds = Get_Map_List();
            if (folds == null)
            {
                return;
            }
            Load_Unit_Level();

            // 地图初始化
            FileReader.inip = new IniOperator(IniFilePath);
            string slat = FileReader.inip.ReadString("mapproperties", "centerlat", "");
            string slng = FileReader.inip.ReadString("mapproperties", "centerlng", "");
            mapHelper1.centerlat = double.Parse(slat); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(slng); //118.5784; //必须设置的属性,不能为空
            string sql = "select LAT, LNG from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '"
                + unitid + "'";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                mapHelper1.centerlat = double.Parse(dt.Rows[0]["LAT"].ToString());
                mapHelper1.centerlng = double.Parse(dt.Rows[0]["LNG"].ToString());
            }

            mapHelper1.webpath = WorkPath + "googlemap"; //必须设置的属性,不能为空
            mapHelper1.roadmappath = MapPath + "\\roadmap"; //必须设置的属性,不能为空
            mapHelper1.satellitemappath = MapPath + "\\satellite_en"; //必须设置的属性,不能为空
            mapHelper1.iconspath = WorkPath + "PNGICONFOLDER"; //必须设置的属性,不能为空
            mapHelper1.maparr = folds;

            // 边界线导入
            Load_Border(unitid);

            // 添加图符按钮
            PictureBox SelectButton = new PictureBox();
            ToolTip stt = new ToolTip();
            stt.SetToolTip(SelectButton, "指针");
            SelectButton.SizeMode = PictureBoxSizeMode.Zoom;
            SelectButton.BorderStyle = BorderStyle.Fixed3D;
            SelectButton.Width = 32;
            SelectButton.Height = 32;
            SelectButton.Click += Vector_Click;
            SelectButton.Name = "指针";
            FileStream pfs = new FileStream(WorkPath + "icon\\指针.png", FileMode.Open, FileAccess.Read);
            SelectButton.Image = Image.FromStream(pfs);
            flowLayoutPanel1.Controls.Add(SelectButton);
            pfs.Close();
            pfs.Dispose();

            SelectButton = new PictureBox();
            stt = new ToolTip();
            stt.SetToolTip(SelectButton, "画线");
            SelectButton.SizeMode = PictureBoxSizeMode.Zoom;
            SelectButton.Width = 32;
            SelectButton.Height = 32;
            SelectButton.Click += Line_Click;
            SelectButton.Name = "画线";
            pfs = new FileStream(WorkPath + "icon\\画线.png", FileMode.Open, FileAccess.Read);
            SelectButton.Image = Image.FromStream(pfs);
            flowLayoutPanel1.Controls.Add(SelectButton);
            pfs.Close();
            pfs.Dispose();

            SelectButton = new PictureBox();
            stt = new ToolTip();
            stt.SetToolTip(SelectButton, "画多边形");
            SelectButton.SizeMode = PictureBoxSizeMode.Zoom;
            SelectButton.Width = 32;
            SelectButton.Height = 32;
            SelectButton.Click += Polygon_Click;
            SelectButton.Name = "画多边形";
            pfs = new FileStream(WorkPath + "icon\\多边形.png", FileMode.Open, FileAccess.Read);
            SelectButton.Image = Image.FromStream(pfs);
            flowLayoutPanel1.Controls.Add(SelectButton);
            pfs.Close();
            pfs.Dispose();

            SelectButton = new PictureBox();
            stt = new ToolTip();
            stt.SetToolTip(SelectButton, "中心点");
            SelectButton.SizeMode = PictureBoxSizeMode.Zoom;
            SelectButton.Width = 32;
            SelectButton.Height = 32;
            SelectButton.MouseDown += Center_MouseDown;
            SelectButton.Click += Center_Click;
            SelectButton.Name = "中心点";
            pfs = new FileStream(WorkPath + "icon\\中心点.png", FileMode.Open, FileAccess.Read);
            SelectButton.Image = Image.FromStream(pfs);
            flowLayoutPanel1.Controls.Add(SelectButton);
            pfs.Close();
            pfs.Dispose();

            mapHelper1.ShowMap(cur_level, cur_level.ToString(), false, map_type, null, null, null, 1, 400);
        }

        private void Polygon_Click(object sender, EventArgs e)
        {
            
        }

        private void Line_Click(object sender, EventArgs e)
        {
            
        }

        private void Vector_Click(object sender, EventArgs e)
        {
            
        }

        private void Center_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox PB = (PictureBox)sender;
            PB.BorderStyle = BorderStyle.Fixed3D;
        }

        private void Center_Click(object sender, EventArgs e)
        {
            double[] point = mapHelper1.GetMapCenter();
            TreeListNode pNode = treeList1.FocusedNode;
            string pguid = pNode.GetValue("pguid").ToString();
            string sql = "select PGUID from ORGCENTERDATA where ISDELETE = 0 and PGUID = '" + pguid + "'";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ORGCENTERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "', LAT = '" + point[0].ToString() + "', LNG = '" + point[1].ToString()
                    + "' where ISDELETE = 0 and PGUID = '" + pguid + "'";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ORGCENTERDATA (PGUID, S_UDTIME, UNITEID, LAT, LNG) values('" + pguid + "', '"
                    + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pguid + "', '" + point[0].ToString()
                    + "', '" + point[1].ToString() + "')";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }
            PictureBox PB = (PictureBox)sender;
            PB.BorderStyle = BorderStyle.None;
            XtraMessageBox.Show("中心点设置成功!");
        }

        private string[] Get_Map_List()
        {
            string[] lists = null;
            if (!Directory.Exists(MapPath + "//roadmap"))
            {
                XtraMessageBox.Show("未导入地图文件!请在数据管理菜单导入地图");
                /*FileReader.often_ahp.CloseConn();
                FileReader.line_ahp.CloseConn();
                FileReader.log_ahp.CloseConn();
                System.Environment.Exit(0);*/
                return null;
            }
            lists = Directory.GetDirectories(MapPath + "//roadmap");

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
            GL_POLY = new Dictionary<string, Dictionary<string, Polygon>>();

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
                //GL_UPGUID[pguid] = dt.Rows[i]["UPGUID"].ToString();
                GL_NAME_PGUID[dt.Rows[i]["JDNAME"].ToString()] = pguid;
            }
            FileReader.once_ahp.CloseConn();

            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb");
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + pguid + "' and UNITEID = '" + unitid + "'";
                DataTable dt1 = FileReader.once_ahp.ExecuteDataTable(sql, null);
                if (dt1.Rows.Count > 0)
                    GL_MAP.Add(pguid, dt1.Rows[0]["MAPLEVEL"].ToString());
                else
                    GL_MAP.Add(pguid, string.Empty);
            }
            FileReader.once_ahp.CloseConn();

            GL_List = new List<GL_Node>();
            treeList1.Nodes.Clear();
            treeList1.Appearance.FocusedCell.BackColor = System.Drawing.Color.SteelBlue;
            treeList1.KeyFieldName = "pguid";
            treeList1.ParentFieldName = "upguid";
            FileReader.once_ahp = new AccessHelper(WorkPath + "data\\PersonMange.mdb");
            sql = "select PGUID, UPPGUID, ORGNAME, ULEVEL from RG_单位注册 where ISDELETE = 0 and PGUID = '" + unitid + "'";
            dt = FileReader.once_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                GL_Node pNode = new GL_Node();

                pNode.pguid = dt.Rows[i]["PGUID"].ToString();
                pNode.upguid = dt.Rows[i]["UPPGUID"].ToString();
                GL_UPGUID[pNode.pguid] = dt.Rows[i]["UPPGUID"].ToString();
                pNode.Name = dt.Rows[i]["ORGNAME"].ToString();
                pNode.level = dt.Rows[i]["ULEVEL"].ToString();
                GL_List.Add(pNode);
                Add_Unit_Node(pNode);
            }
            treeList1.DataSource = GL_List;
            treeList1.HorzScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Auto;
            treeList1.Columns[1].Visible = false;
            treeList1.Columns[2].Visible = false; 
            treeList1.Columns[3].Visible = false;
            treeList1.Columns[4].Visible = false;
            treeList1.Columns[5].Visible = false;
            treeList1.ExpandAll();
            FileReader.once_ahp.CloseConn();
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
                GL_List.Add(pNode);
                Add_Unit_Node(pNode);
            }
        }

        private void Load_Border(string u_guid)
        {
            borList = new Dictionary<string, List<double[]>>();
            borderDic = new Dictionary<string, object>();
            LineData new_borData = new LineData();
            new_borData.Load_Line("边界线");
            if (new_borData.Type != null)
                borData = new_borData;
            borderDic.Add("type", borData.Type);
            borderDic.Add("width", borData.Width);
            borderDic.Add("color", borData.Color);
            borderDic.Add("opacity", borData.Opacity);

            string sql = "select LAT, LNG, BORDERGUID from BORDERDATA where ISDELETE = 0 and UNITID = '" + u_guid + "' order by SHOWINDEX";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["BORDERGUID"].ToString();
                if (borList.ContainsKey(pguid))
                    borList[pguid].Add(new double[] { double.Parse(dt.Rows[i]["LAT"].ToString()), double.Parse(dt.Rows[i]["LNG"].ToString()) });
                else
                {
                    borList[pguid] = new List<double[]>();
                    borList[pguid].Add(new double[] { double.Parse(dt.Rows[i]["LAT"].ToString()), double.Parse(dt.Rows[i]["LNG"].ToString()) });
                }
            }

            if (!GL_UPGUID.ContainsKey(u_guid))
                return;
            
            /*if (dt.Rows.Count <= 0)
                Load_Border(GL_UPGUID[u_guid]);
            else
                borderDic.Add("path", borList);*/
        }

        private Dictionary<string, List<double[]>> Get_Border_Line(string pguid)
        {
            string tmp_guid = pguid;
            Dictionary<string, List<double[]>> res = new Dictionary<string, List<double[]>>();
            //List<double[]> res = new List<double[]>();
            string sql = "select LAT, LNG, BORDERGUID from BORDERDATA where ISDELETE = 0 and UNITID = '" + pguid + "' order by SHOWINDEX";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string borguid = dt.Rows[i]["BORDERGUID"].ToString();
                if (res.ContainsKey(borguid))
                    res[borguid].Add(new double[] { double.Parse(dt.Rows[i]["LAT"].ToString()), double.Parse(dt.Rows[i]["LNG"].ToString()) });
                else
                {
                    res[borguid] = new List<double[]>();
                    res[borguid].Add(new double[] { double.Parse(dt.Rows[i]["LAT"].ToString()), double.Parse(dt.Rows[i]["LNG"].ToString()) });
                }
            }
            /*while (res.Count <= 0)
            {
                if (!GL_UPGUID.ContainsKey(tmp_guid))
                    return res;
                tmp_guid = GL_UPGUID[tmp_guid];
                res = Get_Border_Line(tmp_guid);
            }*/
            if (!GL_POLY.ContainsKey(pguid))
                return res;
            foreach (var item in res)
                GL_POLY[pguid][item.Key] = new Polygon(item.Value);
            return res;
        }

        private void DrawBorder()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            string dlabel = "fb66d40b-50fa-4d88-8156-c590328004cb";
            string arealabel = "e62844cb-2839-4b49-853a-250e11ec1901";
            dic["color"] = borData.Color;
            dic["weight"] = 0;
            dic["fillColor"] = "#C0C0C0";
            dic["fillOpacity"] = 0.5;
            

            if (borList.Count > 1)
            {
                dic["fillColor"] = "#CCFF33";
                foreach (var item in borList) 
                {
                    dic["points"] = item.Value;
                    //mapHelper1.LightArea(arealabel, dic);
                }
            }
            else
            {
                foreach (var item in borList)
                {
                    dic["points"] = item.Value;
                    //mapHelper1.DarkOuter(dlabel, dic);
                }                
            }

            TreeListNode pNode = treeList1.FocusedNode;
            string pguid = pNode.GetValue("pguid").ToString();
            Dictionary<string, List<double[]>> bor_line = Get_Border_Line(pguid);
            foreach (var item in bor_line)
            {
                Dictionary<string, object> bdic = new Dictionary<string, object>();
                bdic["type"] = borData.Type;
                bdic["width"] = borData.Width;
                bdic["color"] = borData.Color;
                bdic["opacity"] = borData.Opacity;
                bdic["path"] = item.Value;
                mapHelper1.DrawBorder(pguid, bdic);
            }

            /*foreach (TreeListNode tln in pNode.Nodes)
            {
                pguid = tln.GetValue("pguid").ToString();
                bor_line = Get_Border_Line(pguid);
                bdic = new Dictionary<string, object>();
                bdic["type"] = "实线";
                bdic["width"] = 1;
                bdic["color"] = "#8B0000";
                bdic["opacity"] = 0.75;
                bdic["path"] = bor_line;
                mapHelper1.DrawBorder(pguid, bdic);
            }*/
        }

        private void EraseBorder()
        {
            TreeListNode pNode = treeList1.FocusedNode;
            mapHelper1.deleteMarker(pNode["pguid"].ToString());
            foreach (TreeListNode tln in pNode.Nodes)
                mapHelper1.deleteMarker(tln["pguid"].ToString());
        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            borList = new Dictionary<string, List<double[]>>();
            if (e.Node == null)
                return;
            levelguid = GL_NAME_PGUID[e.Node.GetValue("level").ToString()];
            string[] maps = GL_MAP[levelguid].Split(',');
            string sql = "select MAPLEVEL from ENVIRMAPDY_H0001Z000E00 where ISDELETE = 0 and UNITID = '"
                    + unitid + "' and PGUID = '" + pNode["pguid"].ToString() + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
                maps = dt.Rows[0]["MAPLEVEL"].ToString().Split(',');
            if (maps[0] != string.Empty)
                cur_level = int.Parse(maps[0]);
            else
                cur_level = int.Parse(folds[0]);

            label1.Text = "当前级别：" + GL_NAME[levelguid];
            Load_Border(e.Node.GetValue("pguid").ToString());
            if (!GL_POLY.ContainsKey(e.Node.GetValue("pguid").ToString()))
                GL_POLY[e.Node.GetValue("pguid").ToString()] = new Dictionary<string, Polygon>();
            foreach(var item in borList)
                GL_POLY[e.Node.GetValue("pguid").ToString()][item.Key] = new Polygon(item.Value);

            bool flag = false;

            sql = "select MARKELAT, MARKELNG from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and MAKRENAME like '%"
                + e.Node.GetValue("Name").ToString() + "%' and UNITEID = '" + unitid + "'";
            dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
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
                //XtraMessageBox.Show(mapHelper1.centerlat + ", " + mapHelper1.centerlng);
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

            if (Before_ShowMap == false)
                mapHelper1.ShowMap(cur_level, cur_level.ToString(), false, map_type, null, null, null, 1, 400);
            else
            {
                mapHelper1.setMapLevel(cur_level, cur_level.ToString());
                mapHelper1.SetMapCenter(mapHelper1.centerlat, mapHelper1.centerlng);
                EraseBorder();
                DrawBorder();
            }

            Before_ShowMap = true;
        }

        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                TreeListNode pNode = treeList1.GetNodeAt(e.X, e.Y);
                if (pNode != null)
                {
                    popupMenu1.ShowPopup(barManager1, MousePosition);
                    MapX = MousePosition.X;
                    MapY = MousePosition.Y;
                    treeList1.FocusedNode = pNode;
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MapLevelForm mplvf = new MapLevelForm();
            TreeListNode pNode = treeList1.FocusedNode;
            levelguid = GL_NAME_PGUID[pNode["level"].ToString()];
            mplvf.StartPosition = FormStartPosition.Manual;
            mplvf.Left = MapX;
            mplvf.Top = MapY;
            if (mplvf.Top + mplvf.Height > this.Height)
                mplvf.Top -= mplvf.Height;
            mplvf.nodeid = pNode["pguid"].ToString();
            mplvf.unitid = unitid;
            mplvf.mapstring = GL_MAP[levelguid];
            mplvf.MapPath = MapPath;
            mplvf.ShowDialog();
            if (pNode.ParentNode != null)
                treeList1.FocusedNode = pNode.ParentNode;
            else if (pNode.Nodes[0] != null)
                treeList1.FocusedNode = pNode.Nodes[0];
            treeList1.FocusedNode = pNode;
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            /*borList = new Dictionary<string,List<double[]>>();
            TreeListNode pNode = treeList1.FocusedNode;
            if (xtraOpenFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string file = xtraOpenFileDialog1.FileName;
            string[] strAll = File.ReadAllLines(file);
            foreach (string str in strAll)
            {
                string[] split = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n', ';' });
                borList.Add(new double[] { double.Parse(split[1]), double.Parse(split[0]) });
            }
            borderDic["path"] = borList;
            string sql = "select PGUID from BORDERDATA where ISDELETE = 0 and UNITID = '" + pNode["pguid"].ToString() + "'";
            DataTable dt = FileReader.line_ahp.ExecuteDataTable(sql, null);
            string Event = "添加" + pNode["Name"].ToString() + "的边界线";
            if (dt.Rows.Count > 0)
            {
                sql = "update BORDERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ISDELETE = 1 where ISDELETE = 0 and UNITID = '" + pNode["pguid"].ToString() + "'";
                FileReader.line_ahp.ExecuteSql(sql, null);
                Event = "修改" + pNode["Name"].ToString() + "的边界线";
            }

            for (int i = 0; i < borList.Count; ++i)
            {
                sql = "insert into BORDERDATA (PGUID, S_UDTIME, UNITID, LAT, LNG, SHOWINDEX) values('" + Guid.NewGuid().ToString("B")
                    + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode["pguid"].ToString() + "', '" + borList[i][0]
                    + "', '" + borList[i][1] + "', '" + i.ToString() + "')";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }
            ComputerInfo.WriteLog("导入边界线", Event);
            mapHelper1.ShowMap(cur_level, cur_level.ToString(), false, map_type, null, null, null, 1, 400);*/
        }

        private void mapHelper1_MapTypeChanged(string mapType)
        {
            map_type = mapType;
        }

        private void mapHelper1_LevelChanged(int lastLevel, int currLevel, string showLevel)
        {
            now_level = currLevel;
            DrawBorder();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            if (xtraOpenFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string file = xtraOpenFileDialog1.FileName;
            string[] strAll = File.ReadAllLines(file);
            string newGUID = Guid.NewGuid().ToString("B");
            borList[newGUID] = new List<double[]>();
            foreach (string str in strAll)
            {
                string[] split = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n', ';' });
                borList[newGUID].Add(new double[] { double.Parse(split[1]), double.Parse(split[0]) });
            }
            borderDic["path"] = borList;
            for (int i = 0; i < borList[newGUID].Count; ++i)
            {
                string sql = "insert into BORDERDATA (PGUID, S_UDTIME, UNITID, LAT, LNG, SHOWINDEX, BORDERGUID) values('" + Guid.NewGuid().ToString("B")
                    + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode["pguid"].ToString() + "', '" + borList[newGUID][i][0]
                    + "', '" + borList[newGUID][i][1] + "', " + i.ToString() + ", '" + newGUID + "')";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }
            string Event = "添加" + pNode["Name"].ToString() + "的边界线";
            ComputerInfo.WriteLog("导入边界线", Event);
            mapHelper1.ShowMap(cur_level, cur_level.ToString(), false, map_type, null, null, null, 1, 400);
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            string sql = "update BORDERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', ISDELETE = 1 where ISDELETE = 0 and UNITID = '" + pNode["pguid"].ToString()
                + "' and BORDERGUID = '" + border_guid + "'";
            FileReader.line_ahp.ExecuteSql(sql, null);
            string Event = "删除" + pNode["Name"].ToString() + "的边界线";
            ComputerInfo.WriteLog("导入边界线", Event);
            mapHelper1.ShowMap(cur_level, cur_level.ToString(), false, map_type, null, null, null, 1, 400);
            borList = new Dictionary<string, List<double[]>>();
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            if (xtraOpenFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string sql = "update BORDERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', ISDELETE = 1 where ISDELETE = 0 and UNITID = '" + pNode["pguid"].ToString()
                + "' and BORDERGUID = '" + border_guid + "'";
            FileReader.line_ahp.ExecuteSql(sql, null);

            string file = xtraOpenFileDialog1.FileName;
            string[] strAll = File.ReadAllLines(file);
            string newGUID = Guid.NewGuid().ToString("B");
            borList[newGUID] = new List<double[]>();
            foreach (string str in strAll)
            {
                string[] split = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n', ';' });
                borList[newGUID].Add(new double[] { double.Parse(split[1]), double.Parse(split[0]) });
            }
            borderDic["path"] = borList;
            for (int i = 0; i < borList[newGUID].Count; ++i)
            {
                sql = "insert into BORDERDATA (PGUID, S_UDTIME, UNITID, LAT, LNG, SHOWINDEX, BORDERGUID) values('" + Guid.NewGuid().ToString("B")
                    + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode["pguid"].ToString() + "', '" + borList[newGUID][i][0]
                    + "', '" + borList[newGUID][i][1] + "', '" + i.ToString() + "', '" + newGUID +  "')";
                FileReader.line_ahp.ExecuteSql(sql, null);
            }

            string Event = "更新" + pNode["Name"].ToString() + "的边界线";
            ComputerInfo.WriteLog("导入边界线", Event);
            mapHelper1.ShowMap(cur_level, cur_level.ToString(), false, map_type, null, null, null, 1, 400);
        }

        string border_guid = "";
        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
            if (Check_InBorder(new dPoint(lat, lng)))
            {
                barButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                barButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
            }
            else
            {
                barButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                barButtonItem5.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            popupMenu2.ShowPopup(barManager1, MousePosition);
        }

        private bool Check_InBorder(dPoint p)
        {
            Polygon tmp = null;
            foreach (var item in borList)
            {
                tmp = new Polygon(item.Value);
                if (tmp.PointInPolygon(p))
                {
                    border_guid = item.Key;
                    return true;
                }
            }
            border_guid = "";
            return false;
        }

    }
}