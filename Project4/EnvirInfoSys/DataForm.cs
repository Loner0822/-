using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ucPropertyGrid;

namespace EnvirInfoSys
{
    public partial class DataForm : Form
    {
        private AccessHelper ahp = null;
        //private IniOperator inip = null;   

        public string Icon_GUID = "";       //当前选择的图标guid
        public string Node_GUID = "";       //当前选择的节点guid
        public string Node_Name = "";       //当前选择的节点名称
        public Dictionary<string, string> FDName_Value;
        
        private string WorkPath = AppDomain.CurrentDomain.BaseDirectory;//当前exe根目录
        private string AccessPath1 = AppDomain.CurrentDomain.BaseDirectory + "data\\ENVIR_H0001Z000E00.mdb";
        private string AccessPath2 = AppDomain.CurrentDomain.BaseDirectory + "data\\ZSK_H0001Z000K00.mdb";
        private string AccessPath3 = AppDomain.CurrentDomain.BaseDirectory + "data\\ZSK_H0001Z000K01.mdb";
        private string IniFilePath = AppDomain.CurrentDomain.BaseDirectory + "parameter.ini";
        private Dictionary<string, string> Show_Name;
        private Dictionary<string, string> Show_FDName;
        private Dictionary<string, string> inherit_GUID;

        public DataForm()
        {
            InitializeComponent();
        }

        private Dictionary<string, string> Get_Prop_Type() 
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            res.Add("{26E232C8-595F-44E5-8E0F-8E0FC1BD7D24}", "固定属性");
            res.Add("{B55806E6-9D63-4666-B6EB-AAB80814648E}", "基础属性");
            return res;
        }

