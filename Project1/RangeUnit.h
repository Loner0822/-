//---------------------------------------------------------------------------

#ifndef RangeUnitH
#define RangeUnitH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "AdvGrid.hpp"
#include "BaseGrid.hpp"
#include <ComCtrls.hpp>
#include <Grids.hpp>
#include <ToolWin.hpp>
#include "MainUnit.h"
#include "SymbFrame.h"
#include "GUID.h"
#include <ExtCtrls.hpp>
#include <Menus.hpp>
#include "TaskDialog.hpp"
#include "TAdvStringGridCenter.h"
//---------------------------------------------------------------------------
class TForm5 : public TForm
{
__published:	// IDE-managed Components
    TAdvStringGrid *AdvStringGrid;
    TToolBar *ToolBar;
    TButton *Button;
    TPanel *Panel;
    TSymbolFrame *SymbolFrame;
    TPopupMenu *PopupMenu;
    TMenuItem *N1;
    TMenuItem *N2;
    TMenuItem *N3;
    TAdvInputTaskDialog *AdvInputTaskDialog;
    TMenuItem *N4;
    TMenuItem *N5;
    TMemo *Memo1;
    void __fastcall ButtonClick(TObject *Sender);
    void __fastcall AdvStringGridGetAlignment(TObject *Sender, int ARow,
          int ACol, TAlignment &HAlign, TVAlignment &VAlign);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall AdvStringGridClick(TObject *Sender);
    void __fastcall AdvStringGridClickCell(TObject *Sender, int ARow,
          int ACol);
    void __fastcall AdvStringGridDblClick(TObject *Sender);
    void __fastcall SymbolFrameButton1Click(TObject *Sender);
    void __fastcall SymbolFrameButton2Click(TObject *Sender);
    void __fastcall SymbolFrameButton3Click(TObject *Sender);
    void __fastcall SymbolFrameButton4Click(TObject *Sender);
    void __fastcall SymbolFrameButton7Click(TObject *Sender);
    void __fastcall SymbolFrameButton5Click(TObject *Sender);
    void __fastcall SymbolFrameButton6Click(TObject *Sender);
    void __fastcall SymbolFrameButton8Click(TObject *Sender);
    void __fastcall SymbolFrameButton9Click(TObject *Sender);
    void __fastcall SymbolFrameButton12Click(TObject *Sender);
    void __fastcall SymbolFrameButton13Click(TObject *Sender);
    void __fastcall SymbolFrameButton14Click(TObject *Sender);
    void __fastcall SymbolFrameButton10Click(TObject *Sender);
    void __fastcall SymbolFrameButton11Click(TObject *Sender);
    void __fastcall AdvStringGridDblClickCell(TObject *Sender, int ARow,
          int ACol);
    void __fastcall N3Click(TObject *Sender);
    void __fastcall FormShow(TObject *Sender);
    void __fastcall N1Click(TObject *Sender);
    void __fastcall N2Click(TObject *Sender);
    void __fastcall N4Click(TObject *Sender);
    void __fastcall N5Click(TObject *Sender);
    void __fastcall AdvStringGridDrawCell(TObject *Sender, int ACol,
          int ARow, TRect &Rect, TGridDrawState State);
    void __fastcall PanelMouseDown(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
private:	// User declarations
public:		// User declarations
    __fastcall TForm5(TComponent* Owner);
    int ColNum;
    String Node_GUID;
    void __fastcall Add_Row_Data(const int &row);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm5 *Form5;
//---------------------------------------------------------------------------
#endif
