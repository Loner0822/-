using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvirInfoSys
{
    public partial class ClassifyForm : Form
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

        public ClassifyForm()
        {
            InitializeComponent();
        }

        private void ClassifyForm_Load(object sender, EventArgs e)
        {
            // 初始化表格控件
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToResizeRows = false;
            DataGridViewTextBoxColumn index = new DataGridViewTextBoxColumn();
            index.Name = "index";
            index.HeaderText = "序号";
            dataGridView1.Columns.Add(index);
            dataGridView1.Columns["index"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["index"].ReadOnly = true;
            dataGridView1.Columns["index"].Frozen = true;
            DataGridViewTextBoxColumn GX_type = new DataGridViewTextBoxColumn();
            GX_type.Name = "type";
            GX_type.DataPropertyName = "type";
            GX_type.HeaderText = "显示名称";
            dataGridView1.Columns.Add(GX_type);
            dataGridView1.Columns["type"].SortMode = DataGridViewColumnSortMode.NotSortable;
            DataGridViewTextBoxColumn GX_guid = new DataGridViewTextBoxColumn();
            GX_guid.Name = "guid";
            GX_guid.DataPropertyName = "guid";
            GX_guid.HeaderText = "PGUID";
            GX_guid.Visible = false;
            dataGridView1.Columns.Add(GX_guid);
            dataGridView1.Columns["guid"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["index"].Width = 37;
            dataGridView1.Columns["type"].Width = 63;

            tabControl1.Controls.Clear();
            ahp2 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            
            string sql = "select UPGUID, PROPVALUE from ZSK_PROP_H0001Z000K00 where ISDELETE = 0 and PROPNAME = '图符库' order by PROPVALUE, SHOWINDEX";
            DataTable dt = ahp2.ExecuteDataTable(sql, null);
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
                for (int j = 0; j < tabControl1.TabPages.Count; ++j)
                {
                    if (tabControl1.TabPages[j].Name == Name)
                    {
                        flag = true;
                        FlowLayoutPanel flp = (FlowLayoutPanel)tabControl1.TabPages[j].Controls[0];
                        Add_Icon(flp, pguid);
                    }
                }
                if (flag == false)
                {
                    tabControl1.TabPages.Add(Name);
                    _index = tabControl1.TabPages.Count - 1;
                    FlowLayoutPanel flp = new FlowLayoutPanel();
                    flp.Dock = DockStyle.Fill;
                    flp.FlowDirection = FlowDirection.LeftToRight;
                    flp.WrapContents = true;
                    flp.AutoScroll = true;
                    flp.MouseDown += dataGridView_MouseDown;
                    Add_Icon(flp, pguid);
                    tabControl1.TabPages[_index].Name = Name;
                    tabControl1.TabPages[_index].BackColor = SystemColors.Control;
                    tabControl1.TabPages[_index].Controls.Add(flp);
                }
            }
            ahp2.CloseConn();
        }

        private void Add_Icon(FlowLayoutPanel flp, string pguid)
        {
            string icon_path = WorkPath + "ICONDER\\b_PNGICON\\";
            ucPictureBox ucPB = new ucPictureBox();
            string sql = "select JDNAME from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 and PGUID = '" + pguid + "'";
            DataTable dt1 = ahp2.ExecuteDataTable(sql, null);
            if (dt1.Rows.Count > 0)
            {
                 ucPB.Parent = flp;
                 ucPB.Name = pguid;
                 ucPB.IconName = dt1.Rows[0]["JDNAME"].ToString();
                 ucPB.IconPguid = pguid;
                 ucPB.IconPath = icon_path + pguid + ".png";
                 ucPB.Single_Click += Icon_SingleClick;
                 ucPB.Double_Click += Icon_DoubleClick;
                 //ucPB.MouseDown += dataGridView_MouseDown;
                 ucPB.IconCheck = false;
            }
        }

        private void ClassifyForm_Shown(object sender, EventArgs e)
        {
            ahp1 = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            ahp2 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
            ahp3 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K01.mdb");
            ahp4 = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000E00.mdb");
            
            // 读取管辖类型
            string sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + gxguid + "' order by SHOWINDEX";
            DataTable dt = ahp1.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                dgvr.CreateCells(dataGridView1);
                dgvr.Cells[0].Value = i + 1;
                dgvr.Cells[1].Value = dt.Rows[i]["FLNAME"].ToString();
                dgvr.Cells[2].Value = dt.Rows[i]["PGUID"].ToString();
                dataGridView1.Rows.Add(dgvr);
            }
        }

        private void Show_Icon_List(string flguid)
        {
            string icon_path = WorkPath + "ICONDER\\b_PNGICON\\";
            string sql = "select PGUID, JDNAME from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            DataTable dt = ahp2.ExecuteDataTable(sql, null);
            string first_guid = "";
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
                        if (first_guid == "")
                            first_guid = pguid;
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
            if (flowLayoutPanel1.Controls.Count > 0)
                Icon_SingleClick(flowLayoutPanel1.Controls[0], new EventArgs(), first_guid);
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int cur_index = dataGridView1.SelectedRows[0].Index;
                if (cur_index != 0 && dataGridView1.Rows[cur_index - 1].Cells["type"].Value == null)
                {
                    dataGridView1.CurrentCell = dataGridView1.Rows[cur_index - 1].Cells["type"];
                    dataGridView1.BeginEdit(false);
                }
                if (dataGridView1.SelectedRows[0].Cells["guid"].Value != null)
                {
                    flowLayoutPanel1.Controls.Clear();
                    string pguid = dataGridView1.SelectedRows[0].Cells["guid"].Value.ToString();
                    Show_Icon_List(pguid);
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int cur_index = e.RowIndex;
            if (cur_index < 0)
                return;
            if (dataGridView1.Rows[cur_index].Cells["guid"].Value != null)
            {
                if (dataGridView1.Rows[cur_index].Cells["type"].Value == null)
                {
                    MessageBox.Show("该值不可为空!");
                    return;
                }
                // 修改
                string sql = "update ENVIRGXFL_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', FLNAME = '"
                    + dataGridView1.Rows[cur_index].Cells["type"].Value.ToString() + "' where ISDELETE = 0 and PGUID = '" + dataGridView1.Rows[cur_index].Cells["guid"].Value.ToString() + "'";
                ahp1.ExecuteSql(sql, null);
                dataGridView1.Rows[cur_index].Cells["index"].Value = cur_index + 1;
            }
            else
            {
                if (dataGridView1.Rows[cur_index].Cells["type"].Value == null)
                {
                    MessageBox.Show("该值不可为空!");
                    return;
                }
                // 添加
                string pguid = Guid.NewGuid().ToString("B");
                //dataGridView1.Rows[cur_index].Cells[1].Value = pguid;
                string sql = "insert into ENVIRGXFL_H0001Z000E00 (PGUID, S_UDTIME, FLNAME, UPGUID, SHOWINDEX) values ('" + pguid + "', '"
                    + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + dataGridView1.Rows[cur_index].Cells["type"].Value.ToString()
                    + "', '" + gxguid + "', " + cur_index.ToString() + ")";
                ahp1.ExecuteSql(sql, null);
                dataGridView1.Rows[cur_index].Cells["guid"].Value = pguid;
                dataGridView1.Rows[cur_index].Cells["index"].Value = cur_index + 1;
            }
        }

        private void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip2.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip3.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pguid = dataGridView1.SelectedRows[0].Cells["guid"].Value.ToString();
            foreach (ucPictureBox item in flowLayoutPanel1.Controls)
            {
                string sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '" + item.IconPguid + "' and FLGUID = '" + pguid + "'";
                ahp1.ExecuteSql(sql, null);
            }
            flowLayoutPanel1.Controls.Clear();
        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pguid = dataGridView1.SelectedRows[0].Cells["guid"].Value.ToString();
            int _index = tabControl1.SelectedIndex;
            FlowLayoutPanel flp = (FlowLayoutPanel)tabControl1.TabPages[_index].Controls[0];
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

                string sql = "select PGUID from ENVIRGXDY_H0001Z000E00 where ICONGUID = '" + item.IconPguid + "' and FLGUID = '" + pguid + "'";
                DataTable dt = ahp1.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '" + item.IconPguid + "' and FLGUID = '" + pguid + "'";
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
            string pguid = dataGridView1.SelectedRows[0].Cells["guid"].Value.ToString();
            ucPictureBox tmp = (ucPictureBox)sender;
            if (tmp.Parent == this.flowLayoutPanel1)
            {
                Control Remove_PB = (Control)tmp;
                flowLayoutPanel1.Controls.Remove(Remove_PB);

                // 删除对应
                string sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '" + iconguid + "' and FLGUID = '" + pguid + "'";
                ahp1.ExecuteSql(sql, null);
            }
            else
            {
                foreach (ucPictureBox ucPB in this.flowLayoutPanel1.Controls)
                    if (ucPB.IconPguid == tmp.IconPguid)
                    {
                        MessageBox.Show("已添加该图符!");
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
                
                string sql = "select PGUID from ENVIRGXDY_H0001Z000E00 where ICONGUID = '" + iconguid + "' and FLGUID = '" + pguid + "'";
                DataTable dt = ahp1.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '" + iconguid + "' and FLGUID = '" + pguid + "'";
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
            Get_Property_Data(dataGridView2, iconguid, typeguid);
            
            // 加载基础属性
            typeguid = "{B55806E6-9D63-4666-B6EB-AAB80814648E}";
            Get_Property_Data(dataGridView3, iconguid, typeguid);

            // 加载扩展属性
            typeguid = "{A86C80C1-DC07-414F-826F-B52B8FC14A9C}";
            Get_Property_Data(dataGridView4, iconguid, typeguid);
        }

        private void Get_Property_Data(DataGridView dgv, string iconguid, string typeguid)
        {
            Prop_GUID = new List<string> ();
            Show_Name = new Dictionary<string, string>();
            Show_FDName = new Dictionary<string, string>();
            inherit_GUID = new Dictionary<string, string>();
            Show_Value = new Dictionary<string, string>();
            dgv.Columns.Clear();
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
                DataGridViewTextBoxColumn propNode = new DataGridViewTextBoxColumn();
                propNode.Name = pguid;
                propNode.HeaderText = Show_Name[pguid];
                dgv.Columns.Add(propNode);
                propValue.Add(Show_Value[pguid]);
            }
            if (flag)
                dgv.Rows.Add(propValue.ToArray());
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage tbpg = tabControl1.SelectedTab;
            FlowLayoutPanel flp = (FlowLayoutPanel)tbpg.Controls[0];
            foreach (ucPictureBox ucPB in flp.Controls)
                ucPB.IconCheck = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cur_index = dataGridView1.SelectedRows[0].Index;
            if (cur_index >= dataGridView1.Rows.Count - 1)
                return;
            if (dataGridView1.CurrentRow != null)
            {
                string pguid = dataGridView1.Rows[cur_index].Cells["guid"].Value.ToString();
                dataGridView1.Rows.RemoveAt(cur_index);
                for (int i = cur_index; i < dataGridView1.Rows.Count - 1; ++i)
                    dataGridView1.Rows[i].Cells["index"].Value = i + 1;
                string sql = "update ENVIRGXFL_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "' where ISDELETE = 0 and PGUID = '" + pguid + "'";
                ahp1.ExecuteSql(sql, null);
                sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "' where ISDELETE = 0 and FLGUID = '" + pguid + "'";
                ahp1.ExecuteSql(sql, null);
            }
        }

        private void 置顶ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cur_index = dataGridView1.SelectedRows[0].Index;
            if (cur_index >= dataGridView1.Rows.Count - 1)
                return;
            if (dataGridView1.CurrentRow != null)
            {
                string tmp_guid = dataGridView1.Rows[cur_index].Cells["guid"].Value.ToString();
                string tmp_type = dataGridView1.Rows[cur_index].Cells["type"].Value.ToString();
                for (int i = cur_index - 1; i >= 0; --i)
                {
                    dataGridView1.Rows[i + 1].Cells["guid"].Value = dataGridView1.Rows[i].Cells["guid"].Value;
                    dataGridView1.Rows[i + 1].Cells["type"].Value = dataGridView1.Rows[i].Cells["type"].Value;
                }
                dataGridView1.Rows[0].Cells["guid"].Value = tmp_guid;
                dataGridView1.Rows[0].Cells["type"].Value = tmp_type;
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells["index"];
                for (int i = 0; i <= cur_index; ++i)
                {
                    string sql = "update ENVIRGXFL_H0001Z000E00 set SHOWINDEX = '" + 
                        dataGridView1.Rows[i].Cells["index"].Value.ToString() +
                        "', S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + 
                        "' where ISDELETE = 0 and PGUID ='" +
                        dataGridView1.Rows[i].Cells["guid"].Value.ToString() + "'";
                    ahp1.ExecuteSql(sql, null);
                }
            }
        }

        private void 上移ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cur_index = dataGridView1.SelectedRows[0].Index;
            if (cur_index >= dataGridView1.Rows.Count - 1 || cur_index <= 0)
                return;
            if (dataGridView1.CurrentRow != null)
            {
                string tmp_guid = dataGridView1.Rows[cur_index].Cells["guid"].Value.ToString();
                string tmp_type = dataGridView1.Rows[cur_index].Cells["type"].Value.ToString();
                dataGridView1.Rows[cur_index].Cells["guid"].Value = dataGridView1.Rows[cur_index - 1].Cells["guid"].Value;
                dataGridView1.Rows[cur_index].Cells["type"].Value = dataGridView1.Rows[cur_index - 1].Cells["type"].Value;
                dataGridView1.Rows[cur_index - 1].Cells["guid"].Value = tmp_guid;
                dataGridView1.Rows[cur_index - 1].Cells["type"].Value = tmp_type;
                dataGridView1.CurrentCell = dataGridView1.Rows[cur_index - 1].Cells["index"];
                for (int i = cur_index - 1; i <= cur_index; ++i)
                {
                    string sql = "update ENVIRGXFL_H0001Z000E00 set SHOWINDEX = '" +
                        dataGridView1.Rows[i].Cells["index"].Value.ToString() +
                        "', S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + 
                        "' where ISDELETE = 0 and PGUID ='" +
                        dataGridView1.Rows[i].Cells["guid"].Value.ToString() + "'";
                    ahp1.ExecuteSql(sql, null);
                }
            }
        }

        private void 下移ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int cur_index = dataGridView1.SelectedRows[0].Index;
            if (cur_index >= dataGridView1.Rows.Count - 2)
                return;
            if (dataGridView1.CurrentRow != null)
            {
                string tmp_guid = dataGridView1.Rows[cur_index].Cells["guid"].Value.ToString();
                string tmp_type = dataGridView1.Rows[cur_index].Cells["type"].Value.ToString();
                dataGridView1.Rows[cur_index].Cells["guid"].Value = dataGridView1.Rows[cur_index + 1].Cells["guid"].Value;
                dataGridView1.Rows[cur_index].Cells["type"].Value = dataGridView1.Rows[cur_index + 1].Cells["type"].Value;
                dataGridView1.Rows[cur_index + 1].Cells["guid"].Value = tmp_guid;
                dataGridView1.Rows[cur_index + 1].Cells["type"].Value = tmp_type;
                dataGridView1.CurrentCell = dataGridView1.Rows[cur_index + 1].Cells["index"];
                for (int i = cur_index; i <= cur_index + 1; ++i)
                {
                    string sql = "update ENVIRGXFL_H0001Z000E00 set SHOWINDEX = '" +
                        dataGridView1.Rows[i].Cells["index"].Value.ToString() +
                        "', S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                        "' where ISDELETE = 0 and PGUID ='" +
                        dataGridView1.Rows[i].Cells["guid"].Value.ToString() + "'";
                    ahp1.ExecuteSql(sql, null);
                }
            }
        }

        private void ClassifyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ahp1.CloseConn();
            ahp2.CloseConn();
            ahp3.CloseConn();
        }

    }
}
