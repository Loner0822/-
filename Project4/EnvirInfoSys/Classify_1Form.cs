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
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils.DragDrop;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using System.Diagnostics;

namespace EnvirInfoSys
{
    public partial class Classify_1Form : DevExpress.XtraEditors.XtraForm
    {
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;    //当前exe根目录
        private AccessHelper ahp1 = null;       // ENVIR_H0001Z000E00.mdb
        private AccessHelper ahp2 = null;       // ZSK_H0001Z000K00.mdb
        private AccessHelper ahp3 = null;       // ZSK_H0001Z000K01.mdb
        private AccessHelper ahp4 = null;       // ZSK_H0001Z000E00.mdb
        public string unitid = "";
        public string gxguid = "-1";

        private List<string> Prop_GUID;                     // 属性GUID
        private Dictionary<string, string> Show_Name;       // 属性名称
        private Dictionary<string, string> Show_FDName;     // 属性表名
        private Dictionary<string, string> inherit_GUID;    // 继承属性GUID
        private Dictionary<string, string> Show_Value;      // 属性值
        private DataTable GX_dt;

        public Classify_1Form()
        {
            InitializeComponent();
        }

        public void SetUpGrid(GridControl grid, DataTable table)
        {
            GridView view = grid.MainView as GridView;
            grid.DataSource = table;
            view.OptionsBehavior.Editable = false;
        }

        public DataTable FillTable()
        {
            GX_dt = new DataTable();
            GX_dt.Columns.Add("guid", typeof(string));
            GX_dt.Columns.Add("序号", typeof(int));
            GX_dt.Columns.Add("显示名称", typeof(string));

            // 读取管辖类型
            string sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + gxguid + "' order by SHOWINDEX";
            DataTable dt = ahp1.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
                GX_dt.Rows.Add(new object[] { dt.Rows[i]["PGUID"].ToString(), i + 1, dt.Rows[i]["FLNAME"].ToString() });
            gridControl1.DataSource = GX_dt;
            gridView1.Columns[0].Visible = false;
            gridView1.Columns[1].Width = 50;
            return GX_dt;
        }

        public void HandleBehaviorDragDropEvents()
        {
            DragDropBehavior gridControlBehavior = behaviorManager1.GetBehavior<DragDropBehavior>(this.gridView1);
            gridControlBehavior.DragDrop += Behavior_DragDrop;
            gridControlBehavior.DragOver += Behavior_DragOver;
            gridControlBehavior.EndDragDrop += Behavior_EndDragDrop;
        }

        private void Behavior_DragDrop(object sender, DevExpress.Utils.DragDrop.DragDropEventArgs e)
        {
            GridView targetGrid = e.Target as GridView;
            GridView sourceGrid = e.Source as GridView;
            if (e.Action == DragDropActions.None || targetGrid != sourceGrid)
                return;
            DataTable sourceTable = sourceGrid.GridControl.DataSource as DataTable;

            Point hitPoint = targetGrid.GridControl.PointToClient(Cursor.Position);
            GridHitInfo hitInfo = targetGrid.CalcHitInfo(hitPoint);

            int[] sourceHandles = e.GetData<int[]>();

            int targetRowHandle = hitInfo.RowHandle;
            int targetRowIndex = targetGrid.GetDataSourceRowIndex(targetRowHandle);

            List<DataRow> draggedRows = new List<DataRow>();
            foreach (int sourceHandle in sourceHandles)
            {
                int oldRowIndex = sourceGrid.GetDataSourceRowIndex(sourceHandle);
                DataRow oldRow = sourceTable.Rows[oldRowIndex];
                draggedRows.Add(oldRow);
            }

            int newRowIndex;

            switch (e.InsertType)
            {
                case InsertType.Before:
                    newRowIndex = targetRowIndex > sourceHandles[sourceHandles.Length - 1] ? targetRowIndex - 1 : targetRowIndex;
                    for (int i = draggedRows.Count - 1; i >= 0; i--)
                    {
                        DataRow oldRow = draggedRows[i];
                        DataRow newRow = sourceTable.NewRow();
                        newRow.ItemArray = oldRow.ItemArray;
                        sourceTable.Rows.Remove(oldRow);
                        sourceTable.Rows.InsertAt(newRow, newRowIndex);
                    }
                    break;
                case InsertType.After:
                    newRowIndex = targetRowIndex < sourceHandles[0] ? targetRowIndex + 1 : targetRowIndex;
                    for (int i = 0; i < draggedRows.Count; i++)
                    {
                        DataRow oldRow = draggedRows[i];
                        DataRow newRow = sourceTable.NewRow();
                        newRow.ItemArray = oldRow.ItemArray;
                        sourceTable.Rows.Remove(oldRow);
                        sourceTable.Rows.InsertAt(newRow, newRowIndex);
                    }
                    break;
                default:
                    newRowIndex = -1;
                    break;
            }
            int insertedIndex = targetGrid.GetRowHandle(newRowIndex);
            targetGrid.FocusedRowHandle = insertedIndex;
            targetGrid.SelectRow(targetGrid.FocusedRowHandle);
        }

