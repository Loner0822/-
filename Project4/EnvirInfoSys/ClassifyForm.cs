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
        private AccessHelper ahp = null;
        public string unitid = "";
        public string gxguid = "-1";

        public ClassifyForm()
        {
            InitializeComponent();
        }

        private void ClassifyForm_Shown(object sender, EventArgs e)
        {
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
            GX_type.HeaderText = "管辖类型";
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

            // 读取管辖类型
            ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            string sql = "select PGUID, FLNAME from ENVIRGXFL_H0001Z000E00 where ISDELETE = 0 and UPGUID = '" + gxguid + "' order by SHOWINDEX";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
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
            ahp = new AccessHelper(WorkPath + "data\\ZSK_H0001Z000K00.mdb");
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
                    ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
                    sql = "select PGUID from ENVIRGXDY_H0001Z000E00 where ISDELETE = 0 and ICONGUID = '" + pguid + "' and FLGUID = '" + flguid + "' and UNITID = '" + unitid + "'";
                    DataTable dt1 = ahp.ExecuteDataTable(sql, null);
                    if (dt1.Rows.Count > 0)
                        ucPB.IconCheck = true;
                    else
                        ucPB.IconCheck = false;
                }
            }
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
                ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
                string sql = "update ENVIRGXFL_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', FLNAME = '"
                    + dataGridView1.Rows[cur_index].Cells["type"].Value.ToString() + "' where ISDELETE = 0 and PGUID = '" + dataGridView1.Rows[cur_index].Cells["guid"].Value.ToString() + "'";
                ahp.ExecuteSql(sql, null);
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
                ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
                string sql = "insert into ENVIRGXFL_H0001Z000E00 (PGUID, S_UDTIME, FLNAME, UPGUID, SHOWINDEX) values ('" + pguid + "', '"
                    + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + dataGridView1.Rows[cur_index].Cells["type"].Value.ToString()
                    + "', '" + gxguid + "', " + cur_index.ToString() + ")";
                ahp.ExecuteSql(sql, null);
                dataGridView1.Rows[cur_index].Cells["guid"].Value = pguid;
                dataGridView1.Rows[cur_index].Cells["index"].Value = cur_index + 1;
                //dataGridView1.CurrentCell = dataGridView1.Rows[cur_index].Cells["index"];
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e, string iconguid, bool ischecked)
        {
            string pguid = dataGridView1.SelectedRows[0].Cells["guid"].Value.ToString();
            ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            if (ischecked)
            {
                string sql = "select PGUID from ENVIRGXDY_H0001Z000E00 where ICONGUID = '" + iconguid + "' and FLGUID = '" + pguid + "'";
                DataTable dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '" + iconguid + "' and FLGUID = '" + pguid + "'";
                    ahp.ExecuteSql(sql, null);
                }
                else
                {
                    sql = "insert into ENVIRGXDY_H0001Z000E00 (PGUID, S_UDTIME, ICONGUID, FLGUID, UNITID) values ('" + Guid.NewGuid().ToString("B")
                        + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + iconguid + "', '" + pguid + "', '" + unitid + "')";
                    ahp.ExecuteSql(sql, null);
                }
            }
            else
            {
                string sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '" + iconguid + "' and FLGUID = '" + pguid + "'";
                ahp.ExecuteSql(sql, null);
            }
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
                ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
                string sql = "update ENVIRGXFL_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "' where ISDELETE = 0 and PGUID = '" + pguid + "'";
                ahp.ExecuteSql(sql, null);
                sql = "update ENVIRGXDY_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "' where ISDELETE = 0 and FLGUID = '" + pguid + "'";
                ahp.ExecuteSql(sql, null);
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
                ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
                for (int i = 0; i <= cur_index; ++i)
                {
                    string sql = "update ENVIRGXFL_H0001Z000E00 set SHOWINDEX = '" + 
                        dataGridView1.Rows[i].Cells["index"].Value.ToString() +
                        "', S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + 
                        "' where ISDELETE = 0 and PGUID ='" +
                        dataGridView1.Rows[i].Cells["guid"].Value.ToString() + "'";
                    ahp.ExecuteSql(sql, null);
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
                ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
                for (int i = cur_index - 1; i <= cur_index; ++i)
                {
                    string sql = "update ENVIRGXFL_H0001Z000E00 set SHOWINDEX = '" +
                        dataGridView1.Rows[i].Cells["index"].Value.ToString() +
                        "', S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + 
                        "' where ISDELETE = 0 and PGUID ='" +
                        dataGridView1.Rows[i].Cells["guid"].Value.ToString() + "'";
                    ahp.ExecuteSql(sql, null);
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
                ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
                for (int i = cur_index; i <= cur_index + 1; ++i)
                {
                    string sql = "update ENVIRGXFL_H0001Z000E00 set SHOWINDEX = '" +
                        dataGridView1.Rows[i].Cells["index"].Value.ToString() +
                        "', S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                        "' where ISDELETE = 0 and PGUID ='" +
                        dataGridView1.Rows[i].Cells["guid"].Value.ToString() + "'";
                    ahp.ExecuteSql(sql, null);
                }
            }
        }

    }
}
