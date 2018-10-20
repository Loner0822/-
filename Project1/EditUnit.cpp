//---------------------------------------------------------------------------

#include <vcl.h>
#include <cstdio>
#include <typeinfo>
#include <objbase.h>
#pragma hdrstop

#include "EditUnit.h"
#include "MainUnit.h"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"

using namespace std;

TForm4 *Form4;
//---------------------------------------------------------------------------
__fastcall TForm4::TForm4(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm4::Button1Click(TObject *Sender)
{
    switch (this->Tip) {
        case 1:
            Form1->InsertNature(this->Edit1->Text);
            break;
            
        case 2:
            Form1->UpdateNature(this->Edit1->Text);
            break;
    }
    this->Close();
}
//---------------------------------------------------------------------------

void __fastcall TForm4::Button2Click(TObject *Sender)
{
    this->Close();    
}
//---------------------------------------------------------------------------
