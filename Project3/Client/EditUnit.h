//---------------------------------------------------------------------------

#ifndef EditUnitH
#define EditUnitH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Buttons.hpp>
//---------------------------------------------------------------------------
class TForm4 : public TForm
{
__published:	// IDE-managed Components
    TLabel *Label;
    TEdit *Edit;
    TBitBtn *BitBtn;
    TLabel *Label1;
private:	// User declarations
public:		// User declarations
    __fastcall TForm4(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm4 *Form4;
//---------------------------------------------------------------------------
#endif
