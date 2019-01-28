using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace EnvirInfoSys
{
    
    public partial class ucRegBox : DevExpress.XtraEditors.XtraUserControl
    {
        private Dictionary<string, string> show_nodes;
        private int level = -1;
        public string unitguid = "";
        public string unitname = "";

        public ucRegBox()
        {
            InitializeComponent();
        }

        private void ucRegBox_Load(object sender, EventArgs e)
        {
            
        }

        public void ucRegBox_Text(string text)
        {
            labelControl1.Text = text;
        }

        public void ucRegBox_Refresh(Dictionary<string, string> nodes, int unitlevel)
        {
            comboBoxEdit1.Enabled = true;
            show_nodes = nodes;
            level = unitlevel;
            comboBoxEdit1.Properties.Items.Clear();
            comboBoxEdit1.Properties.Items.Add("请选择");
            foreach (var item in nodes)
                comboBoxEdit1.Properties.Items.Add(item.Value);
            comboBoxEdit1.SelectedIndex = 0;
        }

        public void ucRegBox_SelectAndLock(string pguid, bool unLock)
        {
            for (int i = 0; i < comboBoxEdit1.Properties.Items.Count; ++i)
            {
                if (comboBoxEdit1.Properties.Items[i].ToString() == show_nodes[pguid])
                {
                    comboBoxEdit1.SelectedIndex = i;
                    unitguid = pguid;
                    unitname = show_nodes[pguid];
                    comboBoxEdit1.Enabled = unLock;
                    break;
                }
            }
        }

        public delegate void SelectedIndexChange(object sender, EventArgs e, string pguid, int unitlevel);
        public event SelectedIndexChange SelectedChange;
        private void Selected_Change(object sender, EventArgs e)
        {
            if (SelectedChange != null && level != -1)
            {
                string uname = comboBoxEdit1.SelectedItem.ToString();
                string pguid = "";
                if (uname == "请选择")
                {
                    unitname = "";
                    unitguid = "";
                }
                else
                {
                    foreach (var item in show_nodes)
                    {
                        if (item.Value == uname)
                        {
                            pguid = item.Key;
                            unitguid = pguid;
                            unitname = uname;
                            break;
                        }
                    }
                }
                SelectedChange((object)this, new EventArgs(), pguid, level);
            }
        }
    }
}
