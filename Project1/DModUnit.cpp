//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop
#include <Registry.hpp>
#include "DModUnit.h"
#include <memory>
using namespace std;
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TDMod *DMod;
//---------------------------------------------------------------------------
__fastcall TDMod::TDMod(TComponent* Owner)
    : TDataModule(Owner)
{
}
//---------------------------------------------------------------------------
void TDMod::Conn(TADOConnection *conn, AnsiString path, AnsiString password) {
    try {
        conn->Close();
        conn->ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Password="";Data Source="
            + path +";Mode=ReadWrite;Persist Security Info=True;Jet OLEDB:Database Password=" + password;
        conn->Open();
    }
    catch(...){
        conn->Close();
        conn->ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Password="";Data Source="
            + path +";Mode=ReadWrite;Persist Security Info=True;Jet OLEDB:Database Password=" + password;
        conn->Open();
    }
}


void TDMod::OpenSql(String SQL, TADOQuery *q) {
    q -> Close();
    q -> SQL -> Text = SQL;
    q -> Open();
}

void TDMod::ExecSql(String SQL, TADOQuery *q) {
    q -> Close();
    q -> SQL -> Text = SQL;
    q -> ExecSQL();
}

void __fastcall TDMod::DataModuleCreate(TObject *Sender)
{
    String path1 = ExtractFilePath( Application->ExeName ) + "data\\YjzhZsk.mdb";
    Conn( this->ADOConnection1, path1, ""); //连接数据库
    String path2 = ExtractFilePath( Application->ExeName ) + "data\\应急指挥结线图.mdb";
    Conn( this->ADOConnection2, path2, "");
    String path3 = ExtractFilePath( Application->ExeName ) + "data\\结点属性.mdb";
    Conn( this->ADOConnection3, path3, "");
}
//---------------------------------------------------------------------------

