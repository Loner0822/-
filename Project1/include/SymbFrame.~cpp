//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop
#include "SymbFrame.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TSymbolFrame *SymbolFrame;
//---------------------------------------------------------------------------
__fastcall TSymbolFrame::TSymbolFrame(TComponent* Owner)
        : TFrame(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TSymbolFrame::Button1Click(TObject *Sender)
{
        TButton * btn = (TButton *)Sender;
        if( btn->Caption == ">")
        {
             State = 0;
             SymbolText  = ">"  ;
        }
        else if( btn->Caption == "<")
        {
             State = 0;
             SymbolText  = "<"  ;
        }
        else if( btn->Caption == "=")
        {
             State = 0;
             SymbolText  = "="  ;
        }
        else if( btn->Caption == "<=")
        {
             State = 0;
             SymbolText  = "<="  ;
        }
        else if( btn->Caption == ">=")
        {
             State = 0;
             SymbolText  = ">="  ;
        }
        else if( btn->Caption == "!=")
        {
             State = 0;
             SymbolText  = "!="  ;
        }
        else if( btn->Caption == "+")
        {
             State = 0;
             SymbolText  = "+"  ;
        }
        else if( btn->Caption == "-")
        {
             State = 0;
             SymbolText  = "-"  ;
        }
        else if( btn->Caption == "x")
        {
             State = 0;
             SymbolText  = "*"  ;
        }
        else if( btn->Caption == "÷")
        {
             State = 0;
             SymbolText  = "/"  ;
        }
        else if( btn->Caption == "(")
        {
             State = 0;
             SymbolText  = "("  ;
        }
        else if( btn->Caption == ")")
        {
             State = 0;
             SymbolText  = ")"  ;
        }
        else if( btn->Caption == "取消")
        {
             State = 2;
             SymbolText  = ""  ;
        }
        else if( btn->Caption == "确定")
        {
             State = 3;
             SymbolText  = ""  ;
        }
        if(State == 0 )
        {
            int s =Edit1->SelStart;
            int l = Edit1->SelLength;
            String str0 =Edit1->Text.SubString(1,s);
            String str1 = Edit1->Text.SubString(s+l+1,Edit1->Text.Length());
            Edit1->Text =  str0+SymbolText+ str1;
            Edit1->SetFocus();
            Edit1->SelStart = s+ SymbolText.Length();
            Edit1->SelLength = 0;
        }
}
//---------------------------------------------------------------------------

void __fastcall TSymbolFrame::Edit1KeyPress(TObject *Sender, char &Key)
{
         if(!(((Key >= '0') && (Key <= '9')) || (Key == VK_BACK) || (Key  == VK_DELETE)))
         {
            Key = 0;
         }
}
//---------------------------------------------------------------------------

