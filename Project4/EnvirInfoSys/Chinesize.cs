using DevExpress.Dialogs.Core.Localization;
using DevExpress.XtraEditors.Controls;
using DevExpress.Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraGrid.Localization;

namespace EnvirInfoSys
{
    /// <summary>
    /// MessageBox 中文
    /// </summary>
    public class MessageboxClass : Localizer
    {
        public override string GetLocalizedString(StringId id)
        {
            switch (id)
            {
                case StringId.XtraMessageBoxCancelButtonText:
                    return "取消";
                case StringId.XtraMessageBoxOkButtonText:
                    return "确定";
                case StringId.XtraMessageBoxYesButtonText:
                    return "是";
                case StringId.XtraMessageBoxNoButtonText:
                    return "否";
                case StringId.XtraMessageBoxIgnoreButtonText:
                    return "忽略";
                case StringId.XtraMessageBoxAbortButtonText:
                    return "中止";
                case StringId.XtraMessageBoxRetryButtonText:
                    return "重试";
                default:
                    return id.ToString();
            }
        }
    }

    public class BrowserFolder : DialogsLocalizer
    {
        public override string GetLocalizedString(DialogsStringId id)
        {
            switch (id)
            {
                case DialogsStringId.MakeNewFolderButtonText:
                    return "新建文件夹";
                case DialogsStringId.OkButtonText:
                    return "确认";
                case DialogsStringId.OpenFileDialogOkButtonText:
                    return "打开";
                case DialogsStringId.CancelButtonText:
                    return "取消";
                case DialogsStringId.FileNameLabelText:
                    return "打开文件";
                case DialogsStringId.NewFolderButtonText:
                    return "新建文件夹";
                case DialogsStringId.OpenFileDialogDefaultTitle:
                    return "打开文件";
                default:
                    return id.ToString();
            }
        }
    }

    public class GridViewer : GridLocalizer
    {
        public override string GetLocalizedString(GridStringId id)
        {
            switch (id)
            {
                case GridStringId.FilterBuilderApplyButton:
                    return "应用";
                case GridStringId.FilterBuilderCancelButton:
                    return "取消";
                case GridStringId.FilterBuilderCaption:
                    return "标题";
                default:
                    return id.ToString();
            }
        }
    }
}
