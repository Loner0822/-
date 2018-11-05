//---------------------------------------------------------------------------

#ifndef MainUnitH
#define MainUnitH
#include <Classes.hpp>
#include <ComCtrls.hpp>
#include <Controls.hpp>
#include <ScktComp.hpp>
#include <StdCtrls.hpp>
//---------------------------------------------------------------------------
#include <vector>
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ScktComp.hpp>
#include <ComCtrls.hpp>
#include "ConfigUnit.h"
#include "ListUnit.h"
#include "EditUnit.h"
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
    TButton *Button1;
    TClientSocket *ClientSocket;
    TButton *Button2;
    TButton *Button3;
    TStatusBar *StatusBar;
    TButton *Button4;
    void __fastcall ClientSocketConnect(TObject *Sender,
          TCustomWinSocket *Socket);
    void __fastcall ClientSocketConnecting(TObject *Sender,
          TCustomWinSocket *Socket);
    void __fastcall ClientSocketDisconnect(TObject *Sender,
          TCustomWinSocket *Socket);
    void __fastcall ClientSocketError(TObject *Sender,
          TCustomWinSocket *Socket, TErrorEvent ErrorEvent,
          int &ErrorCode);
    void __fastcall ClientSocketRead(TObject *Sender,
          TCustomWinSocket *Socket);
    void __fastcall Button1Click(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall Button4Click(TObject *Sender);
    void __fastcall Button2Click(TObject *Sender);
    void __fastcall Button3Click(TObject *Sender);
    void __fastcall FormCloseQuery(TObject *Sender, bool &CanClose);
private:	// User declarations
public:		// User declarations
    __fastcall TForm1(TComponent* Owner);
    String AppName;
    String TimeNow;
    std::vector<String> TableName;
    int ServerPort, RecvPort;
    int op;
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
