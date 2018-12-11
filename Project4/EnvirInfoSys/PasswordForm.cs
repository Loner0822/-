using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnvirInfoSys
{
    public partial class PasswordForm : Form
    {
        public string unitid = "{9543BC02-F32C-41E4-B4AC-E4098B62AFB8}";
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory; // 当前exe根目录
        private AccessHelper ahp = null;
        private string pw_mode = "";

        public PasswordForm()
        {
            InitializeComponent();
        }

        private void PasswordForm_Shown(object sender, EventArgs e)
        {
            //groupBox3.Visible = false;
            ahp = new AccessHelper(WorkPath + "data\\PASSWORD_H0001Z000E00.mdb");
            pw_mode = radioButton3.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string old_pw = textBox1.Text;
            string new_pw = textBox2.Text;
            string confer_pw = textBox3.Text;
            
            string sql = "select PWMD5 from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWNAME = '"
                + pw_mode + "' and PWMD5 = '" + GetMd5_16byte(old_pw) + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                if (new_pw != confer_pw)
                {
                    textBox1.Text = "";
                    textBox2.Text = "";
                    MessageBox.Show("新密码与密码确认不匹配!");
                    return;
                }
                sql = "update PASSWORD_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', PWMD5 = '" + GetMd5_16byte(new_pw)
                    + "' where ISDELETE = 0 and PWNAME = '" + pw_mode + "' and UNITID = '" + unitid + "'";
                ahp.ExecuteSql(sql, null);
                MessageBox.Show("修改成功!");
            }
            else
            {
                MessageBox.Show("密码不正确,请重新输入!");
            }
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
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

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton tmp = (RadioButton)sender;
            if (tmp.Text == "管理员密码")
                groupBox3.Visible = true;
            else
                groupBox3.Visible = false;
            pw_mode = tmp.Text;
            string sql = "select AUTHORITY from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWNAME = '"
                + pw_mode + "' and UNITID = '" + unitid + "'";
            if (ahp == null)
                ahp = new AccessHelper(WorkPath + "data\\PASSWORD_H0001Z000E00.mdb");
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
            {
                string authority = dt.Rows[0]["AUTHORITY"].ToString();
                string[] author_split = authority.Split(';');
                for (int i = 0; i < checkedListBox1.Items.Count; ++i)
                {
                    bool flag = false;
                    for (int j = 0; j < author_split.Length; ++j)
                    {
                        if (checkedListBox1.Items[i].ToString() == author_split[j])
                        {
                            checkedListBox1.SetItemChecked(i, true);
                            flag = true;
                            break;
                        }
                    }
                    if (flag == false)
                        checkedListBox1.SetItemChecked(i, false);
                }
            }
        }
       
        private void checkedListBox1_Leave(object sender, EventArgs e)
        {
            string authority = "";
            for (int i = 0; i < checkedListBox1.Items.Count; ++i)
                if (checkedListBox1.GetItemChecked(i))
                    authority += checkedListBox1.Items[i].ToString() + ";";

            string sql = "update PASSWORD_H0001Z000E00 set S_UDTIME = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', AUTHORITY = '" + authority 
                + "' where ISDELETE = 0 and PWNAME = '" + pw_mode + "' and UNITID = '" + unitid + "'";
            ahp.ExecuteSql(sql, null);
        }

        private void PasswordForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            groupBox1.Focus();
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
