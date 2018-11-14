using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ucPropertyGrid
{
    public partial class ucMultiSelect : UserControl
    {
        public ucMultiSelect()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="allOptions">可选项</param>
        /// <param name="selectedOptions">已经勾选的选项</param>
        public ucMultiSelect(string allOptions, string selectedOptions)
            : this()
        {
            this.allOptions = allOptions;
            this.selectedOptions = selectedOptions;
        }


        /// <summary>
        /// 当前可选项
        /// </summary>
        private string allOptions = string.Empty;


        /// <summary>
        /// 当前选中项
        /// </summary>
        public string SelectedOptions
        {
            get
            {
                //return string.Join(",", listSelectedTreeNodes.Select(tn => tn.Text)); //  返回勾选的项

                var list = new List<TreeNode>();
                foreach (TreeNode item in treeView1.Nodes)
                {
                    if (item.Checked)
                    {
                        list.Add(item);
                    }
                }
                return string.Join(",", list.Select(tn => tn.Text)); //  返回勾选的项
            }
        }

        /// <summary>
        /// 当前选中的项
        /// </summary>
        private string selectedOptions;

        List<TreeNode> listSelectedTreeNodes = new List<TreeNode>();

        private void ucMultiSelect_Load(object sender, EventArgs e)
        {
            string strOptions = this.allOptions;

            string[] allOptions = strOptions.Split(',');
            string[] arrSelectedOptions = selectedOptions.Split(',');

            for (int i = 0; i < allOptions.Length; i++)
            {
                TreeNode tn = treeView1.Nodes.Add(allOptions[i]);

                foreach (var item in arrSelectedOptions)
                {
                    if (item == tn.Text)
                    {
                        tn.Checked = true;
                        break;
                    }
                }
            }

            if (allOptions.Length <= 10)
            {
                this.Height = allOptions.Length * 22;
            }
            else
            {
                this.Height = 220;
            }
        }




        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            // 利用键盘事件收缩下拉框
            API.keybd_event((byte)Keys.Escape, 0, 0, 0); // 按下
            API.keybd_event((byte)Keys.Escape, 0, 2, 0); // 抬起
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
