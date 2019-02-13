//---------------------------------------------------------------------------

#ifndef VideoFormH
#define VideoFormH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "FlashVideoObjects_OCX.h"
#include <OleCtrls.hpp>
#include <ExtCtrls.hpp>
#include <Menus.hpp>
//---------------------------------------------------------------------------
class TForm2 : public TForm
{
__published:	// IDE-managed Components
    TFlashVideo *FlashVideo1;
    TFlashVideo *FlashVideo2;
    TTimer *Timer1;
    TMainMenu *MainMenu1;
    TMenuItem *N1;
    TMenuItem *N2;
    void __fastcall FormShow(TObject *Sender);
    void __fastcall Timer1Timer(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall FormClose(TObject *Sender, TCloseAction &Action);
    void __fastcall FormResize(TObject *Sender);
    void __fastcall N1Click(TObject *Sender);
    void __fastcall N2Click(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm2(TComponent* Owner);
    TDateTime startTime;
    bool isShow;
};
//---------------------------------------------------------------------------
extern PACKAGE TForm2 *Form2;
//---------------------------------------------------------------------------
#endif
