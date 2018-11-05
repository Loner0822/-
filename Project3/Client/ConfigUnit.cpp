//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "ConfigUnit.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm2 *Form2;
//---------------------------------------------------------------------------
__fastcall TForm2::TForm2(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TForm2::FormCreate(TObject *Sender)
{
    ComboBox->Text = "127.0.0.1";
    ComboBox->SelectAll();
    Edit->Text = 900;
}
//---------------------------------------------------------------------------

void __fastcall TForm2::BitBtn1Click(TObject *Sender)
{
    TStringList *IPList = new TStringList;
    IPList->AddStrings(ComboBox->Items);
    int Index;
    if (!IPList->Find(ComboBox->Text, Index)) {
  	    IPList->Append(ComboBox->Text);
  	    ComboBox->Items->Clear();
  	    ComboBox->Items->AddStrings(IPList);
    }
    delete IPList;
}
//---------------------------------------------------------------------------

