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
using ucPropertyGrid;
using DevExpress.XtraTab;
using System.IO;
using DevExpress.XtraBars;
using System.Diagnostics;
using Spire.DocViewer.Forms;

namespace EnvirInfoSys
{
    public partial class InfoForm : DevExpress.XtraEditors.XtraForm
    {
        InputLanguageCollection langs = InputLanguage.InstalledInputLanguages;
        private AccessHelper ahp1 = null;
        private AccessHelper ahp2 = null;
        private AccessHelper ahp3 = null;
        private AccessHelper ahp4 = null;

        public bool Update_Data = false;
        public bool CanEdit = true;
        public string unitid = "";
        public string Node_GUID = "";       //当前选择的节点guid
        public string Icon_GUID = "";       //当前选择的图标guid
        public string Node_Name = "";       //当前选择的节点名称
        public string JdCode = "";
        public Dictionary<string, string> FDName_Value;

        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;    //当前exe根目录
        private string AccessPath1 = AppDomain.CurrentDomain.BaseDirectory + "data\\ENVIR_H0001Z000E00.mdb";
        private string AccessPath2 = AppDomain.CurrentDomain.BaseDirectory + "data\\ZSK_H0001Z000K00.mdb";
        private string AccessPath3 = AppDomain.CurrentDomain.BaseDirectory + "data\\ZSK_H0001Z000K01.mdb";
        private string AccessPath4 = AppDomain.CurrentDomain.BaseDirectory + "data\\ZSK_H0001Z000E00.mdb";
        private string IniFilePath = AppDomain.CurrentDomain.BaseDirectory + "parameter.ini";
        private Dictionary<string, string> Show_Name;
        private Dictionary<string, string> Show_FDName;
        private Dictionary<string, string> inherit_GUID;
        private Dictionary<string, string> Show_Value;
        private Dictionary<string, string> Show_DB;

        private List<string> Menu_GUID;
        private Dictionary<string, string> Menu_Upguid;
        private Dictionary<string, string> Menu_Name;
        private Dictionary<string, string> Menu_Func;
        private Dictionary<string, string> Menu_Addr;
        private Dictionary<string, List<string>> Menu_List;

        DocDocumentViewer documentviewer = new DocDocumentViewer();
        

        public InfoForm()
        {
            InitializeComponent();
            //this.LostFocus += new EventHandler(InfoForm_LostFocus);
        }

        private void InfoForm_LostFocus(object sender, EventArgs e)
        {
            this.Close();
        }

        private Dictionary<string, string> Get_Prop_Type()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            res.Add("{26E232C8-595F-44E5-8E0F-8E0FC1BD7D24}", "固定属性");
            res.Add("{B55806E6-9D63-4666-B6EB-AAB80814648E}", "基础属性");
            res.Add("{D7DE9C5E-253C-491C-A380-06E41C68D2C8}", "扩展属性");
            return res;
        }

