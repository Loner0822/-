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
    TSigViewer *SigViewer1;
    TTimer *Timer;
    TGroupBox *GroupBox4;
    TSplitter *Splitter3;
    TGroupBox *GroupBox5;
    TSplitter *Splitter4;
    TGroupBox *GroupBox6;
    TPanel *Panel;
    TAdvStringGrid *AdvStringGrid1;
    TAdvStringGrid *AdvStringGrid2;
    TToolBar *ToolBar1;
    TButton *Button1;
    TButton *Button2;
    TButton *Button3;
    TPopupMenu *PopupMenu;
    TMenuItem *N1;
    void __fastcall TreeViewChange(TObject *Sender, TTreeNode *Node);
    void __fastcall FormCreate(TObject *Sender);
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
    void __fastcall N1Click(TObject *Sender);
    void __fastcall AdvStringGrid2CanEditCell(TObject *Sender, int ARow,
          int ACol, bool &CanEdit);
    void __fastcall AdvStringGrid2GetEditorType(TObject *Sender, int ACol,
          int ARow, TEditorType &AEditor);
    void __fastcall AdvStringGrid2EditCellDone(TObject *Sender, int ACol,
          int ARow);
    void __fastcall AdvStringGrid1ClickCell(TObject *Sender, int ARow,
          int ACol);
    void __fastcall AdvStringGrid2ComboCloseUp(TObject *Sender, int ARow,
          int ACol);
private:	// User declarations
public:		// User declarations
    __fastcall TForm1(TComponent* Owner);
    //void FindSon(TTreeNode* node, String pguid);
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
    void FindFather(TTreeNode *tnode, int &level);
    void DB_AddPen(const Pen &u_pen);
    void DB_DeletePen(int type, int id);
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
