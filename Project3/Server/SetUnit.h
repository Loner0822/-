//---------------------------------------------------------------------------

#ifndef SetUnitH
#define SetUnitH
//---------------------------------------------------------------------------
#include "logrec.h"
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Buttons.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
//---------------------------------------------------------------------------
class TForm2 : public TForm
{
__published:	// IDE-managed Components
    TBitBtn *BitBtn;
    TLabel *Label1;
    TLabel *Label2;
    TLabel *Label3;
    TEdit *Edit;
    TComboBox *ComboBox;
    TDateTimePicker *DateTimePicker;
    TTimer *Timer;
    void __fastcall FormShow(TObject *Sender);
    void __fastcall BitBtnClick(TObject *Sender);
    void __fastcall TimerTimer(TObject *Sender);
    void __fastcall FormCreate(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm2(TComponent* Owner);
    int Times, Days, Files;
};
//---------------------------------------------------------------------------
extern PACKAGE TForm2 *Form2;
//---------------------------------------------------------------------------
#endif
