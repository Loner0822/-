//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "ExtraUnit.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "AdvGrid"
#pragma link "BaseGrid"
#pragma resource "*.dfm"
TExtraForm *ExtraForm;
//---------------------------------------------------------------------------
__fastcall TExtraForm::TExtraForm(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TExtraForm::FormCreate(TObject *Sender) {

}
//---------------------------------------------------------------------------
void TExtraForm::Move_Pen(const vector<set<Pen>::iterator> &L) {
    AdvStringGridExtra -> Clear();
    AdvStringGridExtra -> Options << goColSizing;
    AdvStringGridExtra -> Options << goRowSizing;
    AdvStringGridExtra -> Options << goRowSelect;
    int len = L.size();
    AdvStringGridExtra -> RowCount = len + 1;
    AdvStringGridExtra -> Cells[0][0] = "��ʱ��";
    AdvStringGridExtra -> Cells[1][0] = "�������";
    AdvStringGridExtra -> Cells[2][0] = "�Ƿ��ƶ�";
    for (int i = 0; i < len; ++ i) {
        AdvStringGridExtra -> Cells[0][i + 1] = L[i] -> id;
        if (L[i] -> type == 1)
            AdvStringGridExtra -> Cells[1][i + 1] = "����";
        if (L[i] -> type == 2)
            AdvStringGridExtra -> Cells[1][i + 1] = "�ڱ��";
        if (L[i] -> type == 3)
            AdvStringGridExtra -> Cells[1][i + 1] = "����ǯ";
        AdvStringGridExtra->AddRadioButton(2, i + 1, false);
    }
}

void TExtraForm::Delete_Pen (const vector<set<Pen>::iterator> &L) {
    AdvStringGridExtra -> Clear();
    AdvStringGridExtra -> Options << goColSizing;
    AdvStringGridExtra -> Options << goRowSizing;
    AdvStringGridExtra -> Options << goRowSelect;
    AdvStringGridExtra -> Options << goEditing;
    int len = L.size();
    AdvStringGridExtra -> RowCount = len + 1;
    AdvStringGridExtra -> Cells[0][0] = "��ʱ��";
    AdvStringGridExtra -> Cells[1][0] = "�������";
    AdvStringGridExtra -> Cells[2][0] = "�Ƿ�ɾ��";
    for (int i = 0; i < len; ++ i) {
        AdvStringGridExtra -> Cells[0][i + 1] = L[i] -> id;
        if (L[i] -> type == 1)
            AdvStringGridExtra -> Cells[1][i + 1] = "����";
        if (L[i] -> type == 2)
            AdvStringGridExtra -> Cells[1][i + 1] = "�ڱ��";
        if (L[i] -> type == 3)
            AdvStringGridExtra -> Cells[1][i + 1] = "����ǯ";
        AdvStringGridExtra->AddCheckBox(2, i + 1, false, false);
    }
}


//---------------------------------------------------------------------------



void __fastcall TExtraForm::AdvStringGridExtraClickCell(TObject *Sender,
      int ARow, int ACol)
{
    //
    //AdvStringGridExtra->CheckCell()
}
//---------------------------------------------------------------------------