        private List<string> Get_Prop_List(Dictionary<string, string> prop_type)
        {
            List<string> res = new List<string> ();
            List<string> _type = new List<string> (prop_type.Keys);
            
            ahp = new AccessHelper(AccessPath2);
            string sql = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROTYPEGUID from ZSK_PROP_H0001Z000K00 where ISDELETE = 0 and UPGUID = '" + Icon_GUID + "' order by SHOWINDEX";
            DataTable dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                //for (int j = 0; j < _type.Count; ++ j)
                {
                    if (dt.Rows[i]["PROTYPEGUID"].ToString() == _type[1])
                    {
                        res.Add(dt.Rows[i]["PGUID"].ToString());
                        Show_Name[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["PROPNAME"].ToString();
                        Show_FDName[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["FDNAME"].ToString();
                        inherit_GUID[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["SOURCEGUID"].ToString();
                    }                        
                }
            }
            ahp = new AccessHelper(AccessPath3);
            sql = "select PGUID, PROPNAME, FDNAME, SOURCEGUID, PROTYPEGUID from ZSK_PROP_H0001Z000K01 where ISDELETE = 0 and UPGUID = '" + Icon_GUID + "' order by SHOWINDEX";
            dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++i)
            {
                //for (int j = 0; j < _type.Count; ++j)
                {
                    if (dt.Rows[i]["PROTYPEGUID"].ToString() == _type[1])
                    {
                        res.Add(dt.Rows[i]["PGUID"].ToString());
                        Show_Name[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["PROPNAME"].ToString();
                        Show_FDName[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["FDNAME"].ToString();
                        inherit_GUID[dt.Rows[i]["PGUID"].ToString()] = dt.Rows[i]["SOURCEGUID"].ToString();
                    }
                }
            }
            return res;
        }

        private string Get_Data_Type(string propguid) 
        {
            string dt_guid = Icon_GUID + "_" + propguid;
            string sql;
            DataTable dt, dt2;
            if (inherit_GUID[propguid] != "") 
            {
                propguid = inherit_GUID[propguid];
                ahp = new AccessHelper(AccessPath2);
                sql = "select UPGUID from ZSK_PROP_H0001Z000K00 where ISDELETE = 0 and PGUID = '" + propguid + "'";
                dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count != 0)
                {
                    dt_guid = dt.Rows[0]["UPGUID"].ToString() + "_" + propguid;
                }
                ahp = new AccessHelper(AccessPath3);
                sql = "select UPGUID from ZSK_PROP_H0001Z000K01 where ISDELETE = 0 and PGUID = '" + propguid + "'";
                dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count != 0)
                {
                    dt_guid = dt.Rows[0]["UPGUID"].ToString() + "_" + propguid;
                }
            }
            
            ahp = new AccessHelper(AccessPath2);
            sql = "select DATATYPE from ZSK_DATATYPE_H0001Z000K00 where ISDELETE = 0 and UPGUID = '" + dt_guid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count != 0)
            {
                if (dt.Rows[0]["DATATYPE"].ToString() != "可选项")
                    return dt.Rows[0]["DATATYPE"].ToString();
                else
                {
                    sql = "select PROPVALUE from ZSK_LIMIT_H0001Z000K00 where ISDELETE = 0 and UPGUID = '" + dt_guid + "'";
                    dt2 = ahp.ExecuteDataTable(sql, null);
                    if (dt2.Rows.Count != 0) {
                        if (dt2.Rows[0]["PROPVALUE"].ToString() == "否")
                            return "可选项";
                        else
                            return "多选";                        
                    }
                }
            }

            ahp = new AccessHelper(AccessPath3);
            sql = "select DATATYPE from  ZSK_DATATYPE_H0001Z000K01 where ISDELETE = 0 and UPGUID = '" + dt_guid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            if (dt.Rows.Count != 0)
            {
                if (dt.Rows[0]["DATATYPE"].ToString() != "可选项")
                    return dt.Rows[0]["DATATYPE"].ToString();
                else
                {
                    sql = "select PROPVALUE from ZSK_LIMIT_H0001Z000K01 where ISDELETE = 0 and UPGUID = '" + dt_guid + "'";
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
            return "Error";
        }

        private string Get_fw(string propguid) 
        {
            string cl_guid = Icon_GUID + "_" + propguid;
            string sql;
            DataTable dt;
            if (inherit_GUID[propguid] != "")
            {
                propguid = inherit_GUID[propguid];
                ahp = new AccessHelper(AccessPath2);
                sql = "select UPGUID from ZSK_PROP_H0001Z000K00 where ISDELETE = 0 and PGUID = '" + propguid + "'";
                dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count != 0)
                {
                    cl_guid = dt.Rows[0]["UPGUID"].ToString() + "_" + propguid;
                }
                ahp = new AccessHelper(AccessPath3);
                sql = "select UPGUID from ZSK_PROP_H0001Z000K01 where ISDELETE = 0 and PGUID = '" + propguid + "'";
                dt = ahp.ExecuteDataTable(sql, null);
                if (dt.Rows.Count != 0)
                {
                    cl_guid = dt.Rows[0]["UPGUID"].ToString() + "_" + propguid;
                }
            }
            
            string res = "";
            ahp = new AccessHelper(AccessPath2);
            sql = "select COMBOSTR from  ZSK_COMBOSTRLIST_H0001Z000K00 where ISDELETE = 0 and UPGUID = '" + cl_guid + "'";
            dt = ahp.ExecuteDataTable(sql, null);
            for (int i = 0; i < dt.Rows.Count; ++ i)
            {
                if (i == 0)
                    res += dt.Rows[i]["COMBOSTR"].ToString();
                else
                    res += "," + dt.Rows[i]["COMBOSTR"].ToString();
            }

            ahp = new AccessHelper(AccessPath3);
            sql = "select COMBOSTR from  ZSK_COMBOSTRLIST_H0001Z000K01 where ISDELETE = 0 and UPGUID = '" + cl_guid + "'";
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

        private void DataForm_Shown(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            PropertyManageCls pmc = new PropertyManageCls(); // 创建属性集合实例 
            Dictionary<string, string> prop_type = new Dictionary<string,string>();
            Show_Name = new Dictionary<string, string>();
            Show_FDName = new Dictionary<string, string>();
            inherit_GUID = new Dictionary<string, string>();
            
            prop_type = Get_Prop_Type();
            List<string> prop_list;
            prop_list = Get_Prop_List(prop_type);
            List<string> type_guid = new List<string> (prop_type.Keys);


            
            //for (int i = 0; i < len; ++i) 
            {
                int i = 0;
                string the_type = prop_type[type_guid[i]];
                for (int j = 0; j < prop_list.Count; ++j)
                {
                    Property p = new Property(Show_Name[prop_list[j]], "", false, true);
                    p.DisplayName = Show_Name[prop_list[j]];
                    p.Category = the_type;
                    p.FdName = Show_FDName[prop_list[j]];
                    p.Value = "";

                    string datatype = Get_Data_Type(prop_list[j]);
                    switch (datatype)
                    {
                        case "文本":
                        case "数字":
                            break;
                        case "可选项":
                            string kxfw = Get_fw(prop_list[j]);
                            p.Converter = new DropDownListConverter(kxfw.Split(','));
                            break;
                        case "时间":
                            p.Editor = new PropertyGridDateTimePickerItem();
                            break;
                        case "日期":
                            p.Editor = new PropertyGridDateItem();
                            break;
                        case "多选":
                            string fw = Get_fw(prop_list[j]);
                            p.Editor = new PropertyGridMultiSelect(fw);
                            break;
                        default:
                            MessageBox.Show(Show_Name[prop_list[j]] + "属性找不到数据类型");
                            break;
                    }
                    pmc.Add(p);
                }
            }
            watch.Stop();
            TimeSpan timespan = watch.Elapsed;
            double ms = timespan.TotalMilliseconds;
            propertyGrid1.SelectedObject = pmc; // 加载属性
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FDName_Value = new Dictionary<string, string>();
            PropertyManageCls pmc = (PropertyManageCls)propertyGrid1.SelectedObject;
            
            foreach (Property item in pmc)
            {
                if (item.DisplayName == "名称")
                {
                    if (item.Value.ToString() == "")
                    {
                        MessageBox.Show("请填入名称!");
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
                }
            }
            this.DialogResult = DialogResult.OK;
            return;
        }
    }
}
