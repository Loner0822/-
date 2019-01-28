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
    public partial class InfoPicForm : DevExpress.XtraEditors.XtraForm
    {
        public string picpath = "";

        public InfoPicForm()
        {
            InitializeComponent();
        }

        private void InfoPicForm_Shown(object sender, EventArgs e)
        {
            FileStream pFileStream = new FileStream(picpath, FileMode.Open, FileAccess.Read);
            pictureEdit1.Image = Image.FromStream(pFileStream);
            pFileStream.Close();
        }
    }
}