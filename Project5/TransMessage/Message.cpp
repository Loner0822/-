//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Message.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "MsgComm"
#pragma link "MsgComm"
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
    AnsiString xml =    "<?xml version=\"1.0\" encoding=\"gb2312\"?>"
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
            perid = perid.SubString(0, ipos);
        fdNode = tempNode->ChildNodes->FindNode("SenderName");
        AnsiString pername = fdNode->GetText();
        tempNode = nodeList->Nodes[1];
        fdNode = tempNode->ChildNodes->FindNode("Content");
        AnsiString lng_lat = fdNode->GetText();
        SendImportMsg(perid + "," + pername + "," + lng_lat, 1111);
    }
    
    if ( MsgType == "4" && MsgState == "")       // PC端视频请求
    {
        
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
    fm = FindWindow(NULL, "绩溪县");
    if (fm != NULL)
    {
         cds.dwData = data;
         cds.cbData = strlen(a) + 1;
         cds.lpData = a;
         SendMessage(fm, WM_COPYDATA, 0, (LPARAM)&cds);
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::OnCopyData(TMessage &msg)
{
    bool flag = false ;
    if (msg.Msg == WM_COPYDATA)
    {
        COPYDATASTRUCT * pCopyData = (COPYDATASTRUCT* )msg.LParam ;
        int type = pCopyData->dwData ;
        int dataLen = pCopyData->cbData ;
        AnsiString a;
        a.SetLength( dataLen );
        strncpy((char*)a.data(), (char*)pCopyData->lpData, dataLen);


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
    tmp += pIniFile->ReadString("版本号", "VerNum", "").SubString(0, 4);
    PC_ExeTitleName = tmp;
    delete pIniFile;
    
    MsgComm1->User = PC_UserName;
    MsgComm1->Password = "0";
    MsgComm1->Host = Host;
    MsgComm1->Server = IP;
    MsgComm1->Open();
}
//---------------------------------------------------------------------------

