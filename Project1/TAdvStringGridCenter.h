//---------------------------------------------------------------------------

#ifndef TAdvStringGridCenterH
#define TAdvStringGridCenterH
//---------------------------------------------------------------------------
#include <vcl.h>
#include "AdvGrid.hpp"
#include "BaseGrid.hpp"
class TAdvStringGridCenter
{
private :
   TMemo * memo ;

public :
   TAdvStringGridCenter(TMemo * amemo);
   ~TAdvStringGridCenter();
   void AutoWordBreakAndCenter(TAdvStringGrid * sg,int ACol,int ARow);
   
}  ;





#endif
