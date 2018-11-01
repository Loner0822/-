//---------------------------------------------------------------------------

#ifndef MainUnitH
#define MainUnitH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <ScktComp.hpp>
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
    TStatusBar *StatusBar;
    TButton *Button;
    TServerSocket *ServerSocket;
    void __fastcall FormShow(TObject *Sender);
    void __fastcall ButtonClick(TObject *Sender);
    void __fastcall ServerSocketAccept(TObject *Sender,
          TCustomWinSocket *Socket);
    void __fastcall ServerSocketClientConnect(TObject *Sender,
          TCustomWinSocket *Socket);
    void __fastcall ServerSocketClientDisconnect(TObject *Sender,
          TCustomWinSocket *Socket);
    void __fastcall ServerSocketClientError(TObject *Sender,
          TCustomWinSocket *Socket, TErrorEvent ErrorEvent,
          int &ErrorCode);
    void __fastcall ServerSocketClientRead(TObject *Sender,
          TCustomWinSocket *Socket);
private:	// User declarations
public:		// User declarations
    __fastcall TForm1(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
