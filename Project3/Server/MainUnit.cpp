//---------------------------------------------------------------------------

#include <vcl.h>
#include <vector>
#include <string>
#include <fstream>
#include <algorithm>
#pragma hdrstop

#include "MainUnit.h"
#include "inifiles.hpp"

//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "trayicon"
#pragma resource "*.dfm"

using namespace std;

TForm1 *Form1;
//---------------------------------------------------------------------------

__fastcall TForm1::TForm1(TComponent* Owner)
    : TForm(Owner)
{
    TrayIcon->Visible = 1;
}
//---------------------------------------------------------------------------


void __fastcall TForm1::ButtonClick(TObject *Sender)
{
    if (!ServerSocket->Active) {
        TrayIcon->IconIndex = 0;
        ServerSocket->Open();
        Button->Caption = "停止服务";
    }
    else {
        TrayIcon->IconIndex = 1;
        ServerSocket->Close();
        Button->Caption = "开始服务";
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ServerSocketAccept(TObject *Sender,
      TCustomWinSocket *Socket)
{
    StatusBar->SimpleText = "接收客户端" + Socket->RemoteAddress + "的连接请求";    
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ServerSocketClientConnect(TObject *Sender,
      TCustomWinSocket *Socket)
{
    StatusBar->SimpleText = "来自" + Socket->RemoteAddress + "的客户端已经接入";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ServerSocketClientDisconnect(TObject *Sender,
      TCustomWinSocket *Socket)
{
    StatusBar->SimpleText = "与" + Socket->RemoteAddress + "的链接已经断开";    
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ServerSocketClientError(TObject *Sender,
      TCustomWinSocket *Socket, TErrorEvent ErrorEvent, int &ErrorCode)
{
    StatusBar->SimpleText = "发生错误了(╯‵□′)╯︵┻━┻";    
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ServerSocketClientRead(TObject *Sender,
      TCustomWinSocket *Socket)
{
    String sRecvText = Socket->ReceiveText();
    String sRemoteAddr = Socket->RemoteAddress;
    logrec(sRecvText, sRemoteAddr);
    //ServerSocket->
    String cmd;                 // 操作类型
    String AppName;             // 软件名称
    String FileName;            // 文件名称
    int TableNum;               // 表个数
    vector<String> TableName;   // 表名称
    // 获取操作类型
    int pos = sRecvText.Pos("\n");
    cmd = sRecvText.SubString(1, pos - 1);
    sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

    if (cmd == "GetList") {
        pos = sRecvText.Pos("\n");
        AppName = sRecvText.SubString(1, pos - 1);
        std::vector<String> dmp_file, auto_file;
        dmp_file.clear();
        auto_file.clear();
        TSearchRec sr;
        String path = DataPath + AppName;
        if (!FindFirst(path + "\\*.dmp", faAnyFile, sr)) {
            do {
                if (sr.Name != "." && sr.Name != "..") {
                    dmp_file.push_back(sr.Name);
                }
            } while (FindNext(sr) == 0);
        }
        FindClose(sr);
        sort(dmp_file.begin(), dmp_file.end());

        if (!FindFirst(path + "\\Auto\\*.dmp", faAnyFile, sr)) {
            do {
                if (sr.Name != "." && sr.Name != "..") {
                    auto_file.push_back(sr.Name);
                }
            } while (FindNext(sr) == 0);
        }
        FindClose(sr);
        sort(auto_file.begin(), auto_file.end());

        cmd = "ReturnList";
        String Msg = cmd + "\n";
        Msg += IntToStr(dmp_file.size()) + "\n";
        for (int i = 0; i < (int)dmp_file.size(); ++ i)
            Msg += dmp_file[i] + "\n";
        Msg += IntToStr(auto_file.size()) + "\n";
        for (int i = 0; i < (int)auto_file.size(); ++ i)
            Msg += auto_file[i] + "\n";
        for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
            if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                ServerSocket->Socket->Connections[i]->SendText(Msg);
                break;
            }
        }
        return;
    }

    if (cmd == "Recover") {
        pos = sRecvText.Pos("\n");
        int op = StrToInt(sRecvText.SubString(1, pos - 1));
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

        pos = sRecvText.Pos("\n");
        AppName = sRecvText.SubString(1, pos - 1);
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

        pos = sRecvText.Pos("\n");
        FileName = sRecvText.SubString(1, pos - 1);
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

        pos = sRecvText.Pos("\n");
        TableNum = StrToInt(sRecvText.SubString(1, pos - 1));
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

        //String path = Datapath + AppName + "\\";
        TableName.clear();
        for (int i = 0; i < TableNum; ++ i) {
            pos = sRecvText.Pos("\n");
            String tmp = sRecvText.SubString(1, pos - 1);
            TableName.push_back(tmp);
            sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
        }

        ofstream d_sql;
        d_sql.open("Delete.sql");
        for (int i = 0; i < TableNum; ++ i)
            d_sql << "drop table " << TableName[i].c_str() << ";" << endl;
        d_sql << "quit;" << endl;
        d_sql.close();

        /*r_bat.open("Recover.bat");
        r_bat << "sqlplus " << SignIn.c_str() << " @Delete.sql" << endl;
        r_bat << "imp " << SignIn.c_str() << " file=" << Datapath.c_str() << AppName.c_str()
              << "\\" << FileName.c_str() << " tables=(";
        for (int i = 0; i < TableNum - 1; ++ i)
            r_bat << TableName[i].c_str() << ",";
        r_bat << TableName[TableNum - 1].c_str();
        r_bat << ")" << endl;
        r_bat.close();*/

        String recover_cmd;
        recover_cmd = "sqlplus " + SignIn + " @Delete.sql\n";

        if (system(recover_cmd.c_str()) != 0) {
            String Msg = "Recover_Error\n";
            for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
                if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                    ServerSocket->Socket->Connections[i]->SendText(Msg);
                    break;
                }
            }
            logError("Recover", sRemoteAddr, recover_cmd);
            return;
        }

        if (op == 1)
            recover_cmd = "imp " + SignIn + " file=" + DataPath + AppName + "\\" + FileName + " tables=(";
        else
            recover_cmd = "imp " + SignIn + " file=" + DataPath + AppName + "\\Auto\\" + FileName + " tables=(";
        for (int i = 0; i < TableNum - 1; ++ i)
            recover_cmd += TableName[i] + ",";
        recover_cmd += TableName[TableNum - 1] + ")\n";
        if (system(recover_cmd.c_str()) != 0) {
            String Msg = "Recover_Error\n";
            for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
                if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                    ServerSocket->Socket->Connections[i]->SendText(Msg);
                    break;
                }
            }
            logError("Recover", sRemoteAddr, recover_cmd);
            return;
        }
        String Msg = "Recover_Success\n";
        for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
            if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                ServerSocket->Socket->Connections[i]->SendText(Msg);
                break;
            }
        }
        /*String path = ExtractFilePath(Application->ExeName) + "Recover.bat";
        HINSTANCE runbat = ShellExecute(NULL, "open", path.c_str(), NULL, NULL, SW_SHOWNORMAL);
        if ((int)runbat <= 32) {
            String Msg = "Recover_Error\n";
            for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
                if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                    ServerSocket->Socket->Connections[i]->SendText(Msg);
                    break;
                }
            }
            //logError("Recover", (int)runbat);
        }
        else {
            String Msg = "Recover_Success\n";
            for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
                if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                    ServerSocket->Socket->Connections[i]->SendText(Msg);
                    break;
                }
            }
        }
        return;*/
    }

    if (cmd == "Backup") {
        String Now_DT = Now().FormatString("YYYYMMDDhhmmss");

        if (!DirectoryExists(DataPath)) {
            CreateDir(DataPath);
        }
        
        pos = sRecvText.Pos("\n");
        AppName = sRecvText.SubString(1, pos - 1);
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

        pos = sRecvText.Pos("\n");
        FileName = sRecvText.SubString(1, pos - 1);
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
        FileName = Now_DT + "_" + FileName;

        pos = sRecvText.Pos("\n");
        TableNum = StrToInt(sRecvText.SubString(1, pos - 1));
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

        TableName.clear();
        for (int i = 0; i < TableNum; ++ i) {
            pos = sRecvText.Pos("\n");
            String tmp = sRecvText.SubString(1, pos - 1);
            TableName.push_back(tmp);
            sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
        }

        if (!DirectoryExists(DataPath + AppName)) {
            CreateDir(DataPath + AppName);
        }

        /*ofstream b_bat;
        b_bat.open("Backup.bat");
        b_bat << "exp " << SignIn.c_str() << " file=" << Datapath.c_str() << AppName.c_str()
              << "\\" << FileName.c_str() << " tables=(";
        for (int i = 0; i < TableNum - 1; ++ i)
            b_bat << TableName[i].c_str() << ",";
        b_bat << TableName[TableNum - 1].c_str();
        b_bat << ")" << endl;
        b_bat.close();*/
        String backup_cmd = "exp " + SignIn + " file=" + DataPath + AppName + "\\" + FileName + " tables=(";
        for (int i = 0; i < TableNum - 1; ++ i)
            backup_cmd += TableName[i].c_str();
        backup_cmd += TableName[TableNum - 1].c_str();
        backup_cmd += ")\n";
        if (system(backup_cmd.c_str()) != 0) {
            String Msg = "Backup_Error\n";
            for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
                if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                    ServerSocket->Socket->Connections[i]->SendText(Msg);
                    break;
                }
            }
            logError("Backup", sRemoteAddr, backup_cmd);
            return;
        }
        else {
            String Msg = "Backup_Success\n";
            for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
                if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                    ServerSocket->Socket->Connections[i]->SendText(Msg);
                    break;
                }
            }
            return;
        }

        /*String path = ExtractFilePath(Application->ExeName) + "Backup.bat";
        HINSTANCE runbat = ShellExecute(NULL, "open", path.c_str(), NULL, NULL, SW_SHOWNORMAL);
        if ((int)runbat <= 32) {
            String Msg = "Backup_Error\n";
            for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
                if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                    ServerSocket->Socket->Connections[i]->SendText(Msg);
                    break;
                }
            }
            logError("Backup", (int)runbat);
        }
        else {
            String Msg = "Backup_Success\n";
            for (int i = 0; i < ServerSocket->Socket->ActiveConnections; ++ i) {
                if (ServerSocket->Socket->Connections[i]->RemoteAddress == sRemoteAddr) {
                    ServerSocket->Socket->Connections[i]->SendText(Msg);
                    break;
                }
            }
        }
        return;*/
    }
    
    /*int pos = sRecvText.Pos("\n");
    cmd = sRecvText.SubString(1, pos - 1);
    sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
    SoftWareName = cmd;
    Datapath += SoftWareName + "\\";

    pos = sRecvText.Pos("\n");
    cmd = sRecvText.SubString(1, pos - 1);
    sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
    Time = cmd;

    pos = sRecvText.Pos("\n");
    cmd = sRecvText.SubString(1, pos - 1);
    sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
    NumTable = StrToIntDef(sRecvText.SubString(1, pos - 1), 0);

    for (int i = 0; i < NumTable; ++ i) {
        String tmp;
        pos = sRecvText.Pos("\n");
        tmp = sRecvText.SubString(1, pos - 1);
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
        NameTable.push_back(tmp);
    }

    ofstream u_bat, r_bat, d_sql; 
    u_bat.open("Update.bat");
    u_bat << "sqlplus " << SignIn.c_str() << " @Delete.sql" << endl;
    u_bat << "imp " << SignIn.c_str() << " " << Datapath.c_str() << Time.c_str()
          << ".dmp tables=(";
    for (int i = 0; i < NumTable; ++ i)
        u_bat << NameTable[i].c_str() << ",";
    u_bat << ")" << endl;

    r_bat.open("Recover.bat");
    r_bat << "exp " << SignIn.c_str() << " " << Datapath.c_str() << Time.c_str()
          << ".dmp tables=(";
    for (int i = 0; i < NumTable; ++ i)
        r_bat << NameTable[i].c_str() << ",";
    r_bat << ")" << endl;

    d_sql.open("Delete.sql");
    for (int i = 0; i < NumTable; ++ i)
        d_sql << "drop table " << NameTable[i].c_str() << endl;
    d_sql << "quit" << endl;*/
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormCloseQuery(TObject *Sender, bool &CanClose)
{
    CanClose = true;
    int result;
    if(ServerSocket->Socket->ActiveConnections > 0) {
        result = MessageBox(Handle, "还有客户端与服务器保持连接,关闭窗口将失去与所有客户端的连接,确定要关闭吗?", "确定", MB_OKCANCEL);
        if(result == IDOK) {
            CanClose = true;
            ServerSocket->Close();
        }
        else {
            CanClose = false;
            return;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::S1Click(TObject *Sender)
{
    Form2->ShowModal();    
}
//---------------------------------------------------------------------------

void __fastcall TForm1::E1Click(TObject *Sender)
{
    bool flag;
    Form1->FormCloseQuery(Form1, flag);
    if (flag)
        Form1->Close();
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormCreate(TObject *Sender)
{
    //MessageBeep(MB_OK);
    TIniFile *ini;
    ini = new TIniFile(ExtractFilePath(Application->ExeName) + "Reg.ini");
    ServerSocket->Port = ini->ReadInteger("Port", "First", 900);
    ServerSocket->Open();
	delete ini;

    ini = new TIniFile(ExtractFilePath(Application->ExeName) + "Reg.ini");
    DataPath = ini->ReadString("Database", "path", "D:\\管理知识库数据备份\\");
    String username = ini->ReadString("Database", "username", "manager");
    String password = ini->ReadString("Database", "password", "zbxhzbxh");
    String ip = ini->ReadString("Database", "ip", "192.168.0.211");
    String port = ini->ReadString("Database", "password", "zbxhzbxh");
    String uid = ini->ReadString("Database", "uid", "ZBXH");
    SignIn = username + "/" + password + "@" + ip + ":" + port + "/" + uid;
    
    delete ini;
}
//---------------------------------------------------------------------------

