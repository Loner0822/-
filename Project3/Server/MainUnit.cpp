//---------------------------------------------------------------------------

#include <vcl.h>
#include <vector>
#include <string>
#include <fstream>
#pragma hdrstop

#include "MainUnit.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"

using namespace std;

TForm1 *Form1;
//---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------
void __fastcall TForm1::FormShow(TObject *Sender)
{
    //MessageBeep(MB_OK);
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ButtonClick(TObject *Sender)
{
    if (!ServerSocket->Active) {
        ServerSocket->Open();
        Button->Caption = "停止服务";
    }
    else {
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
    String Datapath = "flie=D:\\ZSK_Test\\";
    String SignIn = "manager/zbxhzbxh@ZBXH";
    String sRecvText = Socket->ReceiveText();
    String sRemoteAddr = Socket->RemoteAddress;
    String cmd;                 // 命令类型
    String option;              // 操作类型 恢复/备份
    String SoftWareName;        // 软件名称
    String Time;                // 时间参数
    int NumTable;               // 表个数
    vector<String> NameTable;   // 表名称
    
    int pos = sRecvText.Pos("\n");
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
    d_sql << "quit" << endl;
}
//---------------------------------------------------------------------------