        private void Behavior_DragOver(object sender, DragOverEventArgs e)
        {
            DragOverGridEventArgs args = DragOverGridEventArgs.GetDragOverGridEventArgs(e);
            e.InsertType = args.InsertType;
            e.InsertIndicatorLocation = args.InsertIndicatorLocation;
            e.Action = args.Action;
            Cursor.Current = args.Cursor;
            args.Handled = true;
        }

        private void Behavior_EndDragDrop(object sender, EndDragDropEventArgs e)
        {
            for (int i = 0; i < gridView1.RowCount; ++i)
            {
                DataRow dr = gridView1.GetDataRow(i);
                dr["序号"] = i + 1;
                string sql = "update ENVIRGXFL_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "', SHOWINDEX = " + (i + 1).ToString() + " where ISDELETE = 0 and UPGUID = '" + gxguid
                    + "' and PGUID = '" + dr["guid"].ToString() + "'";
                ahp1.ExecuteSql(sql, null);
            }
        }

        private void Classify_1Form_Load(object sender, EventArgs e)
        {
            
            ahp1 = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            ahp2 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            ahp3 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K01.mdb");
            ahp4 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000E00.mdb");
            SetUpGrid(this.gridControl1, FillTable());
            HandleBehaviorDragDropEvents();
            xtraTabControl2.TabPages[0].PageVisible = false;

            // 初始化图符库
            xtraTabControl1.Controls.Clear();
            Build_Icon_Library("H0001Z000K00");
            Build_Icon_Library("H0001Z000E00");
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

        private void Get_Icon_From_Access(string flguid, string database)
        {
            AccessHelper ahp;
            if (database == "H0001Z000K00")
                ahp = ahp2;
            else
                ahp = ahp4;
            string icon_path = WorkPath + "ICONDER\\b_PNGICON\\";
            string sql = "select PGUID, JDNAME from ZSK_OBJECT_" + database + " where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                string name = dt.Rows[i]["JDNAME"].ToString();
                if (File.Exists(icon_path + pguid + ".png"))
                {
                    sql = "select PGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and ICONGUID = '" + pguid + "' and FLGUID = '" + flguid + "'";
                    DataTable dt1 = ahp1.ExecuteDataTable(sql, null);
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
            if (tbpg == null)
                return;
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

            string pguid = gridView1.GetFocusedDataRow()["guid"].ToString();
            ucPictureBox tmp = (ucPictureBox)sender;
            if (tmp.Parent == this.flowLayoutPanel1)
            {
                Control Remove_PB = (Control)tmp;
                flowLayoutPanel1.Controls.Remove(Remove_PB);

                // 删除对应
                string sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "' where ICONGUID = '" + iconguid + "' and FLGUID = '" + pguid + "' and UNITID = '" + unitid + "'";
                ahp1.ExecuteSql(sql, null);
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

                string sql = "select PGUID from ENVIRGXDY_H0001Z000E00 where ICONGUID = '" + iconguid
                    + "' and FLGUID = '" + pguid + "' and UNITID = '" + unitid + "'";
                DataTable dt = ahp1.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        + "' where ICONGUID = '" + iconguid + "' and FLGUID = '" + pguid + "' and UNITID = '" + unitid + "'";
                    ahp1.ExecuteSql(sql, null);
                }
                else
                {
                    sql = "insert into ENVIRGXDY_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, FLGUID, UNITID) values ('" + Guid.NewGuid().ToString("B")
                        + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + iconguid + "', '" + pguid + "', '" + unitid + "')";
                    ahp1.ExecuteSql(sql, null);
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

        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            flowLayoutPanel1.Controls.Clear();
            if (gridView1.GetFocusedDataRow() == null)
                return;
            string pguid = gridView1.GetFocusedDataRow()["guid"].ToString();
            Show_Icon_List(pguid);
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
            string pguid = gridView1.GetFocusedDataRow()["guid"].ToString();
            foreach (ucPictureBox item in flowLayoutPanel1.Controls)
            {
                string sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "' where ICONGUID = '" + item.IconPguid + "' and FLGUID = '" + pguid + "' and UNITID = '" + unitid + "'";
                ahp1.ExecuteSql(sql, null);
            }
            flowLayoutPanel1.Controls.Clear();
        }
        // 全选
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string pguid = gridView1.GetFocusedDataRow()["guid"].ToString();
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

                string sql = "select PGUID from ENVIRGXDY_H0001Z000E00 where ICONGUID = '"
                    + item.IconPguid + "' and FLGUID = '" + pguid + "' and UNITID = '" + unitid + "'";
                DataTable dt = ahp1.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        + "' where ICONGUID = '" + item.IconPguid + "' and FLGUID = '" + pguid + "' and UNITID = '" + unitid + "'";
                    ahp1.ExecuteSql(sql, null);
                }
                else
                {
                    sql = "insert into ENVIRGXDY_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, FLGUID, UNITID) values ('" + Guid.NewGuid().ToString("B")
                        + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + item.IconPguid + "', '" + pguid + "', '" + unitid + "')";
                    ahp1.ExecuteSql(sql, null);
                }
            }
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                ucPictureBox first_PB = (ucPictureBox)flowLayoutPanel1.Controls[0];
                string first_guid = first_PB.IconPguid;
                Icon_SingleClick(flowLayoutPanel1.Controls[0], new EventArgs(), first_guid);
            }
        }

