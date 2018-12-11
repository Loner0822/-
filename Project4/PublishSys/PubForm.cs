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
        private string AccessPath = AppDomain.CurrentDomain.BaseDirectory + "\\Publish\\data\\PersonMange.mdb";
        private Dictionary<string, string> UnitID_Level;


        private void PubForm_Load(object sender, EventArgs e)
        {
            DataGridViewTextBoxColumn id_num = new DataGridViewTextBoxColumn();
            id_num.Name = "ID";
            id_num.DataPropertyName = "ID";
            id_num.HeaderText = "序号";
            dataGridView1.Columns.Add(id_num);
            DataGridViewTextBoxColumn PubUnit = new DataGridViewTextBoxColumn();
            PubUnit.Name = "PubUnit";
            PubUnit.DataPropertyName = "PubUnit";
            PubUnit.HeaderText = "发布单位";
            dataGridView1.Columns.Add(PubUnit);
            DataGridViewTextBoxColumn PubTime = new DataGridViewTextBoxColumn();
            PubTime.Name = "PubTime";
            PubTime.DataPropertyName = "PubTime";
            PubTime.HeaderText = "发布时间";
            dataGridView1.Columns.Add(PubTime);
            DataGridViewTextBoxColumn PubSys = new DataGridViewTextBoxColumn();
            PubSys.Name = "PubSys";
            PubSys.DataPropertyName = "PubSys";
            PubSys.HeaderText = "发布系统";
            dataGridView1.Columns.Add(PubSys);
            DataGridViewTextBoxColumn PubVer = new DataGridViewTextBoxColumn();
            PubVer.Name = "PubVer";
            PubVer.DataPropertyName = "PubVer";
            PubVer.HeaderText = "系统版本";
            dataGridView1.Columns.Add(PubVer);

            DataGridViewTextBoxColumn PubGUID = new DataGridViewTextBoxColumn();
            PubGUID.Name = "PubPGUID";
            PubGUID.DataPropertyName = "PubGUID";
            PubGUID.HeaderText = "GUID";
            PubGUID.Visible = false;
            dataGridView1.Columns.Add(PubGUID);

            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Width = 100;
            dataGridView1.Columns[2].Width = 100;
            dataGridView1.Columns[3].Width = 160;
            dataGridView1.Columns[4].Width = 80;
            dataGridView1.Columns[5].Width = 0;

            ahp = new AccessHelper(WorkPath + "data\\PublishData.mdb");
            string sql = "select * from PUBLISH_H0001Z000E00 where ISDELETE = 0 order by S_UDTIME";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                dgvr.CreateCells(dataGridView1);
                dgvr.Cells[0].Value = i + 1;
                dgvr.Cells[1].Value = dt.Rows[i]["UNITNAME"].ToString();
                dgvr.Cells[2].Value = dt.Rows[i]["S_UDTIME"].ToString();
                dgvr.Cells[3].Value = dt.Rows[i]["UNITNAME"].ToString() + "环境信息化系统";
                dgvr.Cells[4].Value = dt.Rows[i]["VERSION"].ToString();
                dgvr.Cells[5].Value = dt.Rows[i]["PGUID"].ToString();
                dataGridView1.Rows.Add(dgvr);
            }
            ahp.CloseConn();

            button1.Enabled = false;
            button2.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
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
            ahp.CloseConn();
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
            if (treeView1.Nodes.Count > 0)
                treeView1.SelectedNode = treeView1.Nodes[0];
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
            textBox2.Enabled = true;
            textBox3.Enabled = true;
            TreeNode pNode = treeView1.SelectedNode;
            ahp = new AccessHelper(WorkPath + "Publish\\data\\经纬度注册.mdb");
            string sql = "select LAT, LNG from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '" + pNode.Tag.ToString() + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            ahp.CloseConn();
            if (dt.Rows.Count > 0 && dt.Rows[0]["LNG"].ToString() != string.Empty && dt.Rows[0]["LAT"].ToString() != string.Empty)
            {
                textBox2.Text = dt.Rows[0]["LNG"].ToString();
                textBox3.Text = dt.Rows[0]["LAT"].ToString();
            }
            else
            {
                textBox2.Text = "";
                textBox3.Text = "";
                if (MessageBox.Show("是否从网上获取 " + pNode.Text + " 的经纬度?", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string[] Point = mapHelper1.AddressToLocation(pNode.Text);
                    textBox2.Focus();
                    textBox2.Text = Point[1];
                    textBox3.Focus();
                    textBox3.Text = Point[0];
                    textBox2.Focus();
                }
            }
        }

        private void 下载地图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "mapdownload\\imaps.exe");
            p.WaitForExit();
        }

        private void 下载图符ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "Publish\\IconDataDown.exe", "");
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
            Process p = Process.Start(WorkPath + "Publish\\OrgDataDown.exe");
            p.WaitForExit();
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
            ahp.CloseConn();
            System.Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            if (textBox2.Text == "")
            {
                MessageBox.Show("请填写当前单位经度!");
                textBox2.Focus();
                return;
            }
            if (textBox3.Text == "")
            {
                MessageBox.Show("请填写当前单位纬度!");
                textBox3.Focus();
                return;
            }

            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlng", textBox2.Text);
            inip.WriteString("mapproperties", "centerlat", textBox3.Text);

            MapForm mfm = new MapForm();
            //TreeNode pNode = treeView1.SelectedNode;
            mfm.unitid = pNode.Tag.ToString();
            mfm.Text = "地图对应";
            // mfm.unitname = pNode.Text;
            mfm.ShowDialog();                
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("请填写当前单位经度!");
                textBox2.Focus();
                return;
            }
            if (textBox3.Text == "")
            {
                MessageBox.Show("请填写当前单位纬度!");
                textBox3.Focus();
                return;
            }

            TreeNode pNode = treeView1.SelectedNode;
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");

            if (MessageBox.Show("即将发布《" + pNode.Text + "环境信息化系统" + textBox1.Text + "》\n" + "确认是否已经导入过地图文件", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;
            //Process p1 = Process.Start(WorkPath + "Publish\\CreatePng.exe", "0 -1");
            //p1.WaitForExit();
            
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

            ahp = new AccessHelper(WorkPath + "Publish\\data\\PASSWORD_H0001Z000E00.mdb");
            string sql = "update PASSWORD_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', UNITID = '" + pNode.Tag.ToString() + "' where PWNAME = '管理员密码'";
            ahp.ExecuteSql(sql, null);
            sql = "update PASSWORD_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', UNITID = '" + pNode.Tag.ToString() + "' where PWNAME = '编辑模式'";
            ahp.ExecuteSql(sql, null);
            sql = "update PASSWORD_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', UNITID = '" + pNode.Tag.ToString() + "' where PWNAME = '查看模式'";
            ahp.ExecuteSql(sql, null);
            ahp.CloseConn();

            ahp = new AccessHelper(WorkPath + "Publish\\data\\ZSK_AppInfo.mdb");
            sql = "update APPINFO set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+ "', UNITID = '"
                + pNode.Tag.ToString() + "' where ISDELETE = 0 and PGUID = '{8C3B99C5-26D3-48B2-A676-250189FCEA2F}'";
            ahp.ExecuteSql(sql, null);
            ahp.CloseConn();

            Process p = Process.Start(WorkPath + "PackUp.exe");
            p.WaitForExit();
            if (p.ExitCode == -1)
            {
                MessageBox.Show("发布失败!");
                return;
            }

            string Now_Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string pguid = Guid.NewGuid().ToString("B");
            ahp = new AccessHelper(WorkPath + "data\\PublishData.mdb");
            sql = "insert into PUBLISH_H0001Z000E00 (PGUID, S_UDTIME, UNITID, UNITNAME, VERSION, SYSTEMNAME) values ('" +
                        pguid + "', '" + Now_Time + "', '" +
                        pNode.Tag.ToString() + "', '" + pNode.Text + "', '" + textBox1.Text + "', '" + pNode.Text + "环境信息化系统')";
            //sql = "insert into PUBLISH_H0001Z000E00 (PGUID, S_UDTIME, UNITID, UNITNAME, VERSION, SYSTEMNAME) values ('{cab7d79f-342d-49e4-aaac-86a9369ada82}', '2018-11-24 08:15:10', '1', '中华人民共和国', '1.00.00', '123')";
            ahp.ExecuteSql(sql, null);
            ahp.CloseConn();
            

            MessageBox.Show("发布成功!");
            int cnt = dataGridView1.Rows.Count;
            DataGridViewRow dgvr = new DataGridViewRow();
            dgvr.CreateCells(dataGridView1);
            dgvr.Cells[0].Value = cnt + 1;
            dgvr.Cells[1].Value = pNode.Text;
            dgvr.Cells[2].Value = Now_Time;
            dgvr.Cells[3].Value = pNode.Text + "环境信息化系统";
            dgvr.Cells[4].Value = textBox1.Text;
            dgvr.Cells[5].Value = pguid;
            dataGridView1.Rows.Add(dgvr);
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            ahp = new AccessHelper(WorkPath + "Publish\\data\\经纬度注册.mdb");
            string sql = "select PGUID from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '" + pNode.Tag.ToString() + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ORGCENTERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LNG = '" + textBox2.Text + "' where ISDELETE = 0 and UNITEID = '" + pNode.Tag.ToString() + "'";
                ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ORGCENTERDATA (PGUID, S_UDTIME, UNITEID, LNG) values('" + pNode.Tag.ToString() + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode.Tag.ToString() + "', '" + textBox2.Text + "')";
                ahp.ExecuteSql(sql, null);
            }
            ahp.CloseConn();
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            ahp = new AccessHelper(WorkPath + "Publish\\data\\经纬度注册.mdb");
            string sql = "select PGUID from ORGCENTERDATA where ISDELETE = 0 and UNITEID = '" + pNode.Tag.ToString() + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ORGCENTERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LAT = '" + textBox3.Text + "' where ISDELETE = 0 and UNITEID = '" + pNode.Tag.ToString() + "'";
                ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ORGCENTERDATA (PGUID, S_UDTIME, UNITEID, LAT) values('" + pNode.Tag.ToString() + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode.Tag.ToString() + "', '" + textBox3.Text + "')";
                ahp.ExecuteSql(sql, null);
            }
            ahp.CloseConn();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ahp = new AccessHelper(WorkPath + "data\\PublishData.mdb");
            string pguid = string.Empty, sql = string.Empty;
            for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; --i)
            {
                // 数据库操作
                int cur_row = dataGridView1.SelectedRows[i].Index;
                pguid = dataGridView1.Rows[cur_row].Cells[5].Value.ToString();
                sql = "update PUBLISH_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ISDELETE = 0 and PGUID = '" + pguid + "'";
                ahp.ExecuteSql(sql);
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[i]);
            }
            ahp.CloseConn();     
            for (int i = 0; i < dataGridView1.Rows.Count; ++i)
                dataGridView1.Rows[i].Cells[0].Value = i + 1;
        }

        private void 数据同步ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process p = Process.Start(WorkPath + "DataUP.exe", "PublishSys.exe 0");
            p.WaitForExit();
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