        private List<string> Get_Prop_List(Dictionary<string, string> prop_type)
        {
            List<string> res = new List<string>();
            List<string> _type = new List<string>(prop_type.Keys);

            for (int i = 1; i < _type.Count; ++i)
            {
                string database = "";
                AccessHelper ahp = null;
                if (i == 1)
                {
                    database = "H0001Z000K01";
                    string sql1 = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROPVALUE from ZSK_PROP_" + database
                        + " where ISDELETE = 0 and UPGUID = '" + Icon_GUID + "' and PROTYPEGUID = '" + _type[i]
                        + "' order by SHOWINDEX";
                    DataTable dt1 = ahp3.ExecuteDataTable(sql1, null);
                    for (int j = 0; j < dt1.Rows.Count; ++j)
                    {
                        res.Add(dt1.Rows[j]["PGUID"].ToString());
                        Show_Name[dt1.Rows[j]["PGUID"].ToString()] = dt1.Rows[j]["PROPNAME"].ToString();
                        Show_FDName[dt1.Rows[j]["PGUID"].ToString()] = dt1.Rows[j]["FDNAME"].ToString();
                        inherit_GUID[dt1.Rows[j]["PGUID"].ToString()] = dt1.Rows[j]["SOURCEGUID"].ToString();
                        Show_Value[dt1.Rows[j]["PGUID"].ToString()] = dt1.Rows[j]["PROPVALUE"].ToString();
                        Show_DB[dt1.Rows[j]["PGUID"].ToString()] = database;
                    }
                    database = "H0001Z000K00";
                    ahp = ahp2;
                }
                if (i == 2)
                {
                    database = "H0001Z000E00";
                    ahp = ahp4;
                }

                string sql = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROPVALUE from ZSK_PROP_" + database
                    + " where ISDELETE = 0 and UPGUID = '" + Icon_GUID + "' and PROTYPEGUID = '" + _type[i]
                    + "' order by SHOWINDEX";
                DataTable dt = ahp.ExecuteDataTable(sql, null);
                for (int j = 0; j < dt.Rows.Count; ++j)
                {
                    res.Add(dt.Rows[j]["PGUID"].ToString());
                    Show_Name[dt.Rows[j]["PGUID"].ToString()] = dt.Rows[j]["PROPNAME"].ToString();
                    Show_FDName[dt.Rows[j]["PGUID"].ToString()] = dt.Rows[j]["FDNAME"].ToString();
                    inherit_GUID[dt.Rows[j]["PGUID"].ToString()] = dt.Rows[j]["SOURCEGUID"].ToString();
                    Show_Value[dt.Rows[j]["PGUID"].ToString()] = dt.Rows[j]["PROPVALUE"].ToString();
                    Show_DB[dt.Rows[j]["PGUID"].ToString()] = database;
                }
            }
            return res;
        }

        private string Get_Data_Type(string propguid)
        {
            string database = Show_DB[propguid];
            AccessHelper ahp = null; ;
            string sql;
            DataTable dt, dt2;
            if (database == "H0001Z000K00")
                ahp = ahp2;
            else if (database == "H0001Z000K01")
                ahp = ahp3;
            else
                ahp = ahp4;

            string dt_guid = Icon_GUID + "_" + propguid;

            if (inherit_GUID[propguid] != "")
            {
                propguid = inherit_GUID[propguid];
                sql = "select UPGUID from ZSK_PROP_" + database + " where ISDELETE = 0 and PGUID = '" + propguid + "'";
                dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count != 0)
                {
                    dt_guid = dt.Rows[0]["UPGUID"].ToString() + "_" + propguid;
                }
            }

