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
        Button->Caption = "ֹͣ����";
    }
    else {
        ServerSocket->Close();
        Button->Caption = "��ʼ����";
    }
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ServerSocketAccept(TObject *Sender,
      TCustomWinSocket *Socket)
{
    StatusBar->SimpleText = "���տͻ���" + Socket->RemoteAddress + "����������";    
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ServerSocketClientConnect(TObject *Sender,
      TCustomWinSocket *Socket)
{
    StatusBar->SimpleText = "����" + Socket->RemoteAddress + "�Ŀͻ����Ѿ�����";
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ServerSocketClientDisconnect(TObject *Sender,
      TCustomWinSocket *Socket)
{
    StatusBar->SimpleText = "��" + Socket->RemoteAddress + "�������Ѿ��Ͽ�";    
}
//---------------------------------------------------------------------------
void __fastcall TForm1::ServerSocketClientError(TObject *Sender,
      TCustomWinSocket *Socket, TErrorEvent ErrorEvent, int &ErrorCode)
{
    StatusBar->SimpleText = "����������(�s�F����)�s��ߩ���";    
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ServerSocketClientRead(TObject *Sender,
      TCustomWinSocket *Socket)
{
    String Datapath = "flie=D:\\ZSK_Test\\";
    String SignIn = "manager/zbxhzbxh@ZBXH";
    String sRecvText = Socket->ReceiveText();
    String sRemoteAddr = Socket->RemoteAddress;
    String cmd;                 // ��������
    String option;              // �������� �ָ�/����
    String SoftWareName;        // �������
    String Time;                // ʱ�����
    int NumTable;               // �����
    vector<String> NameTable;   // ������
    
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
