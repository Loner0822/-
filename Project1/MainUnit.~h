//---------------------------------------------------------------------------

#ifndef MainUnitH
#define MainUnitH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ComCtrls.hpp>
#include <Buttons.hpp>
#include <ToolWin.hpp>
#include "DataUnit.h"
#include "TreeUnit.h"
#include <AdvGrid.hpp>
#include <BaseGrid.hpp>
#include <Grids.hpp>
#include <ExtCtrls.hpp>
#include "SigViewerLib_OCX.h"
#include <OleCtrls.hpp>
#include <memory>
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
    TPanel *Panel;
    TTimer *Timer;
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
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
