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
#include <Menus.hpp>
#include "logrec.h"
#include "trayicon.h"
#include "SetUnit.h"
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
    TStatusBar *StatusBar;
    TButton *Button;
    TServerSocket *ServerSocket;
    TPopupMenu *PopupMenu;
    TMenuItem *S1;
    TMenuItem *E1;
    TTrayIcon *TrayIcon;
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
    void __fastcall FormCloseQuery(TObject *Sender, bool &CanClose);
    void __fastcall S1Click(TObject *Sender);
    void __fastcall E1Click(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm1(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
