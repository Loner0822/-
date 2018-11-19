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

namespace PublishSys
{
    public partial class HintForm : Form
    {
        public string hinttext = "";                                        // 提示信息

        public HintForm()
        {
            InitializeComponent();
        }

        private void HintForm_Shown(object sender, EventArgs e)
        {
            label1.Text = hinttext;
        }

        public void Get_New_Text()
        {
            label1.Text = hinttext;
            panel2.Refresh();
            panel3.Refresh();
        }
    }
}
