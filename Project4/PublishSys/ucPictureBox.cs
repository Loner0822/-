using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PublishSys
{
    public partial class ucPictureBox : UserControl
    {
        private string _iconname = string.Empty;
        private string _iconpguid = string.Empty;
        private string _iconpath = string.Empty;
        private bool _iconcheck = false;

        public ucPictureBox()
        {
            InitializeComponent();
        }

        public string IconName
        {
            get
            {
                return _iconname;
            }
            set
            {
                _iconname = value;
                this.checkBox1.Text = _iconname;

            }
        }
        public string IconPguid
        {
            get
            {
                return _iconpguid;
            }
            set
            {
                _iconpguid = value;
            }
        }
        public string IconPath
        {
            get
            {
                return _iconpath;
            }
            set
            {
                _iconpath = value;
                this.pictureBox1.Load(_iconpath);
            }
        }
        public bool IconCheck
        {
            get
            {
                return _iconcheck;
            }
            set
            {
                _iconcheck = value;
                this.checkBox1.Checked = _iconcheck;
            }
        }

        public delegate void CheckBoxChangedHandle(object sender, EventArgs e, string iconguid, bool ischecked);
        public event CheckBoxChangedHandle CheckBoxChanged;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _iconcheck = this.checkBox1.Checked;
            if (CheckBoxChanged != null)
            {
                CheckBoxChanged(sender, new EventArgs(), _iconpguid, _iconcheck);
            }
        }

    }
}
