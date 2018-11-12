//---------------------------------------------------------------------------

#ifndef DModUnitH
#define DModUnitH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <ADODB.hpp>
#include <DB.hpp>
//---------------------------------------------------------------------------
class TDMod : public TDataModule
{
__published:	// IDE-managed Components
    TADOConnection *ADOConnection1;
    TADOConnection *ADOConnection2;
    TADOConnection *ADOConnection3;
    void __fastcall DataModuleCreate(TObject *Sender);
private:	// User declarations
    void CreateTable();
public:		// User declarations
    __fastcall TDMod(TComponent* Owner);
    void Conn(TADOConnection *conn, AnsiString path, AnsiString password);    //连接数据库
        //执行sql语句
    void OpenSql(String sql,TADOQuery* qy);
    void ExecSql(String sql,TADOQuery* qy);
};
//---------------------------------------------------------------------------
extern PACKAGE TDMod *DMod;
//---------------------------------------------------------------------------
#endif
