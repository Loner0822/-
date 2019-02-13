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
#ifndef   __FlashVideoObjects_OCX_h__
#define   __FlashVideoObjects_OCX_h__

#pragma option push -b -w-inl

#include <utilcls.h>
#if !defined(__UTILCLS_H_VERSION) || (__UTILCLS_H_VERSION < 0x0500)
//
// The code generated by the TLIBIMP utility or the Import|TypeLibrary 
// and Import|ActiveX feature of C++Builder rely on specific versions of
// the header file UTILCLS.H found in the INCLUDE\VCL directory. If an 
// older version of the file is detected, you probably need an update/patch.
//
#error "This file requires a newer version of the header UTILCLS.H" \
       "You need to apply an update/patch to your copy of C++Builder"
#endif
#include <olectl.h>
#include <ocidl.h>
#if !defined(_NO_VCL)
#include <stdvcl.hpp>
#endif  //   _NO_VCL
#include <ocxproxy.h>

#include "FlashVideoObjects_TLB.h"
namespace Flashvideoobjects_tlb
{

// *********************************************************************//
// HelpString: Shockwave Flash
// Version:    1.0
// *********************************************************************//


// *********************************************************************//
// COM Component Proxy Class Declaration
// Component Name   : TFlashVideo
// Help String      : Shockwave Flash
// Default Interface: IFlashVideo
// Def. Intf. Object: TCOMIFlashVideo
// Def. Intf. DISP? : No
// Event   Interface: _IFlashVideoEvents
// TypeFlags        : (2) CanCreate
// *********************************************************************//

// *********************************************************************//
// Definition of closures to allow VCL handlers to catch OCX events.      
// *********************************************************************//
typedef void __fastcall (__closure * TFlashVideoOnReadyStateChange)(System::TObject * Sender, 
                                                                        long newState);
typedef void __fastcall (__closure * TFlashVideoOnProgress)(System::TObject * Sender, 
                                                                long percentDone);
typedef void __fastcall (__closure * TFlashVideoFSCommand)(System::TObject * Sender,
                                                               BSTR command/*[in]*/,
                                                               BSTR args/*[in]*/);
typedef void __fastcall (__closure * TFlashVideoFlashCall)(System::TObject * Sender,
                                                               BSTR request/*[in]*/);
//+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-
// Proxy class to host Shockwave Flash in CBuilder IDE/Applications.
//-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
class PACKAGE TFlashVideo : public TOleControl
{
  OVERLOADED_PROP_METHODS;
  static TNoParam  OptParam;
  static GUID      DEF_CTL_INTF;

  // Instance of Closures to expose OCX Events as VCL ones
  //
  TFlashVideoOnReadyStateChange FOnReadyStateChange;
  TFlashVideoOnProgress      FOnProgress;
  TFlashVideoFSCommand       FOnFSCommand;
  TFlashVideoFlashCall       FOnFlashCall;

  // Default Interace of OCX
  //
  TCOMIFlashVideo m_OCXIntf;

  // VCL Property Getters/Setters which delegate to OCX
  //
  LPUNKNOWN       __fastcall Get_InlineData(void);
  void            __fastcall Set_InlineData(LPUNKNOWN ppIUnknown/*[in]*/);

  // Static variables used by all instances of OCX proxy
  //
  static int          EventDispIDs[4];
  static TControlData CControlData;
  static GUID         CTL_DEF_INTF;

  // Method providing access to interface as __property 
  //
  TCOMIFlashVideo __fastcall GetDefaultInterface();
  TCOMIFlashVideo __fastcall GetControlInterface()
  { return GetDefaultInterface(); }

protected:
  void     __fastcall CreateControl  ();
  void     __fastcall InitControlData();
    BEGIN_MESSAGE_MAP
        VCL_MESSAGE_HANDLER(WM_SIZE, TWMSize, OnSize)
    END_MESSAGE_MAP(TOleControl)

public:
  virtual  __fastcall TFlashVideo(TComponent* AOwner) : TOleControl(AOwner)
  {InitParams();};
  virtual  __fastcall TFlashVideo(HWND        Parent) : TOleControl(Parent)
  {InitParams();};

