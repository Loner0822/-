//---------------------------------------------------------------------------

#ifndef MainUnitH
#define MainUnitH
#include "AdvGrid.hpp"
#include "BaseGrid.hpp"
#include "SigViewerLib_OCX.h"
#include <Classes.hpp>
#include <ComCtrls.hpp>
#include <Controls.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <OleCtrls.hpp>
#include <StdCtrls.hpp>
#include <ToolWin.hpp>
#include <Menus.hpp>
//---------------------------------------------------------------------------
#include <set>
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <Buttons.hpp>
#include <ToolWin.hpp>
#include "DataUnit.h"
#include "TreeUnit.h"
#include "PenUnit.h"
#include "ExtraUnit.h"
#include <AdvGrid.hpp>
#include <BaseGrid.hpp>
#include <Grids.hpp>
#include <ExtCtrls.hpp>
#include "SigViewerLib_OCX.h"
#include "SystemUnit.h"
#include "TCheckListEditLink.h"
#include <OleCtrls.hpp>
#include <memory>
#include "clisted.hpp"

using namespace std;
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
    TGroupBox *GroupBox1;
    TTreeView *TreeView;
    TSplitter *Splitter1;
    TGroupBox *GroupBox2;
    TAdvStringGrid *AdvStringGrid;
    TSplitter *Splitter2;
    TGroupBox *GroupBox3;
    TTimer *Timer;
    TGroupBox *GroupBox5;
    TSplitter *Splitter4;
    TGroupBox *GroupBox6;
    TAdvStringGrid *AdvStringGrid1;
    TAdvStringGrid *AdvStringGrid2;
    TPopupMenu *PopupMenu;
    TMenuItem *N1;
    TGroupBox *GroupBox4;
    TSigViewer *SigViewer1;
    TPanel *Panel1;
    TSplitter *Splitter3;
    TPanel *Panel;
    TMainMenu *MainMenu1;
    TMenuItem *N2;
    TMenuItem *N3;
    TMenuItem *N4;
    TMenuItem *N5;
    TMenuItem *N6;
    TMenuItem *N7;
    TMenuItem *IP1;
    TMenuItem *N8;
    TMenuItem *N9;
    TPopupMenu *PopupMenu1;
    TMenuItem *A1;
    TMenuItem *U1;
    TMenuItem *D1;
	void __fastcall FormCreate(TObject *Sender);
    void __fastcall TreeViewChange(TObject *Sender, TTreeNode *Node);
	
	void __fastcall AdvStringGridClickCell(TObject *Sender, int ARow,
          int ACol);
	
    void __fastcall SigViewer1Paint(TObject *Sender, long DC);
    void __fastcall SigViewer1MouseDown(TObject *Sender,
          Sigviewerlib_tlb::TxMouseButton Button, long X, long Y, long *Cancel);
    void __fastcall SigViewer1MouseMove(TObject *Sender, long X, long Y,
          long *Cancel);
    void __fastcall SigViewer1MouseUp(TObject *Sender,
          Sigviewerlib_tlb::TxMouseButton Button, long X, long Y, long *Cancel);
    void __fastcall TimerTimer(TObject *Sender);
	
	void __fastcall AdvStringGrid1ClickCell(TObject *Sender, int ARow,
          int ACol);
    void __fastcall AdvStringGrid1EditCellDone(TObject *Sender, int ACol,
          int ARow);
    void __fastcall AdvStringGrid1CellValidate(TObject *Sender, int ACol,
          int ARow, AnsiString &Value, bool &Valid);
    void __fastcall AdvStringGrid1CanEditCell(TObject *Sender, int ARow,
          int ACol, bool &CanEdit);
    void __fastcall AdvStringGrid1RowMove(TObject *Sender, int ARow,
          bool &Allow);
    void __fastcall AdvStringGrid1RowMoved(TObject *Sender, int FromIndex,
          int ToIndex);
    void __fastcall AdvStringGrid1RowMoving(TObject *Sender, int ARow,
          bool &Allow);
	void __fastcall AdvStringGrid1DblClickCell(TObject *Sender, int ARow,
          int ACol);
		  
    void __fastcall N1Click(TObject *Sender);
	
    void __fastcall AdvStringGrid2CanEditCell(TObject *Sender, int ARow,
          int ACol, bool &CanEdit);
    void __fastcall AdvStringGrid2GetEditorType(TObject *Sender, int ACol,
          int ARow, TEditorType &AEditor);
    void __fastcall AdvStringGrid2EditCellDone(TObject *Sender, int ACol,
          int ARow);
    void __fastcall AdvStringGrid2ComboCloseUp(TObject *Sender, int ARow,
          int ACol);
    void __fastcall AdvStringGrid2MouseMove(TObject *Sender,
          TShiftState Shift, int X, int Y);
	void __fastcall AdvStringGrid2DblClickCell(TObject *Sender, int ARow,
          int ACol);
		  
    void __fastcall N5Click(TObject *Sender);
    void __fastcall N6Click(TObject *Sender);
    void __fastcall N7Click(TObject *Sender);
    void __fastcall IP1Click(TObject *Sender);
    void __fastcall N8Click(TObject *Sender);
    void __fastcall N4Click(TObject *Sender);
    void __fastcall N9Click(TObject *Sender);
    void __fastcall AdvStringGrid1GetAlignment(TObject *Sender, int ARow,
          int ACol, TAlignment &HAlign, TVAlignment &VAlign);
    void __fastcall AdvStringGridGetAlignment(TObject *Sender, int ARow,
          int ACol, TAlignment &HAlign, TVAlignment &VAlign);
    void __fastcall AdvStringGrid2GetAlignment(TObject *Sender, int ARow,
          int ACol, TAlignment &HAlign, TVAlignment &VAlign);
    void __fastcall AdvStringGridResize(TObject *Sender);
    void __fastcall AdvStringGrid1Resize(TObject *Sender);
    void __fastcall AdvStringGrid2Resize(TObject *Sender);

    void __fastcall A1Click(TObject *Sender);
    void __fastcall U1Click(TObject *Sender);
    void __fastcall D1Click(TObject *Sender);
    
    
private:	// User declarations
public:		// User declarations
    __fastcall TForm1(TComponent* Owner);
    void FindSon(int u, TTreeNode* tnode);
    void BuildTree();
    void ReadData(TADOQuery *AdoQuery);
    void ReadData2(TADOQuery *AdoQuery);
    void AddEdge(int x, int y);
	
    void DrawPicture(HDC hdcDest, int Orign[4], HDC hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc,int nHeightSrc, UINT crTransparent);
    void emf_Analysis();
    void ExplainLinkPoint( TStream* pStream );
    void Choose_Move_Pen(const vector<set<Pen>::iterator> &L);
    void Choose_Delete_Pen(const vector<set<Pen>::iterator> &L);
    void DB_AddPen(const Pen &u_pen);
    void DB_DeletePen(int type, int id);
	
	void FindFather(TTreeNode *tnode, int &level);
    
    void InsertNature(String u_name);
    void UpdateNature(String u_name);
    void DeleteNature();

    void InsertParm(String u_name);
    void UpdateParm(String u_name);
    void DeleteParm();


};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
