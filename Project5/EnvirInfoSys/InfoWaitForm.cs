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
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace EnvirInfoSys
{
    public partial class InfoWaitForm : DevExpress.XtraEditors.XtraForm
    {
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;    //当前exe根目录
        Thread fThread = null;
        private string ip = "";
        private string port = "";
        private PictureHelper pichelper = null;
        InfoForm tmpiffm = null;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == ImportFromDLL.WM_COPYDATA)
            {
                ImportFromDLL.COPYDATASTRUCT copyData = (ImportFromDLL.COPYDATASTRUCT)m.GetLParam(typeof(ImportFromDLL.COPYDATASTRUCT));//获取数据
                string datanum = copyData.lpData;
                int datatype = copyData.dwData;
                /*if (datatype == "0")
                    this.Initialize(datanum);
                else
                    this.UpdateValue(datanum);*/
                switch (datatype)
                {
                    case 1111:
                        this.Initialize(int.Parse(datanum));
                        break;
                    case 2222:
                        this.UpdateValue(int.Parse(datanum));
                        break;
                    case 3333:
                        this.SetDescription(datanum);
                        break;    
                    case 7777:
                        this.Close();
                        break;
                }                
            }
            base.WndProc(ref m);
        }

        private static void SendMessage(string strText, int data, string FormName)
        {
            IntPtr hwndRecvWindow = ImportFromDLL.FindWindow(null, FormName);
            IntPtr hwndSendWindow = Process.GetCurrentProcess().Handle;

            ImportFromDLL.COPYDATASTRUCT copydata = new ImportFromDLL.COPYDATASTRUCT();
            copydata.cbData = 1000;
            copydata.lpData = strText;//内容  
            copydata.dwData = data;

            //发送消息
            ImportFromDLL.SendMessage(hwndRecvWindow, ImportFromDLL.WM_COPYDATA, hwndSendWindow, ref copydata);
        }

        public InfoWaitForm()
        {
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls = false;
        }

        private void SetCaption(string caption)
        {
            progressPanel1.Caption = caption;
        }

        private void SetDescription(string description)
        {
            progressPanel1.Description = description;
        }

        private void Initialize(int maxvalue)
        {
            progressBarControl1.Visible = true;
            progressBarControl1.Properties.Minimum = 0;
            progressBarControl1.Properties.Maximum = maxvalue;
            progressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            progressBarControl1.Position = 0;
            progressBarControl1.Properties.ShowTitle = true;
            progressBarControl1.Properties.PercentView = true;
        }

        private void UpdateValue(int value)
        {
            progressBarControl1.Position = value;
        }

        private void InfoWaitForm_Shown(object sender, EventArgs e)
        {
            pichelper = new PictureHelper();
            FileReader.inip = new IniOperator(WorkPath + "SyncInfo.ini");
            ip = FileReader.inip.ReadString("Login", "ip", "");
            port = FileReader.inip.ReadString("Login", "port", "");

            fThread = new Thread(new ThreadStart(GetPic));
            fThread.Start();
        }

        private bool TestServerConnection(string host, int port, int millisecondsTimeout)
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    var ar = client.BeginConnect(host, port, null, null);
                    ar.AsyncWaitHandle.WaitOne(millisecondsTimeout);
                    return client.Connected;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    client.Close();
                }
            }
        }

        private void GetPic()
        {
            //SetDescription("正在连接服务器...");
            SendMessage("正在连接服务器...", 3333, this.Text);
            tmpiffm = (InfoForm)this.Owner;
            SendMessage("", 4444, tmpiffm.Text);
            string url = "http://" + ip + ":" + port + "/updataService.asmx";
            string fileurl = "http://" + ip + ":" + port + "/downfile/H0001Z000E00/";
            if (TestServerConnection(ip, int.Parse(port), 3000))
            {
                pichelper.ClearDir(url, new string[] { WorkPath + "picture\\" + tmpiffm.Node_GUID });
                //SetDescription("正在从服务器下载照片...");
                SendMessage("正在从服务器下载照片...", 3333, this.Text);
                DataTable dt = pichelper.QueryData(url, new string[] { "select PFNAME from M_ICONPHOTO_H0001Z000E00 where ISDELETE = 0 and UPGUID = '"
                    + tmpiffm.Node_GUID + "' and TDATATYPE = '照片'" });
                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    string extra_Desc = "(" + (i + 1).ToString() + "/" + dt.Rows.Count.ToString() + ")";
                    //SetDescription("正在从服务器下载照片..." + extra_Desc);
                    SendMessage("正在从服务器下载照片..." + extra_Desc, 3333, this.Text);
                    string picname = Path.GetFileNameWithoutExtension(dt.Rows[i]["PFNAME"].ToString());
                    string[] picparameter = new string[] { WorkPath + "picture\\" + tmpiffm.Node_GUID + "\\", picname + ".jpg" };
                    pichelper.DownloadPic(fileurl + dt.Rows[i]["PFNAME"].ToString(), picparameter, this.Text);
                }
            }
            else
            {
                //SetDescription("连接服务器失败,即将从本地导入照片");
                SendMessage("连接服务器失败,即将从本地导入照片", 3333, this.Text);
                Thread.Sleep(1000);
            }
            //SetDescription("正在导入照片...");
            SendMessage("正在导入照片...", 3333, this.Text);
            if (Directory.Exists(WorkPath + "picture\\" + tmpiffm.Node_GUID))
            {
                DirectoryInfo dir = new DirectoryInfo(WorkPath + "picture\\" + tmpiffm.Node_GUID);
                FileInfo[] piclist = dir.GetFiles();
                foreach (FileInfo f in piclist)
                {
                    SendMessage(f.FullName, 5555, tmpiffm.Text);
                    //tmpiffm.Add_Image(f.Name, img);
                }
            }
            SendMessage("", 6666, tmpiffm.Text);
            SendMessage("", 7777, this.Text);
        }

        private void InfoWaitForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            fThread.Abort();
        }

    }
}