//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "MsgComm.h"
#pragma package(smart_init)
//---------------------------------------------------------------------------

#include "CommMsg.h"
typedef bool __stdcall (*xmcallbackfunc)( void* xmhandle, void* param, int data, void* extdata );

typedef void* __stdcall (*f_xmopen)( const char* user, const char* pwd, const char* host, const char* server, const int &port );
typedef int __stdcall (*f_xmreg)( void* handle, xmcallbackfunc functions[], void* param );
typedef int __stdcall (*f_xmstart)( void* handle, int connect, int timeout );
typedef int __stdcall (*f_xmsend)( void* handle, const char* user, const char* subject, const char* data, int length );
typedef int __stdcall (*f_xmsendbin)( void* handle, const char* user, const char* subject, const char* data, int length );
typedef int __stdcall (*f_xmclose)( void* handle );
typedef void __stdcall (*f_xmdebug)( int mode, const char* data );

static f_xmopen xmopen = 0;
static f_xmreg xmreg = 0;
static f_xmstart xmstart = 0;
static f_xmsend xmsend = 0;
static f_xmsendbin xmsendbin = 0;
static f_xmclose xmclose = 0;
static f_xmdebug xmdebug = 0;

static bool s_funcInited = false;
static bool InitFunctions()
{
    if( s_funcInited )
        return xmopen;

    HINSTANCE hInst = LoadLibrary("LibComm.dll");
    if( hInst )
    {
        xmopen = (f_xmopen)GetProcAddress(hInst, "xmopen");
        xmreg = (f_xmreg)GetProcAddress(hInst, "xmreg");
        xmstart = (f_xmstart)GetProcAddress(hInst, "xmstart");
        xmsend = (f_xmsend)GetProcAddress(hInst, "xmsend");
        xmsendbin = (f_xmsendbin)GetProcAddress(hInst, "xmsendbin");
        xmclose = (f_xmclose)GetProcAddress(hInst, "xmclose");
        xmdebug = (f_xmdebug)GetProcAddress(hInst, "xmdebug");
    }
    return xmopen;
}

class TMsgComm::MsgCallback
{
public:
    static bool __stdcall OnConnect( void* xmhandle, void* param, int data, void* msg )
    {
        TMsgComm* comm = (TMsgComm*)param;
        if( comm->OnConnect )
            comm->OnConnect( comm );
        return true;
    }
    static bool __stdcall OnLogin( void* xmhandle, void* param, int data, void* msg )
    {
        TMsgComm* comm = (TMsgComm*)param;
        if( comm->OnConnect )
            comm->OnConnect( comm );
        return true;
    }
    static bool __stdcall OnMessage( void* xmhandle, void* param, int data, void* msg )
    {
        XmMsg* m = (XmMsg*)msg;
        TMsgComm* comm = (TMsgComm*)param;
        if( comm->OnMessage )
            comm->OnMessage( comm, m->from, m->subject, m->body );
        return true;
    }
    static bool __stdcall OnDisconnect( void* xmhandle, void* param, int data, void* msg )
    {
        TMsgComm* comm = (TMsgComm*)param;
        if( comm->OnDisconnect )
            comm->OnDisconnect( comm );
        return true;
    }
};



// ValidCtrCheck is used to assure that the components created do not have
// any pure virtual functions.
//

static inline void ValidCtrCheck(TMsgComm *)
{
    new TMsgComm(NULL);
}
//---------------------------------------------------------------------------
__fastcall TMsgComm::TMsgComm(TComponent* Owner)
    : TComponent(Owner)
{
    FHandle = 0;
    FOnConnect = 0;
    FOnDisconnect = 0;
    FOnMessage = 0;
    FPort = 5222;
}
//---------------------------------------------------------------------------
namespace Msgcomm
{
    void __fastcall PACKAGE Register()
    {
         TComponentClass classes[1] = {__classid(TMsgComm)};
         RegisterComponents("SimpComm", classes, 0);
    }
}
//---------------------------------------------------------------------------


void __fastcall TMsgComm::SetUser(AnsiString value)
{
    if(FUser != value) {
        FUser = value;
    }
}
AnsiString __fastcall TMsgComm::GetUser()
{
    return FUser;
}

void __fastcall TMsgComm::SetHost(AnsiString value)
{
    if(FHost != value) {
        FHost = value;
    }
}

AnsiString __fastcall TMsgComm::GetHost()
{
    return FHost;
}

void __fastcall TMsgComm::SetPassword(AnsiString value)
{
    if(FPassword != value) {
        FPassword = value;
    }
}

AnsiString __fastcall TMsgComm::GetPassword()
{
    return FPassword;
}

void __fastcall TMsgComm::SetServer(AnsiString value)
{
    if(FServer != value) {
        FServer = value;
    }
}

AnsiString __fastcall TMsgComm::GetServer()
{
    return FServer;
}

void __fastcall TMsgComm::SetPort(int value)
{
    if( FPort != value ) {
        FPort = value;
    } 
}

int __fastcall TMsgComm::GetPort()
{
    return FPort;
}

void __fastcall TMsgComm::SetOnConnect(TNotifyEvent value)
{
    if(FOnConnect != value) {
        FOnConnect = value;
    }
}

TNotifyEvent __fastcall TMsgComm::GetOnConnect()
{
    return FOnConnect;
}

void __fastcall TMsgComm::SetOnDisconnect(TNotifyEvent value)
{
    if(FOnDisconnect != value) {
        FOnDisconnect = value;
    }
}

TNotifyEvent __fastcall TMsgComm::GetOnDisconnect()
{
    return FOnDisconnect;
}

void __fastcall TMsgComm::SetOnMessage(TMsgCommEvent value)
{
    if(FOnMessage != value) {
        FOnMessage = value;
    }
}

TMsgCommEvent __fastcall TMsgComm::GetOnMessage()
{
    return FOnMessage;
}

//------------------------------------------------------------------------------
void TMsgComm::Debug( int mode, const char* param )
{
    if( !InitFunctions() )
        return;
    xmdebug( mode, param );
}

bool TMsgComm::Open()
{
    if( !InitFunctions() )
        return false;
    if( FHandle )
        return true;
    if( FHandle == 0 )
    {
        FHandle = xmopen( FUser.c_str(), FPassword.c_str(), FHost.c_str(), FServer.c_str(), FPort );
        if( FHandle )
        {
            xmcallbackfunc func[XM_FUNC_NUM] = { MsgCallback::OnConnect,
                                                MsgCallback::OnLogin,
                                                MsgCallback::OnMessage,
                                                MsgCallback::OnDisconnect };

            xmreg( FHandle, func, this);
            xmstart( FHandle, 0, 0 );
        }
    }
    return true;
}

bool TMsgComm::Close()
{
    if( !InitFunctions() )
        return false;
    if( FHandle )
    {
        void* h = FHandle;
        FHandle = 0;
        xmclose( h );
    }
    return true;
}

bool TMsgComm::Send(AnsiString to, AnsiString subject, AnsiString msg)
{
    if( !InitFunctions() )
        return false;
    if( FHandle )
    {
        xmsend( FHandle, to.c_str(), subject.c_str(), msg.c_str(), -1 );
    }
    return true;
}


