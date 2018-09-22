//---------------------------------------------------------------------------

#ifndef SystemUnitH
#define SystemUnitH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "csDataTypeDef_ocxProj1_OCX.h"
#include <OleCtrls.hpp>
#include "AdvGrid.hpp"
#include "BaseGrid.hpp"
#include <Grids.hpp>
//---------------------------------------------------------------------------
class TForm2 : public TForm
{
__published:	// IDE-managed Components
    TcsDataTypeDef_ocx *csDataTypeDef_ocx1;
    TGroupBox *GroupBox1;
    TAdvStringGrid *AdvStringGrid1;
    TGroupBox *GroupBox2;
    TAdvStringGrid *AdvStringGrid2;
private:	// User declarations
public:		// User declarations
    __fastcall TForm2(TComponent* Owner);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm2 *Form2;
//---------------------------------------------------------------------------
#endif
