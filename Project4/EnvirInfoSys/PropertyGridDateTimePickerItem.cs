using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace EnvirInfoSys
{
    /// <summary>
    /// 在PropertyGrid 上显示日期控件
    /// </summary>
    public class PropertyGridDateTimePickerItem : UITypeEditor
    {

        DateTimePicker dateControl = new DateTimePicker();


        public PropertyGridDateTimePickerItem()
        {


        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {

            return UITypeEditorEditStyle.DropDown;

        }


        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context,

System.IServiceProvider provider, object value)
        {

            try
            {
                //IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {

                    if (value is string)
                    {
                        if (value.Equals(""))
                        {
                            value = "00:00:00";
                        }
                        else
                        {
                            dateControl.Text = value.ToString();
                        }
                        dateControl.Format = DateTimePickerFormat.Time;
                        dateControl.ShowUpDown = true;
                        edSvc.DropDownControl(dateControl);


                        return dateControl.Text;


                    }

                    else if (value is DateTime)
                    {

                        dateControl.Text = value.ToString();
                        edSvc.DropDownControl(dateControl);
                        return dateControl.Text;


                    }

                }

            }

            catch (Exception ex)
            {

                System.Console.WriteLine("PropertyGridDateItem Error : " + ex.Message);

                return value;

            }

            return value;

        }

    }
}
