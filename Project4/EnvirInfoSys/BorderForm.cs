using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvirInfoSys_Demo
{
    public partial class BorderForm : DevExpress.XtraEditors.XtraForm
    {
        public ToPointData borData = new ToPointData();
        public bool IsPoint = false;
        public bool IsLine = false;

        public BorderForm()
        {
            InitializeComponent();
        }

        private void BorderForm_Shown(object sender, EventArgs e)
        {
            if (!IsPoint)
            {
                textEdit2.Visible = false;
                textEdit3.Visible = false;
                labelControl5.Visible = false;
                labelControl6.Visible = false;
            }
            else
            {
                if (!IsLine)
                {
                    textEdit2.Text = borData.lng.ToString();
                    textEdit3.Text = borData.lat.ToString();
                }
                else
                {
                    textEdit2.Text = borData.lng.ToString();
                    textEdit3.Text = borData.lat.ToString();
                    textEdit2.Enabled = false;
                    textEdit3.Enabled = false;
                }
            }

            if (borData.line_data.Type == "实线")
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;

            textEdit1.Text = borData.line_data.Width.ToString();

            textEdit4.BackColor = ColorTranslator.FromHtml(borData.line_data.Color);

            labelControl7.Text = borData.line_data.Opacity.ToString("f");
            trackBar1.Value = (int)(borData.line_data.Opacity * 20);

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                borData.line_data.Type = "实线";
            else
                borData.line_data.Type = "虚线";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == false)
                borData.line_data.Type = "实线";
            else
                borData.line_data.Type = "虚线";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            borData.line_data.Width = int.Parse(textEdit1.Text);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b' && !Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                textEdit4.BackColor = colorDialog1.Color;
                borData.line_data.Color = ColorTranslator.ToHtml(colorDialog1.Color);
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            labelControl7.Text = (trackBar1.Value / 20.0).ToString("f");
            borData.line_data.Opacity = trackBar1.Value / 20.0;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textEdit2.Text != "")
                borData.lng = double.Parse(textEdit2.Text);
            else
                borData.lng = 0;
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textEdit3.Text != "")
                borData.lat = double.Parse(textEdit3.Text);
            else
                borData.lat = 0;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
