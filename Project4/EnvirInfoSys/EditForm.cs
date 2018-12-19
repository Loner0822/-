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

namespace EnvirInfoSys_Demo
{
    public partial class EditForm : DevExpress.XtraEditors.XtraForm
    {
        public string EditText = "";

        public EditForm()
        {
            InitializeComponent();
        }

        private void EditForm_Shown(object sender, EventArgs e)
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
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

    }
}