  // OCX methods
  //
  void            __fastcall SetZoomRect(long left/*[in]*/, long top/*[in]*/, long right/*[in]*/, 
                                         long bottom/*[in]*/);
  void            __fastcall Zoom(int factor/*[in]*/);
  void            __fastcall Pan(long x/*[in]*/, long y/*[in]*/, int mode/*[in]*/);
  void            __fastcall Play(void);
  void            __fastcall Stop(void);
  void            __fastcall Back(void);
  void            __fastcall Forward(void);
  void            __fastcall Rewind(void);
  void            __fastcall StopPlay(void);
  void            __fastcall GotoFrame(long FrameNum/*[in]*/);
  long            __fastcall CurrentFrame(void);
  TOLEBOOL        __fastcall IsPlaying(void);
  long            __fastcall PercentLoaded(void);
  TOLEBOOL        __fastcall FrameLoaded(long FrameNum/*[in]*/);
  long            __fastcall FlashVersion(void);
  void            __fastcall LoadMovie(int layer/*[in]*/, BSTR url/*[in]*/);
  void            __fastcall TGotoFrame(BSTR target/*[in]*/, long FrameNum/*[in]*/);
  void            __fastcall TGotoLabel(BSTR target/*[in]*/, BSTR label/*[in]*/);
  long            __fastcall TCurrentFrame(BSTR target/*[in]*/);
  BSTR            __fastcall TCurrentLabel(BSTR target/*[in]*/);
  void            __fastcall TPlay(BSTR target/*[in]*/);
  void            __fastcall TStopPlay(BSTR target/*[in]*/);
  void            __fastcall SetVariable(BSTR name/*[in]*/, BSTR value/*[in]*/);
  BSTR            __fastcall GetVariable(BSTR name/*[in]*/);
  void            __fastcall TSetProperty(BSTR target/*[in]*/, int property/*[in]*/, 
                                          BSTR value/*[in]*/);
  BSTR            __fastcall TGetProperty(BSTR target/*[in]*/, int property/*[in]*/);
  void            __fastcall TCallFrame(BSTR target/*[in]*/, int FrameNum/*[in]*/);
  void            __fastcall TCallLabel(BSTR target/*[in]*/, BSTR label/*[in]*/);
  void            __fastcall TSetPropertyNum(BSTR target/*[in]*/, int property/*[in]*/, 
                                             double value/*[in]*/);
  double          __fastcall TGetPropertyNum(BSTR target/*[in]*/, int property/*[in]*/);
  double          __fastcall TGetPropertyAsNumber(BSTR target/*[in]*/, int property/*[in]*/);
  void            __fastcall EnforceLocalSecurity(void);
  BSTR            __fastcall CallFunction(BSTR request/*[in]*/);
  void            __fastcall SetReturnValue(BSTR returnValue/*[in]*/);
  void            __fastcall DisableLocalSecurity(void);

  // OCX properties
  //
  __property int ReadyState={ read=GetIntegerProp, index=-525 };
  __property int TotalFrames={ read=GetIntegerProp, index=124 };
#if __BORLANDC__ >= 0x0560
  __property System::_di_IInterface InlineData={ read=GetIUnknownProp, write=SetIUnknownProp, index=191 };
#else
  __property _di_IUnknown InlineData={ read=GetIUnknownProp, write=SetIUnknownProp, index=191 };
#endif
  __property TCOMIFlashVideo ControlInterface={ read=GetDefaultInterface };

  // Published properties
  //
__published:

  // Standard/Extended properties
  //
  __property TabStop;
  __property Align;
  __property DragCursor;
  __property DragMode;
  __property ParentShowHint;
  __property PopupMenu;
  __property ShowHint;
  __property TabOrder;
  __property Visible;
  __property OnDragDrop;
  __property OnDragOver;
  __property OnEndDrag;
  __property OnEnter;
  __property OnExit;
  __property OnStartDrag;
  __property OnDblClick;
  // OCX properties
  //
  __property bool Playing={ read=GetWordBoolProp, write=SetWordBoolProp, stored=false, index=125 };
  __property int Quality={ read=GetIntegerProp, write=SetIntegerProp, stored=false, index=105 };
  __property int ScaleMode={ read=GetIntegerProp, write=SetIntegerProp, stored=false, index=120 };
  __property int AlignMode={ read=GetIntegerProp, write=SetIntegerProp, stored=false, index=121 };
  __property int BackgroundColor={ read=GetIntegerProp, write=SetIntegerProp, stored=false, index=123 };
  __property bool Loop={ read=GetWordBoolProp, write=SetWordBoolProp, stored=false, index=106 };
  __property System::WideString Movie={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=102 };
  __property int FrameNum={ read=GetIntegerProp, write=SetIntegerProp, stored=false, index=107 };
  __property System::WideString WMode={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=133 };
  __property System::WideString SAlign={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=134 };
  __property bool Menu={ read=GetWordBoolProp, write=SetWordBoolProp, stored=false, index=135 };
  __property System::WideString Base={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=136 };
  __property System::WideString Scale={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=137 };
  __property bool DeviceFont={ read=GetWordBoolProp, write=SetWordBoolProp, stored=false, index=138 };
  __property bool EmbedMovie={ read=GetWordBoolProp, write=SetWordBoolProp, stored=false, index=139 };
  __property System::WideString BGColor={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=140 };
  __property System::WideString Quality2={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=141 };
  __property System::WideString SWRemote={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=159 };
  __property System::WideString FlashVars={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=170 };
  __property System::WideString AllowScriptAccess={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=171 };
  __property System::WideString MovieData={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=190 };
  __property bool SeamlessTabbing={ read=GetWordBoolProp, write=SetWordBoolProp, stored=false, index=192 };
  __property bool Profile={ read=GetWordBoolProp, write=SetWordBoolProp, stored=false, index=194 };
  __property System::WideString ProfileAddress={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=195 };
  __property int ProfilePort={ read=GetIntegerProp, write=SetIntegerProp, stored=false, index=196 };
  __property System::WideString AllowNetworking={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=201 };
  __property System::WideString AllowFullScreen={ read=GetWideStringProp, write=SetWideStringProp, stored=false, index=202 };

