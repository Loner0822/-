using System;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ucPropertyGrid;
using System.Windows.Forms.Design;

namespace EnvirInfoSys
{
    public class PropertyGridNumber : UITypeEditor
    {
        TextEdit textedit = new TextEdit();

        private string originvalue = "";
        private string danwei = "";
        private double? upper = null;
        private double? limit = null;
        private int? afterdecpoint = 0;

        public PropertyGridNumber(Dictionary<string, string> dicdata)
        {
            textedit.KeyPress += textedit_KeyPress;
            
            danwei = dicdata["danwei"];
            originvalue = dicdata["defvalue"];
            if (dicdata["upper"] != string.Empty)
                upper = double.Parse(dicdata["upper"]);
            if (dicdata["limit"] != string.Empty)
                limit = double.Parse(dicdata["limit"]);
            if (dicdata["afterdecpoint"] != string.Empty)
                afterdecpoint = int.Parse(dicdata["afterdecpoint"]);
            textedit.ToolTip = "单位：" + danwei;
            textedit.Properties.Mask.UseMaskAsDisplayFormat = true;
            textedit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            textedit.Properties.Mask.EditMask = "[0-9]{1,}[.]{0,1}[0-9]{0," + afterdecpoint.ToString() + "}";
            textedit.Text = dicdata["defvalue"];
        }

        private void textedit_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar < '0' || e.KeyChar > '9')
                e.Handled = true;
            if (e.KeyChar == '.')
                e.Handled = false;
            if (e.KeyChar == 8)
                e.Handled = false;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            try
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                    edSvc.DropDownControl(textedit);
                    if (!Check_Input(textedit.Text))
                        textedit.Text = "";
                    if (textedit.Text != "")
                    {
                        originvalue = textedit.Text;
                        return textedit.Text + danwei;
                    }
                    else
                    {
                        if (originvalue == "")
                            return "";
                        else
                            return originvalue + danwei;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("PropertyGridNumber Error : " + ex.Message);
                return value;
            }
            return value;
        }

        private bool Check_Input(string num)
        {
            double res = double.Parse(num);
            if (upper != null)
                if (res > upper)
                {
                    XtraMessageBox.Show("该值已超出上限，上限为" + upper.ToString());
                    return false;
                }
            if (limit != null)
                if (res < limit)
                {
                    XtraMessageBox.Show("该值已低于下限，下限为" + limit.ToString());
                    return false;
                }
            return true;
        }

    }
}
