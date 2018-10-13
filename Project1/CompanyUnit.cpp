//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "CompanyUnit.h"
#include "TBuildSoftwareF.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm3 *Form3;
//---------------------------------------------------------------------------
__fastcall TForm3::TForm3(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm3::ComboBox1Change(TObject *Sender)
{
    //ShowMessage(ComboBox1->Text);    
}
//---------------------------------------------------------------------------

void __fastcall TForm3::Button1Click(TObject *Sender)
{
    if (ComboBox1->Text != "") {
        // 打包发布
        BuildSoftware->Package(ComboBox1->Text);
    }
    ShowMessage("打包完成！");
    Form3->Close();
}
//---------------------------------------------------------------------------

