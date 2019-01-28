using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ucPropertyGrid;

namespace EnvirInfoSys
{
    /// <summary>
    /// 在PropertyGrid 上显示日期控件
    /// </summary>
    public class PropertyGridDateItem : UITypeEditor
    {

        MonthCalendar dateControl = new MonthCalendar();


        public PropertyGridDateItem()
        {

            dateControl.MaxSelectionCount = 1;

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

                IWindowsFormsEditorService edSvc =

(IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    // 给日历控件注册事件
                    dateControl.DateSelected += new DateRangeEventHandler(dateControl_DateSelected);


                    if (value is string)
                    {
                        if (value.Equals(""))
                        {
                            dateControl.SelectionStart = DateTime.Now.Date;
                        }
                        else
                        {
                            dateControl.SelectionStart = DateTime.Parse(value as String);
                        }

                        edSvc.DropDownControl(dateControl);


                        return dateControl.SelectionStart.Date.ToString("yyyy-MM-dd");

                    }

                    else if (value is DateTime)
                    {

                        dateControl.SelectionStart = (DateTime)value;

                        edSvc.DropDownControl(dateControl);


                        return dateControl.SelectionStart.Date.ToString("yyyy-MM-dd");

                    }

                }

            }

            catch (Exception ex)
            {
                XtraMessageBox.Show("当前日期格式错误！\n标准格式：" + DateTime.Now.ToLongDateString(), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                System.Console.WriteLine("PropertyGridDateItem Error : " + ex.Message);

                return value;

            }

            return value;

        }
        void dateControl_DateSelected(object sender, DateRangeEventArgs e)
        {
            // 利用键盘事件收缩下拉框
            API.keybd_event((byte)Keys.Escape, 0, 0, 0); // 按下
            API.keybd_event((byte)Keys.Escape, 0, 2, 0); // 抬起
        }

    }
}
