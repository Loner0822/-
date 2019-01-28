//---------------------------------------------------------------------------

#ifndef MessageH
#define MessageH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ExtCtrls.hpp>
#include "MsgComm.h"
#include <inifiles.hpp>
#include <msxmldom.hpp>
#include <XMLDoc.hpp>
#include <xmldom.hpp>
#include <XMLIntf.hpp>
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
    TMsgComm *MsgComm1;
    TTimer *Timer1;
    TXMLDocument *XMLDocument1;
    TMemo *Memo1;
    void __fastcall MsgComm1Connect(TObject *Sender);
    void __fastcall MsgComm1Disconnect(TObject *Sender);
    void __fastcall Timer1Timer(TObject *Sender);
    void __fastcall MsgComm1Message(TObject *Sender, const char *from,
          const char *subject, const char *message);
    void __fastcall FormCreate(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm1(TComponent* Owner);
    AnsiString WorkPath;
    AnsiString PC_UserID, PC_UserName, PC_PersonName, PC_ExeTitleName;
    AnsiString IP, Port, Host;
    AnsiString GenerateGuidStr();
    void __fastcall SendImportMsg(AnsiString Explain, int data);
    void __fastcall OnCopyData(TMessage &msg);
        BEGIN_MESSAGE_MAP
                VCL_MESSAGE_HANDLER(WM_COPYDATA,TMessage,OnCopyData);
        END_MESSAGE_MAP(TForm)
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
