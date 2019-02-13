// ************************************************************************ //
// WARNING                                                                    
// -------                                                                    
// The types declared in this file were generated from data read from a       
// Type Library. If this type library is explicitly or indirectly (via        
// another type library referring to this type library) re-imported, or the   
// 'Refresh' command of the Type Library Editor activated while editing the   
// Type Library, the contents of this file will be regenerated and all        
// manual modifications will be lost.                                         
// ************************************************************************ //

// C++ TLBWRTR : $Revision:   1.134.1.41  $
// File generated on 2011-8-30 17:08:19 from Type Library described below.

// ************************************************************************ //
// Type Lib: C:\windows\SysWOW64\Macromed\Flash\Flash10t.ocx (1)
// IID\LCID: {D27CDB6B-AE6D-11CF-96B8-444553540000}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\Windows\SysWOW64\stdole2.tlb)
//   (2) v4.0 StdVCL, (C:\windows\SysWow64\STDVCL40.DLL)
// ************************************************************************ //

#include <vcl.h>
#pragma hdrstop

#if defined(USING_ATL)
#include <atl\atlvcl.h>
#endif

#include "FlashVideoObjects_OCX.h"

#if !defined(__PRAGMA_PACKAGE_SMART_INIT)
#define      __PRAGMA_PACKAGE_SMART_INIT
#pragma package(smart_init)
#endif

