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

namespace EnvirInfoSys
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        public int Mode;
        public string unitid = "";
        private string Workpath = AppDomain.CurrentDomain.BaseDirectory;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            textEdit1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pwname = "";
            string pwd = GetMd5_16byte(textEdit1.Text);
            AccessHelper ahp = new AccessHelper(Workpath + "data\\PASSWORD_H0001Z000E00.mdb");
            string sql = "select PWNAME from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWMD5 = '"
                + pwd + "' and UNITID = '" + unitid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            ahp.CloseConn();
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                pwname = dt.Rows[i]["PWNAME"].ToString();
                if (pwname != "管理员密码")
                    break;
            }
            if (pwname == "编辑模式")
            {
                this.DialogResult = DialogResult.OK;
                Mode = 1;
                this.Close();
            }
            else if (pwname == "查看模式")
            {
                this.DialogResult = DialogResult.OK;
                Mode = 2;
                this.Close();
            }
            else
            {
                XtraMessageBox.Show("密码错误!");
                textEdit1.Focus();
                textEdit1.SelectAll();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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

    }
}