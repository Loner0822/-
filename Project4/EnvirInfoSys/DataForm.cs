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
using System.Reflection;

namespace EnvirInfoSys
{
    public partial class DataForm : DevExpress.XtraEditors.XtraForm
    {
        InputLanguageCollection langs = InputLanguage.InstalledInputLanguages;
        private AccessHelper ahp1 = null;
        private AccessHelper ahp2 = null;
        private AccessHelper ahp3 = null;
        private AccessHelper ahp4 = null;

        public bool Update_Data = false;
        public bool CanEdit = true;
        public string Node_GUID = "";       //当前选择的节点guid
        public string Icon_GUID = "";       //当前选择的图标guid
        public string Node_Name = "";       //当前选择的节点名称
        public string JdCode = "";
        public string unitid = "";
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
        private Dictionary<string, string> Real_Value;
        private Dictionary<string, string> Show_DB;

        public DataForm()
        {
            InitializeComponent();
        }

        public void ReNew()
        {
            FDName_Value = new Dictionary<string, string>();
            PropertyManageCls pmc = (PropertyManageCls)propertyGrid1.SelectedObject;
            foreach (Property item in pmc)
            {
                FDName_Value.Add(item.FdName, item.Value.ToString());
            }
            this.DialogResult = DialogResult.OK;
            Close();
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
                    + "'";
                if (i == 2)
                    sql += " and UNITID = '" + unitid + "'";
                sql += " order by SHOWINDEX";

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
                if (dt.Rows.Count > 0)
                {
                    dt_guid = dt.Rows[0]["UPGUID"].ToString() + "_" + propguid;
                }
            }

            sql = "select DATATYPE from  ZSK_DATATYPE_" + database + " where ISDELETE = 0 and UPGUID = '" + dt_guid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count > 0)
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

        private Dictionary<string, string> Get_dw(string propguid)
        {
            Dictionary<string, string> res = new Dictionary<string,string>{
                {"danwei", ""},
                {"afterdecpoint", ""},
                {"upper", ""},
                {"limit", ""},
                {"defvalue", ""}
            };
            string database = Show_DB[propguid];
            AccessHelper ahp = null; ;
            string sql;
            DataTable dt;
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
                if (dt.Rows.Count > 0)
                {
                    dt_guid = dt.Rows[0]["UPGUID"].ToString() + "_" + propguid;
                }
            }