        // 添加
        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int cnt = gridView1.RowCount + 1;
            DataRow dr = GX_dt.NewRow();
            string pguid = Guid.NewGuid().ToString("B");
            dr["guid"] = pguid;
            EditForm edfm = new EditForm();
            if (edfm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dr["显示名称"] = edfm.EditText;
                dr["序号"] = cnt;
                string sql = "insert into ENVIRGXFL_H0001Z000E00 (PGUID, S_UDTIME, FLNAME, UPGUID, SHOWINDEX) values ('" + pguid + "', '"
                    + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + edfm.EditText + "', '" + gxguid + "', " + cnt.ToString() + ")";
                ahp1.ExecuteSql(sql, null);
                GX_dt.Rows.Add(dr);
                gridView1.FocusedRowHandle = cnt - 1;
            }
        }

        // 删除
        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int cur_index = gridView1.FocusedRowHandle;
            string pguid = gridView1.GetFocusedDataRow()["guid"].ToString();
            string sql = "update ENVIRGXFL_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "' where ISDELETE = 0 and PGUID = '" + pguid + "'";
            ahp1.ExecuteSql(sql, null);
            sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" +
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                "' where ISDELETE = 0 and FLGUID = '" + pguid + "'";
            ahp1.ExecuteSql(sql, null);
            gridView1.DeleteSelectedRows();
            for (int i = cur_index; i < gridView1.RowCount; ++i)
                gridView1.GetDataRow(i)["序号"] = i + 1;
        }

        // 编辑
        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            EditForm edfm = new EditForm();
            edfm.EditText = gridView1.GetFocusedDataRow()["显示名称"].ToString();
            if (edfm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                gridView1.GetFocusedDataRow()["显示名称"] = edfm.EditText;
                string sql = "update ENVIRGXFL_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "', FLNAME = '" + edfm.EditText + "' where ISDELETE = 0 and PGUID = '"
                    + gridView1.GetFocusedDataRow()["guid"].ToString() + "'";
                ahp1.ExecuteSql(sql, null);
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

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                popupMenu3.ShowPopup(barManager1, MousePosition);
            }
        }

        private void Classify_1Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            ahp1.CloseConn();
            ahp2.CloseConn();
            ahp3.CloseConn();
            ahp4.CloseConn();
        }

        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Classify_2Form clcfm = new Classify_2Form();
            clcfm.unitid = unitid;
            clcfm.ShowDialog();

            string Event = "修改图符对应设置";
            ComputerInfo.WriteLog("图符对应设置", Event);
        }

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Process p = Process.Start(WorkPath + "tfkzdy.exe");
            p.WaitForExit();

            ahp2.CloseConn();
            ahp4.CloseConn();
            ahp2 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            ahp4 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000E00.mdb");



            xtraTabControl1.Controls.Clear();
            xtraTabControl1.TabPages.Clear();
            Build_Icon_Library("H0001Z000K00");
            Build_Icon_Library("H0001Z000E00");

            flowLayoutPanel1.Controls.Clear();
            string pguid = gridView1.GetFocusedDataRow()["guid"].ToString();
            Show_Icon_List(pguid);
        }
    }
}