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

namespace EnvirInfoSys_Demo
{
    public partial class CheckPwForm : DevExpress.XtraEditors.XtraForm
    {
        public string unitid = "";
        private string Workpath = AppDomain.CurrentDomain.BaseDirectory;

        public CheckPwForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pwd = GetMd5_16byte(textEdit1.Text);
            AccessHelper ahp = new AccessHelper(Workpath + "data\\PASSWORD_H0001Z000E00.mdb");
            string sql = "select PGUID from PASSWORD_H0001Z000E00 where ISDELETE = 0 and PWNAME = '管理员密码' and PWMD5 = '"
                + pwd + "' and UNITID = '" + unitid + "'";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            ahp.CloseConn();
            if (dt.Rows.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
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

        private void CheckPwForm_Shown(object sender, EventArgs e)
        {
            textEdit1.Focus();
        }
    }
}