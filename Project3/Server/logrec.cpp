//---------------------------------------------------------------------------


#pragma hdrstop

#include "logrec.h"

//---------------------------------------------------------------------------

#pragma package(smart_init)

void logrec(const String &Text, const String &Addr) {
    AnsiString Path = ExtractFilePath(Application->ExeName) + "Record.log";
    ofstream fout(Path.c_str(), ios::app);
    fout << Now().FormatString("yyyy-MM-dd hh:mm:ss").c_str() << " 客户端:" << Addr.c_str() << endl;
    fout << Text.c_str() << endl;
    fout.close();
}

void logAuto(const String &App, const String &Error) {
    AnsiString Path = ExtractFilePath(Application->ExeName) + "Backup\\Auto.log";
    ofstream fout(Path.c_str(), ios::app);
    fout << "Error" << endl;
    fout << Now().FormatString("yyyy-MM-dd hh:mm:ss").c_str() << " 软件:" << App.c_str() << endl;
    fout << Error.c_str() << endl;
    fout.close();
}

void logError(const String &option, const String &Addr, const String &Error) {

    AnsiString Path = ExtractFilePath(Application->ExeName) + "Record.log";
    ofstream fout(Path.c_str(), ios::app);
    fout << option.c_str() << " Error" << endl;
    fout << Now().FormatString("yyyy-MM-dd hh:mm:ss").c_str() << " 客户端:" << Addr.c_str() << endl;
    fout << Error.c_str() << endl;
    fout.close();
    /*String Error = "";
    switch (Error_Num) {
        case 0:
            Error = "The operating system is out of memory or resources 内存不足";
            break;
        case 2:
            Error = "ERROR_FILE_NOT_FOUND 文件名错误";
            break;
        case 3:
            Error = "ERROR_PATH_NOT_FOUND 路径名错误";
            break;
        case 11:
            Error = "ERROR_BAD_FORMA EXE 文件无效";
            break;
        case 26:
            Error = "SE_ERR_SHARE 发生共享错误";
            break;
        case 27:
            Error = "SE_ERR_ASSOCINCOMPLETE 文件名不完全或无效";
            break;
        case 28:
            Error = "SE_ERR_DDETIMEOUT 超时";
            break;
        case 29:
            Error = "SE_ERR_DDEFAIL DDE 事务失败";
            break;
        case 30:
            Error = "SE_ERR_DDEBUSY 正在处理其他 DDE 事务而不能完成该 DDE 事务";
            break;
        case 31:
            Error = "SE_ERR_NOASSOC 没有相关联的应用程序";
            break;
        default:
            Error = "Other_Error 其他错误";
            break;
    } */
}