            sql = "select PROPNAME, PROPVALUE from ZSK_LIMIT_" + database + " where ISDELETE = 0 and UPGUID = '" + dt_guid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                if (dt.Rows[i]["PROPNAME"].ToString() == "单位")
                    res["danwei"] = dt.Rows[i]["PROPVALUE"].ToString();
                if (dt.Rows[i]["PROPNAME"].ToString() == "小数位数")
                    res["afterdecpoint"] = dt.Rows[i]["PROPVALUE"].ToString();
                if (dt.Rows[i]["PROPNAME"].ToString() == "上限")
                    res["upper"] = dt.Rows[i]["PROPVALUE"].ToString();
                if (dt.Rows[i]["PROPNAME"].ToString() == "下限")
                    res["limit"] = dt.Rows[i]["PROPVALUE"].ToString();
                //if (dt.Rows[i]["PROPNAME"].ToString() == "默认数字")
                    //res["defvalue"] = dt.Rows[i]["PROPVALUE"].ToString();                    
            }
            return res;
        }

        public void Load_Prop()
        {
            ahp1 = new AccessHelper(AccessPath1);
            ahp2 = new AccessHelper(AccessPath2);
            ahp3 = new AccessHelper(AccessPath3);
            ahp4 = new AccessHelper(AccessPath4);
            PropertyManageCls pmc = new PropertyManageCls(); // 创建属性集合实例 
            Dictionary<string, string> prop_type = new Dictionary<string, string>();
            Show_Name = new Dictionary<string, string>();
            Show_FDName = new Dictionary<string, string>();
            Show_Value = new Dictionary<string, string>();
            inherit_GUID = new Dictionary<string, string>();
            Show_DB = new Dictionary<string, string>();
            Real_Value = new Dictionary<string, string>();

            prop_type = Get_Prop_Type();
            List<string> prop_list;
            prop_list = Get_Prop_List(prop_type);
            //List<string> type_guid = new List<string> (prop_type.Keys);

            if (CanEdit == false)
                propertyGrid1.Enabled = false;
            else
                propertyGrid1.Enabled = true;

            List<string> type_guid = new List<string>(prop_type.Keys);
            for (int i = 0; i < prop_list.Count; ++i)
            {
                if (Show_Name[prop_list[i]] == "名称")
                    continue;
                Property p = new Property(Show_Name[prop_list[i]], "");
                p.DisplayName = Show_Name[prop_list[i]];
                int k;
                if (Show_DB[prop_list[i]] == "H0001Z000K00" || Show_DB[prop_list[i]] == "H0001Z000K01")
                    k = 1;
                else
                    k = 2;
                p.Category = prop_type[type_guid[k]];
                
                p.FdName = Show_FDName[prop_list[i]];
                if (Update_Data == false)
                    p.Value = Show_Value[prop_list[i]];
                else
                {
                    string sql = "select " + Show_FDName[prop_list[i]] + " from " + JdCode + " where ISDELETE = 0 and PGUID = '" + Node_GUID + "'";
                    DataTable dt = ahp1.ExecuteDataTable(sql, null);
                    if (dt.Rows.Count > 0)
                        p.Value = dt.Rows[0][Show_FDName[prop_list[i]]].ToString();
                    else
                        p.Value = "";
                }

                string datatype = Get_Data_Type(prop_list[i]);
                switch (datatype)
                {
                    case "文本":
                        break;
                    case "数字":
                        Dictionary<string, string> dicdata = Get_dw(prop_list[i]);
                        dicdata["defvalue"] = p.Value.ToString();
                        if (p.Value.ToString() != "")
                            p.Value += dicdata["danwei"];
                        p.isNum = true;
                        p.Editor = new PropertyGridNumber(dicdata);
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
                        //MessageBox.Show(Show_Name[prop_list[i]] + "属性找不到数据类型");
                        break;
                }
                p.ReadOnly = false;
                pmc.Add(p);
            }

            propertyGrid1.SelectedObject = pmc; // 加载属性
        }

        private void DataForm_Shown(object sender, EventArgs e)
        {
            Load_Prop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FDName_Value = new Dictionary<string, string>();
            PropertyManageCls pmc = (PropertyManageCls)propertyGrid1.SelectedObject;
            //bool find_name = false;
            foreach (Property item in pmc)
            {
                /*if (item.DisplayName == "名称")
                {
                    find_name = true;
                    if (item.Value.ToString() == "")
                    {
                        XtraMessageBox.Show("请填入名称!");
                        return;
                    }
                    else
                    {
                        Node_Name = item.Value.ToString();
                        FDName_Value.Add(item.FdName, item.Value.ToString());
                    }
                }
                else
                {
                    FDName_Value.Add(item.FdName, item.Value.ToString());
                }*/
                if (item.isNum == false)
                    FDName_Value.Add(item.FdName, item.Value.ToString());
                else
                {
                    string ret_value = string.Empty;
                    foreach (char ch in item.Value.ToString())
                    {
                        if (('0' <= ch && ch <= '9') || ch == '.')
                            ret_value += ch;
                    }
                    FDName_Value.Add(item.FdName, ret_value);
                }
                //item.ReadOnly = false;
            }
            /*if (find_name == false)
            {
                XtraMessageBox.Show("没有名称属性!");
                return;
            }*/
            this.DialogResult = DialogResult.OK;
            return;
        }

        public void Close_Conn()
        {
            ahp1.CloseConn();
            ahp2.CloseConn();
            ahp3.CloseConn();
            ahp4.CloseConn();
        }

        private void DataForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close_Conn();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            foreach (Property item in (PropertyManageCls)propertyGrid1.SelectedObject)
            {
                XtraMessageBox.Show(item.ReadOnly.ToString());
            }
        }
    }
}