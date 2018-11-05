//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "ListUnit.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "AdvGrid"
#pragma link "BaseGrid"
#pragma link "AdvGrid"
#pragma link "BaseGrid"
#pragma resource "*.dfm"
TForm3 *Form3;
//---------------------------------------------------------------------------
__fastcall TForm3::TForm3(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm3::AdvStringGridGetAlignment(TObject *Sender,
      int ARow, int ACol, TAlignment &HAlign, TVAlignment &VAlign)
{
    if (ARow == 0 || ACol != 1) {
        HAlign = taCenter;
        VAlign = vtaCenter;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm3::AdvStringGridResize(TObject *Sender)
{
    AdvStringGrid->ColWidths[0] = 64;
    AdvStringGrid->ColWidths[2] = 64;
    AdvStringGrid->ColWidths[1] = AdvStringGrid->Width - 64 - 64 - 20;
}
//---------------------------------------------------------------------------

void __fastcall TForm3::FormRefresh() {
    FileNum = -1;
    AdvStringGrid->Clear();
    AdvStringGrid->Options << goRowSelect;
    AdvStringGrid->Options << goEditing;
    AdvStringGrid->Cells[0][0] = "文件序号";
    AdvStringGrid->Cells[1][0] = "文件名称";
    AdvStringGrid->Cells[2][0] = "是否选择";
    AdvStringGrid->ColCount = 3;
    AdvStringGrid->RowCount = FileName.size() + 1;
    if (AdvStringGrid->RowCount < 2)
        AdvStringGrid->RowCount = 2;
    AdvStringGrid->FixedCols = 1;
    AdvStringGrid->FixedRows = 1;
    AdvStringGrid->ColWidths[0] = 64;
    AdvStringGrid->ColWidths[2] = 64;
    AdvStringGrid->ColWidths[1] = AdvStringGrid->Width - 64 - 64 - 20;
    for (int i = 0; i < (int)FileName.size(); ++ i) {
        AdvStringGrid->Cells[0][i + 1] = i + 1;
        AdvStringGrid->Cells[1][i + 1] = FileName[i];
        AdvStringGrid->AddRadioButton(2, i + 1, false);     
    }

    AdvStringGrid1->Clear();
    AdvStringGrid1->Options << goRowSelect;
    AdvStringGrid1->Options << goEditing;
    AdvStringGrid1->Cells[0][0] = "文件序号";
    AdvStringGrid1->Cells[1][0] = "文件名称";
    AdvStringGrid1->Cells[2][0] = "是否选择";
    AdvStringGrid1->ColCount = 3;
    AdvStringGrid1->RowCount = FileName2.size() + 1;
    if (AdvStringGrid1->RowCount < 2)
        AdvStringGrid1->RowCount = 2;
    AdvStringGrid1->FixedCols = 1;
    AdvStringGrid1->FixedRows = 1;
    AdvStringGrid1->ColWidths[0] = 64;
    AdvStringGrid1->ColWidths[2] = 64;
    AdvStringGrid1->ColWidths[1] = AdvStringGrid->Width - 64 - 64 - 20;
    for (int i = 0; i < (int)FileName2.size(); ++ i) {
        AdvStringGrid1->Cells[0][i + 1] = i + 1;
        AdvStringGrid1->Cells[1][i + 1] = FileName2[i];
        AdvStringGrid1->AddRadioButton(2, i + 1, false);
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm3::BitBtnClick(TObject *Sender)
{
    //ShowMessage(PageControl->ActivePageIndex);
    if (PageControl->ActivePageIndex == 0) {
        op = 1;
        for (int i = 1; i < AdvStringGrid->RowCount; ++ i) {
            if (AdvStringGrid->IsRadioButtonChecked(2, i)) {
                FileNum = i - 1;
                break;
            }
        }
    }
    else {
        op = 2;
        for (int i = 1; i < AdvStringGrid1->RowCount; ++ i) {
            if (AdvStringGrid1->IsRadioButtonChecked(2, i)) {
                FileNum = i - 1;
                break;
            }
        }
    }
}
//---------------------------------------------------------------------------

