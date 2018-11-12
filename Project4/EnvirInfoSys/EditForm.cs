using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace EnvirInfoSys
{
    public partial class EditForm : Form
    {
        public string title = "";
        public string message = "";            

        public EditForm()
        {
            InitializeComponent();
        }

        private void EditForm_Load(object sender, EventArgs e)
        {

        }

        private void EditForm_Shown(object sender, EventArgs e)
        {
            textBox1.Text = title;
            dataGridView1.Rows.Clear();

            JavaScriptSerializer jss = new JavaScriptSerializer();
            var jarr = jss.Deserialize<Dictionary<string, object>>(message);
            if (jarr != null && jarr.Count > 0)
            {
                dataGridView1.Rows.Add(jarr.Count);
                int k = 0;
                foreach (var itm in jarr)
                {
                    dataGridView1.Rows[k].Cells[0].Value = itm.Key;
                    dataGridView1.Rows[k].Cells[1].Value = itm.Value;
                    k++;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) // 确定
        {
            title = textBox1.Text.Trim();
            if (title.Equals(""))
            {
                MessageBox.Show("请输入标题");
                textBox1.Focus();
                return;
            }
            message = "";
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                if (dataGridView1.Rows[r].Cells[0].Value == null || dataGridView1.Rows[r].Cells[1].Value == null)
                {
                    continue;
                }
                string name = dataGridView1.Rows[r].Cells[0].Value.ToString().Trim();
                string val = dataGridView1.Rows[r].Cells[1].Value.ToString().Trim();
                if (name.Equals("") || val.Equals(""))
                {
                    continue;
                }
                string tmp = "\"" + name + "\":\"" + val + "\"";
                message = message.Equals("") ? tmp : message + "," + tmp;
            }
            message = "{" + message + "}";
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e) //取消
        {
            this.DialogResult = DialogResult.Cancel;
        }


    }
}
