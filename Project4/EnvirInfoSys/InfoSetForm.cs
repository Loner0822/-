using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using System.Collections;
using DevExpress.XtraTreeList.Columns;

namespace EnvirInfoSys
{

    public partial class InfoSetForm : DevExpress.XtraEditors.XtraForm
    {
        private AccessHelper ahp = null;
        public string unitid = "";
        public string markerguid = "";
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;
        private bool NeedChange = true;

        private List<Menu_Node> Menu_List;

        public InfoSetForm()
        {
            InitializeComponent();
        }

        private void InfoSetForm_Shown(object sender, EventArgs e)
        {
            comboBoxEdit1.Enabled = false;
            simpleButton1.Enabled = false;
            simpleButton2.Enabled = false;
            textEdit2.Text = "";
            textEdit2.Enabled = false;
            ahp = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
            Read_Tree();
        }

        private void Read_Tree()
        {
            Menu_List = new List<Menu_Node>();
            string sql = "select PGUID, UPGUID, FUNCNAME, FUNCTION, ADDRESS from ENVIRLIST_H0001Z000E00 where ISDELETE = 0 and UNITID = '"
                + unitid + "' and MARKERID = '" + markerguid + "' order by SHOWINDEX";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                Menu_Node pNode = new Menu_Node();
                string pguid = dt.Rows[i]["PGUID"].ToString();
                string upguid = dt.Rows[i]["UPGUID"].ToString();
                pNode.pguid = pguid;
                pNode.upguid = upguid;
                pNode.name = dt.Rows[i]["FUNCNAME"].ToString();
                pNode.func = dt.Rows[i]["FUNCTION"].ToString();
                pNode.addr = dt.Rows[i]["ADDRESS"].ToString();
                Menu_List.Add(pNode);
            }

            treeList1.Nodes.Clear();
            treeList1.OptionsBehavior.ShowEditorOnMouseUp = true;
            treeList1.Appearance.FocusedCell.BackColor = System.Drawing.Color.SteelBlue;
            treeList1.KeyFieldName = "pguid";
            treeList1.ParentFieldName = "upguid";
            
            treeList1.DataSource = Menu_List;
            treeList1.Columns[1].Visible = false;
            treeList1.Columns[2].Visible = false;
            treeList1.ExpandAll();
            // treeList1.FocusedNode = treeList1.Nodes[0];
        }

