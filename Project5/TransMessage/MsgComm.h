//---------------------------------------------------------------------------

#ifndef MsgCommH
#define MsgCommH
//---------------------------------------------------------------------------
#include <SysUtils.hpp>
#include <Controls.hpp>
#include <Classes.hpp>
#include <Forms.hpp>
//---------------------------------------------------------------------------
typedef void __fastcall (__closure *TMsgCommEvent)(System::TObject* Sender, const char* from, const char* subject, const char* message);

class PACKAGE TMsgComm : public TComponent
{
private:
    AnsiString FUser;
    AnsiString FHost;
    AnsiString FPassword;
    AnsiString FServer;
    int FPort;
    TNotifyEvent FOnConnect;
    TNotifyEvent FOnDisconnect;
    TMsgCommEvent FOnMessage;
    void __fastcall SetUser(AnsiString value);
    AnsiString __fastcall GetUser();
    void __fastcall SetHost(AnsiString value);
    AnsiString __fastcall GetHost();
    void __fastcall SetPassword(AnsiString value);
    AnsiString __fastcall GetPassword();
    void __fastcall SetServer(AnsiString value);
    AnsiString __fastcall GetServer();
    void __fastcall SetPort(int value);
    int __fastcall GetPort();
    void __fastcall SetOnConnect(TNotifyEvent value);
    TNotifyEvent __fastcall GetOnConnect();
    void __fastcall SetOnDisconnect(TNotifyEvent value);
    TNotifyEvent __fastcall GetOnDisconnect();
    void __fastcall SetOnMessage(TMsgCommEvent value);
    TMsgCommEvent __fastcall GetOnMessage();
protected:
    void* FHandle;
    class MsgCallback;
    friend class MsgCallback;
public:
    __fastcall TMsgComm(TComponent* Owner);
    bool Open();
    bool Close();
    bool Send(AnsiString to, AnsiString subject, AnsiString msg);
    void Debug( int mode, const char* param );
__published:
    __property AnsiString User  = { read=GetUser, write=SetUser };
    __property AnsiString Host  = { read=GetHost, write=SetHost };
    __property AnsiString Password  = { read=GetPassword, write=SetPassword };
    __property AnsiString Server  = { read=GetServer, write=SetServer };
    __property int Port  = { read=GetPort, write=SetPort };
    __property TNotifyEvent OnConnect  = { read=GetOnConnect, write=SetOnConnect };
    __property TNotifyEvent OnDisconnect  = { read=GetOnDisconnect, write=SetOnDisconnect };
    __property TMsgCommEvent OnMessage  = { read=GetOnMessage, write=SetOnMessage };
};
//---------------------------------------------------------------------------
#endif
 