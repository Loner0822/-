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

namespace EnvirInfoSys
{
    public partial class RegForm : DevExpress.XtraEditors.XtraForm
    {
        public string unitid = "";
        public string levelid = "";
        public string nodeid = "";
        public string markerguid = "";
        public bool unLock = false;
        public List<string> Reg_Guid;
        public Dictionary<string, string> Reg_Name;

        public string textName = "";
        public string regaddr = "";
        public string regguid = "";

        private AccessHelper ahp = null;
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;

        public RegForm()
        {
            InitializeComponent();
        }

        public void Draw_Form()
        {
            for (int i = 0; i < Reg_Name.Count; ++i)
            {
                ucRegBox ucRB = new ucRegBox();
                ucRB.Parent = this;
                ucRB.Top = 10;
                ucRB.Left = 150 * i;
                ucRB.Name = Reg_Guid[i];
                ucRB.ucRegBox_Text(Reg_Name[Reg_Guid[i]]);
                ucRB.SelectedChange += ucRB_SelectedChange;
            }
            this.Width = Math.Max(400, 150 * Reg_Name.Count + 30);
        }

        private void ucRB_SelectedChange(object sender, EventArgs e, string pguid, int level)
        {
            if (level + 1 >= Reg_Guid.Count)
                return;
            string sql = "select PGUID, ORGNAME from RG_单位注册 where ISDELETE = 0 and UPPGUID = '" + pguid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            Dictionary<string, string> unitlist = new Dictionary<string, string>();
            for (int i = 0; i < dt.Rows.Count; ++i)
                unitlist[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["ORGNAME"].ToString();
            foreach (var item in this.Controls)
            {
                if (item is ucRegBox)
                {
                    ucRegBox tmp = (ucRegBox)item;
                    if (tmp.Name == Reg_Guid[level + 1])
                    {
                        tmp.ucRegBox_Refresh(unitlist, level + 1);
                        break;
                    }
                }
            }
        }

        private void Lock_Reg(string pguid, ref int step)
        {
            if (pguid == unitid)
            {
                step = 0;
                foreach (var item in this.Controls)
                {
                    if (item is ucRegBox)
                    {
                        ucRegBox tmp = (ucRegBox)item;
                        if (tmp.Name == Reg_Guid[step])
                        {
                            tmp.ucRegBox_SelectAndLock(pguid, unLock);
                            break;
                        }
                    }
                }
                return;
            }
            string sql = "select UPPGUID from RG_单位注册 where ISDELETE = 0 and PGUID = '" + pguid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                string upguid = dt.Rows[0]["UPPGUID"].ToString();
                Lock_Reg(upguid, ref step);
                ++step;
                foreach (var item in this.Controls)
                {
                    if (item is ucRegBox)
                    {
                        ucRegBox tmp = (ucRegBox)item;
                        if (tmp.Name == Reg_Guid[step])
                        {
                            tmp.ucRegBox_SelectAndLock(pguid, unLock);
                            break;
                        }
                    }
                }
            }
        }

        private void RegForm_Load(object sender, EventArgs e)
        {
            
        }

        private void RegForm_Shown(object sender, EventArgs e)
        {
            ahp = new AccessHelper(WorkPath + "data\\PersonMange.mdb");
            textEdit1.Text = textName;

            string sql = "select PGUID, ORGNAME from RG_单位注册 where ISDELETE = 0 and PGUID = '" + unitid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            Dictionary<string, string> unitlist = new Dictionary<string, string>();
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                unitlist[pguid] = dt.Rows[i]["ORGNAME"].ToString();
            }
            foreach (var item in this.Controls)
            {
                if (item is ucRegBox)
                {
                    ucRegBox tmp = (ucRegBox)item;
                    if (tmp.Name == levelid)
                    {
                        tmp.ucRegBox_Refresh(unitlist, 0);
                        break;
                    }
                }
            }

            int step = 0;
            Lock_Reg(nodeid, ref step);

        }

        private void RegForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ahp.CloseConn();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            regaddr = "";
            textName = textEdit1.Text;
            for (int i = 0; i < Reg_Guid.Count; ++i)
            {
                foreach (var item in this.Controls)
                {
                    if (item is ucRegBox)
                    {
                        ucRegBox tmp = (ucRegBox)item;
                        if (tmp.Name == Reg_Guid[i] && tmp.unitguid != "")
                        {
                            regaddr += tmp.unitname + ";";
                            regguid = tmp.unitguid;
                        }
                    }
                }
            }
            if (textEdit1.Text != "")
            {
                AccessHelper ahp1 = new AccessHelper(WorkPath + "data\\ENVIR_H0001Z000E00.mdb");
                string sql = "select PGUID from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and REGGUID = '"
                    + regguid + "' and MAKRENAME = '" + textEdit1.Text + "' and PGUID <> '"
                    + markerguid + "'";
                DataTable dt = ahp1.ExecuteDataTable(sql, null);
                ahp1.CloseConn();
                if (dt.Rows.Count > 0)
                {
                    XtraMessageBox.Show("该名称已被使用!");
                    textEdit1.Focus();
                    textEdit1.SelectAll();
                    return;
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            else
            {
                XtraMessageBox.Show("请输入注册名称!");
                return;
            }
        }
    }
}