using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using ucPropertyGrid;

namespace EnvirInfoSys
{
    // <summary>
    /// 实现弹窗
    /// </summary>
    public class PropertyGridMultiSelect : UITypeEditor
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PropertyGridMultiSelect()
        {

        }

        /// <summary>
        /// 已经选中的选项
        /// </summary>
        private string allOptions = string.Empty;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="allOptions">可选项</param>
        /// <param name="curSelectedOptions">已经选中的项</param>
        public PropertyGridMultiSelect(string allOptions)
        {
            this.allOptions = allOptions;

        }


        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
            ;

        }
        /// <summary>
        /// 编辑属性的值
        /// </summary>
        /// <param name="context">描述信息提供对象</param>
        /// <param name="provider">属性对象</param>
        /// <param name="value">属性的值</param>
        /// <returns></returns>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (edSvc != null)
            {
                #region 下拉框方式

                ucMultiSelect uc = new ucMultiSelect(allOptions, value.ToString());

                edSvc.DropDownControl(uc);

                return uc.SelectedOptions;

                #endregion
            }

            return value;

        }
    }
}
