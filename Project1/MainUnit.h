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
#include <OleCtrls.hpp>
#include <memory>
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
    void __fastcall GetDataButtonClick(TObject *Sender);
    void __fastcall TreeViewChange(TObject *Sender, TTreeNode *Node);
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall AdvStringGridClickCell(TObject *Sender, int ARow,
          int ACol);
    void __fastcall SigViewer1Paint(TObject *Sender, long DC);
    void __fastcall SigViewer1MouseDown(TObject *Sender,
          TxMouseButton Button, long X, long Y, long *Cancel);
    void __fastcall SigViewer1MouseMove(TObject *Sender, long X, long Y,
          long *Cancel);
    void __fastcall SigViewer1MouseUp(TObject *Sender,
          TxMouseButton Button, long X, long Y, long *Cancel);
    void __fastcall TimerTimer(TObject *Sender);
    void __fastcall AdvStringGrid1EditCellDone(TObject *Sender, int ACol,
          int ARow);
    void __fastcall AdvStringGrid1CellValidate(TObject *Sender, int ACol,
          int ARow, AnsiString &Value, bool &Valid);
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
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
