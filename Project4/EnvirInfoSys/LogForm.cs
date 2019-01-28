using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;

namespace EnvirInfoSys
{
    public partial class LogForm : DevExpress.XtraEditors.XtraForm
    {
        public string unitid = "";
        private DataTable Log_dt;

        public LogForm()
        {
            InitializeComponent();
        }

        private void LogForm_Shown(object sender, EventArgs e)
        {
            Log_dt = new DataTable();
            Log_dt.Columns.Add("序号", typeof(int));
            Log_dt.Columns.Add("操作用户", typeof(string));
            Log_dt.Columns.Add("操作时间", typeof(string));
            Log_dt.Columns.Add("操作类型", typeof(string));
            Log_dt.Columns.Add("操作内容", typeof(string));

            string sql = "select * from LOG_H0001Z000E00 where ISDELETE = 0 and UNITID = '" + unitid + "'order by RUNTIME desc";
            DataTable dt = FileReader.log_ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                Log_dt.Rows.Add(new object[] {
                    i + 1,
                    dt.Rows[i]["OSNAME"].ToString(),
                    dt.Rows[i]["RUNTIME"].ToString(),
                    dt.Rows[i]["EVENT"].ToString(),
                    dt.Rows[i]["REMARK"].ToString()
                });
            }
            gridControl1.DataSource = Log_dt;
            GridView view = gridControl1.MainView as GridView;
            view.OptionsBehavior.Editable = false;
            gridView1.Columns[0].Width = 40;
            gridView1.Columns[1].Width = 150;
            gridView1.Columns[2].Width = 200;
            gridView1.Columns[3].Width = 150;
        }
    }
}