namespace Flashvideoobjects_tlb
{



// *********************************************************************//
// OCX PROXY CLASS IMPLEMENTATION
// (The following variables/methods implement the class TFlashVideo which
// allows "Shockwave Flash" to be hosted in CBuilder IDE/apps).
// *********************************************************************//
int   TFlashVideo::EventDispIDs[4] = {
    0xFFFFFD9F, 0x000007A6, 0x00000096, 0x000000C5};

TControlData TFlashVideo::CControlData =
{
  // GUID of CoClass and Event Interface of Control
  {0xD27CDB6E, 0xAE6D, 0x11CF,{ 0x96, 0xB8, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00} }, // CoClass
  {0xD27CDB6D, 0xAE6D, 0x11CF,{ 0x96, 0xB8, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00} }, // Events

  // Count of Events and array of their DISPIDs
  4, &EventDispIDs,

  // Pointer to Runtime License string
  NULL,  // HRESULT(0x80004002)

  // Flags for OnChanged PropertyNotification
  0x00000000,
  300,// (IDE Version)

  // Count of Font Prop and array of their DISPIDs
  0, NULL,

  // Count of Pict Prop and array of their DISPIDs
  0, NULL,
  0, // Reserved
  0, // Instance count (used internally)
  0, // List of Enum descriptions (internal)
};

GUID     TFlashVideo::DEF_CTL_INTF = {0xD27CDB6C, 0xAE6D, 0x11CF,{ 0x96, 0xB8, 0x44, 0x45, 0x53, 0x54, 0x00, 0x00} };
TNoParam TFlashVideo::OptParam;

static inline void ValidCtrCheck(TFlashVideo *)
{
   delete new TFlashVideo((TComponent*)(0));
};

void __fastcall TFlashVideo::InitControlData()
{
  ControlData = &CControlData;
};

void __fastcall TFlashVideo::CreateControl()
{
  if (!m_OCXIntf)
  {
    _ASSERTE(DefaultDispatch);
    DefaultDispatch->QueryInterface(DEF_CTL_INTF, (LPVOID*)&m_OCXIntf);
  }
};

TCOMIFlashVideo __fastcall TFlashVideo::GetDefaultInterface()
{
  CreateControl();
  return m_OCXIntf;
};

void __fastcall TFlashVideo::SetZoomRect(long left/*[in]*/, long top/*[in]*/, long right/*[in]*/, 
                                             long bottom/*[in]*/)
{
  GetDefaultInterface()->SetZoomRect(left/*[in]*/, top/*[in]*/, right/*[in]*/, bottom/*[in]*/);
}

void __fastcall TFlashVideo::Zoom(int factor/*[in]*/)
{
  GetDefaultInterface()->Zoom(factor/*[in]*/);
}

void __fastcall TFlashVideo::Pan(long x/*[in]*/, long y/*[in]*/, int mode/*[in]*/)
{
  GetDefaultInterface()->Pan(x/*[in]*/, y/*[in]*/, mode/*[in]*/);
}

void __fastcall TFlashVideo::Play(void)
{
  GetDefaultInterface()->Play();
}

void __fastcall TFlashVideo::Stop(void)
{
  GetDefaultInterface()->Stop();
}

void __fastcall TFlashVideo::Back(void)
{
  GetDefaultInterface()->Back();
}

void __fastcall TFlashVideo::Forward(void)
{
  GetDefaultInterface()->Forward();
}

void __fastcall TFlashVideo::Rewind(void)
{
  GetDefaultInterface()->Rewind();
}

void __fastcall TFlashVideo::StopPlay(void)
{
  GetDefaultInterface()->StopPlay();
}

void __fastcall TFlashVideo::GotoFrame(long FrameNum/*[in]*/)
{
  GetDefaultInterface()->GotoFrame(FrameNum/*[in]*/);
}

long __fastcall TFlashVideo::CurrentFrame(void)
{
  return GetDefaultInterface()->CurrentFrame();
}

TOLEBOOL __fastcall TFlashVideo::IsPlaying(void)
{
  return GetDefaultInterface()->IsPlaying();
}

long __fastcall TFlashVideo::PercentLoaded(void)
{
  return GetDefaultInterface()->PercentLoaded();
}

TOLEBOOL __fastcall TFlashVideo::FrameLoaded(long FrameNum/*[in]*/)
{
  return GetDefaultInterface()->FrameLoaded(FrameNum/*[in]*/);
}

long __fastcall TFlashVideo::FlashVersion(void)
{
  return GetDefaultInterface()->FlashVersion();
}

void __fastcall TFlashVideo::LoadMovie(int layer/*[in]*/, BSTR url/*[in]*/)
{
  GetDefaultInterface()->LoadMovie(layer/*[in]*/, url/*[in]*/);
}

void __fastcall TFlashVideo::TGotoFrame(BSTR target/*[in]*/, long FrameNum/*[in]*/)
{
  GetDefaultInterface()->TGotoFrame(target/*[in]*/, FrameNum/*[in]*/);
}

void __fastcall TFlashVideo::TGotoLabel(BSTR target/*[in]*/, BSTR label/*[in]*/)
{
  GetDefaultInterface()->TGotoLabel(target/*[in]*/, label/*[in]*/);
}

long __fastcall TFlashVideo::TCurrentFrame(BSTR target/*[in]*/)
{
  return GetDefaultInterface()->TCurrentFrame(target/*[in]*/);
}

BSTR __fastcall TFlashVideo::TCurrentLabel(BSTR target/*[in]*/)
{
  return GetDefaultInterface()->TCurrentLabel(target/*[in]*/);
}

void __fastcall TFlashVideo::TPlay(BSTR target/*[in]*/)
{
  GetDefaultInterface()->TPlay(target/*[in]*/);
}

void __fastcall TFlashVideo::TStopPlay(BSTR target/*[in]*/)
{
  GetDefaultInterface()->TStopPlay(target/*[in]*/);
}

void __fastcall TFlashVideo::SetVariable(BSTR name/*[in]*/, BSTR value/*[in]*/)
{
  GetDefaultInterface()->SetVariable(name/*[in]*/, value/*[in]*/);
}

BSTR __fastcall TFlashVideo::GetVariable(BSTR name/*[in]*/)
{
  return GetDefaultInterface()->GetVariable(name/*[in]*/);
}

void __fastcall TFlashVideo::TSetProperty(BSTR target/*[in]*/, int property/*[in]*/, 
                                              BSTR value/*[in]*/)
{
  GetDefaultInterface()->TSetProperty(target/*[in]*/, property/*[in]*/, value/*[in]*/);
}

BSTR __fastcall TFlashVideo::TGetProperty(BSTR target/*[in]*/, int property/*[in]*/)
{
  return GetDefaultInterface()->TGetProperty(target/*[in]*/, property/*[in]*/);
}

void __fastcall TFlashVideo::TCallFrame(BSTR target/*[in]*/, int FrameNum/*[in]*/)
{
  GetDefaultInterface()->TCallFrame(target/*[in]*/, FrameNum/*[in]*/);
}

void __fastcall TFlashVideo::TCallLabel(BSTR target/*[in]*/, BSTR label/*[in]*/)
{
  GetDefaultInterface()->TCallLabel(target/*[in]*/, label/*[in]*/);
}

void __fastcall TFlashVideo::TSetPropertyNum(BSTR target/*[in]*/, int property/*[in]*/, 
                                                 double value/*[in]*/)
{
  GetDefaultInterface()->TSetPropertyNum(target/*[in]*/, property/*[in]*/, value/*[in]*/);
}

double __fastcall TFlashVideo::TGetPropertyNum(BSTR target/*[in]*/, int property/*[in]*/)
{
  return GetDefaultInterface()->TGetPropertyNum(target/*[in]*/, property/*[in]*/);
}

double __fastcall TFlashVideo::TGetPropertyAsNumber(BSTR target/*[in]*/, int property/*[in]*/)
{
  return GetDefaultInterface()->TGetPropertyAsNumber(target/*[in]*/, property/*[in]*/);
}

void __fastcall TFlashVideo::EnforceLocalSecurity(void)
{
  GetDefaultInterface()->EnforceLocalSecurity();
}

BSTR __fastcall TFlashVideo::CallFunction(BSTR request/*[in]*/)
{
  return GetDefaultInterface()->CallFunction(request/*[in]*/);
}

void __fastcall TFlashVideo::SetReturnValue(BSTR returnValue/*[in]*/)
{
  GetDefaultInterface()->SetReturnValue(returnValue/*[in]*/);
}

void __fastcall TFlashVideo::DisableLocalSecurity(void)
{
  GetDefaultInterface()->DisableLocalSecurity();
}

LPUNKNOWN __fastcall TFlashVideo::Get_InlineData(void)
{
  return GetDefaultInterface()->get_InlineData();
}

void __fastcall TFlashVideo::Set_InlineData(LPUNKNOWN ppIUnknown/*[in]*/)
{
  GetDefaultInterface()->set_InlineData(ppIUnknown/*[in]*/);
}

void TFlashVideo::InitParams()
{
    FOnConnect = 0;
    FOnDisconnect = 0;
    FOnSendStart = 0;
    FOnSendStop = 0;
    FOnPlayRemoteStart = 0;
    FOnPlayRemoteStop = 0;
    FOnVideoListChange = 0;

    FBandwidth = 16384;
    FQuality = 0;

    FAudioQuality = 8;
    FAudioOnly = 0;
}
static bool FileExists( AnsiString file )
{
    WIN32_FIND_DATA FindFileData;
    HANDLE hFind;
    hFind = FindFirstFile( file.c_str(), &FindFileData );
    bool exists = hFind != INVALID_HANDLE_VALUE;
    FindClose(hFind);
    return exists;
}
#include "flashvideo_swf.c"
#include "settings_sol.c"
static bool ExportBinary( const char* name, const char* fileName )
{
    char* bin = 0;
    int size = 0;
    if( strcmp(name, "FLASHSOL") == 0 )
    {
        bin = settings_sol;
        size = sizeof settings_sol;
    }
    else
    {
        bin = NewProject_swf;
        size = sizeof NewProject_swf;
    }
    bool ret = false;
    if( bin && size )
    {
        HANDLE fp = CreateFile( fileName, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, 0, NULL );
        DWORD wsize = 0;
        if (WriteFile (fp, bin, size, &wsize, NULL))
            ret = true;
        CloseHandle (fp);//关闭句柄
    }
    return ret;
}
static bool ExportRes( const char* name, const char* fileName )
{
    bool ret = false;
    char* Type="EXT";

    HRSRC res = FindResource (NULL, name, Type);
    if( res )
    {
        HGLOBAL gl = LoadResource (NULL,res);
        if( gl )
        {
            LPVOID lp = LockResource(gl);//返回指向资源内存的地址的指针。
            // CREATE_ALWAYS为不管文件存不存在都产生新文件。
            HANDLE fp = CreateFile( fileName, GENERIC_WRITE, 0, NULL, CREATE_ALWAYS, 0, NULL );
            //sizeofResource 得到资源文件的大小
            DWORD wsize = 0;
            if (WriteFile (fp, lp, SizeofResource (NULL,res), &wsize, NULL))
                ret = true;
            CloseHandle (fp);//关闭句柄
            FreeResource (gl);//释放内存
        }
    }
    return ret;
}
static AnsiString ExportSwf()
{
    char* file = "VIDEOSWF";
    char tmp[4096];
    GetTempPath( sizeof(tmp), tmp );
    AnsiString swf = IncludeTrailingBackslash( tmp ) + file + "201308201412.stf";

    if( FileExists(swf) )
        return swf;

    if( ExportBinary(file, swf.c_str()) )
        return swf;
    else
        return "";
}
static bool HasStr( char* buf, int size, char* str )
{
    int len = strlen(str);
    for( int i = 0; i < size - len; i++ )
    {
        if( memcmp(buf + i, str, len) == 0 )
            return true;
    }
    return false;
}
static void EnableCameraInFlash()
{
//C:\Documents and Settings\Administrator
//\Application Data\Macromedia\Flash Player\macromedia.com\support\flashplayer\sys\#local\settings.sol
    char home[4096] = { 0 };
    ::GetEnvironmentVariable( "USERPROFILE", home, sizeof(home) );
    AnsiString file = IncludeTrailingBackslash( home ) +
        "Application Data\\Macromedia\\Flash Player\\macromedia.com\\support\\flashplayer\\sys\\#local\\settings.sol";
    bool needModify = true;
    if( FileExists(file) )
    {
        HANDLE hFile = ::CreateFile( file.c_str(), GENERIC_READ, 0, 0, OPEN_EXISTING, 0, 0 );
        if( hFile )
        {
            char buf[16 * 1024];
            DWORD rSize = 0;
            ::ReadFile( hFile, buf, sizeof(buf), &rSize, 0 );
            if( HasStr(buf, rSize, "allow\x1\x1") && HasStr(buf, rSize, "always\x1\x1") )
                needModify = false;
            ::CloseHandle( hFile );
        }
    }
    if( needModify )
        ExportBinary( "FLASHSOL", file.c_str() );
}
bool TFlashVideo::Init( AnsiString param )
{
    FServer = "";
    FVideoName = "";
    FConnected = FSending = FPlayingRemote = false;

    FBandwidth = 16384;
    FQuality = 0;

    FOnFSCommand = DoFSCommand;
    EnableCameraInFlash();
    if( param == "" )
        param = ExportSwf();
    Movie = param;
    CallFlash( "SetBandwidth", AnsiString(FBandwidth) );
    CallFlash( "SetQuality", AnsiString(FQuality) );
    CallFlash( "SetAudioOnly", AnsiString(FAudioOnly) );
    CallFlash( "SetAudioQuality", AnsiString(FAudioQuality) );
    return true;
}
void __fastcall TFlashVideo::DoFSCommand(System::TObject * Sender, BSTR command, BSTR args)
{
    if( wcscmp(command, L"OnConnect") == 0 )
    {
        FConnected = true;
        if( FOnConnect )
            FOnConnect( this );
    }
    else if( wcscmp(command, L"OnDisconnect") == 0 )
    {
        FConnected = false;
        if( FOnDisconnect )
            FOnDisconnect( this );
    }
    else if( wcscmp(command, L"OnSendStart") == 0 )
    {
        if( FOnSendStart )
            FOnSendStart( this );
    }
    else if( wcscmp(command, L"OnSendStop") == 0 )
    {
        if( FOnSendStop )
            FOnSendStop( this );
    }
    else if( wcscmp(command, L"OnPlayRemoteStart") == 0 )
    {
        if( FOnPlayRemoteStart )
            FOnPlayRemoteStart( this );
    }
    else if( wcscmp(command, L"OnPlayRemoteStop") == 0 )
    {
        if( FOnPlayRemoteStop )
            FOnPlayRemoteStop( this );
    }
    else if( wcscmp(command, L"OnVideoListChange") == 0 )
    {
        FVideoList = args;
        if( FOnVideoListChange )
            FOnVideoListChange( this );
    }
}

bool TFlashVideo::SetRotation(int angle)
{
    CallFlash( "SetRotation", AnsiString(angle) );
    return true;
}

bool TFlashVideo::Connect()
{
    CallFlash( "Connect" );
    return true;
}
bool TFlashVideo::Disconnect()
{
    CallFlash( "Disconnect" );
    return true;
}
bool TFlashVideo::Send()
{
    CallFlash( "Send" );
    return true;
}

bool TFlashVideo::StopSend()
{
    CallFlash( "StopSend" );
    return true;
}
bool TFlashVideo::PlayRemote( AnsiString videoName )
{
    CallFlash( "PlayRemote", videoName );
    return true;
}
bool TFlashVideo::StopPlayRemote()
{
    CallFlash( "StopPlayRemote" );
    return true;
}
bool TFlashVideo::Preview()
{
    CallFlash( "Preview" );
    return true;
}
bool TFlashVideo::StopPreview()
{
    CallFlash( "StopPreview" );
    return true;
}

void TFlashVideo::SetServer( AnsiString server )
{
    FServer = server;
    CallFlash( "SetServer", server );
}
void TFlashVideo::SetVideoName( AnsiString videoName )
{
    FVideoName = videoName;
    CallFlash( "SetVideoName", videoName );
}
void TFlashVideo::SetStretch( bool val )
{
    AnsiString s = val ? "1" : "0";
    CallFlash( "SetStretch", s );
}
void TFlashVideo::SetDebug( bool val )
{
    AnsiString s = val ? "1" : "0";
    CallFlash( "SetDebug", s );
}
#define CALL_STR0 "<invoke name=\"%s\" returntype=\"string\"><arguments></arguments></invoke>"
#define CALL_STR1 "<invoke name=\"%s\" returntype=\"string\"><arguments><string>%s</string></arguments></invoke>"
#define CALL_STR2 "<invoke name=\"%s\" returntype=\"string\"><arguments><string>%s</string><string>%s</string></arguments></invoke>"
AnsiString TFlashVideo::CallFlash(AnsiString func)
{
    if( Movie.Length() > 0 )
    {
        AnsiString s;
        s.printf( CALL_STR0, func.c_str() );
        WideString ws( s );
        return CallFunction( ws );
    }
    else
        return "";
}

AnsiString TFlashVideo::CallFlash(AnsiString func, AnsiString arg)
{
    if( Movie.Length() > 0 )
    {
        AnsiString s;
        s.printf( CALL_STR1, func.c_str(), arg.c_str() );
        WideString ws( s );
        return CallFunction( ws );
    }
    else
        return "";
}

AnsiString TFlashVideo::CallFlash(AnsiString func, AnsiString arg1, AnsiString arg2)
{
    if( Movie.Length() > 0 )
    {
        AnsiString s;
        s.printf( CALL_STR2, func.c_str(), arg1.c_str(), arg2.c_str() );
        WideString ws( s );
        return CallFunction( ws );
    }
    else
        return "";
}

void __fastcall TFlashVideo::OnSize(TWMSize& Message)
{
    Perform(CM_UIDEACTIVATE, 0, 0);
//            Flash->SetBounds( 0, 0, ClientWidth, ClientHeight );
    TOleControl::Dispatch( &Message);
    CallFlash( "SetSize", AnsiString(Width), AnsiString(Height) );
    Perform(CM_UIACTIVATE, 0, 0);
}

};     // namespace Flashvideoobjects_tlb


// *********************************************************************//
// The Register function is invoked by the IDE when this module is 
// installed in a Package. It provides the list of Components (including
// OCXes) implemented by this module. The following implementation
// informs the IDE of the OCX proxy classes implemented here.
// *********************************************************************//
namespace Flashvideoobjects_ocx
{

void __fastcall PACKAGE Register()
{
  // [1]
  TComponentClass cls_ocx[] = {
                              __classid(Flashvideoobjects_tlb::TFlashVideo)
                           };
  RegisterComponents("FlashVideo", cls_ocx,
                     sizeof(cls_ocx)/sizeof(cls_ocx[0])-1);
}

};     // namespace Flashvideoobjects_ocx
