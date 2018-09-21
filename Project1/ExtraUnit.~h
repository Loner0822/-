//---------------------------------------------------------------------------

#ifndef ExtraUnitH
#define ExtraUnitH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include "AdvGrid.hpp"
#include "BaseGrid.hpp"
#include <Grids.hpp>
#include "PenUnit.h"
#include <ExtCtrls.hpp>
#include <set>
#include <vector>
using namespace std;
//---------------------------------------------------------------------------
class TExtraForm : public TForm
{
__published:	// IDE-managed Components
    TAdvStringGrid *AdvStringGridExtra;
    TPanel *PanelExtra;
    TButton *ButtonExtra;
    void __fastcall ButtonExtraClick(TObject *Sender);
private:	// User declarations
public:		// User declarations
    int Move_num;
    vector<int> Delete_num;
    __fastcall TExtraForm(TComponent* Owner);
    void __fastcall FormCreate(TObject *Sender);
    void Move_Pen(const vector<set<Pen>::iterator> &L);
    void Delete_Pen(const vector<set<Pen>::iterator> &L);
};
//---------------------------------------------------------------------------
extern PACKAGE TExtraForm *ExtraForm;
//---------------------------------------------------------------------------
#endif
