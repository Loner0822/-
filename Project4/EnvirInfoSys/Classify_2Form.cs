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
using DevExpress.XtraTab;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace EnvirInfoSys
{
    public partial class Classify_2Form : DevExpress.XtraEditors.XtraForm
    {
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;    //当前exe根目录
        private AccessHelper ahp1 = null;       // ENVIR_H0001Z000E00.mdb
        private AccessHelper ahp2 = null;       // ZSK_H0001Z000K00.mdb
        private AccessHelper ahp3 = null;       // ZSK_H0001Z000K01.mdb
        private AccessHelper ahp4 = null;       // ZSK_H0001Z000E00.mdb
        private AccessHelper ahp5 = null;       // ENVIRDYDATA_H0001Z000E00.mdb
        public string unitid = "";
        public string gxguid = "-1";
        public int maxlevel = 0;

        private List<string> Prop_GUID;                     // 属性GUID
        private Dictionary<string, string> Show_Name;       // 属性名称
        private Dictionary<string, string> Show_FDName;     // 属性表名
        private Dictionary<string, string> inherit_GUID;    // 继承属性GUID
        private Dictionary<string, string> Show_Value;      // 属性值
        

        public Classify_2Form()
        {
            InitializeComponent();
        }

        private void Classify_1Form_Load(object sender, EventArgs e)
        {

            ahp1 = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            ahp2 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            ahp3 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K01.mdb");
            ahp4 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000E00.mdb");
            ahp5 = new AccessHelper(WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb");
            
            xtraTabControl2.TabPages[0].PageVisible = false;
            // 初始化图符库
            xtraTabControl1.Controls.Clear();
            Build_Icon_Library("H0001Z000K00");
            Build_Icon_Library("H0001Z000E00");

            // 获取管辖范围
            Load_Unit_Level();
        }

        private Dictionary<string, string> GL_NAME;
        private Dictionary<string, string> GL_JDCODE;
        private Dictionary<string, string> GL_UPGUID;
        private Dictionary<string, string> GL_MAP;
        private List<GL_Point> GL_List;
        private void Load_Unit_Level()
        {
            GL_NAME = new Dictionary<string, string>();
            GL_JDCODE = new Dictionary<string, string>();
            GL_UPGUID = new Dictionary<string, string>();
            GL_MAP = new Dictionary<string, string>();
            GL_List = new List<GL_Point>();
            string sql = "select PGUID, JDNAME, JDCODE, UPGUID from ZSK_OBJECT_H0001Z000K01 where ISDELETE = 0 and LEVELNUM >= " + maxlevel.ToString();
            DataTable dt = ahp3.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                GL_NAME[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["JDNAME"].ToString();
                GL_JDCODE[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["JDCODE"].ToString();
                GL_UPGUID[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["UPGUID"].ToString();
                GL_Point pNode = new GL_Point();
                pNode.Name = dt.Rows[i]["JDNAME"].ToString();
                pNode.pguid = dt.Rows[i]["PGUID"].ToString();
                pNode.upguid = dt.Rows[i]["UPGUID"].ToString();
                GL_List.Add(pNode);
            }

            treeList1.Nodes.Clear();
            treeList1.Appearance.FocusedCell.BackColor = System.Drawing.Color.SteelBlue;
            treeList1.KeyFieldName = "pguid";
            treeList1.ParentFieldName = "upguid";
            treeList1.DataSource = GL_List;
            treeList1.HorzScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Auto;
            treeList1.ExpandAll();
            if (treeList1.Nodes.Count > 0)
                treeList1.FocusedNode = treeList1.Nodes[0];
        }

        private void Add_Unit_Node(TreeNode pa)
        {
            foreach (var item in GL_UPGUID)
            {
                if (item.Value == pa.Tag.ToString())
                {
                    TreeNode pNode = new TreeNode();
                    pNode.Text = GL_NAME[item.Key];
                    pNode.Tag = item.Key;
                    pa.Nodes.Add(pNode);
                    Add_Unit_Node(pNode);
                }
            }
        }

        private void Build_Icon_Library(string database)
        {
            AccessHelper ahp = null;
            if (database == "H0001Z000K00")
                ahp = ahp2;
            else
                ahp = ahp4;
            string sql = "select UPGUID, PROPVALUE from ZSK_PROP_" + database + " where ISDELETE = 0 and PROPNAME = '图符库' order by PROPVALUE, SHOWINDEX";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string Name = dt.Rows[i]["PROPVALUE"].ToString();
                string pguid = dt.Rows[i]["UPGUID"].ToString();
                int _index = Name.IndexOf("图符库");
                if (_index < 0)
                    continue;
                Name = Name.Substring(0, _index);
                if (Name == "备用")
                    continue;
                bool flag = false;
                for (int j = 0; j < xtraTabControl1.TabPages.Count; ++j)
                {
                    if (xtraTabControl1.TabPages[j].Name == Name)
                    {
                        flag = true;
                        FlowLayoutPanel flp = (FlowLayoutPanel)xtraTabControl1.TabPages[j].Controls[0];
                        Add_Icon(flp, pguid, database);
                    }
                }
                if (flag == false)
                {
                    xtraTabControl1.TabPages.Add(Name);
                    _index = xtraTabControl1.TabPages.Count - 1;
                    FlowLayoutPanel flp = new FlowLayoutPanel();
                    flp.Dock = DockStyle.Fill;
                    flp.FlowDirection = FlowDirection.LeftToRight;
                    flp.WrapContents = true;
                    flp.AutoScroll = true;
                    flp.MouseDown += flowLayoutPanel_MouseDown;
                    Add_Icon(flp, pguid, database);
                    xtraTabControl1.TabPages[_index].Name = Name;
                    xtraTabControl1.TabPages[_index].BackColor = SystemColors.Control;
                    xtraTabControl1.TabPages[_index].Controls.Add(flp);
                }
            }
        }

        private void Add_Icon(FlowLayoutPanel flp, string pguid, string database)
        {
            AccessHelper ahp = null;
            if (database == "H0001Z000K00")
                ahp = ahp2;
            else
                ahp = ahp4;
            string icon_path = WorkPath + "ICONDER\\b_PNGICON\\";
            ucPictureBox ucPB = new ucPictureBox();
            string sql = "select JDNAME from ZSK_OBJECT_" + database + " where ISDELETE = 0 and PGUID = '" + pguid + "'";

            if (database == "H0001Z000E00")
                sql += " and UNITID = '" + unitid + "'";
            
            DataTable dt1 = ahp.ExecuteDataTable(sql, null);
            if (dt1.Rows.Count > 0)
            {
                ucPB.Parent = flp;
                ucPB.Name = pguid;
                ucPB.IconName = dt1.Rows[0]["JDNAME"].ToString();
                ucPB.IconPguid = pguid;
                ucPB.IconPath = icon_path + pguid + ".png";
                ucPB.Single_Click += Icon_SingleClick;
                ucPB.Double_Click += Icon_DoubleClick;
                ucPB.IconCheck = false;
            }
        }

        private void Get_Icon_From_Access(string levelguid, string database)
        {
            AccessHelper ahp;
            if (database == "H0001Z000K00")
                ahp = ahp2;
            else
                ahp = ahp4;
            string icon_path = WorkPath + "ICONDER\\b_PNGICON\\";
            string sql = "select PGUID, JDNAME from ZSK_OBJECT_" + database + " where ISDELETE = 0";

            if (database == "H0001Z000K00")
                sql += " order by LEVELNUM, SHOWINDEX";
            else
                sql += " and UNITID = '" + unitid + "' order by LEVELNUM, SHOWINDEX";
            
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                string name = dt.Rows[i]["JDNAME"].ToString();
                if (File.Exists(icon_path + pguid + ".png"))
                {
                    sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and ICONGUID = '" + pguid
                        + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                    DataTable dt1 = ahp5.ExecuteDataTable(sql, null);
                    if (dt1.Rows.Count > 0)
                    {
                        ucPictureBox ucPB = new ucPictureBox();
                        ucPB.Parent = this.flowLayoutPanel1;
                        ucPB.Name = pguid;
                        ucPB.IconName = name;
                        ucPB.IconPguid = pguid;
                        ucPB.IconPath = icon_path + pguid + ".png";
                        ucPB.Single_Click += Icon_SingleClick;
                        ucPB.Double_Click += Icon_DoubleClick;
                        ucPB.IconCheck = false;
                    }
                }
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            XtraTabPage tbpg = xtraTabControl1.SelectedTabPage;
            if (tbpg.Controls.Count > 0)
            {
                FlowLayoutPanel flp = (FlowLayoutPanel)tbpg.Controls[0];
                foreach (ucPictureBox ucPB in flp.Controls)
                    ucPB.IconCheck = false;
            }
        }

        private void Icon_SingleClick(object sender, EventArgs e, string iconguid)
        {
            ucPictureBox tmp = (ucPictureBox)sender;
            if (tmp.IconCheck == true)
                return;
            if (tmp.Parent == this.flowLayoutPanel1)
            {
                foreach (ucPictureBox ucPB in this.flowLayoutPanel1.Controls)
                    ucPB.IconCheck = false;
                tmp.IconCheck = true;
                // 显示属性
                Show_Icon_Property(iconguid);
            }
            else
            {
                FlowLayoutPanel flp = (FlowLayoutPanel)tmp.Parent;
                foreach (ucPictureBox ucPB in flp.Controls)
                    ucPB.IconCheck = false;
                tmp.IconCheck = true;
            }
        }

        private void Icon_DoubleClick(object sender, EventArgs e, string iconguid)
        {
            string levelguid = treeList1.FocusedNode["pguid"].ToString();
            ucPictureBox tmp = (ucPictureBox)sender;
            if (tmp.Parent == this.flowLayoutPanel1)
            {
                Control Remove_PB = (Control)tmp;
                flowLayoutPanel1.Controls.Remove(Remove_PB);

                // 删除对应
                string sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "' where ICONGUID = '" + iconguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                ahp5.ExecuteSql(sql, null);
            }
            else
            {
                foreach (ucPictureBox ucPB in this.flowLayoutPanel1.Controls)
                    if (ucPB.IconPguid == tmp.IconPguid)
                    {
                        XtraMessageBox.Show("已添加该图符!");
                        return;
                    }
                ucPictureBox new_PB = new ucPictureBox();
                new_PB.IconName = tmp.IconName;
                new_PB.IconPguid = tmp.IconPguid;
                new_PB.IconPath = tmp.IconPath;
                new_PB.IconCheck = false;
                new_PB.Single_Click += Icon_SingleClick;
                new_PB.Double_Click += Icon_DoubleClick;
                flowLayoutPanel1.Controls.Add(new_PB);

                string sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ICONGUID = '" + iconguid
                    + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                DataTable dt = ahp5.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        + "' where ICONGUID = '" + iconguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                    ahp5.ExecuteSql(sql, null);
                }
                else
                {
                    sql = "insert into ICONDUIYING_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, LEVELGUID, UNITEID) values ('" + Guid.NewGuid().ToString("B")
                        + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + iconguid + "', '" + levelguid + "', '" + unitid + "')";
                    ahp5.ExecuteSql(sql, null);
                }
            }
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                ucPictureBox first_PB = (ucPictureBox)flowLayoutPanel1.Controls[0];
                string first_guid = first_PB.IconPguid;
                Icon_SingleClick(flowLayoutPanel1.Controls[0], new EventArgs(), first_guid);
            }
        }

        private void Show_Icon_Property(string iconguid)
        {
            string typeguid = "";
            // 加载固定属性
            typeguid = "{26E232C8-595F-44E5-8E0F-8E0FC1BD7D24}";
            Get_Property_Data(gridControl2, gridView2, iconguid, typeguid);

            // 加载基础属性
            typeguid = "{B55806E6-9D63-4666-B6EB-AAB80814648E}";
            Get_Property_Data(gridControl3, gridView3, iconguid, typeguid);

            // 加载扩展属性
            typeguid = "{D7DE9C5E-253C-491C-A380-06E41C68D2C8}";
            Get_Property_Data(gridControl4, gridView4, iconguid, typeguid);
        }

        private void Get_Property_Data(GridControl gc, GridView gv, string iconguid, string typeguid)
        {
            Prop_GUID = new List<string>();
            Show_Name = new Dictionary<string, string>();
            Show_FDName = new Dictionary<string, string>();
            inherit_GUID = new Dictionary<string, string>();
            Show_Value = new Dictionary<string, string>();

            gc.DataSource = null;
            gv.Columns.Clear();
            DataTable sum_dt = new DataTable();
            string sql = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROPVALUE from ZSK_PROP_H0001Z000K00 where ISDELETE = 0 and UPGUID = '"
                + iconguid + "' and PROTYPEGUID = '" + typeguid + "' order by SHOWINDEX";
            DataTable dt = ahp2.ExecuteDataTable(sql, null);
            Add_Prop(dt);

            sql = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROPVALUE from ZSK_PROP_H0001Z000K01 where ISDELETE = 0 and UPGUID = '"
                + iconguid + "' and PROTYPEGUID = '" + typeguid + "' order by SHOWINDEX";
            dt = ahp3.ExecuteDataTable(sql, null);
            Add_Prop(dt);

            sql = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROPVALUE from ZSK_PROP_H0001Z000E00 where ISDELETE = 0 and UPGUID = '"
                + iconguid + "' and PROTYPEGUID = '" + typeguid + "' order by SHOWINDEX";
            dt = ahp4.ExecuteDataTable(sql, null);
            Add_Prop(dt);

            List<string> propValue = new List<string>();
            bool flag = false;
            for (int i = 0; i < Prop_GUID.Count; ++i)
            {
                string pguid = Prop_GUID[i];
                flag = true;
                sum_dt.Columns.Add(Show_Name[pguid]);
                propValue.Add(Show_Value[pguid]);
            }
            DataRow newRow = sum_dt.NewRow();
            if (flag)
            {
                for (int i = 0; i < propValue.Count; ++i)
                    newRow[i] = propValue[i];
            }
            sum_dt.Rows.Add(newRow);
            gc.DataSource = sum_dt;
        }

        private void Add_Prop(DataTable proptable)
        {
            for (int i = 0; i < proptable.Rows.Count; ++i)
            {
                Prop_GUID.Add(proptable.Rows[i]["PGUID"].ToString());
                Show_Name[proptable.Rows[i]["PGUID"].ToString()] = proptable.Rows[i]["PROPNAME"].ToString();
                Show_FDName[proptable.Rows[i]["PGUID"].ToString()] = proptable.Rows[i]["FDNAME"].ToString();
                inherit_GUID[proptable.Rows[i]["PGUID"].ToString()] = proptable.Rows[i]["SOURCEGUID"].ToString();
                Show_Value[proptable.Rows[i]["PGUID"].ToString()] = proptable.Rows[i]["PROPVALUE"].ToString();
            }
        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            string levelguid = treeList1.FocusedNode["pguid"].ToString();
            flowLayoutPanel1.Controls.Clear();
            Show_Icon_List(levelguid);
        }

        private void Show_Icon_List(string flguid)
        {
            Get_Icon_From_Access(flguid, "H0001Z000K00");
            Get_Icon_From_Access(flguid, "H0001Z000E00");
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                ucPictureBox tmp = (ucPictureBox)flowLayoutPanel1.Controls[0];
                Icon_SingleClick(flowLayoutPanel1.Controls[0], new EventArgs(), tmp.IconPguid);
            }
        }

        // 清空
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string levelguid = treeList1.FocusedNode["pguid"].ToString();
            foreach (ucPictureBox item in flowLayoutPanel1.Controls)
            {
                string sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "' where ICONGUID = '" + item.IconPguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                ahp5.ExecuteSql(sql, null);
            }
            flowLayoutPanel1.Controls.Clear();
        }

        // 全选
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string levelguid = treeList1.FocusedNode["pguid"].ToString();
            int _index = xtraTabControl1.SelectedTabPageIndex;
            FlowLayoutPanel flp = (FlowLayoutPanel)xtraTabControl1.TabPages[_index].Controls[0];
            foreach (ucPictureBox item in flp.Controls)
            {
                bool flag = false;
                foreach (ucPictureBox ucPB in flowLayoutPanel1.Controls)
                {
                    if (item.IconPguid == ucPB.IconPguid)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    continue;
                ucPictureBox new_PB = new ucPictureBox();
                new_PB.IconName = item.IconName;
                new_PB.IconPguid = item.IconPguid;
                new_PB.IconPath = item.IconPath;
                new_PB.IconCheck = false;
                new_PB.Single_Click += Icon_SingleClick;
                new_PB.Double_Click += Icon_DoubleClick;
                flowLayoutPanel1.Controls.Add(new_PB);

                string sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ICONGUID = '" + item.IconPguid
                    + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                DataTable dt = ahp5.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        + "' where ICONGUID = '" + item.IconPguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                    ahp5.ExecuteSql(sql, null);
                }
                else
                {
                    sql = "insert into ICONDUIYING_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, LEVELGUID, UNITEID) values ('" + Guid.NewGuid().ToString("B")
                        + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + item.IconPguid + "', '" + levelguid + "', '" + unitid + "')";
                    ahp5.ExecuteSql(sql, null);
                }
            }
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                ucPictureBox first_PB = (ucPictureBox)flowLayoutPanel1.Controls[0];
                string first_guid = first_PB.IconPguid;
                Icon_SingleClick(flowLayoutPanel1.Controls[0], new EventArgs(), first_guid);
            }
        }
        
        private void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                popupMenu1.ShowPopup(barManager1, MousePosition);
            }
        }

        private void flowLayoutPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                popupMenu2.ShowPopup(barManager1, MousePosition);
            }
        }

        private void Classify_1Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            ahp1.CloseConn();
            ahp2.CloseConn();
            ahp3.CloseConn();
            ahp4.CloseConn();
            ahp5.CloseConn();
        }

    }

    public class GL_Point
    {
        public string pguid { set; get; }
        public string upguid { set; get; }
        public string Name { set; get; }
    }
}