  // OCX Events
  //
  __property TFlashVideoOnReadyStateChange OnReadyStateChange={ read=FOnReadyStateChange, write=FOnReadyStateChange };
  __property TFlashVideoOnProgress OnProgress={ read=FOnProgress, write=FOnProgress };
//  __property TFlashVideoFSCommand OnFSCommand={ read=FOnFSCommand, write=FOnFSCommand };
//  __property TFlashVideoFlashCall OnFlashCall={ read=FOnFlashCall, write=FOnFlashCall };
__published:
  __property TNotifyEvent OnConnect     = { read=FOnConnect, write=FOnConnect };
  __property TNotifyEvent OnDisconnect  = { read=FOnDisconnect, write=FOnDisconnect };
  __property TNotifyEvent OnSendStart   = { read=FOnSendStart, write=FOnSendStart };
  __property TNotifyEvent OnSendStop    = { read=FOnSendStop, write=FOnSendStop };
  __property TNotifyEvent OnPlayRemoteStart   = { read=FOnPlayRemoteStart, write=FOnPlayRemoteStart };
  __property TNotifyEvent OnPlayRemoteStop    = { read=FOnPlayRemoteStop, write=FOnPlayRemoteStop };
  __property TNotifyEvent OnVideoListChange   = { read=FOnVideoListChange, write=FOnVideoListChange };
  __property int VideoBandwidth = { read=FBandwidth, write=FBandwidth };
  __property int VideoQuality   = { read=FQuality, write=FQuality };
  // ��Ƶ������ Ŀǰ�ǲ���Ƶ�ʣ�8, 11, 22
  __property int AudioQuality   = { read=FAudioQuality, write=FAudioQuality };
  // ģʽ: 0 - ��Ƶ/��Ƶ, 1 - ��Ƶ
  __property int AudioOnly   = { read=FAudioOnly, write=FAudioOnly };
public:
  __property AnsiString Server    = { read=FServer, write=SetServer };
  __property AnsiString VideoName = { read=FVideoName, write=SetVideoName };
  __property AnsiString VideoList = { read=FVideoList };
  __property bool Connected = { read=FConnected };
  __property bool Sending = { read=FSending };
  __property bool PlayingRemote   = { read=FPlayingRemote };
protected:
    virtual void __fastcall WndProc(Messages::TMessage &Message){
        if( Message.Msg == WM_LBUTTONDBLCLK ){
            if( OnDblClick )
                OnDblClick( this );
        }
        TOleControl::WndProc( Message );
    }
public:
  bool Init( AnsiString param );
  bool SetRotation( int angle );
  bool Connect();
  bool Disconnect();
  bool Preview();
  bool StopPreview();
  bool Send();
  bool StopSend();
  bool PlayRemote( AnsiString videoName );
  bool StopPlayRemote();
  void SetStretch( bool val );
  void SetDebug( bool val );  
private:
  TNotifyEvent FOnConnect;
  TNotifyEvent FOnDisconnect;
  TNotifyEvent FOnSendStart;
  TNotifyEvent FOnSendStop;
  TNotifyEvent FOnPlayRemoteStart;
  TNotifyEvent FOnPlayRemoteStop;
  TNotifyEvent FOnVideoListChange;

  AnsiString FServer;
  AnsiString FVideoName;
  AnsiString FVideoList;
  bool FConnected;
  bool FSending;
  bool FPlayingRemote;

  int FBandwidth;
  int FQuality;

  int FAudioQuality;
  int FAudioOnly;

  void __fastcall DoFSCommand(System::TObject * Sender, BSTR command, BSTR args);

  AnsiString CallFlash(AnsiString func);
  AnsiString CallFlash(AnsiString func, AnsiString arg);
  AnsiString CallFlash(AnsiString func, AnsiString arg1, AnsiString arg2);

  void InitParams();
  void SetServer( AnsiString server );
  void SetVideoName( AnsiString videoName );
    void __fastcall OnSize(TWMSize& Message);
};
typedef TFlashVideo  TFlashVideoProxy;

};     // namespace Flashvideoobjects_tlb

#if !defined(NO_IMPLICIT_NAMESPACE_USE)
using  namespace Flashvideoobjects_tlb;
#endif

#pragma option pop

#endif // __FlashVideoObjects_OCX_h__

