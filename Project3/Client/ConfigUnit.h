//---------------------------------------------------------------------------

#ifndef ConfigUnitH
#define ConfigUnitH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Buttons.hpp>
//---------------------------------------------------------------------------
class TForm2 : public TForm
{
__published:	// IDE-managed Components
    TBitBtn *BitBtn1;
    TBitBtn *BitBtn2;
    TLabel *Label1;
    TLabel *Label2;
    TComboBox *ComboBox;
    TEdit *Edit;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall BitBtn1Click(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm2(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm2 *Form2;
//---------------------------------------------------------------------------
#endif
