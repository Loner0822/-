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

namespace EnvirInfoSys
{
    public partial class InfoEditForm : DevExpress.XtraEditors.XtraForm
    {
        public string EditText = "";
        public string markerguid = "";

        public InfoEditForm()
        {
            InitializeComponent();
        }

        private void InfoEditForm_Shown(object sender, EventArgs e)
        {
            textEdit1.Text = EditText;
            textEdit1.Focus();
            textEdit1.SelectAll();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            EditText = textEdit1.Text;
            if (EditText == "")
            {
                XtraMessageBox.Show("名称不可为空!");
                textEdit1.Focus();
                return;
            }
            string sql = "select PGUID from ENVIRLIST_H0001Z000E00 where ISDELETE = 0 and FUNCNAME = '" + EditText + "' and MARKERID = '" + markerguid + "'";
            DataTable dt = FileReader.list_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                XtraMessageBox.Show("菜单内名称不可重复!");
                textEdit1.Focus();
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


    }
}