            sql = "select DATATYPE from  ZSK_DATATYPE_" + database + " where ISDELETE = 0 and UPGUID = '" + dt_guid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count != 0)
            {
                if (dt.Rows[0]["DATATYPE"].ToString() != "可选项")
                    return dt.Rows[0]["DATATYPE"].ToString();
                else
                {
                    sql = "select PROPVALUE from ZSK_LIMIT_" + database + " where ISDELETE = 0 and UPGUID = '" + dt_guid + "'";
                    dt2 = ahp.ExecuteDataTable(sql, null);
                    if (dt2.Rows.Count != 0)
                    {
                        if (dt2.Rows[0]["PROPVALUE"].ToString() == "否")
                            return "可选项";
                        else
                            return "多选";
                    }
                }
            }

            return "文本";
        }

        private string Get_fw(string propguid)
        {
            string database = Show_DB[propguid];
            string cl_guid = Icon_GUID + "_" + propguid;
            string sql;
            string res = "";
            AccessHelper ahp;
            DataTable dt;

            if (database == "H0001Z000K00")
                ahp = ahp2;
            else if (database == "H0001Z000K01")
                ahp = ahp3;
            else
                ahp = ahp4;

            if (inherit_GUID[propguid] != "")
            {
                propguid = inherit_GUID[propguid];
                sql = "select UPGUID from ZSK_PROP_" + database + " where ISDELETE = 0 and PGUID = '" + propguid + "'";
                dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count != 0)
                {
                    cl_guid = dt.Rows[0]["UPGUID"].ToString() + "_" + propguid;
                }
            }
            sql = "select COMBOSTR from  ZSK_COMBOSTRLIST_" + database + " where ISDELETE = 0 and UPGUID = '" + cl_guid + "' order by SHOWINDEX";
            dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                if (i == 0)
                    res += dt.Rows[i]["COMBOSTR"].ToString();
                else
                    res += "," + dt.Rows[i]["COMBOSTR"].ToString();
            }
            return res;
        }

        private void BaseInfo()
        {
            PropertyManageCls pmc = new PropertyManageCls(); // 创建属性集合实例 
            Dictionary<string, string> prop_type = new Dictionary<string, string>();
            Show_Name = new Dictionary<string, string>();
            Show_FDName = new Dictionary<string, string>();
            Show_Value = new Dictionary<string, string>();
            inherit_GUID = new Dictionary<string, string>();
            Show_DB = new Dictionary<string, string>();

            prop_type = Get_Prop_Type();
            List<string> prop_list;
            prop_list = Get_Prop_List(prop_type);
            //List<string> type_guid = new List<string> (prop_type.Keys);

            if (CanEdit == false)
                propertyGrid1.Enabled = false;
            else
                propertyGrid1.Enabled = true;

            string sql = "select MAKRENAME, REGINFO from ENVIRICONDATA_H0001Z000E00 where ISDELETE = 0 and PGUID = '" + Node_GUID + "'";
            DataTable dt = ahp1.ExecuteDataTable(sql, null);
            Property fp = new Property("名称", dt.Rows[0]["MAKRENAME"]);
            fp.Category = "\t注册信息";
            pmc.Add(fp);
            fp = new Property("注册地址", dt.Rows[0]["REGINFO"]);
            fp.Category = "\t注册信息";
            pmc.Add(fp);

            List<string> type_guid = new List<string>(prop_type.Keys);
            for (int i = 0; i < prop_list.Count; ++i)
            {
                if (Show_Name[prop_list[i]] == "名称")
                    continue;
                Property p = new Property(Show_Name[prop_list[i]], "", false, true);
                p.DisplayName = Show_Name[prop_list[i]];
                int k;
                if (Show_DB[prop_list[i]] == "H0001Z000K00" || Show_DB[prop_list[i]] == "H0001Z000K01")
                    k = 1;
                else
                    k = 2;
                p.Category = prop_type[type_guid[k]];
                p.ReadOnly = false;
                p.FdName = Show_FDName[prop_list[i]];
                if (Update_Data == false)
                    p.Value = Show_Value[prop_list[i]];
                else
                {
                    sql = "select " + Show_FDName[prop_list[i]] + " from " + JdCode + " where ISDELETE = 0 and PGUID = '" + Node_GUID + "'";
                    dt = ahp1.ExecuteDataTable(sql, null);
                    if (dt.Rows.Count > 0)
                        p.Value = dt.Rows[0][Show_FDName[prop_list[i]]];
                    else
                        p.Value = "";
                }

                string datatype = Get_Data_Type(prop_list[i]);
                switch (datatype)
                {
                    case "文本":
                    case "数字":
                        break;
                    case "可选项":
                        string kxfw = Get_fw(prop_list[i]);
                        p.Converter = new DropDownListConverter(kxfw.Split(','));
                        break;
                    case "时间":
                        p.Editor = new PropertyGridDateTimePickerItem();
                        break;
                    case "日期":
                        p.Editor = new PropertyGridDateItem();
                        break;
                    case "多选":
                        string fw = Get_fw(prop_list[i]);
                        p.Editor = new PropertyGridMultiSelect(fw);
                        break;
                    default:
                        break;
                }
                pmc.Add(p);
            }
            propertyGrid1.SelectedObject = pmc; // 加载属性
        }

        private void LoadPicture()
        {
            imageList1.Images.Clear();
            listView1.Items.Clear();
            imageList1.ImageSize = new System.Drawing.Size(120, 90);
            DirectoryInfo dir = new DirectoryInfo(WorkPath + "picture");
            FileInfo[] piclist = dir.GetFiles();
            foreach (FileInfo f in piclist)
            {
                FileStream pFileStream = new FileStream(f.FullName, FileMode.Open, FileAccess.Read);
                Image img = Image.FromStream(pFileStream);
                imageList1.Images.Add(f.Name, img);
                pFileStream.Close();
            }
            
            listView1.View = View.LargeIcon;
            listView1.LargeImageList = imageList1;
            for (int i = 0; i < imageList1.Images.Count; ++i)
            {
                listView1.Items.Add(imageList1.Images.Keys[i]);
                listView1.Items[i].ImageIndex = i;
            }
        }

        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            InfoPicForm ipf = new InfoPicForm();
            ipf.Owner = this;
            ListViewItem lvi = lv.FocusedItem;
            int index = lvi.Text.IndexOf(".");
            ipf.Text = lvi.Text.Substring(0, index);
            ipf.picpath = WorkPath + "picture\\" + lvi.Text;
            ipf.ShowDialog();
        }

        private void GetMenuList()
        {
            Menu_GUID = new List<string>();
            Menu_Upguid = new Dictionary<string, string>();
            Menu_Name = new Dictionary<string, string>();
            Menu_Func = new Dictionary<string, string>();
            Menu_Addr = new Dictionary<string, string>();
            Menu_List = new Dictionary<string, List<string>>();
            ahp1.CloseConn();
            ahp1 = new AccessHelper(AccessPath1);
            string sql = "select PGUID, UPGUID, FUNCNAME, FUNCTION, ADDRESS from ENVIRLIST_H0001Z000E00 where ISDELETE = 0 and UNITID = '" + 
                unitid + "' and MARKERID in('" + Node_GUID + "', 'all') order by SHOWINDEX desc";
            DataTable dt = ahp1.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                string pguid = dt.Rows[i]["PGUID"].ToString();
                string upguid = dt.Rows[i]["UPGUID"].ToString();
                Menu_GUID.Add(pguid);
                Menu_Upguid[pguid] = upguid;
                Menu_Name[pguid] = dt.Rows[i]["FUNCNAME"].ToString();
                Menu_Func[pguid] = dt.Rows[i]["FUNCTION"].ToString();
                Menu_Addr[pguid] = dt.Rows[i]["ADDRESS"].ToString();
                if (Menu_List.Keys.Contains(upguid))
                {
                    Menu_List[upguid].Add(pguid);
                }
                else
                {
                    Menu_List[upguid] = new List<string>();
                    Menu_List[upguid].Add(pguid);
                }
            }

            BarButtonItem btnitem = new BarButtonItem();
            btnitem.Caption = "设置";
            btnitem.ItemClick += barButtonItem3_ItemClick;
            bar2.AddItem(btnitem);

            btnitem = new BarButtonItem();
            btnitem.Caption = "关闭";
            btnitem.ItemClick += barButtonItem4_ItemClick;
            bar2.AddItem(btnitem);

            for (int i = 0; i < Menu_GUID.Count; ++i)
            {
                string guid = Menu_GUID[i];
                if (Menu_Upguid[guid] == "")
                {
                    BarButtonItem bbi = new BarButtonItem();
                    bbi.Caption = Menu_Name[guid];
                    bbi.Tag = guid;
                    bbi.ItemClick += MenuStripItem_Click;
                    bar2.InsertItem(bar2.ItemLinks[0], bbi);
                }
            }
            
            if (bar2.ItemLinks[0].Item.Tag != null)
            {
                BarButtonItem now_bbi = (BarButtonItem)bar2.ItemLinks[0].Item;
                MenuStripItem_Click(barManager1, new ItemClickEventArgs(now_bbi, bar2.ItemLinks[0]));
            }
        }

        private void Show_Info(string pguid, bool second_bar)
        {
            string func = Menu_Func[pguid];
            bool Exist = File.Exists(WorkPath + "file\\" + Menu_Addr[pguid]) || File.Exists(Menu_Addr[pguid]);
            switch (func)
            {
                case "list":
                    bar1.Visible = true;
                    panelControl1.Visible = false;
                    bar1.BeginUpdate();
                    bar1.ClearLinks();
                    bar1.Offset = 0;
                    bar1.ApplyDockRowCol();
                    // 数据库读
                    string sql = "select PGUID, FUNCNAME from ENVIRLIST_H0001Z000E00 where ISDELETE = 0 and UPGUID = '"
                        + pguid + "' and UNITID = '" + unitid + "' and MARKERID in('" + Node_GUID + "', 'all') order by SHOWINDEX";
                    DataTable dt = ahp1.ExecuteDataTable(sql, null);
                    for (int i = 0; i < dt.Rows.Count; ++i)
                    {
                        BarButtonItem bbi = new BarButtonItem();
                        bbi.Caption = dt.Rows[i]["FUNCNAME"].ToString();
                        bbi.Tag = dt.Rows[i]["PGUID"].ToString();
                        bbi.ItemClick += ToolStripItem_Click;
                        bar1.AddItem(bbi);
                    }
                    bar1.EndUpdate();
                    // 点击
                    if (bar1.ItemLinks.Count > 0)
                        ToolStripItem_Click(barManager1, new ItemClickEventArgs(bar1.ItemLinks[0].Item, bar1.ItemLinks[0]));
                    break;
                case "pdf":
                    bar1.Visible = second_bar;
                    panelControl1.Visible = true;
                    propertyGrid1.Visible = false;
                    listView1.Visible = false;
                    webBrowser1.Visible = true;
                    documentviewer.Visible = false;
                    webBrowser1.Dock = DockStyle.Fill;
                    if (Exist)
                        webBrowser1.Navigate(WorkPath + "file\\" + Menu_Addr[pguid]);
                    else
                        XtraMessageBox.Show("文件不存在!");
                    break;
                case "web":
                    bar1.Visible = second_bar;
                    panelControl1.Visible = true;
                    propertyGrid1.Visible = false;
                    listView1.Visible = false;
                    webBrowser1.Visible = true;
                    documentviewer.Visible = false;
                    webBrowser1.Dock = DockStyle.Fill;
                    webBrowser1.Navigate(Menu_Addr[pguid]);
                    break;
                case "word":
                    bar1.Visible = second_bar;
                    panelControl1.Visible = true;
                    propertyGrid1.Visible = false;
                    listView1.Visible = false;
                    webBrowser1.Visible = false;
                    documentviewer.Visible = true;
                 
                    documentviewer.Dock = DockStyle.Fill;
                    if (Exist)
                        documentviewer.LoadFromFile(WorkPath + "file\\" + Menu_Addr[pguid]);
                    else
                        XtraMessageBox.Show("文件不存在!");
                    break;
                case "exe":
                    bar1.Visible = second_bar;
                    panelControl1.Visible = false;
                    if (Exist)
                    {
                        Process p = Process.Start(Menu_Addr[pguid]);
                        p.WaitForExit();
                    }
                    else
                        XtraMessageBox.Show("文件不存在!");
                    break;
                case "info":
                    bar1.Visible = second_bar;
                    panelControl1.Visible = true;
                    propertyGrid1.Visible = true;
                    listView1.Visible = false;
                    webBrowser1.Visible = false;
                    documentviewer.Visible = false;
                    propertyGrid1.Dock = DockStyle.Fill;
                    break;
                case "pic":
                    bar1.Visible = second_bar;
                    panelControl1.Visible = true;
                    propertyGrid1.Visible = false;
                    listView1.Visible = true;
                    webBrowser1.Visible = false;
                    documentviewer.Visible = false;
                    listView1.Dock = DockStyle.Fill;
                    break;
            }
        }

        private void MenuStripItem_Click(object sender, ItemClickEventArgs e)
        {
            foreach (BarItemLink it in bar2.ItemLinks)
            {
                BarButtonItem barbtn = (BarButtonItem)it.Item;
                if (barbtn.Border == DevExpress.XtraEditors.Controls.BorderStyles.Style3D)
                    barbtn.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            }
            e.Item.Border = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            string pguid = e.Item.Tag.ToString();
            Show_Info(pguid, false);            
        }

        private void ToolStripItem_Click(object sender, ItemClickEventArgs e)
        {
            foreach (BarItemLink it in bar1.ItemLinks)
            {
                BarButtonItem barbtn = (BarButtonItem)it.Item;
                if (barbtn.Border == DevExpress.XtraEditors.Controls.BorderStyles.Style3D)
                    barbtn.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            }
            e.Item.Border = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            string pguid = e.Item.Tag.ToString();
            Show_Info(pguid, true);
        }

        private void InfoForm_Shown(object sender, EventArgs e)
        {
            
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            for (int i = 0; i < FileReader.Authority.Length; ++i)
            {
                if (FileReader.Authority[i] == "一件一档菜单设置权限")
                {
                    CheckPwForm ckpwf = new CheckPwForm();
                    ckpwf.unitid = unitid;
                    if (ckpwf.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        XtraMessageBox.Show("未能获取管理员权限");
                        return;
                    }
                    break;
                }
            }
            InfoSetForm ifsf = new InfoSetForm();
            ifsf.Owner = this;
            ifsf.unitid = unitid;
            ifsf.markerguid = Node_GUID;
            ifsf.ShowDialog();
            
            bar2.ItemLinks.Clear();
            //bar2.ClearLinks();
            
            GetMenuList();
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }

        // 不可拖动
        /*protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);//基类执行
            if (m.Msg == 132)//鼠标的移动消息（包括非窗口的移动）
            {
                //基类执行后m有了返回值,鼠标在窗口的每个地方的返回值都不同
                if ((IntPtr)2 == m.Result)//如果返回值是2，则说明鼠标是在标题拦
                {
                    //将返回值改为1(窗口的客户区)，这样系统就以为是
                    //在客户区拖动的鼠标，窗体就不会移动
                    m.Result = (IntPtr)1;
                }
            }
        }*/

        public delegate void StopShine(string pguid);
        public event StopShine stopshine;
        private void DataForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopshine(Node_GUID);
            ahp1.CloseConn();
            ahp2.CloseConn();
            ahp3.CloseConn();
            ahp4.CloseConn();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            ((WebBrowser)sender).Document.Window.Error += new HtmlElementErrorEventHandler(Window_Error);
            //将所有的链接的目标，指向本窗体
            foreach (HtmlElement archor in this.webBrowser1.Document.Links)
            {
                archor.SetAttribute("target", "_self");
            }
            //将所有的FORM的提交目标，指向本窗体
            foreach (HtmlElement form in this.webBrowser1.Document.Forms)
            {
                form.SetAttribute("target", "_self");
            }
        }

        private void Window_Error(object sender, HtmlElementErrorEventArgs e)
        {
            e.Handled = true;
        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                popupMenu1.ShowPopup(barManager1, MousePosition);
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xtraOpenFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string imagepath = xtraOpenFileDialog1.FileName;
            int index = imagepath.LastIndexOf('\\');
            string imagename = imagepath.Substring(index);
            File.Copy(imagepath, WorkPath + "picture" + imagename, true);
            LoadPicture();    
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            var items = listView1.SelectedItems;
            foreach (ListViewItem item in items)
                File.Delete(WorkPath + "picture\\" + item.Text);
            LoadPicture();            
        }

        private void InfoForm_Load(object sender, EventArgs e)
        {
            ahp1 = new AccessHelper(AccessPath1);
            ahp2 = new AccessHelper(AccessPath2);
            ahp3 = new AccessHelper(AccessPath3);
            ahp4 = new AccessHelper(AccessPath4);

            // 属性信息
            BaseInfo();

            // 加载图片
            LoadPicture();

            // 数据库获取菜单
            GetMenuList();

            documentviewer.Parent = panelControl1;
        }

    }
}