        private void treeList1_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            if (e.Node == null)
                return;
            if (e.Node.ParentNode != null)
            {
                if (comboBoxEdit1.Properties.Items.Count >= 5)
                    comboBoxEdit1.Properties.Items.RemoveAt(4);
            }
            else
            {
                if (comboBoxEdit1.Properties.Items.Count < 5)
                    comboBoxEdit1.Properties.Items.Add("列表");
            }
            if (e.Node.Nodes.Count > 0)
            {
                comboBoxEdit1.Text = "列表";
                comboBoxEdit1.Enabled = false;
                simpleButton1.Enabled = false;
                simpleButton2.Enabled = false;
                textEdit2.Text = "";
                textEdit2.Enabled = false;
            }
            else
            {
                comboBoxEdit1.Enabled = true;
                simpleButton1.Enabled = true;
                simpleButton2.Enabled = true;
                textEdit2.Enabled = true;
                switch (e.Node.GetValue("func").ToString())
                {
                    case "word":
                        comboBoxEdit1.SelectedIndex = 0;
                        break;
                    case "pdf":
                        comboBoxEdit1.SelectedIndex = 1;
                        break;
                    case "web":
                        simpleButton1.Enabled = false;
                        comboBoxEdit1.SelectedIndex = 2;
                        break;
                    case "exe":
                        comboBoxEdit1.SelectedIndex = 3;
                        break;
                    case "list":
                        simpleButton1.Enabled = false;
                        comboBoxEdit1.SelectedIndex = 4;
                        break;
                }
                if (NeedChange)
                    textEdit2.Text = e.Node.GetValue("addr").ToString();
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textEdit2.Text = "";
            if (comboBoxEdit1.SelectedIndex == 2 || comboBoxEdit1.SelectedIndex == 4)
                simpleButton1.Enabled = false;
            else
                simpleButton1.Enabled = true;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            if (pNode == null)
                return;
            switch (pNode.GetValue("func").ToString())
            {
                case "word":
                    xtraOpenFileDialog1.Filter = "Word文档|*.doc;*.docx";
                    break;
                case "pdf":
                    xtraOpenFileDialog1.Filter = "PDF文档|*.pdf";
                    break;
                case "exe":
                    xtraOpenFileDialog1.Filter = "应用程序|*.exe";
                    break;
            }
            if (xtraOpenFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            textEdit2.Text = xtraOpenFileDialog1.FileName;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            if (pNode == null)
                return;
            NeedChange = false;
            pNode.SetValue("func", textEdit2.Text);
            switch (comboBoxEdit1.SelectedIndex)
            {
                case 0:
                    pNode.SetValue("func", "word");
                    break;
                case 1:
                    pNode.SetValue("func", "pdf");
                    break;
                case 2:
                    pNode.SetValue("func", "web");
                    break;
                case 3:
                    pNode.SetValue("func", "exe");
                    break;
                case 4:
                    pNode.SetValue("func", "list");
                    break;
            }
            
            string sql = "update ENVIRLIST_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                "', FUNCTION = '" + pNode.GetValue("func").ToString() + "', ADDRESS = '" + textEdit2.Text + 
                "' where ISDELETE = 0 and PGUID = '" + pNode.GetValue("pguid") + "'";
            ahp.ExecuteSql(sql, null);
            NeedChange = true;
            XtraMessageBox.Show("保存成功!");
        }

        private void treeList1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                TreeListNode pNode = treeList1.GetNodeAt(e.X, e.Y);
                if (pNode != null)
                {
                    if (pNode.ParentNode != null)
                        barButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    else
                        barButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;

                    barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    barButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Always;
                    popupMenu1.ShowPopup(barManager1, MousePosition);
                    treeList1.FocusedNode = pNode;
                }
                if (treeList1.Nodes.Count == 0)
                {
                    barButtonItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem3.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    barButtonItem4.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    popupMenu1.ShowPopup(barManager1, MousePosition);
                }
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode tln = treeList1.FocusedNode;
            Menu_Node pNode = new Menu_Node();
            pNode.pguid = Guid.NewGuid().ToString("B");
            if (tln == null || tln.ParentNode == null)
                pNode.upguid = "";
            else
                pNode.upguid = tln.ParentNode.GetValue("pguid").ToString();
            InfoEditForm ifef = new InfoEditForm();
            ifef.Owner = this;
            if (ifef.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pNode.name = ifef.EditText;
                pNode.func = "list";
                pNode.addr = "";
                string sql = "insert into ENVIRLIST_H0001Z000E00 (PGUID, S_UDTIME, UPGUID, FUNCNAME, FUNCTION, UNITID, MARKERID) values('" +
                    pNode.pguid + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode.upguid + "', '"  + pNode.name + 
                    "', '" + pNode.func + "', '" + unitid + "', '" + markerguid + "')";
                ahp.ExecuteSql(sql, null);
                Read_Tree();
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode tln = treeList1.FocusedNode;
            Menu_Node pNode = new Menu_Node();
            pNode.pguid = Guid.NewGuid().ToString("B");
            pNode.upguid = tln.GetValue("pguid").ToString();
            InfoEditForm ifef = new InfoEditForm();
            if (ifef.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pNode.name = ifef.EditText;
                pNode.func = "list";
                pNode.addr = "";
                string sql = "insert into ENVIRLIST_H0001Z000E00 (PGUID, S_UDTIME, UPGUID, FUNCNAME, FUNCTION, UNITID, MARKERID) values('" +
                    pNode.pguid + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + pNode.upguid + "', '" + pNode.name +
                    "', '" + pNode.func + "', '" + unitid + "', '" + markerguid + "')";
                ahp.ExecuteSql(sql, null);
                Read_Tree();
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            InfoEditForm ifef = new InfoEditForm();
            ifef.EditText = pNode.GetValue("name").ToString();
            if (ifef.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pNode.SetValue("name", ifef.EditText);
                string sql = "update ENVIRLIST_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "', FUNCNAME = '" + ifef.EditText + "' where ISDELETE = 0 and PGUID = '" + pNode.GetValue("pguid").ToString() + "'";
                ahp.ExecuteSql(sql, null);
                treeList1.RefreshDataSource();
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            TreeListNode pNode = treeList1.FocusedNode;
            string pguid = pNode.GetValue("pguid").ToString();
            treeList1.DeleteNode(pNode);
            string sql = "update ENVIRLIST_H0001Z000E00 set ISDELETE = 1 where PGUID = '" + pguid + "' or UPGUID = '" + pguid + "'";
            ahp.ExecuteSql(sql, null);
            treeList1.RefreshDataSource();
        }

        private void InfoSetForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ahp.CloseConn();
        }

        private void treeList1_DragDrop(object sender, DragEventArgs e)
        {
            TreeListNode dragNode, targetNode;
            TreeList tl = sender as TreeList;
            Point p = tl.PointToClient(new Point(e.X, e.Y));
            dragNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
            targetNode = tl.CalcHitInfo(p).Node;
            tl.SetNodeIndex(dragNode, tl.GetNodeIndex(targetNode));
            e.Effect = DragDropEffects.None;
        }

        private void treeList1_AfterDragNode(object sender, AfterDragNodeEventArgs e)
        {
            int cnt = 0;
            foreach (TreeListNode tln in treeList1.Nodes)
            {
                if (tln.ParentNode == null)
                {
                    string pguid = tln.GetValue("pguid").ToString();
                    string sql = "update ENVIRLIST_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "', SHOWINDEX = " + cnt.ToString() + " where ISDELETE = 0 and PGUID = '" + pguid + "'";
                    ahp.ExecuteSql(sql, null);
                    ++cnt;
                    UpDateNode(tln);
                }
            }
        }

        private void UpDateNode(TreeListNode pNode)
        {
            int cnt = 0;
            foreach (TreeListNode tln in pNode.Nodes)
            {
                string pguid = tln.GetValue("pguid").ToString();
                string sql = "update ENVIRLIST_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                + "', SHOWINDEX = " + cnt.ToString() + " where ISDELETE = 0 and PGUID = '" + pguid + "'";
                ahp.ExecuteSql(sql, null);
                ++cnt;
                UpDateNode(tln);
            }
        }
    }

    public class Menu_Node
    {
        public string pguid { set; get; }
        public string upguid { set; get; }
        public string name { set; get; }
        public string func { set; get; }
        public string addr { set; get; }
    }

}