//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop
//---------------------------------------------------------------------------
USEFORM("MainUnit.cpp", Form1);
USEFORM("DModUnit.cpp", DMod); /* TDataModule: File Type */
USEFORM("ExtraUnit.cpp", ExtraForm);
USEFORM("SystemUnit.cpp", Form2);
USEFORM("CompanyUnit.cpp", Form3);
USEFORM("EditUnit.cpp", Form4);
USEFORM("include\SymbFrame.cpp", SymbolFrame); /* TFrame: File Type */
//---------------------------------------------------------------------------
WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
    try
    {
        Application->Initialize();
        // �ж��ظ�����
        Application->Title = "���м���׼֪ʶ��";
        HANDLE hnd = CreateMutex(NULL, TRUE, "���м���׼֪ʶ��");
        if (NULL == hnd) {
            return 0;
        }
        if (GetLastError() == ERROR_ALREADY_EXISTS) {
            ::MessageBox(NULL, "�����Ѿ�����", Application->Title.c_str(), MB_OK|MB_SYSTEMMODAL);
            ReleaseMutex(hnd);
            CloseHandle(hnd);
            return 0;
        }
        Application->CreateForm(__classid(TDMod), &DMod);
        Application->CreateForm(__classid(TForm1), &Form1);
         Application->CreateForm(__classid(TForm2), &Form2);
         Application->CreateForm(__classid(TForm3), &Form3);
         Application->CreateForm(__classid(TForm4), &Form4);
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
