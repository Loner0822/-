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
#include "DModUnit.h"
#include <Grids.hpp>
#include <Menus.hpp>
#include <ADODB.hpp>
#include <DB.hpp>
#include <cstdio>
//---------------------------------------------------------------------------
class TForm2 : public TForm
{
__published:	// IDE-managed Components
    TGroupBox *GroupBox1;
    TcsDataTypeDef_ocx *csDataTypeDef_ocx1;
    TAdvStringGrid *AdvStringGrid1;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall AdvStringGrid1CellValidate(TObject *Sender, int ACol,
          int ARow, AnsiString &Value, bool &Valid);
    void __fastcall AdvStringGrid1CanEditCell(TObject *Sender, int ARow,
          int ACol, bool &CanEdit);
    void __fastcall AdvStringGrid1RowMoved(TObject *Sender, int FromIndex,
          int ToIndex);
    void __fastcall AdvStringGrid1RowMove(TObject *Sender, int ARow,
          bool &Allow);
    void __fastcall AdvStringGrid1RowMoving(TObject *Sender, int ARow,
          bool &Allow);
    void __fastcall AdvStringGrid1SelectCell(TObject *Sender, int ACol,
          int ARow, bool &CanSelect);
private:	// User declarations
public:		// User declarations
    String PGUID;
    __fastcall TForm2(TComponent* Owner);

};
//---------------------------------------------------------------------------
extern PACKAGE TForm2 *Form2;
//---------------------------------------------------------------------------
#endif
