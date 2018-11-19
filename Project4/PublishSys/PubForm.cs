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
        private string AccessPath = AppDomain.CurrentDomain.BaseDirectory + "data\\PersonMange.mdb";
        private Dictionary<string, string> UnitID_Level;


        private void PubForm_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            this.treeView1.HideSelection = false;
            this.treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);
            Get_TreeView();
        }

        private void Get_TreeView() {
            if (!File.Exists(AccessPath))
                return;
            UnitID_Level = new Dictionary<string, string>();
            ahp = new AccessHelper(AccessPath);
            string sql = "select * from RG_单位注册 where ISDELETE = 0 and ULEVEL in ('国', '省', '市', '县')";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            List<District> d_list = new List<District>();
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string id, pid, name, level;
                id = dt.Rows[i]["PGUID"].ToString();
                pid = dt.Rows[i]["UPPGUID"].ToString();
                name = dt.Rows[i]["ORGNAME"].ToString();
                level = dt.Rows[i]["ULEVEL"].ToString();
                UnitID_Level.Add(id, level);
                d_list.Add(new District(id, pid, level, name));
            }
            Add_Tree_Node(d_list);
            treeView1.ExpandAll();
        }

        private void Add_Tree_Node(List<District> d_list) {
            for (int i = 0; i < d_list.Count; ++i) {
                if (d_list[i].pid == "0") {
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
                if (d_list[i].pid == pNode.Tag.ToString())
                {
                    TreeNode cNode = new TreeNode();
                    cNode.Tag = d_list[i].id;
                    cNode.Text = d_list[i].name;
                    pNode.Nodes.Add(cNode);
                    Add_Child_Node(d_list, cNode);
                }
            }
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色，只需要在TreeView失去焦点时选中节点仍然突显  
            return;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
        }

        private void 下载地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "mapdownload\\imaps.exe");
            p.WaitForExit();
        }

        private void 下载图符ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "Publish\\DataUp.exe", "");
            p.WaitForExit();
        }

        private bool Check_File(string file)
        {
            if (file == "")
                return false;
            if (file.IndexOf("PersonMange") < 0)
                return false;
            return true;
        }

        private void 导入单位ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string file = openFileDialog1.FileName;
            if (!Check_File(file))
                return;
            File.Copy(file, AccessPath, true);
            treeView1.Nodes.Clear();
            Get_TreeView();
        }

        private void 数据备份ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataBF.exe");
            p.WaitForExit();
        }

        private void 数据恢复ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataHF.exe");
            p.WaitForExit();
        }

        private void iP设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "Publish\\SetIP.exe");
            p.WaitForExit();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //LoadMapForm lmf = new LoadMapForm();
            //lmf.ShowDialog();
            
            MapForm mfm = new MapForm();
            TreeNode pNode = treeView1.SelectedNode;
            mfm.unitid = pNode.Tag.ToString();
            //mfm.unitname = pNode.Text;
            mfm.ShowDialog();                
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认是否已经导入过地图文件", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;
            Process p1 = Process.Start(WorkPath + "Publish\\CreatePng.exe", "0 -1");
            p1.WaitForExit();

            TreeNode pNode = treeView1.SelectedNode;
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            inip.WriteString("Public", "UnitName", pNode.Text);
            inip.WriteString("Public", "UnitLevel", UnitID_Level[pNode.Tag.ToString()]);
            inip.WriteString("Public", "UnitID", pNode.Tag.ToString());
            inip.WriteString("Public", "AppName", "环境信息化系统");
            inip.WriteString("版本号", "VerNum", textBox1.Text);

            inip = new IniOperator(WorkPath + "PackUp.ini");
            inip.WriteString("packup", "my_app_name", "环境信息化系统");
            inip.WriteString("packup", "my_app_version", textBox1.Text);
            inip.WriteString("packup", "my_app_publisher", pNode.Text);
            inip.WriteString("packup", "my_app_exe_name", "EnvirInfoSys.exe");
            inip.WriteString("packup", "my_app_id", "{" + Guid.NewGuid().ToString("B"));
            inip.WriteString("packup", "source_exe_path", WorkPath + "Publish\\EnvirInfoSys.exe");
            inip.WriteString("packup", "source_path", WorkPath + "Publish");
            inip.WriteString("packup", "registry_subkey", "环境信息化系统");

            Process p = Process.Start(WorkPath + "PackUp.exe");
            p.WaitForExit();

            MessageBox.Show("发布成功!");
        }
    }

    public class District
    {
        public string id { get; set; }
        public string pid { get; set; }
        public string level { get; set; }
        public string name { get; set; }

        public District(string id, string pid, string level, string name)
        {
            this.id = id;
            this.pid = pid;
            this.level = level;
            this.name = name;
        }
    }
}
