//---------------------------------------------------------------------------

#ifndef SystemUnitH
#define SystemUnitH
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
#include <ExtCtrls.hpp>
#include <cstdio>
#include <map>

using namespace std;
//---------------------------------------------------------------------------
class TForm2 : public TForm
{
__published:	// IDE-managed Components
    TGroupBox *GroupBox1;
    TcsDataTypeDef_ocx *csDataTypeDef_ocx1;
    TAdvStringGrid *AdvStringGrid1;
    TPopupMenu *PopupMenu1;
    TMenuItem *A1;
    TMenuItem *U1;
    TMenuItem *D1;
    TSplitter *Splitter1;
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
    void __fastcall AdvStringGrid1DblClickCell(TObject *Sender, int ARow,
          int ACol);
    void __fastcall A1Click(TObject *Sender);
    void __fastcall AdvStringGrid1GetAlignment(TObject *Sender, int ARow,
          int ACol, TAlignment &HAlign, TVAlignment &VAlign);
    void __fastcall AdvStringGrid1Resize(TObject *Sender);
    void __fastcall U1Click(TObject *Sender);
    void __fastcall D1Click(TObject *Sender);
private:	// User declarations
public:		// User declarations
    String PGUID;
    String sql_Dep, Department;
    map<int, String> Department_Name;
    __fastcall TForm2(TComponent* Owner);

    void Refresh();

    void InsertParm(String u_name);
    void UpdateParm(String u_name);
    void DeleteParm();

};
//---------------------------------------------------------------------------
extern PACKAGE TForm2 *Form2;
//---------------------------------------------------------------------------
#endif
