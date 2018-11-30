using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvirInfoSys
{
    public partial class LoginForm : Form
    {
        public int Mode;
        private string PassWord1 = "1";
        private string PassWord2 = "2";

        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == PassWord1)
            {
                this.DialogResult = DialogResult.OK;
                Mode = 1;
                this.Close();
            }
            else if (textBox1.Text == PassWord2)
            {
                this.DialogResult = DialogResult.OK;
                Mode = 2;
                this.Close();
            }
            else 
            {
                MessageBox.Show("密码错误!");
                textBox1.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
