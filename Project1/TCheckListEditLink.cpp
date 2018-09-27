//---------------------------------------------------------------------------


#pragma hdrstop

#include "TCheckListEditLink.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)
__fastcall TCheckListEditLink::TCheckListEditLink(TComponent* Owner)
        : TEditLink(Owner)
{
     FCommaText = "";
}
__fastcall  TCheckListEditLink::  ~TCheckListEditLink(void)
{
     
}
void __fastcall TCheckListEditLink::CreateEditor(Controls::TWinControl* AParent)
{
     FEdit = new TCheckListEdit(Grid);
     FEdit->Parent = AParent;
     FEdit->Width = 0;
     FEdit->Height = 0;
     FEdit->SetFocus();
     FEdit->OnKeyDown = EditKeyDown;
     FEdit->OnExit = EditExit;
     WantKeyReturn = true;
}
void __fastcall TCheckListEditLink::DestroyEditor()
{
     if(FEdit)
     {
         delete FEdit;
         FEdit = NULL;
     }
}
void __fastcall TCheckListEditLink::SetRect(const TRect &r)
{
     FEdit->Top = r.Top;
     FEdit->Left = r.Left;
     FEdit->Items->CommaText = FCommaText;
     FEdit->Height = r.Bottom-r.Top;
     FEdit->Width = r.Right-r.left;

}
Controls::TWinControl* __fastcall TCheckListEditLink::GetEditControl(void)
{
       return   (TWinControl*)FEdit;
}
AnsiString __fastcall TCheckListEditLink::GetEditorValue()
{
      return   FEdit->Text ;
}
void __fastcall TCheckListEditLink::SetEditorValue(AnsiString s)
{
      FEdit->Text = s;
}
void __fastcall TCheckListEditLink::EditExit(System::TObject* Sender)
{
      HideEditor();
}
void __fastcall TCheckListEditLink::SetCommaText(String value)
{
    if(FCommaText != value) {
        FCommaText = value;
    }
}
String __fastcall TCheckListEditLink::GetCommaText()
{
    return FCommaText;
}
//===============================================
__fastcall TEditMemoLink::TEditMemoLink(TComponent* Owner)
        : TEditLink(Owner)
{
    
}
__fastcall  TEditMemoLink::  ~TEditMemoLink(void)
{
     
}
void __fastcall TEditMemoLink::CreateEditor(Controls::TWinControl* AParent)
{
     FEdit = new TMemo(Grid);
     FEdit->Parent = AParent;
     FEdit->SetFocus();
     FEdit->OnKeyDown = EditKeyDown;
     FEdit->OnExit = EditExit;
     FEdit->Width = 0;
     FEdit->Height = 0;
     WantKeyReturn = true;
}
void __fastcall TEditMemoLink::DestroyEditor()
{
     if(FEdit)
     {
         delete FEdit;
         FEdit = NULL;
     }
}
void __fastcall TEditMemoLink::SetRect(const TRect &r)
{
     FEdit->Top = r.Top;
     FEdit->Left = r.Left;
     FEdit->Height = 80;
     FEdit->Width = r.Right-r.left;
}
Controls::TWinControl* __fastcall TEditMemoLink::GetEditControl(void)
{
       return   (TWinControl*)FEdit;
}
AnsiString __fastcall TEditMemoLink::GetEditorValue()
{
      return   FEdit->Text ;
}
void __fastcall TEditMemoLink::SetEditorValue(AnsiString s)
{
      FEdit->Text = s;
}
void __fastcall TEditMemoLink::EditExit(System::TObject* Sender)
{
      HideEditor();
}
