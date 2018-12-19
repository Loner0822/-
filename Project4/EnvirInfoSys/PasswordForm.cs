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
using System.Security.Cryptography;
using DevExpress.XtraEditors.Controls;

namespace EnvirInfoSys_Demo
{
    public partial class PasswordForm : DevExpress.XtraEditors.XtraForm
    {
        public string unitid = "{9543BC02-F32C-41E4-B4AC-E4098B62AFB8}";
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory; // 当前exe根目录
        private AccessHelper ahp = null;
        private string pw_mode = "";

        public PasswordForm()
        {
            InitializeComponent();
        }

        CheckedListBoxItem[] items = 
        {
            new CheckedListBoxItem("服务器IP设置权限", false),
            new CheckedListBoxItem("边界线属性设置权限", false),
            new CheckedListBoxItem("管辖分类设置权限", false),
            new CheckedListBoxItem("图符对应设置权限", false),
            new CheckedListBoxItem("图符扩展设置权限", false),
            new CheckedListBoxItem("查看日志权限", false)
        };

        private void PasswordForm_Shown(object sender, EventArgs e)
        {
            ahp = new AccessHelper(WorkPath + "data\\PASSWORD_H0001Z000E00.mdb");
            pw_mode = radioGroup1.Properties.Items[0].Description;
            checkedListBoxControl1.Items.AddRange(items);
            radioGroup1.SelectedIndex = 1;
            radioGroup1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string old_pw = textEdit1.Text;
            string new_pw = textEdit2.Text;
            string confer_pw = textEdit3.Text;

            string sql = "select PWMD5 from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWNAME = '"
                + pw_mode + "' and PWMD5 = '" + GetMd5_16byte(old_pw) + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                if (new_pw != confer_pw)
                {
                    textEdit1.Text = "";
                    textEdit2.Text = "";
                    XtraMessageBox.Show("新密码与密码确认不匹配!");
                    return;
                }
                sql = "update PASSWORD_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', PWMD5 = '" + GetMd5_16byte(new_pw)
                    + "' where ISDELETE = 0 and PWNAME = '" + pw_mode + "' and UNITID = '" + unitid + "'";
                ahp.ExecuteSql(sql, null);
                XtraMessageBox.Show("修改成功!");
            }
            else
            {
                XtraMessageBox.Show("密码不正确,请重新输入!");
            }
            textEdit1.Text = "";
            textEdit2.Text = "";
            textEdit3.Text = "";
        }

        public static string GetMd5_16byte(string ConvertString)
        {
            string md5Pwd = string.Empty;
            //使用加密服务提供程序
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //将指定的字节子数组的每个元素的数值转换为它的等效十六进制字符串表示形式。
            md5Pwd = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
            md5Pwd = md5Pwd.Replace("-", "");
            md5Pwd = md5Pwd.ToLower();
            return md5Pwd;
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadioGroupItem tmp = radioGroup1.Properties.Items[radioGroup1.SelectedIndex];
            if (tmp.Description == "管理员密码")
                groupControl2.Visible = true;
            else
                groupControl2.Visible = false;
            pw_mode = tmp.Description;
            string sql = "select AUTHORITY from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWNAME = '"
                + pw_mode + "' and UNITID = '" + unitid + "'";
            if (ahp == null)
                ahp = new AccessHelper(WorkPath + "data\\PASSWORD_H0001Z000E00.mdb");
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                string authority = dt.Rows[0]["AUTHORITY"].ToString();
                string[] author_split = authority.Split(';');
                for (int i = 0; i < checkedListBoxControl1.Items.Count; ++i)
                {
                    bool flag = false;
                    for (int j = 0; j < author_split.Length; ++j)
                    {
                        if (checkedListBoxControl1.Items[i].ToString() == author_split[j])
                        {
                            checkedListBoxControl1.SetItemChecked(i, true);
                            flag = true;
                            break;
                        }
                    }
                    if (flag == false)
                        checkedListBoxControl1.SetItemChecked(i, false);
                }
            }
        }

        private void checkedListBoxControl1_Leave(object sender, EventArgs e)
        {
            string authority = "";
            for (int i = 0; i < checkedListBoxControl1.Items.Count; ++i)
                if (checkedListBoxControl1.GetItemChecked(i))
                    authority += checkedListBoxControl1.Items[i].ToString() + ";";

            string sql = "update PASSWORD_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', AUTHORITY = '" + authority
                + "' where ISDELETE = 0 and PWNAME = '" + pw_mode + "' and UNITID = '" + unitid + "'";
            ahp.ExecuteSql(sql, null);
        }

        private void PasswordForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            groupControl1.Focus();
            string sql = "select AUTHORITY from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWNAME = '管理员密码' and UNITID = '" + unitid + "'";
            if (ahp == null)
                ahp = new AccessHelper(WorkPath + "data\\PASSWORD_H0001Z000E00.mdb");
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                string author_list = dt.Rows[0]["AUTHORITY"].ToString();
                FileReader.Authority = author_list.Split(';');
            }
            ahp.CloseConn();
        }

    }
}