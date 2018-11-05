//---------------------------------------------------------------------------

#ifndef ListUnitH
#define ListUnitH
#include "AdvGrid.hpp"
#include "BaseGrid.hpp"
#include <Buttons.hpp>
#include <Classes.hpp>
#include <Controls.hpp>
#include <ExtCtrls.hpp>
#include <Grids.hpp>
#include <StdCtrls.hpp>
#include <ComCtrls.hpp>
//---------------------------------------------------------------------------
#include <vector>
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "AdvGrid.hpp"
#include "BaseGrid.hpp"
#include <Grids.hpp>
#include <Buttons.hpp>
#include <ExtCtrls.hpp>
//---------------------------------------------------------------------------
class TForm3 : public TForm
{
__published:	// IDE-managed Components
    TAdvStringGrid *AdvStringGrid;
    TPanel *Panel;
    TBitBtn *BitBtn;
    TPageControl *PageControl;
    TTabSheet *TabSheet1;
    TTabSheet *TabSheet2;
    TAdvStringGrid *AdvStringGrid1;
    void __fastcall AdvStringGridGetAlignment(TObject *Sender, int ARow,
          int ACol, TAlignment &HAlign, TVAlignment &VAlign);
    void __fastcall AdvStringGridResize(TObject *Sender);
    void __fastcall BitBtnClick(TObject *Sender);
private:	// User declarations
public:		// User declarations
    __fastcall TForm3(TComponent* Owner);
    void __fastcall FormRefresh();
    std::vector<String> FileName, FileName2;
    int FileNum, op;
};
//---------------------------------------------------------------------------
extern PACKAGE TForm3 *Form3;
//---------------------------------------------------------------------------
#endif
