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
        private int UnitID = -1;
        private int MapLevel;
        private Dictionary<string, string> GUID_Icon;
        private Dictionary<string, string> Icon_JDCode;
        private Dictionary<string, string> FDName_Value;   
        

        private void 数据备份ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataBF.exe");
        }

        private void 数据恢复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataHF.exe");
        }

        private void 数据同步ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataUP.exe");
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void mapHelper1_Load(object sender, EventArgs e)
        {
            //读入图标对应数据
            Icon_JDCode = new Dictionary<string, string>();
            ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            string sql = "select PGUID, JDCODE from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
                Icon_JDCode.Add(dt.Rows[i]["PGUID"].ToString(), dt.Rows[i]["JDCODE"].ToString());

            //控件属性初始化
            inip = new IniOperator(IniFilePath);
            string slat = inip.ReadString("mapproperties", "centerlat", "");
            string slng = inip.ReadString("mapproperties", "centerlng", "");

            mapHelper1.centerlat = double.Parse(slat); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(slng); //118.5784; //必须设置的属性,不能为空
            mapHelper1.webpath = WorkPath + "googlemap"; //必须设置的属性,不能为空
            mapHelper1.roadmappath = WorkPath + "googlemap\\map"; //必须设置的属性,不能为空
            mapHelper1.satellitemappath = WorkPath + "googlemap\\satellite"; //必须设置的属性,不能为空
            mapHelper1.iconspath = WorkPath + "PNGICONFOLDER"; //必须设置的属性,不能为空
            
            // 临时
            UnitID = 0;
            mapHelper1.maparr = new string[] { "13", "14", "15", "16" }; //必须设置的属性,不能为空

            Add_TreeNode();
        }

        public void Add_TreeNode() {
            //自已绘制 保证Treeview节点失去焦点后仍然突显
            this.treeView1.HideSelection = false;
            this.treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);

            inip = new IniOperator(IniFilePath);
            string sminl = inip.ReadString("mapproperties", "minlevel", "");
            string smaxl = inip.ReadString("mapproperties", "maxlevel", "");
            string scurrl = inip.ReadString("mapproperties", "currlevel", sminl);
            int minl, maxl, currl;
            minl = int.Parse(sminl);
            maxl = int.Parse(smaxl);
            currl = int.Parse(scurrl);

            for (int i = minl; i <= maxl; ++i)
            {
                TreeNode pNode = new TreeNode();
                pNode.Text = i.ToString();
                pNode.Tag = i;
                treeView1.Nodes.Add(pNode);
                if (i == currl)
                    treeView1.SelectedNode = pNode;
            }

        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色，只需要在TreeView失去焦点时选中节点仍然突显  
            return;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            GUID_Icon = new Dictionary<string, string>();
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();//标注list，从数据库获取
            MapLevel = (int)e.Node.Tag;
            ahp = new AccessHelper(AccessPath);
            string sql = "select * from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and MAPLEVEL = '" + MapLevel.ToString() + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++ i) {
                GUID_Icon[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["ICONGUID"].ToString();
                Dictionary<string, object> dic = new Dictionary<string, object>();//添加每个标注
                dic.Add("guid", dt.Rows[i]["PGUID"].ToString());//必须加载的标准属性，从数据库查询得到值
                dic.Add("name", dt.Rows[i]["MAKRENAME"].ToString());//必须加载的标准属性，从数据库查询得到值
                dic.Add("level", MapLevel.ToString());//必须加载的标准属性，从数据库查询得到值
                dic.Add("canedit", dt.Rows[i]["UNITEID"].ToString() == UnitID.ToString());//必须加载的标准属性，根据上层单位判断
                dic.Add("type", dt.Rows[i]["MARKETYPE"].ToString());//必须加载的标准属性，从数据库查询得到值
                dic.Add("lat", dt.Rows[i]["MARKELAT"].ToString());//必须加载的标准属性，从数据库查询得到值
                dic.Add("lng", dt.Rows[i]["MARKELNG"].ToString());//必须加载的标准属性，从数据库查询得到值
                string icon_path = WorkPath + "googlemap\\mapfiles\\icons\\bicon" + MapLevel.ToString() + "\\" + dt.Rows[i]["ICONGUID"].ToString() + ".png";
                dic.Add("iconpath", icon_path);//必须加载的标准属性
                dic.Add("message", /*sdic*/null);//必须加载，内容随便，此处无用
                lst.Add(dic);//给list添加一个标注
            }
            mapHelper1.ShowMap(MapLevel, true, lst);
        }

        private void mapHelper1_IconSelected(string level, string iconPath)
        {
            Icon_GUID = iconPath;//小图标选择事件
            //MessageBox.Show("1");
        }

        private void mapHelper1_MapMouseup(string Mousebutton, bool canedit, double lat, double lng, int x, int y, string markerguid)
        {
            if (markerguid.Equals("") && !Icon_GUID.Equals(""))
            {
                string iconguid = Path.GetFileNameWithoutExtension(Icon_GUID);
                DataForm dtf = new DataForm();
                dtf.Icon_GUID = iconguid;
                dtf.Node_GUID = markerguid;
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
            string sql = "update ENVIRICONDATA_H0001Z000E00 set MARKELAT = '" + lat.ToString() + "', MARKELNG = '" + lng.ToString() + "' where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);
        }

        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
           // contextMenuStrip1.Show(x, y);
        }

        private string Delete_GUID = "";
        private void mapHelper1_MarkerRightClick(int sx, int sy, string level, string sguid, string name, bool canedit, string message)
        {
            contextMenuStrip1.Show(sx + groupBox2.Left, sy + groupBox2.Top + 80);
            Delete_GUID = sguid;
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Delete_GUID != "") {
                mapHelper1.deleteMarker(Delete_GUID);
            }
        }

        private void mapHelper1_AddMarkerFinished(string markerguid, double lat, double lng, string name, bool canEdit, string iconpath, string message)
        {
            // 添加完成事件，调用addMarker后触发
            // 数据库  insert
            ahp = new AccessHelper(AccessPath);
            string iconguid = Path.GetFileNameWithoutExtension(iconpath);
            string sql = "insert into ENVIRICONDATA_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, MAPLEVEL, MARKELAT, MARKELNG, MAKRENAME, UNITEID) values('" 
                         + markerguid + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + iconguid + "', '" + MapLevel.ToString()
                         + "', '" + lat.ToString() + "', '" + lng.ToString() + "', '" + name + "', '" + UnitID.ToString() + "')";
            ahp.ExecuteSql(sql, null);
            GUID_Icon[markerguid] = iconguid;
            string table_name = Icon_JDCode[iconguid];
            sql = "insert into " + table_name + " (PGUID, S_UDTIME";
            //bool flag = false;
            foreach (string key in FDName_Value.Keys) 
                sql += ", " + key;
            System.DateTime curTime = new System.DateTime();
            curTime = System.DateTime.Now;
            sql += ") values('" + markerguid + "', '" + curTime.ToString("yyyy-MM-dd hh:mm:ss");
            
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
            string sql = "update ENVIRICONDATA_H0001Z000E00 set ISDELETE = 1 where ISDELETE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);

            string icon = GUID_Icon[markerguid];
            string table_name = Icon_JDCode[icon];
            sql = "update " + table_name + " set ISDELETE = 1 where ISDELEtE = 0 and PGUID = '" + markerguid + "'";
            ahp.ExecuteSql(sql, null);

            Delete_GUID = "";
        }

        private void mapHelper1_ModifyMarkerFinished(string markerguid, double lat, double lng, string name, bool canEdit, string iconpath, string message)
        {
            // 更新完成事件，调用ModifyMarker后触发
            // 数据库  update 
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
