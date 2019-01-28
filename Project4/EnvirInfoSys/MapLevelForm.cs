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
using System.IO;

namespace EnvirInfoSys
{
    public partial class MapLevelForm : DevExpress.XtraEditors.XtraForm
    {
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;
        private string[] folds = null;
        public string unitid = "";
        public string nodeid = "";
        public string MapPath = "";
        public string mapstring = "";


        public MapLevelForm()
        {
            InitializeComponent();
        }

        private void MapLevelForm_Load(object sender, EventArgs e)
        {
            checkedListBoxControl1.Items.Clear();
            string mappath = MapPath + "\\roadmap";
            folds = Directory.GetDirectories(mappath);

            for (int i = 0; i < folds.Length; i++)
            {
                int tmp = folds[i].LastIndexOf("\\");
                folds[i] = folds[i].Substring(tmp + 1);
                checkedListBoxControl1.Items.Add(folds[i]);
            }

            string sql = "select MAPLEVEL from ENVIRMAPDY_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + nodeid + "' and UNITID = '" + unitid + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                string lists = dt.Rows[0]["MAPLEVEL"].ToString();
                string[] checkedlist = lists.Split(',');
                for (int i = 0; i < checkedlist.Length; ++i)
                {
                    for (int j = 0; j < checkedListBoxControl1.Items.Count; ++j)
                    {
                        if (checkedListBoxControl1.Items[j].Value.ToString() == checkedlist[i])
                        {
                            checkedListBoxControl1.SetItemChecked(j, true);
                        }
                    }
                }
            }
            else
            {
                //FileReader.once_ahp = new AccessHelper(WorkPath + "data\\ENVIRDYDATA_H0001Z000E00.mdb");
                //sql = "select "
                //FileReader.once_ahp.CloseConn();
                string[] checkedlist = mapstring.Split(',');
                for (int i = 0; i < checkedlist.Length; ++i)
                {
                    for (int j = 0; j < checkedListBoxControl1.Items.Count; ++j)
                    {
                        if (checkedListBoxControl1.Items[j].Value.ToString() == checkedlist[i])
                        {
                            checkedListBoxControl1.SetItemChecked(j, true);
                        }
                    }
                }
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string lists = "";
            List<string> checkedlist = new List<string>();
            for (int i = 0; i < checkedListBoxControl1.Items.Count; i++)
            {
                if (checkedListBoxControl1.GetItemChecked(i))
                {
                    string a = checkedListBoxControl1.GetItemValue(i).ToString();
                    checkedlist.Add(a);
                }
            }
            lists = string.Join(",", checkedlist.ToArray());
            if (checkedlist.Count <= 0)            
            {
                if (XtraMessageBox.Show("未选中任何级别!是否要重新对应?", "提示", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                    return;
                else
                    Close();
            }
            string sql = "select PGUID from ENVIRMAPDY_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + nodeid + "' and UNITID = '" + unitid + "'";
            DataTable dt = FileReader.often_ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                sql = "update ENVIRMAPDY_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    + "', MAPLEVEL = '" + lists + "' where ISDELETE = 0 and PGUID = '" + nodeid + "'";
                FileReader.often_ahp.ExecuteSql(sql, null);
            }
            else
            {
                sql = "insert into ENVIRMAPDY_H0001Z000E00 (PGUID, S_UDTIME, UNITID, MAPLEVEL) values('" + nodeid
                    + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + unitid + "', '" + lists + "')";
                FileReader.often_ahp.ExecuteSql(sql, null);
            }
            Close();
        }
    }
}