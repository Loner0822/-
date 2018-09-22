//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop
//---------------------------------------------------------------------------
USEFORM("MainUnit.cpp", Form1);
USEFORM("DModUnit.cpp", DMod); /* TDataModule: File Type */
USEFORM("ExtraUnit.cpp", ExtraForm);
//---------------------------------------------------------------------------
WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
    try
    {
         Application->Initialize();
         Application->CreateForm(__classid(TDMod), &DMod);
         Application->CreateForm(__classid(TForm1), &Form1);
         Application->CreateForm(__classid(TExtraForm), &ExtraForm);
         Application->Run();
    }
    catch (Exception &exception)
    {
         Application->ShowException(&exception);
    }
    catch (...)
    {
         try
         {
             throw Exception("");
         }
         catch (Exception &exception)
         {
             Application->ShowException(&exception);
         }
    }
    return 0;
}
//---------------------------------------------------------------------------
