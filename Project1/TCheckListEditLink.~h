//---------------------------------------------------------------------------

#ifndef TCheckListEditLinkH
#define TCheckListEditLinkH
#include <vcl.h>
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include "AdvGrid.hpp"
#include "BaseGrid.hpp"
#include <Grids.hpp>
#include "clisted.hpp"
//---------------------------------------------------------------------------
class TCheckListEditLink  : public TEditLink
{
public:
        __fastcall  TCheckListEditLink(Classes::TComponent* AOwner);
        __fastcall  ~TCheckListEditLink(void);
        void __fastcall CreateEditor(Controls::TWinControl* AParent);
        void __fastcall DestroyEditor();
        void __fastcall SetRect(const TRect &r);
        Controls::TWinControl* __fastcall GetEditControl(void);
        AnsiString __fastcall GetEditorValue();
        void __fastcall SetEditorValue(AnsiString s);
        void __fastcall EditExit(System::TObject* Sender);
        __property AnsiString CommaText = {read=GetCommaText, write=SetCommaText};
private:
        TCheckListEdit *FEdit;
        AnsiString FCommaText;
        void __fastcall SetCommaText(AnsiString value);
        AnsiString __fastcall GetCommaText();

};
class TEditMemoLink  : public TEditLink
{
public:
        __fastcall  TEditMemoLink(Classes::TComponent* AOwner);
        __fastcall  ~TEditMemoLink(void);
        void __fastcall CreateEditor(Controls::TWinControl* AParent);
        void __fastcall DestroyEditor();
        void __fastcall SetRect(const TRect &r);
        Controls::TWinControl* __fastcall GetEditControl(void);
        AnsiString __fastcall GetEditorValue();
        void __fastcall SetEditorValue(AnsiString s);
        void __fastcall EditExit(System::TObject* Sender);
private:
        TMemo *FEdit;

};
#endif
