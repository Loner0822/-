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
    public partial class HelpForm : DevExpress.XtraEditors.XtraForm
    {
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory; // 当前exe根目录        

        public HelpForm()
        {
            InitializeComponent();
        }

        private void HelpForm_Shown(object sender, EventArgs e)
        {
            webBrowser1.Url = new Uri(WorkPath + "help.mht");
        }
    }
}