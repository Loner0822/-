//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "RangeUnit.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "AdvGrid"
#pragma link "BaseGrid"
#pragma link "SymbFrame"
#pragma link "TaskDialog"
#pragma resource "*.dfm"
TForm5 *Form5;
//---------------------------------------------------------------------------
__fastcall TForm5::TForm5(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

String myClipboard = "";

void __fastcall TForm5::ButtonClick(TObject *Sender)
{
    this->Close();    
}
//---------------------------------------------------------------------------

void __fastcall TForm5::AdvStringGridGetAlignment(TObject *Sender,
      int ARow, int ACol, TAlignment &HAlign, TVAlignment &VAlign)
{
    if (ARow == 0 || ACol == 0) {
        HAlign = taCenter;
        VAlign = vtaCenter;
    }
}
//---------------------------------------------------------------------------
void __fastcall TForm5::FormCreate(TObject *Sender)
{
    this->Panel->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::AdvStringGridClick(TObject *Sender)
{
    this->Panel->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::AdvStringGridClickCell(TObject *Sender, int ARow,
      int ACol)
{
    this->Panel->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::AdvStringGridDblClick(TObject *Sender)
{
    this->Panel->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton1Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton2Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton4Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton5Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton6Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton7Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton8Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton9Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton12Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton13Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton14Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton3Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton10Click(TObject *Sender)
{
    SymbolFrame->Button1Click(Sender);
    this->Panel->Visible = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::SymbolFrameButton11Click(TObject *Sender)
{
    if (AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row] == "")
        Add_Row_Data(AdvStringGrid->Row);
    SymbolFrame->Button1Click(Sender);
    this->Panel->Visible = false;
    if (AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row] == "")
        return;
    AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] = this->SymbolFrame->Edit1->Text;
    TADOQuery *AdoQ = new TADOQuery(NULL);
    AdoQ->Connection = DMod->ADOConnection3;
    String row_guid = AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row];
    String Code_GUID = Form1->AdvStringGrid1->Cells[6][AdvStringGrid->Col];
    String sql = "select FDVALUE from ZSK_CANSHUFW_H0000Z000K06 where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "' and FDNAME = '" + Code_GUID + "'";
    DMod->OpenSql(sql, AdoQ);
    if (!AdoQ->Eof) {
        sql = "update ZSK_CANSHUFW_H0000Z000K06 set FDVALUE = '" + AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] + "' where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "' and FDNAME = '" + Code_GUID + "'";
        DMod->ExecSql(sql, AdoQ);
    }
    else {
        CoInitialize(NULL);
        String pguid = newGUID();
        sql = "insert into ZSK_CANSHUFW_H0000Z000K06 (PGUID, S_UDTIME, JDGUID, YYGUID, FDNAME, FDVALUE) values('" + pguid + "', '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "', '" + Node_GUID + "', '" + row_guid + "', '" + Code_GUID + "', '" + AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] + "')";
        DMod->ExecSql(sql, AdoQ);
    }
    delete AdoQ;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::AdvStringGridDblClickCell(TObject *Sender,
      int ARow, int ACol)
{
    //
    this->Panel->Visible = false;
    if (AdvStringGrid->Cells[1][0] == "")
        return;
    if (ARow > 0 && ACol > 0 && ACol < AdvStringGrid->ColCount - 2)
    {
        this->SymbolFrame->Edit1->Text = AdvStringGrid->Cells[ACol][ARow]  ;
        this->Panel->Visible = true;
        TRect rect = AdvStringGrid->CellRect(ACol,ARow) ;
        this->Panel->Left = rect.left;
        this->Panel->Top = rect.top+rect.Height();
        if (this->Panel->Left + this->Panel->Width > this->AdvStringGrid->Left + this->AdvStringGrid->Width)
            this->Panel->Left = this->AdvStringGrid->Left + this->AdvStringGrid->Width - this->Panel->Width;
        if (this->Panel->Top + this->Panel->Height > this->AdvStringGrid->Top + this->AdvStringGrid->Height)
            this->Panel->Top = rect.top - this->Panel->Height;
        this->SymbolFrame->Edit1->SetFocus();
    }

    if (ARow> 0 && ACol > 0 && ACol == AdvStringGrid->ColCount - 2) {
        if (AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][ARow] != "") {
            AdvInputTaskDialog->Title="编辑原因"; // 对话框标题
            AdvInputTaskDialog->Instruction="输入原因：";  //输入内容label
            AdvInputTaskDialog->CustomButtons->CommaText = "确定"; //对话框按钮名称
            AdvInputTaskDialog->InputType = itEdit;//编辑类型 ,文本日期可选项
            AdvInputTaskDialog->InputText=AdvStringGrid->Cells[ACol][ARow];   //初始值
            int result =  AdvInputTaskDialog->Execute();
            if (result == 100) {
                String text = AdvInputTaskDialog->InputText;
                AdvStringGrid->Cells[ACol][ARow] = text;

                TADOQuery *AdoQ = new TADOQuery(NULL);
                AdoQ->Connection = DMod->ADOConnection3;
                String row_guid = AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row];
                String sql = "select YYVALUE from ZSK_GZYUANYIN_H0000Z000K06 where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and PGUID = '" + row_guid + "'";
                DMod->OpenSql(sql, AdoQ);
                if (!AdoQ->Eof) {
                    sql = "update ZSK_GZYUANYIN_H0000Z000K06 set YYVALUE = '" + text + "' where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and PGUID = '" + row_guid + "'";
                    DMod->ExecSql(sql, AdoQ);
                }
                delete AdoQ;
            }

        }
        else {
            AdvInputTaskDialog->Title="编辑原因"; // 对话框标题
            AdvInputTaskDialog->Instruction="输入原因：";  //输入内容label
            AdvInputTaskDialog->CustomButtons->CommaText = "确定"; //对话框按钮名称
            AdvInputTaskDialog->InputType = itEdit;//编辑类型 ,文本日期可选项
            AdvInputTaskDialog->InputText="";   //初始值
            int result =  AdvInputTaskDialog->Execute();
            if (result == 100) {
                Add_Row_Data(ARow);
                String text = AdvInputTaskDialog->InputText;
                AdvStringGrid->Cells[ACol][ARow] = text;

                TADOQuery *AdoQ = new TADOQuery(NULL);
                AdoQ->Connection = DMod->ADOConnection3;
                String row_guid = AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row];
                String sql = "select YYVALUE from ZSK_GZYUANYIN_H0000Z000K06 where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and PGUID = '" + row_guid + "'";
                DMod->OpenSql(sql, AdoQ);
                if (!AdoQ->Eof) {
                    sql = "update ZSK_GZYUANYIN_H0000Z000K06 set YYVALUE = '" + text + "' where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and PGUID = '" + row_guid + "'";
                    DMod->ExecSql(sql, AdoQ);
                }
                delete AdoQ;
            }            
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm5::Add_Row_Data(const int & row) {
    CoInitialize(NULL);
    String pguid = newGUID();
    TADOQuery *AdoQ = new TADOQuery(NULL);
    AdoQ->Connection = DMod->ADOConnection3;
    String sql = "insert into ZSK_GZYUANYIN_H0000Z000K06 (PGUID, S_UDTIME, JDGUID, YYVALUE) values('" + pguid + "', '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "', '" + Node_GUID + "', '(默认)')" ;
    AdvStringGrid->Cells[AdvStringGrid->ColCount - 2][row] = "(默认)";
    AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][row] = pguid;
    DMod->ExecSql(sql, AdoQ);
    delete AdoQ;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::N3Click(TObject *Sender)    // 添加行
{
    if (AdvStringGrid->Cells[0][AdvStringGrid->RowCount - 1] != "")
        AdvStringGrid->AddRow();
    //if (AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->RowCount - 1] != "")

    int len = AdvStringGrid->RowCount;
    AdvStringGrid->Cells[0][len - 1] = len - 1;
    //Add_Row_Data(len - 1);
}
//---------------------------------------------------------------------------

void __fastcall TForm5::FormShow(TObject *Sender)
{
    TADOQuery *AdoQ = new TADOQuery(NULL);
    AdoQ->Connection = DMod->ADOConnection3;
    String sql = "select PGUID, YYVALUE from ZSK_GZYUANYIN_H0000Z000K06 where ISDELETE = 0 and JDGUID = '" + Node_GUID + "'";
    DMod->OpenSql(sql, AdoQ);
    int cnt = 0;
    while (!AdoQ->Eof) {
        ++ cnt;
        if (AdvStringGrid->RowCount <= cnt)
            AdvStringGrid->AddRow();
        String row_guid = AdoQ->FieldByName("PGUID")->AsString;
        AdvStringGrid->Cells[AdvStringGrid->ColCount - 2][cnt] = AdoQ->FieldByName("YYVALUE")->AsString;
        AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][cnt] = row_guid;
        AdvStringGrid->Cells[0][cnt] = cnt;
        for (int i = 1; i < AdvStringGrid->ColCount - 2; ++ i) {
            TADOQuery *tempQuery = new TADOQuery(NULL);
            tempQuery->Connection = DMod->ADOConnection3;
            String Code_GUID = Form1->AdvStringGrid1->Cells[6][i];
            sql = "select FDVALUE from ZSK_CANSHUFW_H0000Z000K06 where JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "' and FDNAME = '" + Code_GUID + "'";
            DMod->OpenSql(sql, tempQuery);
            if (!tempQuery->Eof) {
                AdvStringGrid->Cells[i][cnt] = tempQuery->FieldByName("FDVALUE")->AsString;
            }
            else {
                AdvStringGrid->Cells[i][cnt] = "";
            }
            delete tempQuery;
        }
        AdoQ->Next();
    }
    delete AdoQ;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::N1Click(TObject *Sender)       // 删除格
{
    if (AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row] == "")
        return;
    if (AdvStringGrid->Col != AdvStringGrid->ColCount - 2) {
        AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] = "";
        TADOQuery *tempQuery = new TADOQuery(NULL);
        tempQuery->Connection = DMod->ADOConnection3;
        String row_guid = AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row];
        String Code_GUID = Form1->AdvStringGrid1->Cells[6][AdvStringGrid->Col];
        String sql = "update ZSK_CANSHUFW_H0000Z000K06 set FDVALUE = '' where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "' and FDNAME = '" + Code_GUID + "'";
        DMod->ExecSql(sql, tempQuery);
        delete tempQuery;
    }
    else {
        AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] = "";
        TADOQuery *tempQuery = new TADOQuery(NULL);
        tempQuery->Connection = DMod->ADOConnection3;
        String row_guid = AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row];
        String sql = "update ZSK_GZYUANYIN_H0000Z000K06 set YYVALUE = '' where ISDELETE = 0 and PGUID = '" + row_guid + "' and JDGUID = '" + Node_GUID + "'";
        DMod->ExecSql(sql, tempQuery);
        delete tempQuery;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm5::N2Click(TObject *Sender)        // 删除行
{
    if (AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row] == "") {
        if (AdvStringGrid->RowCount <= 2)
            AdvStringGrid->ClearRows(AdvStringGrid->Row, 1);
        else {
            AdvStringGrid->RemoveRows(AdvStringGrid->Row, 1);
            for (int i = 1; i < AdvStringGrid->RowCount; ++ i)
                AdvStringGrid->Cells[0][i] = i;
        }
        return;
    }
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String row_guid = AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row];
    String sql = "update ZSK_GZYUANYIN_H0000Z000K06 set ISDELETE = 1 where ISDELETE = 0 and PGUID = '" + row_guid + "' and JDGUID = '" + Node_GUID + "'";
    DMod->ExecSql(sql, tempQuery);
    delete tempQuery;
    tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    sql = "update ZSK_CANSHUFW_H0000Z000K06 set ISDELETE = 1 where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "'";
    DMod->ExecSql(sql, tempQuery);
    delete tempQuery;
    if (AdvStringGrid->RowCount <= 2)
        AdvStringGrid->ClearRows(AdvStringGrid->Row, 1);
    else {
        AdvStringGrid->RemoveRows(AdvStringGrid->Row, 1);
        for (int i = 1; i < AdvStringGrid->RowCount; ++ i)
            AdvStringGrid->Cells[0][i] = i;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm5::N4Click(TObject *Sender)
{
    myClipboard = AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row];
    TADOQuery *AdoQ = new TADOQuery(NULL);
    AdoQ->Connection = DMod->ADOConnection3;
    String row_guid = AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row];
    String Code_GUID = Form1->AdvStringGrid1->Cells[6][AdvStringGrid->Col];
    String sql = "select FDVALUE from ZSK_CANSHUFW_H0000Z000K06 where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "' and FDNAME = '" + Code_GUID + "'";
    DMod->OpenSql(sql, AdoQ);
    if (!AdoQ->Eof) {
        sql = "update ZSK_CANSHUFW_H0000Z000K06 set FDVALUE = '" + AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] + "' where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "' and FDNAME = '" + Code_GUID + "'";
        DMod->ExecSql(sql, AdoQ);
    }
    else {
        CoInitialize(NULL);
        String pguid = newGUID();
        sql = "insert into ZSK_CANSHUFW_H0000Z000K06 (PGUID, S_UDTIME, JDGUID, YYGUID, FDNAME, FDVALUE) values('" + pguid + "', '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "', '" + Node_GUID + "', '" + row_guid + "', '" + Code_GUID + "', '" + AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] + "')";
        DMod->ExecSql(sql, AdoQ);
    }
    delete AdoQ;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::N5Click(TObject *Sender)
{
    AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] = myClipboard;
    TADOQuery *AdoQ = new TADOQuery(NULL);
    AdoQ->Connection = DMod->ADOConnection3;
    String row_guid = AdvStringGrid->Cells[AdvStringGrid->ColCount - 1][AdvStringGrid->Row];
    String Code_GUID = Form1->AdvStringGrid1->Cells[6][AdvStringGrid->Col];
    String sql = "select FDVALUE from ZSK_CANSHUFW_H0000Z000K06 where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "' and FDNAME = '" + Code_GUID + "'";
    DMod->OpenSql(sql, AdoQ);
    if (!AdoQ->Eof) {
        sql = "update ZSK_CANSHUFW_H0000Z000K06 set FDVALUE = '" + AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] + "' where ISDELETE = 0 and JDGUID = '" + Node_GUID + "' and YYGUID = '" + row_guid + "' and FDNAME = '" + Code_GUID + "'";
        DMod->ExecSql(sql, AdoQ);
    }
    else {
        CoInitialize(NULL);
        String pguid = newGUID();
        sql = "insert into ZSK_CANSHUFW_H0000Z000K06 (PGUID, S_UDTIME, JDGUID, YYGUID, FDNAME, FDVALUE) values('" + pguid + "', '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "', '" + Node_GUID + "', '" + row_guid + "', '" + Code_GUID + "', '" + AdvStringGrid->Cells[AdvStringGrid->Col][AdvStringGrid->Row] + "')";
        DMod->ExecSql(sql, AdoQ);
    }
    delete AdoQ;
}
//---------------------------------------------------------------------------

void __fastcall TForm5::AdvStringGridDrawCell(TObject *Sender, int ACol,
      int ARow, TRect &Rect, TGridDrawState State)
{
     TAdvStringGrid * grid = (TAdvStringGrid *)Sender;
     TAdvStringGridCenter* AdvStringGridCenter = new TAdvStringGridCenter(Memo1);
     AdvStringGridCenter->AutoWordBreakAndCenter(grid,ACol,ARow);
     delete AdvStringGridCenter;
     AdvStringGridCenter = NULL;

}
//---------------------------------------------------------------------------


void __fastcall TForm5::PanelMouseDown(TObject *Sender,
      TMouseButton Button, TShiftState Shift, int X, int Y)
{
    ReleaseCapture();
    SendMessage(Panel-> Handle,WM_SYSCOMMAND,0XF012,0);
}
//---------------------------------------------------------------------------

