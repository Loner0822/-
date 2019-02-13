//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop
#include "VideoForm.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "FlashVideoObjects_OCX"
#pragma resource "*.dfm"
TForm2 *Form2;
//---------------------------------------------------------------------------
__fastcall TForm2::TForm2(TComponent* Owner)
    : TForm(Owner)
{
}

//---------------------------------------------------------------------------
void __fastcall TForm2::FormShow(TObject *Sender)
{
    isShow = true;
    startTime = Now();
    SetWindowPos(this->Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
    this->FlashVideo1->Width = this->FlashVideo2-> Width / 3;
    this->FlashVideo1->Height = this->FlashVideo2-> Height / 3;
    this->FlashVideo1->Left = this->FlashVideo2->Left + this->FlashVideo2->Width - this->FlashVideo2-> Width / 3 - 20;
    this->FlashVideo1->Top = this->FlashVideo2->Top + this->FlashVideo2->Height - this->FlashVideo2-> Height / 3 - 20;
}
//---------------------------------------------------------------------------


void __fastcall TForm2::Timer1Timer(TObject *Sender)
{
    TDateTime nowTime = Now();
    if ((double)nowTime - (double)startTime > 20.0 / (24.0 * 60 * 60) && isShow)
    {
        isShow = false;
        ShowMessage("暂未收到对方视频信号,准备关闭界面");
        this->Close();
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm2::FormCreate(TObject *Sender)
{
    isShow = false;
    this->FlashVideo1->Init("");
    this->FlashVideo2->Init("");
}
//---------------------------------------------------------------------------

void __fastcall TForm2::FormClose(TObject *Sender, TCloseAction &Action)
{
    isShow = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm2::FormResize(TObject *Sender)
{
    this->FlashVideo1->Width = this->FlashVideo2-> Width / 3;
    this->FlashVideo1->Height = this->FlashVideo2-> Height / 3;
    this->FlashVideo1->Left = this->FlashVideo2->Left + this->FlashVideo2->Width - this->FlashVideo2-> Width / 3 - 20;
    this->FlashVideo1->Top = this->FlashVideo2->Top + this->FlashVideo2->Height - this->FlashVideo2-> Height / 3 - 10;    
}
//---------------------------------------------------------------------------

void __fastcall TForm2::N1Click(TObject *Sender)
{
    if (this->N1->Caption == "隐藏本地摄像头")
    {
        this->N1->Caption = "显示本地摄像头";
        this->FlashVideo1->Visible = true;
    }
    else
    {
        this->N1->Caption = "隐藏本地摄像头";
        this->FlashVideo1->Visible = false;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm2::N2Click(TObject *Sender)
{
    this->Close();
}
//---------------------------------------------------------------------------

