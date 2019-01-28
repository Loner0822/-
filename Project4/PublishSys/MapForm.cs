using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace PublishSys
{
    public partial class MapForm : Form
    { 
        public MapForm()
        {
            InitializeComponent();
        }

        private IniOperator inip = null;

        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;    //当前exe根目录
        private AccessHelper ahp1 = null;       // ENVIR_H0001Z000E00.mdb
        private AccessHelper ahp2 = null;       // ZSK_H0001Z000K00.mdb
        private AccessHelper ahp3 = null;       // ZSK_H0001Z000K01.mdb
        private AccessHelper ahp4 = null;       // ZSK_H0001Z000E00.mdb
        private AccessHelper ahp5 = null;       // 经纬度注册.mdb
        private AccessHelper ahp6 = null;       // ENVIRDYDATA_H0001Z000E00.mdb

        public string unitid = "";
        private string[] folds = null;
        private string map_type = "g_map";
        private bool Before_ShowMap = false;

        private List<string> Prop_GUID;                     // 属性GUID
        private Dictionary<string, string> Show_Name;       // 属性名称
        private Dictionary<string, string> Show_FDName;     // 属性表名
        private Dictionary<string, string> inherit_GUID;    // 继承属性GUID
        private Dictionary<string, string> Show_Value;      // 属性值

        private Dictionary<string, string> GUID_ICON = new Dictionary<string,string>();

        // 边界线
        Dictionary<string, object> borderDic = null;
        private Dictionary<string, List<double[]>> borList = new Dictionary<string, List<double[]>>();

        public int maxlevel = 0;

        private void MapForm_Load(object sender, EventArgs e)
        {
            tabControl2.Controls.Clear();
            ahp2 = new AccessHelper(WorkPath + "Publish\\data\\ZSK_H0001Z000K00.mdb");

            string sql = "select UPGUID, PROPVALUE from ZSK_PROP_H0001Z000K00 where ISDELETE = 0 and PROPNAME = '图符库' order by PROPVALUE, SHOWINDEX";
            DataTable dt = ahp2.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string Name = dt.Rows[i]["PROPVALUE"].ToString();
                string pguid = dt.Rows[i]["UPGUID"].ToString();
                int _index = Name.IndexOf("图符库");
                if (_index < 0)
                    continue;
                Name = Name.Substring(0, _index);
                if (Name == "备用")
                    continue;
                bool flag = false;
                for (int j = 0; j < tabControl2.TabPages.Count; ++j)
                {
                    if (tabControl2.TabPages[j].Name == Name)
                    {
                        flag = true;
                        FlowLayoutPanel flp = (FlowLayoutPanel)tabControl2.TabPages[j].Controls[0];
                        Add_Icon(flp, pguid);
                    }
                }
                if (flag == false)
                {
                    tabControl2.TabPages.Add(Name);
                    _index = tabControl2.TabPages.Count - 1;
                    FlowLayoutPanel flp = new FlowLayoutPanel();
                    flp.Dock = DockStyle.Fill;
                    flp.FlowDirection = FlowDirection.LeftToRight;
                    flp.WrapContents = true;
                    flp.AutoScroll = true;
                    flp.MouseDown += dataGridView_MouseDown;
                    Add_Icon(flp, pguid);
                    tabControl2.TabPages[_index].Name = Name;
                    tabControl2.TabPages[_index].BackColor = SystemColors.Control;
                    tabControl2.TabPages[_index].Controls.Add(flp);
                }
            }
            ahp2.CloseConn();
        }

        private void MapForm_Shown(object sender, EventArgs e)
        {
            ahp1 = new AccessHelper(WorkPath + "Publish\\data\\ENVIR_H0001Z000E00.mdb");
            ahp2 = new AccessHelper(WorkPath + "Publish\\data\\ZSK_H0001Z000K00.mdb");
            ahp3 = new AccessHelper(WorkPath + "Publish\\data\\ZSK_H0001Z000K01.mdb");
            ahp4 = new AccessHelper(WorkPath + "Publish\\data\\ZSK_H0001Z000E00.mdb");
            ahp5 = new AccessHelper(WorkPath + "Publish\\data\\经纬度注册.mdb");
            ahp6 = new AccessHelper(WorkPath + "Publish\\data\\ENVIRDYDATA_H0001Z000E00.mdb");

            checkedListBox1.Items.Clear();

            // 导入组织结构
            Load_Unit_Level();
            if (treeView1.Nodes.Count <= 0)
            {
                MessageBox.Show("未导入组织结构数据，即将关闭窗口");
                this.Close();
                return;
            }

            // 图符对应初始化

            // 地图对应初始化
            button3.Enabled = false;
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            textBox1.Text = inip.ReadString("mapproperties", unitid, "");
            textBox2.Text = inip.ReadString("mapproperties", "centerlng", "0");
            textBox3.Text = inip.ReadString("mapproperties", "centerlat", "0");
            if (textBox2.Text == "0" || textBox3.Text == "0")
                MessageBox.Show("获取不到当前经纬度");
            checkedListBox1.Items.Clear();
            if (textBox1.Text != string.Empty)
                Show_Map_List(textBox1.Text);

            // 导入边界线
            borList = new Dictionary<string, List<double[]>>();
            borderDic = new Dictionary<string, object>();
            borderDic.Add("type", "实线");
            borderDic.Add("width", 1);
            borderDic.Add("color", "#000000");
            borderDic.Add("opacity", 1);
            string sql = "select LAT, LNG, BORDERGUID from BORDERDATA where ISDELETE = 0 and UNITID = '" + unitid + "' order by SHOWINDEX";
            DataTable dt = ahp5.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["BORDERGUID"].ToString();
                if (borList.ContainsKey(pguid))
                    borList[pguid].Add(new double[] { double.Parse(dt.Rows[i]["LAT"].ToString()), double.Parse(dt.Rows[i]["LNG"].ToString()) });
                else
                {
                    borList[pguid] = new List<double[]>();
                    borList[pguid].Add(new double[] { double.Parse(dt.Rows[i]["LAT"].ToString()), double.Parse(dt.Rows[i]["LNG"].ToString()) });
                }
            }
            if (dt.Rows.Count > 0)
                borderDic.Add("path", borList);
            else
                borderDic = null;
            
            // 刷新地图、图符对应
            if (treeView1.Nodes.Count > 0)
            {
                treeView1.SelectedNode = null;
                treeView1.SelectedNode = treeView1.Nodes[0];
            }

            tabControl3.TabPages[0].Parent = null;
            timer1.Enabled = true;
        }

        private void Add_Icon(FlowLayoutPanel flp, string pguid)
        {
            string icon_path = WorkPath + "Publish\\ICONDER\\b_PNGICON\\";
            ucPictureBox ucPB = new ucPictureBox();
            string sql = "select JDNAME from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 and PGUID = '" + pguid + "'";
            DataTable dt1 = ahp2.ExecuteDataTable(sql, null);
            if (dt1.Rows.Count > 0)
            {
                ucPB.Parent = flp;
                ucPB.Name = pguid;
                ucPB.IconName = dt1.Rows[0]["JDNAME"].ToString();
                ucPB.IconPguid = pguid;
                ucPB.IconPath = icon_path + pguid + ".png";
                ucPB.Single_Click += Icon_SingleClick;
                ucPB.Double_Click += Icon_DoubleClick;
                ucPB.IconCheck = false;
            }
        }

        private void Show_Icon_List(string levelguid)
        {
            string icon_path = WorkPath + "Publish\\ICONDER\\b_PNGICON\\";
            string sql = "select PGUID, JDNAME from ZSK_OBJECT_H0001Z000K00 where ISDELETE = 0 order by LEVELNUM, SHOWINDEX";
            DataTable dt = ahp2.ExecuteDataTable(sql, null);
            string first_guid = "";
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                string name = dt.Rows[i]["JDNAME"].ToString();
                if (File.Exists(icon_path + pguid + ".png"))
                {
                    sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and ICONGUID = '" + pguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                    DataTable dt1 = ahp6.ExecuteDataTable(sql, null);
                    if (dt1.Rows.Count > 0)
                    {
                        if (first_guid == "")
                            first_guid = pguid;
                        ucPictureBox ucPB = new ucPictureBox();
                        ucPB.Parent = this.flowLayoutPanel1;
                        ucPB.Name = pguid;
                        ucPB.IconName = name;
                        ucPB.IconPguid = pguid;
                        ucPB.IconPath = icon_path + pguid + ".png";
                        ucPB.Single_Click += Icon_SingleClick;
                        ucPB.Double_Click += Icon_DoubleClick;
                        ucPB.IconCheck = false;
                    }
                }
            }
        }

        private void Icon_SingleClick(object sender, EventArgs e, string iconguid)
        {
            ucPictureBox tmp = (ucPictureBox)sender;
            if (tmp.IconCheck)
                return;
            if (tmp.Parent == this.flowLayoutPanel1)
            {
                foreach (ucPictureBox ucPB in this.flowLayoutPanel1.Controls)
                    ucPB.IconCheck = false;
                tmp.IconCheck = true;
                Show_Icon_Property(iconguid);
            }
            else
            {
                FlowLayoutPanel flp = (FlowLayoutPanel)tmp.Parent;
                foreach (ucPictureBox ucPB in flp.Controls)
                    ucPB.IconCheck = false;
                tmp.IconCheck = true;
            }
        }

        private void Icon_DoubleClick(object sender, EventArgs e, string iconguid)
        {
            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();
            ucPictureBox tmp = (ucPictureBox)sender;
            if (tmp.Parent == this.flowLayoutPanel1)
            {
                Control Remove_PB = (Control)tmp;
                flowLayoutPanel1.Controls.Remove(Remove_PB);
                string sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '"
                    + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '"
                    + iconguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                ahp6.ExecuteSql(sql, null);
            }
            else
            {
                foreach (ucPictureBox ucPB in flowLayoutPanel1.Controls)
                {
                    if (ucPB.IconPguid == tmp.IconPguid)
                    {
                        MessageBox.Show("已添加该图符!");
                        return;
                    }
                }
                ucPictureBox new_PB = new ucPictureBox();
                new_PB.IconName = tmp.IconName;
                new_PB.IconPguid = tmp.IconPguid;
                new_PB.IconPath = tmp.IconPath;
                new_PB.IconCheck = false;
                new_PB.Single_Click += Icon_SingleClick;
                new_PB.Double_Click += Icon_DoubleClick;
                flowLayoutPanel1.Controls.Add(new_PB);

                string sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ISDELETE = 0 and ICONGUID = '" + iconguid
                    + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                DataTable dt = ahp6.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '"
                        + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '"
                        + iconguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                    ahp6.ExecuteSql(sql, null);
                }
                else
                {
                    sql = "insert into ICONDUIYING_H0001Z000E00 (PGUID, S_UDTIME, LEVELGUID, ICONGUID, UNITEID) values ('"
                        + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + levelguid
                        + "', '" + iconguid + "', '" + unitid + "')";
                    ahp6.ExecuteSql(sql, null);
                }
            }
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                ucPictureBox first_PB = (ucPictureBox)flowLayoutPanel1.Controls[0];
                string first_guid = first_PB.IconPguid;
                Icon_SingleClick(flowLayoutPanel1.Controls[0], new EventArgs(), first_guid);
            }
        }

        private void Show_Icon_Property(string iconguid)
        {
            string typeguid = "";
            // 加载固定属性
            typeguid = "{26E232C8-595F-44E5-8E0F-8E0FC1BD7D24}";
            Get_Property_Data(dataGridView2, iconguid, typeguid);

            // 加载基础属性
            typeguid = "{B55806E6-9D63-4666-B6EB-AAB80814648E}";
            Get_Property_Data(dataGridView3, iconguid, typeguid);
        }

        private void Get_Property_Data(DataGridView dgv, string iconguid, string typeguid)
        {
            Prop_GUID = new List<string>();
            Show_Name = new Dictionary<string, string>();
            Show_FDName = new Dictionary<string, string>();
            inherit_GUID = new Dictionary<string, string>();
            Show_Value = new Dictionary<string, string>();
            dgv.Columns.Clear();
            string sql = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROPVALUE from ZSK_PROP_H0001Z000K00 where ISDELETE = 0 and UPGUID = '"
                + iconguid + "' and PROTYPEGUID = '" + typeguid + "' order by SHOWINDEX";
            DataTable dt = ahp2.ExecuteDataTable(sql, null);
            Add_Prop(dt);

            sql = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROPVALUE from ZSK_PROP_H0001Z000K01 where ISDELETE = 0 and UPGUID = '"
                + iconguid + "' and PROTYPEGUID = '" + typeguid + "' order by SHOWINDEX";
            dt = ahp3.ExecuteDataTable(sql, null);
            Add_Prop(dt);

            List<string> propValue = new List<string>();
            bool flag = false;
            for (int i = 0; i < Prop_GUID.Count; ++i)
            {
                string pguid = Prop_GUID[i];

                flag = true;
                DataGridViewTextBoxColumn propNode = new DataGridViewTextBoxColumn();
                propNode.Name = pguid;
                propNode.HeaderText = Show_Name[pguid];
                dgv.Columns.Add(propNode);
                propValue.Add(Show_Value[pguid]);
            }
            if (flag)
                dgv.Rows.Add(propValue.ToArray());
        }

        private void Add_Prop(DataTable proptable)
        {
            for (int i = 0; i < proptable.Rows.Count; ++i)
            {
                Prop_GUID.Add(proptable.Rows[i]["PGUID"].ToString());
                Show_Name[proptable.Rows[i]["PGUID"].ToString()] = proptable.Rows[i]["PROPNAME"].ToString();
                Show_FDName[proptable.Rows[i]["PGUID"].ToString()] = proptable.Rows[i]["FDNAME"].ToString();
                inherit_GUID[proptable.Rows[i]["PGUID"].ToString()] = proptable.Rows[i]["SOURCEGUID"].ToString();
                Show_Value[proptable.Rows[i]["PGUID"].ToString()] = proptable.Rows[i]["PROPVALUE"].ToString();
            }
        }

        private void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip3.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void dataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip2.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();
            int _index = tabControl2.SelectedIndex;
            FlowLayoutPanel flp = (FlowLayoutPanel)tabControl2.TabPages[_index].Controls[0];
            foreach (ucPictureBox item in flp.Controls)
            {
                bool flag = false;
                foreach (ucPictureBox ucPB in flowLayoutPanel1.Controls)
                {
                    if (item.IconPguid == ucPB.IconPguid)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    continue;
                ucPictureBox new_PB = new ucPictureBox();
                new_PB.IconName = item.IconName;
                new_PB.IconPguid = item.IconPguid;
                new_PB.IconPath = item.IconPath;
                new_PB.IconCheck = false;
                new_PB.Single_Click += Icon_SingleClick;
                new_PB.Double_Click += Icon_DoubleClick;
                flowLayoutPanel1.Controls.Add(new_PB);

                string sql = "select PGUID from ICONDUIYING_H0001Z000E00 where ICONGUID = '" + item.IconPguid
                    + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                DataTable dt = ahp6.ExecuteDataTable(sql, null);
                if (dt.Rows.Count > 0)
                {
                    sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 0, S_UDTIME = '"
                        + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '"
                        + item.IconPguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                    ahp6.ExecuteSql(sql, null);
                }
                else
                {
                    sql = "insert into ICONDUIYING_H0001Z000E00 (PGUID, S_UDTIME, LEVELGUID, ICONGUID, UNITEID) values ('"
                        + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + levelguid
                        + "', '" + item.IconPguid + "', '" + unitid + "')";
                    ahp6.ExecuteSql(sql, null);
                }
            }
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                ucPictureBox first_PB = (ucPictureBox)flowLayoutPanel1.Controls[0];
                string first_guid = first_PB.IconPguid;
                Icon_SingleClick(flowLayoutPanel1.Controls[0], new EventArgs(), first_guid);
            }
        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();
            foreach (ucPictureBox item in flowLayoutPanel1.Controls)
            {
                string sql = "update ICONDUIYING_H0001Z000E00 set ISDELETE = 1, S_UDTIME = '"
                    + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where ICONGUID = '" 
                    + item.IconPguid + "' and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                ahp6.ExecuteSql(sql, null);
            }
            flowLayoutPanel1.Controls.Clear();
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage tbpg = tabControl2.SelectedTab;
            FlowLayoutPanel flp = (FlowLayoutPanel)tbpg.Controls[0];
            foreach (ucPictureBox ucPB in flp.Controls)
                ucPB.IconCheck = false;
        }

        private void Delete_Path()
        {
            string mapPath = WorkPath + "Publish\\googlemap\\map";
            string starPath = WorkPath + "Publish\\googlemap\\satellite";
            string deletePath = "";
            if (Directory.Exists(mapPath))
            {
                deletePath += mapPath + ",";
            }
            if (Directory.Exists(starPath))
            {
                deletePath += starPath;
            }
            Process p = Process.Start(WorkPath + "DeleteDir.exe", deletePath);
            p.WaitForExit();
        }

        private Dictionary<string, string> GL_NAME;
        private Dictionary<string, string> GL_JDCODE;
        private Dictionary<string, string> GL_UPGUID;
        private Dictionary<string, string> GL_MAP;
        private void Load_Unit_Level()
        {
            GL_NAME = new Dictionary<string, string>();
            GL_JDCODE = new Dictionary<string, string>();
            GL_UPGUID = new Dictionary<string, string>();
            GL_MAP = new Dictionary<string, string>();

            string sql = "select PGUID, JDNAME, JDCODE, UPGUID, LEVELNUM from ZSK_OBJECT_H0001Z000K01 where ISDELETE = 0 and LEVELNUM >= " + maxlevel.ToString();
            DataTable dt = ahp3.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i) 
            {
                GL_NAME[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["JDNAME"].ToString();
                GL_JDCODE[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["JDCODE"].ToString();
                GL_UPGUID[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["UPGUID"].ToString();
            }

            treeView1.Nodes.Clear();
            treeView1.HideSelection = false;
            treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeView1.DrawNode += new DrawTreeNodeEventHandler(treeView1_DrawNode);

            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                if (dt.Rows[i]["LEVELNUM"].ToString() == maxlevel.ToString())
                {
                    TreeNode pNode = new TreeNode();
                    pNode.Text = GL_NAME[dt.Rows[i]["PGUID"].ToString()];
                    pNode.Tag = dt.Rows[i]["PGUID"].ToString();
                    treeView1.Nodes.Add(pNode);
                    Add_Unit_Node(pNode);
                }
            }
            treeView1.ExpandAll();
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true; //用默认颜色，只需要在TreeView失去焦点时选中节点仍然突显  
            return;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();
            string sql;
            DataTable dt;
            // 图符对应
            flowLayoutPanel1.Controls.Clear();
            Show_Icon_List(levelguid);

            // 地图对应
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
                if (checkedListBox1.GetItemChecked(i))
                    checkedListBox1.SetItemChecked(i, false);
            sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
            dt = ahp6.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                string maplevel = dt.Rows[0]["MAPLEVEL"].ToString();
                string[] level = maplevel.Split(',');
                for (int i = 0; i < level.Length; ++i)
                {
                    for (int j = 0; j < checkedListBox1.Items.Count; ++j)
                        if (checkedListBox1.Items[j].ToString() == level[i])
                        {
                            checkedListBox1.SetItemChecked(j, true);
                            break;
                        }
                }
            }
 
            int index = checkedListBox1.SelectedIndex;
            if (index >= 0)
            {
                if (Before_ShowMap == false)
                    mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), checkedListBox1.Items[index].ToString(), false, map_type, null, null, null, 1, 400);
                else
                    mapHelper1.setMapLevel(int.Parse(checkedListBox1.Items[index].ToString()), checkedListBox1.Items[index].ToString().ToString());
                Before_ShowMap = true;
            }
        }

        private void Add_Unit_Node(TreeNode pa) 
        {
            foreach (var item in GL_UPGUID)
            {
                if (item.Value == pa.Tag.ToString())
                {
                    TreeNode pNode = new TreeNode();
                    pNode.Text = GL_NAME[item.Key];
                    pNode.Tag = item.Key;
                    pa.Nodes.Add(pNode);
                    Add_Unit_Node(pNode);
                }
            }
        }

        private void Show_Map_List(string tmp) 
        {
            int k = tmp.LastIndexOf("roadmap");
            if (k < 0)
            {
                k = tmp.LastIndexOf("satellite");
            }
            if (k < 0)
            {
                k = tmp.LastIndexOf("satellite_en");
            }
            if (k > 0)
            {
                tmp = tmp.Substring(0, k);
            }
            textBox1.Text = tmp;

            // 清除checkedListBox控件
            checkedListBox1.Items.Clear();
 
            // 检查地图子文件夹是否存在
            string gpath = textBox1.Text + "\\satellite_en";
            if (!Directory.Exists(gpath))
            {
                MessageBox.Show("请下载或导入混合图(无偏移)");
                textBox1.Text = "";
                return;
            }
            gpath = textBox1.Text + "\\roadmap";
            if (!Directory.Exists(gpath))
            {
                MessageBox.Show("请下载或导入街道图");
                textBox1.Text = "";
                return;
            }

            // 检查所下载地图是否对应当前经纬度
            /*if (!Check_Map_LngLat())
            {
                MessageBox.Show("当前下载地图与经纬度不对应");
                textBox1.Text = "";
                return;
            }*/
            inip.WriteString("mapproperties", unitid, textBox1.Text);

            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("Individuation", "mappath", textBox1.Text);
            // 获取纯文件夹名
            folds = Directory.GetDirectories(gpath);
            for (int i = 0; i < folds.Length; i++)
            {
                k = folds[i].LastIndexOf("\\");
                folds[i] = folds[i].Substring(k + 1);
            }

            // 按文件序号排序
            Array.Sort(folds, new CompStr());

            // 添加入checkedListBox控件
            for (int i = 0; i < folds.Length; ++i)
                checkedListBox1.Items.Add(folds[i]);

            // 初始化MapHelper
            mapHelper1.centerlat = double.Parse(textBox3.Text); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(textBox2.Text); //118.5784; //必须设置的属性,不能为空
            mapHelper1.webpath = WorkPath + "Publish\\googlemap"; //必须设置的属性,不能为空
            mapHelper1.roadmappath = tmp + "\\roadmap"; //必须设置的属性,不能为空
            mapHelper1.satellitemappath = tmp + "\\satellite_en"; //必须设置的属性,不能为空
            mapHelper1.iconspath = WorkPath + "Publish\\PNGICONFOLDER"; //必须设置的属性,不能为空
            mapHelper1.maparr = folds; //必须设置的属性,不能为空
            
            // 选择checkedListBox第一项
            if (checkedListBox1.Items.Count > 0)
                checkedListBox1.SelectedIndex = 0;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = true;
            int index = checkedListBox1.SelectedIndex;
            if (Before_ShowMap == false)
                mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), checkedListBox1.Items[index].ToString(), false, map_type, null, null, null, 1, 400);
            else
                mapHelper1.setMapLevel(int.Parse(checkedListBox1.Items[index].ToString()), checkedListBox1.Items[index].ToString());


            Before_ShowMap = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 显示地图需要经纬度
            if (textBox2.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入经度");
                textBox2.Focus();
                return;
            }
            if (textBox3.Text.Trim().Equals(""))
            {
                MessageBox.Show("请输入纬度");
                textBox3.Focus();
                return;
            }

            // 载入已下载地图
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            folderBrowserDialog1.SelectedPath = inip.ReadString("mapproperties", unitid, "");
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // 获取地图文件夹的上级目录
                // 允许用户选择地图文件夹的总目录、其下的：roadmap、satellite或者satellite_en子目录
                string tmp = folderBrowserDialog1.SelectedPath;
                Show_Map_List(tmp);
            }
        }

        private bool Check_Map_LngLat()
        {
            string gpath = textBox1.Text + "\\satellite_en";
            folds = Directory.GetDirectories(gpath);
            for (int i = 0; i < folds.Length; i++)
            {
                int k = folds[i].LastIndexOf("\\");
                folds[i] = folds[i].Substring(k + 1);
            }
            Array.Sort(folds, new CompStr());
            double Lng = double.Parse(textBox2.Text);
            double Lat = double.Parse(textBox3.Text);
            Lng = Math.Pow(2, int.Parse(folds[folds.Length - 1]) - 1) * (1 + Lng / 180.0);
            Lat = Math.Pow(2, int.Parse(folds[folds.Length - 1]) - 1) * (1 - Math.Log(Math.Tan(Math.PI * Lat / 180.0) + 1 / Math.Cos(Math.PI * Lat / 180.0)) / Math.PI);
            string search_path = gpath + "\\" + folds[folds.Length - 1];
            int c_Lng = (int)Math.Floor(Lng);
            int c_Lat = (int)Math.Ceiling(Lat);
            search_path += "\\" + c_Lng.ToString();
            if (Directory.Exists(search_path))
            {
                search_path += "\\" + c_Lat.ToString() + ".jpg";
                if (File.Exists(search_path))
                    return true;
            }
            return false;
        }

        private string slat, slng;
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            textBox2.Text = slng;
            textBox3.Text = slat;
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlat", slat);
            inip.WriteString("mapproperties", "centerlng", slng);
            string sql = "update ORGCENTERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', LNG = '" + slng + "', LAT = '" + slat + "' where ISDELETE = 0 and UNITEID = '" + unitid + "'";
            ahp5.ExecuteSql(sql, null);

            mapHelper1.centerlat = double.Parse(textBox3.Text); //30.067;//必须设置的属性,不能为空
            mapHelper1.centerlng = double.Parse(textBox2.Text); //118.5784; //必须设置的属性,不能为空
            int index = checkedListBox1.SelectedIndex;
            if (Before_ShowMap == false)
                mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), checkedListBox1.Items[index].ToString(), false, map_type, null, null, null, 1, 400);
            else
                mapHelper1.setMapLevel(int.Parse(checkedListBox1.Items[index].ToString()), checkedListBox1.Items[index].ToString());
            Before_ShowMap = true;
        }

        private void mapHelper1_MapRightClick(bool canedit, double lat, double lng, int x, int y)
        {
            contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
            slat = "" + lat;
            slng = "" + lng;
        }

        private void checkedListBox1_Leave(object sender, EventArgs e)
        {
            List<string> mpLst = new List<string>();
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    mpLst.Add(checkedListBox1.Items[i].ToString());
                }
            }

            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();
            string sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
            DataTable dt = ahp6.ExecuteDataTable(sql, null);
            //ahp.CloseConn();
            string maplevel = string.Join(",", mpLst);
            GL_MAP[levelguid] = maplevel;
            if (dt.Rows.Count > 0)
            {
                sql = "update MAPDUIYING_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', MAPLEVEL = '" + maplevel + "' where ISDELETE = 0 and LEVELGUID = '" + levelguid + "' and UNITEID = '" + unitid + "'";
                ahp6.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into MAPDUIYING_H0001Z000E00 (PGUID, S_UDTIME, LEVELGUID, MAPLEVEL, UNITEID) values ('"
                    + Guid.NewGuid().ToString("B") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '"
                    + levelguid + "', '" + maplevel + "', '" + unitid + "')";
                ahp6.ExecuteSql(sql, null);
            }
        }

        private void MapForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            groupBox1.Focus();

            TreeNode pNode = treeView1.SelectedNode;
            if (pNode == null)
                return;
            string levelguid = pNode.Tag.ToString();

            List<string> cur_mplst = new List<string>();
            List<string> need_mplst = new List<string>();

            // 读取当前地图列表
            string mappath = WorkPath + "Publish\\googlemap\\map";
            if (Directory.Exists(mappath))
                cur_mplst = Directory.GetDirectories(mappath).ToList();
            for (int i = 0; i < cur_mplst.Count; ++i)
            {
                cur_mplst[i] = Path.GetFileNameWithoutExtension(cur_mplst[i]);
            }

            // 获取当前需导入地图列表
            string sql = "select MAPLEVEL from MAPDUIYING_H0001Z000E00 where ISDELETE = 0 and UNITEID = '" + unitid + "'";
            DataTable dt = ahp6.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string tmplevel = dt.Rows[i]["MAPLEVEL"].ToString();
                if (tmplevel != string.Empty)
                {
                    need_mplst.AddRange(tmplevel.Split(','));
                }
            }
            need_mplst = need_mplst.Distinct().ToList();
            for (int i = need_mplst.Count - 1; i >= 0; --i)
            {
                if (need_mplst[i] == string.Empty)
                    need_mplst.Remove(need_mplst[i]);
            }

            List<string> del_mplst = cur_mplst.Except(need_mplst).ToList();
            List<string> add_mplst = need_mplst.Except(cur_mplst).ToList();

            if (del_mplst.Count + add_mplst.Count == 0)
                return;

            /*if (MessageBox.Show("是否更新对应地图文件?", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            string lat = textBox3.Text.Trim();
            string lng = textBox2.Text.Trim();
            inip = new IniOperator(WorkPath + "Publish\\parameter.ini");
            inip.WriteString("mapproperties", "centerlat", lat);
            inip.WriteString("mapproperties", "centerlng", lng);

            string rootPath = textBox1.Text.Trim();
            string mapPath = WorkPath + "Publish\\googlemap\\map";
            string starPath = WorkPath + "Publish\\googlemap\\satellite";

            string Copy_File = string.Empty;
            string Delete_File = string.Empty;

            if (!Directory.Exists(mapPath))
            {
                Directory.CreateDirectory(mapPath);
            }
            if (!Directory.Exists(starPath))
            {
                Directory.CreateDirectory(starPath);
            }

            string smapPath = rootPath + "\\roadmap";
            for (int i = 0; i < add_mplst.Count; i++)
            {
                string destPath = mapPath + "\\" + add_mplst[i];
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string sourPath = smapPath + "\\" + add_mplst[i];
                Copy_File += sourPath + "：" + destPath + ",";
            }

            smapPath = rootPath + "\\satellite_en";
            for (int i = 0; i < add_mplst.Count; i++)
            {
                string destPath = starPath + "\\" + add_mplst[i];
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
                string sourPath = smapPath + "\\" + add_mplst[i];
                Copy_File += sourPath + "：" + destPath + ",";
            }
            Process p;
            if (Copy_File != string.Empty)
            {
                Copy_File = Copy_File.Substring(0, Copy_File.Length - 1);
                p = Process.Start(WorkPath + "CopyDir.exe", Copy_File + " 1");
                p.WaitForExit();
            }

            for (int i = 0; i < del_mplst.Count; ++i)
            {
                string destPath = mapPath + "\\" + del_mplst[i];
                while (Directory.Exists(destPath))
                {
                    Delete_File += destPath + ",";
                }
            }

            for (int i = 0; i < del_mplst.Count; ++i)
            {
                string destPath = starPath + "\\" + del_mplst[i];
                while (Directory.Exists(destPath))
                {
                    Delete_File += destPath + ",";
                }
            }
            p = Process.Start(WorkPath + "DeleteDir.exe", Delete_File);
            
            MessageBox.Show("地图更新成功!");*/
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            inip.WriteString("Public", "UnitID", unitid);
            ahp1.CloseConn();
            ahp2.CloseConn();
            ahp3.CloseConn();
            ahp4.CloseConn();
            ahp5.CloseConn();
            ahp6.CloseConn();
        }

        private void MapForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ahp1.CloseConn();
            ahp2.CloseConn();
            ahp3.CloseConn();
            ahp4.CloseConn();
            ahp5.CloseConn();
            ahp6.CloseConn();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            // 清除地图缓存
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            string Read_id = inip.ReadString("Public", "UnitID", "");
            if (Read_id != unitid)
            {
                Delete_Path();
            }
        }

        private void mapHelper1_MapTypeChanged(string mapType)
        {
            map_type = mapType;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            inip = new IniOperator(WorkPath + "Publish\\RegInfo.ini");
            inip.WriteString("Public", "UnitID", unitid);
            inip.WriteString("Individuation", "mappath", textBox1.Text);

            Process p = Process.Start(WorkPath + "Publish\\MapSet.exe");
            /*int index = checkedListBox1.SelectedIndex;
            borList = new List<double[]>();
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;
            string file = openFileDialog1.FileName;
            string[] strAll = File.ReadAllLines(file);
            foreach (string str in strAll)
            {
                string[] split = str.Split(new Char[] { ' ', ',', ':', '\t', '\r', '\n', ';' });
                borList.Add(new double[] { double.Parse(split[1]), double.Parse(split[0]) });
            }
            borderDic["path"] = borList;
            string sql = "select PGUID from BORDERDATA where ISDELETE = 0 and UNITID = '" + unitid + "'";
            DataTable dt = ahp5.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update BORDERDATA set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ISDELETE = 1 where ISDELETE = 0 and UNITID = '" + unitid + "'";
                ahp5.ExecuteSql(sql, null);
            }

            for (int i = 0; i < borList.Count; ++i)
            {
                sql = "insert into BORDERDATA (PGUID, S_UDTIME, UNITID, LAT, LNG, SHOWINDEX) values('" + Guid.NewGuid().ToString("B")
                    + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + unitid + "', '" + borList[i][0]
                    + "', '" + borList[i][1] + "', '" + i.ToString() + "')";
                ahp5.ExecuteSql(sql, null);
            }
            mapHelper1.ShowMap(int.Parse(checkedListBox1.Items[index].ToString()), checkedListBox1.Items[index].ToString(), false, map_type, null, null, null, 1, -1);*/
        }

        private void mapHelper1_LevelChanged(int lastLevel, int currLevel, string showLevel)
        {
            if (borderDic != null)
            {
                Dictionary<string, List<double[]>> tmp_bor = (Dictionary<string, List<double[]>>)borderDic["path"];
                foreach (var item in tmp_bor)
                {
                    Dictionary<string, object> bdic = new Dictionary<string, object>();
                    bdic["type"] = borderDic["type"];
                    bdic["width"] = borderDic["width"];
                    bdic["color"] = borderDic["color"];
                    bdic["opacity"] = borderDic["opacity"];
                    bdic["path"] = item.Value;
                    mapHelper1.DrawBorder(unitid, bdic);
                }
                
            }
        }        
    }

    public class CompStr : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int tx, ty;
            tx = int.Parse(x);
            ty = int.Parse(y);
            if (tx > ty)
                return 1;
            if (tx < ty)
                return -1;
            return 0;
        }
    }
}
