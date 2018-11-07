//---------------------------------------------------------------------------


#ifndef SymbFrameH
#define SymbFrameH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
//---------------------------------------------------------------------------
class TSymbolFrame : public TFrame
{
__published:	// IDE-managed Components
        TButton *Button1;
        TButton *Button2;
        TButton *Button3;
        TButton *Button4;
        TButton *Button5;
        TButton *Button6;
        TButton *Button7;
        TButton *Button10;
        TButton *Button11;
        TEdit *Edit1;
        TButton *Button8;
        TButton *Button9;
        TButton *Button12;
        TButton *Button13;
        TButton *Button14;
        void __fastcall Button1Click(TObject *Sender);
        void __fastcall Edit1KeyPress(TObject *Sender, char &Key);
private:	// User declarations

public:		// User declarations
        __fastcall TSymbolFrame(TComponent* Owner);
        String SymbolText ;
        int State; //0:选择了符号；1：下一个；2：取消；3：确定
};
//---------------------------------------------------------------------------
extern PACKAGE TSymbolFrame *SymbolFrame;
//---------------------------------------------------------------------------
#endif
