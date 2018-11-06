//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "MainUnit.h"
#include "inifiles.hpp"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm1 *Form1;
//---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormCreate(TObject *Sender)
{
    option = ParamStr(1).ToInt();
    Button1->Enabled = false;
    TIniFile *Reg, *SyncDB;
    Reg = new TIniFile(ExtractFilePath(Application->ExeName) + "RegInfo.ini");
    AppName = Reg->ReadString("Public", "AppName", "Empty_Name");
    //ShowMessage(AppName);
    delete Reg;
    SyncDB = new TIniFile(ExtractFilePath(Application->ExeName) + "SyncDBInfo.ini");
    int cnt;
    TableName.clear();
    cnt = SyncDB->ReadInteger("up_down", "count", 0);
    for (int i = 0; i < cnt; ++ i) {
        String tmp = SyncDB->ReadString("up_down", IntToStr(i + 1), "");
        TableName.push_back(tmp);
    }
    delete SyncDB;


    TIniFile *ini;
    ini = new TIniFile(ExtractFilePath(Application->ExeName) + "SyncInfo.ini");
    ServerPort = RecvPort = ini->ReadInteger("Login", "port_up_down", 900);
    ClientSocket->Port = ServerPort;
    ClientSocket->Address = ini->ReadString("Login", "ip", "127.0.0.1");
    delete ini;
    ClientSocket->Open();
    Timer->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ClientSocketConnect(TObject *Sender, TCustomWinSocket *Socket)
{
    StatusBar->SimpleText = "与服务器连接成功";
    isActive = true;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ClientSocketConnecting(TObject *Sender,
      TCustomWinSocket *Socket)
{
    StatusBar->SimpleText = "正在连接服务器...";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ClientSocketDisconnect(TObject *Sender,
      TCustomWinSocket *Socket)
{
    Button1->Caption = "连接";
    StatusBar->SimpleText = "未连接到服务器或与服务器断开连接";    
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ClientSocketError(TObject *Sender,
      TCustomWinSocket *Socket, TErrorEvent ErrorEvent, int &ErrorCode)
{
    StatusBar->SimpleText = "连接到服务器时发生错误";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::ClientSocketRead(TObject *Sender,
      TCustomWinSocket *Socket)
{
    String sRecvText = Socket->ReceiveText();
    String cmd;                 // 命令类型
    int pos = sRecvText.Pos("\n");
    cmd = sRecvText.SubString(1, pos - 1);
    sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

    if (cmd == "ReturnList") {
        Form3->FileName.clear();
        Form3->FileName2.clear();

        pos = sRecvText.Pos("\n");
        int FileCnt = StrToInt(sRecvText.SubString(1, pos - 1));
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

        for (int i = 0; i < FileCnt; ++ i) {
            pos = sRecvText.Pos("\n");
            String tmp = sRecvText.SubString(1, pos - 1);
            Form3->FileName.push_back(tmp);
            sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
        }

        pos = sRecvText.Pos("\n");
        FileCnt = StrToInt(sRecvText.SubString(1, pos - 1));
        sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);

        for (int i = 0; i < FileCnt; ++ i) {
            pos = sRecvText.Pos("\n");
            String tmp = sRecvText.SubString(1, pos - 1);
            Form3->FileName2.push_back(tmp);
            sRecvText = sRecvText.SubString(pos + 1, sRecvText.Length() - pos);
        }
        Form3->FormRefresh();

        if (Form3->ShowModal() == mrOk) {
            if (Form3->FileNum == -1) {
                ShowMessage("尚未选择文件");
                return;
            }
            if (Form3->op == 1) {
                cmd = "Recover";
                String Msg = cmd + "\n" +
                            IntToStr(Form3->op) + "\n" +
                            AppName + "\n" +
                            Form3->FileName[Form3->FileNum] + "\n" +
                            IntToStr(TableName.size()) + "\n";
                for (int i = 0; i < (int)TableName.size(); ++ i)
                    Msg += TableName[i] + "\n";
                ClientSocket->Socket->SendText(Msg);
            }
            else {
                cmd = "Recover";
                String Msg = cmd + "\n" +
                            IntToStr(Form3->op) + "\n" +
                            AppName + "\n" +
                            Form3->FileName2[Form3->FileNum] + "\n" +
                            IntToStr(TableName.size()) + "\n";
                for (int i = 0; i < (int)TableName.size(); ++ i)
                    Msg += TableName[i] + "\n";
                ClientSocket->Socket->SendText(Msg);
            }
        }
        return;
    }

    if (cmd == "Recover_Error") {
        ShowMessage("恢复数据失败!");
        ClientSocket->Close();
        Close();
        return;
    }
    
    if (cmd == "Recover_Success") {
        ShowMessage("恢复数据成功!");
        ClientSocket->Close();
        Close();
        return;
    }

    if (cmd == "Backup_Error") {
        ShowMessage("备份数据失败!");
        ClientSocket->Close();
        Close();
        return;
    }

    if (cmd == "Backup_Success") {
        ShowMessage("备份数据成功!");
        ClientSocket->Close();
        Close();
        return;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button1Click(TObject *Sender)
{
    isActive = false;
    if (ClientSocket->Active) {
        ClientSocket->Close();
        Button1->Caption = "连接";
    }
    else {
        ClientSocket->Open();
        Button1->Caption = "断开连接";
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button4Click(TObject *Sender)
{
    if (Form2->ShowModal() == mrOk) {
        ServerPort = RecvPort = StrToInt(Form2->Edit->Text);
        ClientSocket->Address = Form2->ComboBox->Text;
        ClientSocket->Port = StrToInt(Form2->Edit->Text);
        Button1->Enabled = true;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button2Click(TObject *Sender)
{
    if (!ClientSocket->Active) {
        ShowMessage("服务器尚未连接!");
        return;
    }
    String cmd = "GetList";
    String Msg = cmd + "\n" +
                 AppName + "\n";
    ClientSocket->Socket->SendText(Msg);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button3Click(TObject *Sender)
{
    if (!ClientSocket->Active) {
        ShowMessage("服务器尚未连接!");
        return;
    }
    String cmd = "Backup";
    //TimeNow = Now().FormatString("yyyyMMddhhmmss");
    //ShowMessage(TimeNow);
    Form4->Edit->Text = "备份";
    Form4->Edit->SelectAll();
    Form4->Label1->Caption = ".dmp";
    if (Form4->ShowModal() == mrOk) {
        TimeNow = Form4->Edit->Text + Form4->Label1->Caption;
        String Msg = cmd + "\n" +
                     AppName + "\n" +
                     TimeNow + "\n" +
                     IntToStr(TableName.size()) + "\n";
        for (int i = 0; i < (int)TableName.size(); ++ i)
            Msg += TableName[i] + "\n";
        ClientSocket->Socket->SendText(Msg);
    }
    return;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormCloseQuery(TObject *Sender, bool &CanClose)
{
    //关闭客户端窗体
    int result;
    if(ClientSocket->Active) {
        result = MessageBox(Handle, "与服务器连接还没有断开,确定要退出吗?", "确定", MB_OKCANCEL);
        if(result == IDOK) {
            ClientSocket->Close();
            CanClose = true;
        }
        else {
            CanClose = false;
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::TimerTimer(TObject *Sender)
{
    Timer->Enabled = false;

    try {
        for (int i = 0; i < 100; ++ i) {
            Sleep(100);
            if (isActive) {
                Timer->Enabled = false;
                break;
            }
        }
        if (!isActive) {
            Timer->Enabled = false;
            throw 1;
        }
    }
    catch(...) {
        ShowMessage("与服务器连接失败!");
        Application->Terminate();
        return;
    }

    if (option == 1) {
        Button2Click(Button2);
    }
    if (option == 2) {
        Button3Click(Button3);
    }
}
//---------------------------------------------------------------------------

