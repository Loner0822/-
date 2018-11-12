using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace PublishSys
{
    public partial class PubForm : Form
    {        
        public PubForm()
        {
            InitializeComponent();
        }

        private AccessHelper ahp = null;
        private IniOperator inip = null;
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;//当前exe根目录
        private string AccessPath = AppDomain.CurrentDomain.BaseDirectory + "data\\通用单位注册.accdb";

        private void PubForm_Load(object sender, EventArgs e)
        {
            this.treeView1.HideSelection = false;
            this.treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);
            Get_TreeView();
        }

        private void Get_TreeView() {
            ahp = new AccessHelper(AccessPath);
            string sql = "select * from RG_单位注册";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            List<District> d_list = new List<District>();
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string id, pid, name;
                id = dt.Rows[i]["ORGID"].ToString();
                pid = dt.Rows[i]["UPORGID"].ToString();
                name = dt.Rows[i]["ORGNAME"].ToString();
                d_list.Add(new District(int.Parse(id), int.Parse(pid), name));
            }
            ahp.CloseConn();
            Add_Tree_Node(d_list);
            treeView1.ExpandAll();
        }

        private void Add_Tree_Node(List<District> d_list) {
            for (int i = 0; i < d_list.Count; ++i) {
                if (d_list[i].pid == 0) {
                    TreeNode pNode = new TreeNode();
                    pNode.Tag = d_list[i].id;
                    pNode.Text = d_list[i].name;
                    treeView1.Nodes.Add(pNode);
                    Add_Child_Node(d_list, pNode);
                }
            }
        }

        private void Add_Child_Node(List<District> d_list, TreeNode pNode)
        {
            for (int i = 0; i < d_list.Count; ++i)
            {
                if (d_list[i].pid == (int)pNode.Tag)
                {
                    TreeNode cNode = new TreeNode();
                    cNode.Tag = d_list[i].id;
                    cNode.Text = d_list[i].name;
                    pNode.Nodes.Add(cNode);
                    Add_Child_Node(d_list, cNode);
                }
            }
        }


        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色，只需要在TreeView失去焦点时选中节点仍然突显  
            return;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void 下载地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "mapdownload\\imaps.exe");
            p.WaitForExit();
        }

        private void 下载图符ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 导入单位ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string file = openFileDialog1.FileName;
            if (file == "")
                return;
            File.Copy(file, AccessPath, true);
            treeView1.Nodes.Clear();
            Get_TreeView();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadMapForm lmf = new LoadMapForm();
            if (lmf.ShowDialog() == DialogResult.OK) {
                button2.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            inip.WriteString("Public", "UnitName", pNode.Text);
            inip.WriteString("Pubilc", "UnitID", pNode.Tag.ToString());
            inip.WriteString("Public", "AppName", "环境信息化系统");
            inip.WriteString("版本号", "VerNum", textBox1.Text);
            inip = new IniOperator(WorkPath + "PackUp.ini");
            inip.WriteString("packup", "my_app_name", "");
        }

    }

    public class District
    {
        public int id { get; set; }
        public int pid { get; set; }
        public string name { get; set; }

        public District(int id, int pid, string name)
        {
            this.id = id;
            this.pid = pid;
            this.name = name;
        }
    }
}
