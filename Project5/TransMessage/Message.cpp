//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Message.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "MsgComm"
#pragma link "MsgComm"
#pragma link "FlashVideoObjects_OCX"
#pragma resource "*.dfm"
TForm1 *Form1;
//---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------


void __fastcall TForm1::MsgComm1Connect(TObject *Sender)
{
    this->Timer1->Enabled = true;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::MsgComm1Disconnect(TObject *Sender)
{
    this->Timer1->Enabled = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Timer1Timer(TObject *Sender)
{
    // 人员ID.账户类型.host   =>  PC_UserName
    // PC端账户 1.7.jxx.org
    // 移动端账户 1.1.jxx.org
    AnsiString xml =        "<?xml version=\"1.0\" encoding=\"gb2312\"?>"
                            "<root>"
                            "<ZSMSMessageHeader>"
                            "<SenderID>" + PC_UserName + "</SenderID>"
                            "<TimeStamp>" + Now() + "</TimeStamp>"
                            "<MsgState>10</MsgState>"
                            "<DataType>1</DataType>"
                            "<MsgID>" + GenerateGuidStr() + "</MsgID>"
                            "<SenderName>" + PC_UserID + "</SenderName>"
                            "<MsgType>4</MsgType>"
                            "</ZSMSMessageHeader>"
                            "<ZSMSMessageBody>"
                            "<ClientType>1</ClientType>"
                            "<ClientName>" + PC_UserID + "</ClientName>"
                            "</ZSMSMessageBody>"
                            "</root>";
    MsgComm1->Send("6004." + Host + "@" + Host, "subject", xml);
    // 账户名@host -> 给谁发
}
//---------------------------------------------------------------------------

void __fastcall TForm1::MsgComm1Message(TObject *Sender, const char *from,
      const char *subject, const char *message)
{
    ::CoInitialize(NULL);
    AnsiString xml = message;
    Memo1->Lines->Add(xml);
    this->XMLDocument1->Active = false;
    this->XMLDocument1->LoadFromXML(xml);
    this->XMLDocument1->Active = true;
    _di_IXMLNode Node = this->XMLDocument1->DocumentElement;
    _di_IXMLNodeList nodeList = Node->GetChildNodes();
    _di_IXMLNode tempNode = nodeList->Nodes[0];
    _di_IXMLNode fdNode = tempNode->ChildNodes->FindNode("MsgType");
    String MsgType =  fdNode->GetText();
    fdNode = tempNode->ChildNodes->FindNode("MsgState");
    String MsgState =  fdNode->GetText();
    if( MsgType== "4" && MsgState == "23")      // 手机定位信息
    {
        /*perid, pername, lat, lng*/
        fdNode = tempNode->ChildNodes->FindNode("SenderID");
        AnsiString perid = fdNode->GetText();
        int ipos = perid.Pos(".");
        if (ipos > 0)
            perid = perid.SubString(0, ipos - 1);
        fdNode = tempNode->ChildNodes->FindNode("SenderName");
        AnsiString pername = fdNode->GetText();
        tempNode = nodeList->Nodes[1];
        fdNode = tempNode->ChildNodes->FindNode("Content");
        AnsiString lng_lat = fdNode->GetText();
        SendImportMsg(perid + "," + pername + "," + lng_lat, 1111);
    }
    if (MsgType == "4" && MsgState == "3")      // 视频传入
    {
        if (isConnect == false)
        {
            this->Visible = true;
            this->Refresh();
            isConnect = true;
            tempNode = nodeList->Nodes[1];
            fdNode = tempNode->ChildNodes->FindNode("Content");
            AnsiString option = fdNode->GetText().SubString(0, 4);
            if (option == "busy")
            {
                ShowMessage("当前手机端正在通话中!");
                Form2->Close();
            }
            else
            {
                AnsiString recefile = fdNode->GetText().SubString(6, fdNode->GetText().Length() - 5);
                Form2->FlashVideo2->VideoName = recefile;
                Form2->FlashVideo2->PlayRemote(recefile);
            }
        }
        else
        {
            tempNode = nodeList->Nodes[0];
            fdNode = tempNode->ChildNodes->FindNode("SenderID");
            AnsiString perid = fdNode->GetText();
            int index = perid.Pos(".");
            perid = perid.SubString(0, index - 1);
            AnsiString xml ="<?xml version=\"1.0\" encoding=\"gb2312\"?>"
                            "<root>"
                            "<ZSMSMessageHeader>"
                            "<SenderID>" + PC_UserName + "</SenderID>"
                            "<TimeStamp>" + Now() + "</TimeStamp>"
                            "<MsgState>3</MsgState>"
                            "<DataType>1</DataType>"
                            "<MsgID>" + GenerateGuidStr() + "</MsgID>"
                            "<SenderName>" + PC_UserID + "</SenderName>"
                            "<MsgType>4</MsgType>"
                            "</ZSMSMessageHeader>"
                            "<ZSMSMessageBody>"
                            "<AlarmID></AlarmID>"
                            "<LineName></LineName>"
                            "<StationName></StationName>"
                            "<DevName></DevName>"
                            "<AlarmTime></AlarmTime>"
                            "<Content>busy," + "" + "</Content>"
                            "<StepID></StepID>"
                            "<Receiver>" + perid + ".1." + Host + "</Receiver>"
                            "</ZSMSMessageBody>"
                            "</root>";
            MsgComm1->Send(perid + ".1." + Host + "@" + Host, "subject", xml);
        }
    }
}
//---------------------------------------------------------------------------

AnsiString TForm1::GenerateGuidStr()
{
    AnsiString str;
    GUID uid;
    CoCreateGuid(&uid);
    str = Sysutils::GUIDToString(uid);
    return str;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SendImportMsg(AnsiString Explain, int data)
{
    char a[4096] = {0};
    strncpy( a, Explain.c_str(), sizeof(a) - 1 );
    COPYDATASTRUCT cds;
    HANDLE fm;
    fm = FindWindow(NULL, (ProgName + "1.00").c_str());
    if (fm != NULL)
    {
         cds.dwData = data;
         cds.cbData = strlen(a) + 1;
         cds.lpData = a;
         SendMessage(fm, WM_COPYDATA, 0, (LPARAM)&cds);
    }
}
//---------------------------------------------------------------------------

AnsiString TForm1::Open_monitor()
{
    AnsiString fileName = PC_UserID + "_" + Now().FormatString("YYYYMMDDHHmmss");
    tmpForm->Show();
    tmpForm->FlashVideo1->Server = "rtmp://" + IP + ":1935/xnsj"  ;
    tmpForm->FlashVideo1->SetStretch(true);
    tmpForm->FlashVideo1->Connect();
    tmpForm->FlashVideo1->Preview();
    tmpForm->FlashVideo1->VideoName = fileName;
    tmpForm->FlashVideo1->Send();
    tmpForm->FlashVideo2->Server = "rtmp://" + IP + ":1935/xnsj"  ;
    tmpForm->FlashVideo2->SetStretch(true);
    tmpForm->FlashVideo2->Connect();
    return fileName;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::OnCopyData(TMessage &msg)
{
    bool flag = false ;
    if (msg.Msg == WM_COPYDATA)
    {
        COPYDATASTRUCT * pCopyData = (COPYDATASTRUCT* )msg.LParam;
        int type = pCopyData->dwData;
        int dataLen = pCopyData->cbData;
        if (type == 1111)
        {
            AnsiString alldata;
            alldata.SetLength(dataLen);
            strncpy((char*)alldata.data(), (char*)pCopyData->lpData, dataLen);
            int index = alldata.Pos(",");
            AnsiString perid = alldata.SubString(0, index - 1);
            AnsiString perName = alldata.SubString(index + 1, alldata.Length() - index - 4);
            AnsiString fileName = "";
            if (tmpForm != NULL)
                delete tmpForm;
            tmpForm = new TForm2(NULL);
            tmpForm->Caption = "正在与" + perName + "通话中...";

            try
            {
               fileName  = Open_monitor();
            }
            catch (...)
            {

            }
            AnsiString xml ="<?xml version=\"1.0\" encoding=\"gb2312\"?>"
                            "<root>"
                            "<ZSMSMessageHeader>"
                            "<SenderID>" + PC_UserName + "</SenderID>"
                            "<TimeStamp>" + Now() + "</TimeStamp>"
                            "<MsgState>3</MsgState>"
                            "<DataType>1</DataType>"
                            "<MsgID>" + GenerateGuidStr() + "</MsgID>"
                            "<SenderName>" + PC_PersonName + "</SenderName>"
                            "<MsgType>4</MsgType>"
                            "</ZSMSMessageHeader>"
                            "<ZSMSMessageBody>"
                            "<AlarmID></AlarmID>"
                            "<LineName></LineName>"
                            "<StationName></StationName>"
                            "<DevName></DevName>"
                            "<AlarmTime></AlarmTime>"
                            "<Content>open," + fileName + "</Content>"
                            "<StepID></StepID>"
                            "<Receiver>" + perid + "." + Host + "</Receiver>"
                            "</ZSMSMessageBody>"
                            "</root>";
            MsgComm1->Send(perid + "." + Host + "@" + Host, "subject", xml);
        }
        flag = true ;
    }
    if (!flag)
    {
        TForm::WndProc(msg);
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormCreate(TObject *Sender)
{
    this->Timer1->Enabled = false;
    this->Timer1->Interval = 5000;
    isConnect = false;
    
    WorkPath = ExtractFilePath(Application->ExeName);
    TIniFile *pIniFile = new TIniFile(WorkPath + "SyncInfo.ini");
    IP = pIniFile->ReadString("login", "ip", "");
    Port = pIniFile->ReadString("login", "port", "");
    Host = pIniFile->ReadString("login", "host", "");
    delete pIniFile;
    pIniFile = new TIniFile(WorkPath + "Loginconfig.ini");
    PC_UserID = pIniFile->ReadString("login", "perid", "0");
    PC_PersonName = pIniFile->ReadString("login", "pername", "");
    PC_UserName = PC_UserID + ".7." + Host;
    delete pIniFile;
    pIniFile = new TIniFile(WorkPath + "RegInfo.ini");
    AnsiString tmp = pIniFile->ReadString("Public", "UnitName", "");
    tmp += pIniFile->ReadString("Public", "AppName", "");
    ProgName = tmp;
    tmp += pIniFile->ReadString("版本号", "VerNum", "").SubString(0, 4);
    PC_ExeTitleName = tmp;
    this->Caption = ProgName + "trans";
    delete pIniFile;
    this->Caption = PC_UserName;

    MsgComm1->User = PC_UserName;
    MsgComm1->Password = "0";
    MsgComm1->Host = Host;
    MsgComm1->Server = IP;
    MsgComm1->Open();
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button1Click(TObject *Sender)
{
    this->FlashVideo1->Init("");
    this->FlashVideo1->Server = "rtmp://192.168.0.109:1935/xnsj"  ;
    this->FlashVideo1->SetStretch(true);
    this->FlashVideo1->Connect();
    this->FlashVideo1->Preview();
    this->FlashVideo1->VideoName = "1212";
    this->FlashVideo1->Send();
}
//---------------------------------------------------------------------------

