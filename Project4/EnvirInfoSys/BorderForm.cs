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
    public partial class BorderForm : Form
    {
        public LineData borData = null;
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
                textBox3.Visible = false;
                textBox4.Visible = false;
                label6.Visible = false;
                label7.Visible = false;
            }
            else
            {
                if (!IsLine)
                {
                    textBox3.Text = borData.lng.ToString();
                    textBox4.Text = borData.lat.ToString();
                }
                else
                {
                    textBox3.Text = borData.lng.ToString();
                    textBox4.Text = borData.lat.ToString();
                    textBox3.Enabled = false;
                    textBox4.Enabled = false;
                }
            }

            if (borData.Type == "实线")
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;

            textBox2.Text = borData.Width.ToString();

            textBox1.BackColor = ColorTranslator.FromHtml(borData.Color);

            label5.Text = borData.Opacity.ToString("f");
            trackBar1.Value = (int) (borData.Opacity * 20);

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                borData.Type = "实线";
            else
                borData.Type = "虚线";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == false)
                borData.Type = "实线";
            else
                borData.Type = "虚线";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            borData.Width = int.Parse(textBox2.Text);
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
                textBox1.BackColor = colorDialog1.Color;
                borData.Color = ColorTranslator.ToHtml(colorDialog1.Color);
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label5.Text = (trackBar1.Value / 20.0).ToString("f");
            borData.Opacity = trackBar1.Value / 20.0;
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
                borData.lng = double.Parse(textBox3.Text);
            else
                borData.lng = 0;
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
                borData.lat = double.Parse(textBox4.Text);
            else
                borData.lat = 0;
        }
    }
}
