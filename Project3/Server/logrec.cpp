//---------------------------------------------------------------------------


#pragma hdrstop

#include "logrec.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

void logrec(const String &Text, const String &Addr) {
    AnsiString Path = ExtractFilePath(Application->ExeName) + "Record.log";
    ofstream fout(Path.c_str(), ios::app);
    fout << Now().FormatString("yyyy-MM-dd hh:mm:ss").c_str() << " �ͻ���:" << Addr.c_str() << endl;
    fout << Text.c_str() << endl;
    fout.close();
}

void logAuto(const String &App, const String &Error) {
    AnsiString Path = ExtractFilePath(Application->ExeName) + "Backup\\Auto.log";
    ofstream fout(Path.c_str(), ios::app);
    fout << "Error" << endl;
    fout << Now().FormatString("yyyy-MM-dd hh:mm:ss").c_str() << " ���:" << App.c_str() << endl;
    fout << Error.c_str() << endl;
    fout.close();
}

void logError(const String &option, const String &Addr, const String &Error) {

    AnsiString Path = ExtractFilePath(Application->ExeName) + "Record.log";
    ofstream fout(Path.c_str(), ios::app);
    fout << option.c_str() << " Error" << endl;
    fout << Now().FormatString("yyyy-MM-dd hh:mm:ss").c_str() << " �ͻ���:" << Addr.c_str() << endl;
    fout << Error.c_str() << endl;
    fout.close();
    /*String Error = "";
    switch (Error_Num) {
        case 0:
            Error = "The operating system is out of memory or resources �ڴ治��";
            break;
        case 2:
            Error = "ERROR_FILE_NOT_FOUND �ļ�������";
            break;
        case 3:
            Error = "ERROR_PATH_NOT_FOUND ·��������";
            break;
        case 11:
            Error = "ERROR_BAD_FORMA EXE �ļ���Ч";
            break;
        case 26:
            Error = "SE_ERR_SHARE �����������";
            break;
        case 27:
            Error = "SE_ERR_ASSOCINCOMPLETE �ļ�������ȫ����Ч";
            break;
        case 28:
            Error = "SE_ERR_DDETIMEOUT ��ʱ";
            break;
        case 29:
            Error = "SE_ERR_DDEFAIL DDE ����ʧ��";
            break;
        case 30:
            Error = "SE_ERR_DDEBUSY ���ڴ������� DDE �����������ɸ� DDE ����";
            break;
        case 31:
            Error = "SE_ERR_NOASSOC û���������Ӧ�ó���";
            break;
        default:
            Error = "Other_Error ��������";
            break;
    } */
}

