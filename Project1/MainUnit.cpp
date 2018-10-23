//---------------------------------------------------------------------------

#include <vcl.h>
#include <map>
#include <set>
#include <vector>
#include <typeinfo>
#include <objbase.h>
#include <cstdio>
#pragma hdrstop
#include "DModUnit.h"
#include "MainUnit.h"
#include "DataUnit.h"
#include "TreeUnit.h"
#include "PenUnit.h"
#include "ExtraUnit.h"
#include "SystemUnit.h"
#include "TCheckListEditLink.h"
#include "TProcess.h"
#include "TBuildSoftwareF.h"
#include "ExceptLog.h"
#include "EditUnit.h"
#include "GUID.h"
using namespace std;
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "AdvGrid"
#pragma link "BaseGrid"
#pragma link "SigViewerLib_OCX"
#pragma link "AdvGrid"
#pragma link "BaseGrid"
#pragma link "SigViewerLib_OCX"
#pragma resource "*.dfm"
TForm1 *Form1;
TCheckListEditLink *edCheckListEdit;
map<String, int> P_ID;
multimap<String, TData2> Find_Map;
DynamicArray<String> Map_PGUID;
DynamicArray< TNode<TData> > node;
DynamicArray<TEdge> edge;
int num_of_pic = 0;

int used_red_pen, used_black_pen, used_clamp_pen;
int using_red, using_black, using_clamp;
bool ClickDown;
int Partner;
pair<long, long> Coordinate;
const long dx[4] = {0, 35, 3, 6};
const long dy[4] = {0, 23, 22, 20};
set<pair<long, long> > Map_Node;    // x, y
set<Pen> Pen_Node;    				// 表笔编号, 表笔类型, x, y

int Now_Node;
String Now_Nature;
map<String, String>Data_Type;
vector<int> Department;
map<int, String> Department_Name;
String sql_Dep;

//---------------------------------------------------------------------------

__fastcall TForm1::TForm1(TComponent* Owner)
    : TForm(Owner) {
    edCheckListEdit = new TCheckListEditLink(this->AdvStringGrid2);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormCreate(TObject *Sender) {
    // 获取单位名称及上级名称
    Department.clear();
    std::auto_ptr<TIniFile> pIniFile (new TIniFile(ExtractFilePath(Application->ExeName) + "RegInfo.ini"));
    int u_id = pIniFile->ReadInteger("Public", "UnitID", 0);
    String UnitName = pIniFile->ReadString("Public", "UnitName", "");
    TADOQuery *tempQuery;
    while (u_id != 0) {
        tempQuery = new TADOQuery(NULL);
        String sql = "select 上级单位, 名称 from BD_AU_单位字典表 where 单位编号 = " + IntToStr(u_id);
        tempQuery->Connection = DMod->ADOConnection4;
        DMod->OpenSql(sql, tempQuery);
        UnitName = tempQuery->FieldByName("名称")->AsString;
        Department.push_back(u_id);
        Department_Name[u_id] = UnitName;
        u_id = tempQuery->FieldByName("上级单位")->AsInteger;
        delete tempQuery;
    }
    Department.push_back(u_id);
    Department_Name[u_id] = "中北信号";
    BuildSoftware->Department_Level = Department[0];
    sql_Dep = " and ( DEPARTMENT = 0";
    for (int i = 0; i < Department.size() - 1; ++ i)
        sql_Dep += " or DEPARTMENT = " + IntToStr(Department[i]);
    sql_Dep += ")";
    //ShowMessage(sql_Dep);
    /*for (int i = 0; i < Department.size(); ++ i) {
        ShowMessage(IntToStr(Department[i]));
    } */

    // SigViewer 初始化
    SigViewer1->OnMouseDown = SigViewer1MouseDown;
    SigViewer1->Init( L"123", L"" );
    SigViewer1->ShowToolbar = false;
    Panel->Visible = true;

    // 读取数据库yjzhzsk.mdb
    TADOQuery *AdoQ = new TADOQuery(NULL);
    node.Length = 0;
    edge.Length = 0;
    AdoQ->Connection = DMod->ADOConnection1;
    String sql = "select * from YJZH_MapFenLei where ISDELETE = 0 order by Index_ desc";
    DMod->OpenSql(sql, AdoQ);
    int cnt = 0;
    while (!AdoQ->Eof) {
        ++ cnt;
        ReadData(AdoQ);
        TData tmpdata;
        tmpdata = node[node.High].Data;
        //ShowMessage(tmpdata.PGUID);
        P_ID[tmpdata.PGUID] = cnt;
        AdoQ->Next();
    }
    delete AdoQ;

    AdoQ = new TADOQuery(NULL);
    AdoQ->Connection = DMod->ADOConnection1;
    sql = "select * from YJZH_SheJiYuan where ISDELETE = 0 ";
    DMod->OpenSql(sql, AdoQ);
    while (!AdoQ->Eof) {
        ReadData2(AdoQ);
        AdoQ->Next();
    }
    this->TreeView->Items->Clear();
    BuildTree();
    delete AdoQ;

    // AdvStringGird 初始化
    AdvStringGrid->RowCount = 2;
    AdvStringGrid->ColCount = 4;
    AdvStringGrid->FixedRows = 1;
    AdvStringGrid->FixedCols = 1;
    AdvStringGrid->Options >> goColSizing;
    AdvStringGrid->Options << goRowSizing;
    AdvStringGrid->Options << goRowSelect;
    AdvStringGrid->ColWidths[0] = 32;
    AdvStringGrid->ColWidths[1] = 64;
    AdvStringGrid->ColWidths[2] = 128;
    AdvStringGrid->ColWidths[3] = 0;
    GroupBox2->Width = AdvStringGrid->ColWidths[0] + AdvStringGrid->ColWidths[1] * 3 + 8;

    AdvStringGrid->Clear();
    AdvStringGrid->Cells[0][0] = "序号";
    AdvStringGrid->Cells[1][0] = "设计院";
    AdvStringGrid->Cells[2][0] = "图纸名称";


    //初始化属性列表 Parm
    AdvStringGrid1->Options << goEditing;
    //AdvStringGrid1->Options << goRowSelect;
    AdvStringGrid1->Options << goColSizing;
    AdvStringGrid1->Options >> goRowSizing;
    AdvStringGrid1->Options << goRowMoving;
    AdvStringGrid1->RowCount = 2;
    AdvStringGrid1->ColCount = 7;
    AdvStringGrid1->FixedRows = 1;
    AdvStringGrid1->FixedCols = 1;
    AdvStringGrid1->ColWidths[0] = 32;
    AdvStringGrid1->ColWidths[1] = 160;
    AdvStringGrid1->ColWidths[2] = 160;
    AdvStringGrid1->ColWidths[3] = 0;
	AdvStringGrid1->ColWidths[4] = 0;
    AdvStringGrid1->ColWidths[5] = 0;
	AdvStringGrid1->ColWidths[6] = 0;

    AdvStringGrid1->Clear();
    AdvStringGrid1->Cells[0][0] = "序号";
    AdvStringGrid1->Cells[1][0] = "节点名称";
    AdvStringGrid1->Cells[2][0] = "配线名称";

    AdvStringGrid2->Options << goEditing;
    AdvStringGrid2->Options << goColSizing;
    AdvStringGrid2->Options >> goRowSizing;
    AdvStringGrid2->RowCount = 2;
    AdvStringGrid2->ColCount = 6;
    AdvStringGrid2->FixedRows = 1;
    AdvStringGrid2->FixedCols = 1;
    AdvStringGrid2->ColWidths[0] = 32;
    AdvStringGrid2->ColWidths[3] = 0;
    AdvStringGrid2->ColWidths[4] = 0;
    AdvStringGrid2->ColWidths[5] = 0;

    AdvStringGrid2->Clear();
    AdvStringGrid2->Cells[0][0] = "序号";
    AdvStringGrid2->Cells[1][0] = "定义参数";
    AdvStringGrid2->Cells[2][0] = "定义值";
}
//---------------------------------------------------------------------------

void __fastcall TForm1::TreeViewChange(TObject *Sender, TTreeNode *Node) {
    Now_Node = (int)Node->Data;
    Map_PGUID.Length = 0;

    AdvStringGrid->Clear();
    AdvStringGrid->Cells[0][0] = "序号";
    AdvStringGrid->Cells[1][0] = "设计院";
    AdvStringGrid->Cells[2][0] = "图纸名称";
    String name = Node->Text;
    //ShowMessage(node[tindex].Data.JdText);
    String uid = node[Now_Node].Data.PGUID;
    multimap<String, TData2>::iterator it;
    int cnt = 0;
    for (it = Find_Map.equal_range(uid).first; it != Find_Map.equal_range(uid).second; ++ it)
        ++ cnt;
    if (cnt == 0)
        cnt = 1;
    AdvStringGrid->RowCount = cnt + 1;
    //ShowMessage((*it).second.TypeName);
    for (cnt = 1, it = Find_Map.equal_range(uid).first; it != Find_Map.equal_range(uid).second; ++ it, ++ cnt) {
        AdvStringGrid->Cells[0][cnt] = cnt;
        AdvStringGrid->Cells[1][cnt] = it->second.TypeName;
        AdvStringGrid->Cells[2][cnt] = it->second.MapName;
        AdvStringGrid->Cells[3][cnt] = it->second.PGUID;
        Map_PGUID.Length ++;
        Map_PGUID[cnt - 1] = it->second.MapID;
    }
    AdvStringGridClickCell(AdvStringGrid, 1, 1);

    AdvStringGrid1->Clear();
    AdvStringGrid1->RowCount = 2;
    AdvStringGrid1->Cells[0][0] = "序号";
    AdvStringGrid1->Cells[1][0] = "节点名称";
    AdvStringGrid1->Cells[2][0] = "配线名称";
    FindFather(Node, 1);
    //AdvStringGrid1->Cells[1][AdvStringGrid1->RowCount - 1] = node[Now_Node].Data.JdText;
    AdvStringGrid1ClickCell(AdvStringGrid1, 1, 2);

    AdvStringGrid2Refresh();

    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String sql = "select UPGUID, DATATYPE from ZSK_DATATYPE_H0000Z000K06 where ISDELETE = 0";
    DMod->OpenSql(sql, tempQuery);
    Data_Type.clear();
    while (!tempQuery->Eof) {
        String upguid, datatype;
        upguid = tempQuery->FieldByName("UPGUID")->AsString;
        datatype = tempQuery->FieldByName("DATATYPE")->AsString;
        Data_Type[upguid] = datatype;
        tempQuery->Next();
    }
    delete tempQuery;
}
//---------------------------------------------------------------------------

void TForm1::FindFather(TTreeNode *tnode, int &level) {
    int u = (int)tnode->Data;
    TTreeNode* pa = tnode->Parent;
    if (node[u].fa != u)
        FindFather(pa, level);
    String pguid = node[u].Data.PGUID;
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String sql = "select PGUID, NATURE, DEPARTMENT, CodeGUID from ZSK_NATURE_H0000Z000K06 where UPGUID = '" + pguid + "' and ISDELETE = 0";
    DMod->OpenSql(sql + " order by Index_, ID", tempQuery);
    if (tempQuery->Eof) {
        delete tempQuery;
        return;
    }
    int cnt = 0;
    while (!tempQuery->Eof) {
        if (AdvStringGrid1->Cells[1][AdvStringGrid1->RowCount - 2] != "节点名称"
         || AdvStringGrid1->Cells[1][AdvStringGrid1->RowCount - 1] != "")
            AdvStringGrid1->AddRow();
        AdvStringGrid1->Cells[0][level + cnt] = level + cnt;
        AdvStringGrid1->Cells[1][level + cnt] = node[u].Data.JdText;
        AdvStringGrid1->Cells[2][level + cnt] = tempQuery->FieldByName("NATURE")->AsString;
        AdvStringGrid1->Cells[3][level + cnt] = node[u].Data.PGUID;
        AdvStringGrid1->Cells[4][level + cnt] = tempQuery->FieldByName("PGUID")->AsString;
		AdvStringGrid1->Cells[5][level + cnt] = tempQuery->FieldByName("DEPARTMENT")->AsString;
		AdvStringGrid1->Cells[6][level + cnt] = tempQuery->FieldByName("CodeGUID")->AsString;
        tempQuery->Next();
        ++ cnt;

    }
    level += cnt;
    delete tempQuery;
}
//---------------------------------------------------------------------------

void TForm1::FindSon(int u, TTreeNode* tnode) {
    tnode->Data = (void *)u;
    for (int i = node[u].Head; i != -1; i = edge[i].next) {
        TTreeNode* pa;
        int v = edge[i].to;
        pa = this->TreeView->Items->AddChild(tnode, node[v].Data.JdText);
        FindSon(v, pa);
    }
}
//---------------------------------------------------------------------------

void TForm1::BuildTree() {
    for (int i = node.Low; i <= node.High; ++ i) {
         if (node[i].Data.UPGUID != "") {
            int fa = P_ID.find(node[i].Data.UPGUID)->second;
            if (fa > 0)
                AddEdge(fa - 1, i);
        }
    }
    P_ID.clear();
    for (int i = node.High; i >= node.Low; -- i) {
        TData tmpdata = node[i].Data;
        if (tmpdata.UPGUID == "") {
            TTreeNode* parent = this->TreeView->Items->Add(NULL, tmpdata.JdText);
            FindSon(i, parent);
        }
    }
}
//---------------------------------------------------------------------------

void TForm1::ReadData(TADOQuery *AdoQuery) {
    TData tmpdata;
    tmpdata.ID = AdoQuery->FieldByName("ID")->AsInteger;
    tmpdata.ISDELETE = AdoQuery->FieldByName("ISDELETE")->AsInteger;
    tmpdata.JdContentType = AdoQuery->FieldByName("JdContentType")->AsInteger;
    tmpdata.Index_ = AdoQuery->FieldByName("Index_")->AsInteger;
    tmpdata.PGUID = AdoQuery->FieldByName("PGUID")->AsString;
    tmpdata.UPGUID = AdoQuery->FieldByName("UPGUID")->AsString;
    tmpdata.S_UDTIME = AdoQuery->FieldByName("S_UDTIME")->AsString;
    tmpdata.SYNC_TIME = AdoQuery->FieldByName("SYNC_TIME")->AsString;
    tmpdata.JdText = AdoQuery->FieldByName("JdText")->AsString;
    tmpdata.JdType = AdoQuery->FieldByName("JdType")->AsString;
    tmpdata.DevName = AdoQuery->FieldByName("DevName")->AsString;
    tmpdata.IconSxName = AdoQuery->FieldByName("IconSxName")->AsString;
    tmpdata.MapGuid = AdoQuery->FieldByName("MapGuid")->AsString;
    tmpdata.DevID = AdoQuery->FieldByName("DevID")->AsString;
    node.Length ++;
    node[node.High].Data = tmpdata;
    node[node.High].Head = -1;
    node[node.High].fa = node.High;
}
//---------------------------------------------------------------------------

void TForm1::ReadData2(TADOQuery *AdoQuery) {
    TData2 tmpdata;
    tmpdata.ID = AdoQuery->FieldByName("ID")->AsInteger;
    tmpdata.ISDELETE = AdoQuery->FieldByName("ISDELETE")->AsInteger;
    tmpdata.OrgID = AdoQuery->FieldByName("OrgID")->AsInteger;
    tmpdata.ShowIndex = AdoQuery->FieldByName("ShowIndex")->AsInteger;
    tmpdata.PGUID = AdoQuery->FieldByName("PGUID")->AsString;
    tmpdata.UPGUID = AdoQuery->FieldByName("UPGUID")->AsString;
    tmpdata.S_UDTIME = AdoQuery->FieldByName("S_UDTIME")->AsString;
    tmpdata.SYNC_TIME = AdoQuery->FieldByName("SYNC_TIME")->AsString;
    tmpdata.TypeName = AdoQuery->FieldByName("TypeName")->AsString;
    tmpdata.MapID = AdoQuery->FieldByName("MapID")->AsString;
    tmpdata.MapName = AdoQuery->FieldByName("MapName")->AsString;
    //multimap<String, TData2>::iterator it;
    Find_Map.insert(pair<String, TData2>(tmpdata.UPGUID, tmpdata));
}
//---------------------------------------------------------------------------

void TForm1::AddEdge(int x, int y) {
    edge.Length++;
    edge[edge.High].to = y;
    edge[edge.High].next = node[x].Head;
    node[x].Head = edge.High;
    node[y].fa = x;
}
//---------------------------------------------------------------------------


void __fastcall TForm1::AdvStringGridClickCell(TObject *Sender, int ARow, int ACol) {
    if (ARow == 0 || ACol == 0)
        return;
    Panel->Visible = true;
    if (Map_PGUID.Length < ARow)
        return;
    TADOQuery *AdoQ = new TADOQuery(NULL);
    AdoQ->Connection = DMod->ADOConnection2;
    String sql = "select EmfMapData from JXMAP where PGUID = '" + Map_PGUID[ARow - 1] + "'";
    DMod->OpenSql(sql, AdoQ);
    //ShowMessage( Map_PGUID[ARow - 1]);
    if (AdoQ->Eof) {
        delete AdoQ;
        return;
    }
    if((TBlobField*)AdoQ->FieldByName("EmfMapData")->IsNull) {
        delete AdoQ;
        return;
    }
    TADOBlobStream* stream = new TADOBlobStream( (TBlobField*)AdoQ->FieldByName("EmfMapData"), bmRead );
    AnsiString s = ExtractFilePath( Application->ExeName ) + "1.emf";
    stream->SaveToFile(s);
    delete stream;
    Panel->Visible = false;
    s = ExtractFilePath( Application->ExeName ) + "1.emf";
    SigViewer1->OpenDrawingFile(++ num_of_pic , WideString(s), L"" );
    emf_Analysis();
    AdvStringGrid1ClickCell(AdvStringGrid1, 1, 2);
}
//---------------------------------------------------------------------------


int Check_Pen(long x, long y) {
    if (Form1->SigViewer1->Width - 200 <= x && x <= Form1->SigViewer1->Width - 162
        && 20 <= y && y <= 20 + 25)
        return 1;
    if (Form1->SigViewer1->Width - 150 <= x && x <= Form1->SigViewer1->Width - 112
        && 20 <= y && y <= 20 + 25)
        return 2;
    if (Form1->SigViewer1->Width - 100 <= x && x <= Form1->SigViewer1->Width - 63
        && 20 <= y && y <= 20 + 27)
        return 3;
    return 0;
}
//---------------------------------------------------------------------------

bool Check_Point(long x, long y) {
    set<pair<long, long> >::iterator it;
    it = Map_Node.find(make_pair(x, y));
    if (it == Map_Node.end())
        return 0;
    else
        return 1;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SigViewer1Paint(TObject *Sender, long DC) {
    HDC hdc = (HDC)DC;
    double zoom =(double)this->SigViewer1->RealZoom/10000;     //缩放系数
    if( zoom == 0)
    {
        zoom = 1;
    }
    String p_path = ExtractFilePath( Application->ExeName )+"picture\\";

    //画红表笔
    TPicture  *  r_pen0;  //红表笔
    long r_x0,r_y0;
    r_pen0 = new TPicture;
    r_pen0->LoadFromFile(p_path+"红表笔.bmp");
    int bmp_width = 0;
    int bmp_height = 0;
    Graphics::TBitmap *bm = r_pen0->Bitmap; //
    bmp_width = bm->Width;
    bmp_height = bm->Height;
    long x0 =  this->SigViewer1->Width - 200;
    SigViewer1->WindowToView(x0, 20, &r_x0, &r_y0);  //屏幕坐标转换成控件坐标
    int orgin[4]; //图片坐标与宽高
    orgin[0] = r_x0;
    orgin[1] = r_y0;
    orgin[2] = bmp_width/zoom;  //保持绘制的图片固定尺寸，不随图纸变大缩小
    orgin[3] = bmp_height/zoom;
    DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
    delete r_pen0;

    //画黑表笔
    TPicture  *  b_pen0;  //黑表笔
    b_pen0 = new  TPicture;
    b_pen0->LoadFromFile(p_path+"黑表笔.bmp");
    long b_x0,b_y0;
    bm = b_pen0->Bitmap;
    bmp_width = bm->Width;
    bmp_height = bm->Height;

    x0 =  this->SigViewer1->Width - 150;
    SigViewer1->WindowToView(x0, 20, &b_x0, &b_y0);
    //图片坐标与宽高
    orgin[0] = b_x0;
    orgin[1] = b_y0;
    orgin[2] = bmp_width/zoom;
    orgin[3] = bmp_height/zoom;
    DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
    delete b_pen0;

    //画电流钳
    TPicture  *c_pen0; //电流钳
    long c_x0,c_y0;
    c_pen0 = new TPicture;
    c_pen0->LoadFromFile(p_path+"电流钳.bmp");
    bm = c_pen0->Bitmap;
    bmp_width = bm->Width;
    bmp_height = bm->Height;
    x0 = this->SigViewer1->Width - 100;
    SigViewer1->WindowToView(x0, 20, &c_x0, &c_y0);
    //图片坐标与宽高
    orgin[0] = c_x0;
    orgin[1] = c_y0;
    orgin[2] = bmp_width/zoom;
    orgin[3] = bmp_height/zoom;
    DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
    delete c_pen0;

    //画矩形框
    this->SigViewer1->DcSetPenColor(DC,clRed) ;
    this->SigViewer1->DcSetPenWidth(DC,1) ;
    this->SigViewer1->DcMoveTo(DC,r_x0-5/zoom ,r_y0-5/zoom);
    this->SigViewer1->DcLineTo(DC,c_x0+bmp_width/zoom+5/zoom ,r_y0-5/zoom);
    this->SigViewer1->DcLineTo(DC,c_x0+bmp_width/zoom+5/zoom ,r_y0+ bmp_height/zoom+5/zoom);
    this->SigViewer1->DcLineTo(DC,r_x0-5/zoom ,r_y0+ bmp_height/zoom+5/zoom);
    this->SigViewer1->DcLineTo(DC,r_x0-5/zoom ,r_y0-5/zoom);

    //
    //this->SigViewer1->DcEllipse(DC,)

    //画移动表笔
    long tx = Coordinate.first, ty = Coordinate.second;
    if (ClickDown == 1) {
        TPicture  *movepen;
        if (using_red) {
            movepen = new TPicture;
            movepen->LoadFromFile(p_path+"红表笔.bmp");
            bm = movepen->Bitmap;
            bmp_width = bm->Width;
            bmp_height = bm->Height;
            bm->Canvas->Font->Color = clRed;
            bm->Canvas->TextOutA(dx[1] - 10, dy[1] - 22, IntToStr(using_red));
            bm->Canvas->Pen->Color = clGreen;
            bm->Canvas->Rectangle(1,1,3,3);
            bm->Canvas->Rectangle(1,12,3,14);
            bm->Canvas->Rectangle(1,23,3,25);
            bm->Canvas->Rectangle(18,1,20,3);
            bm->Canvas->Rectangle(36,1,38,3);
            bm->Canvas->Rectangle(18,23,20,25);
            bm->Canvas->Rectangle(36,12,38,14);
            bm->Canvas->Rectangle(36,23,38,25);
            SigViewer1->WindowToView(tx - dx[1], ty - dy[1], &r_x0, &r_y0);
            orgin[0] = r_x0;
            orgin[1] = r_y0;
            orgin[2] = bmp_width/zoom;
            orgin[3] = bmp_height/zoom;
            DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
            delete movepen;
        }
        else if(using_black) {
            movepen = new  TPicture;
            movepen->LoadFromFile(p_path+"黑表笔.bmp");
            bm = movepen->Bitmap;
            bmp_width = bm->Width;
            bmp_height = bm->Height;
            bm->Canvas->Font->Color = clBlack;
            bm->Canvas->TextOutA(dx[2], dy[2] - 22, IntToStr(using_black));
            bm->Canvas->Pen->Color = clGreen;
            bm->Canvas->Rectangle(1,1,3,3);
            bm->Canvas->Rectangle(1,12,3,14);
            bm->Canvas->Rectangle(1,23,3,25);
            bm->Canvas->Rectangle(18,1,20,3);
            bm->Canvas->Rectangle(36,1,38,3);
            bm->Canvas->Rectangle(18,23,20,25);
            bm->Canvas->Rectangle(36,12,38,14);
            bm->Canvas->Rectangle(36,23,38,25);
            SigViewer1->WindowToView(tx - dx[2], ty - dy[2], &b_x0, &b_y0);
            orgin[0] = b_x0;
            orgin[1] = b_y0;
            orgin[2] = bmp_width/zoom;
            orgin[3] = bmp_height/zoom;
            DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
            delete movepen;
        }
        else if (using_clamp){
            movepen = new TPicture;
            movepen->LoadFromFile(p_path+"电流钳.bmp");
            bm = movepen->Bitmap;
            bmp_width = bm->Width;
            bmp_height = bm->Height;
            bm->Canvas->Font->Color = clBlue;
            bm->Canvas->TextOutA(dx[3], dy[3] - 22, IntToStr(using_clamp));
            bm->Canvas->Pen->Color = clGreen;
            bm->Canvas->Rectangle(1,1,3,3);
            bm->Canvas->Rectangle(1,13,3,15);
            bm->Canvas->Rectangle(1,25,3,27);
            bm->Canvas->Rectangle(16,1,18,3);
            bm->Canvas->Rectangle(35,1,37,3);
            bm->Canvas->Rectangle(16,25,18,27);
            bm->Canvas->Rectangle(35,13,37,15);
            bm->Canvas->Rectangle(35,25,37,27);
            SigViewer1->WindowToView(tx - dx[3], ty - dy[3], &c_x0, &c_y0);
            orgin[0] = c_x0;
            orgin[1] = c_y0;
            orgin[2] = bmp_width/zoom;
            orgin[3] = bmp_height/zoom;
            DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
            delete movepen;
        }

    }

    //画选中表笔
    set<Pen>::iterator it;

    for (it = Pen_Node.begin(); it != Pen_Node.end(); ++ it) {
        TPicture  *nodepen;
        tx = it->x, ty = it->y;
        if (it->type == 1) {
            nodepen = new TPicture;
            nodepen->LoadFromFile(p_path+"红表笔.bmp");
            bm = nodepen->Bitmap;
            bmp_width = bm->Width;
            bmp_height = bm->Height;
            bm->Canvas->Font->Color = clRed;
            bm->Canvas->TextOutA(dx[1] - 10, dy[1] - 22, IntToStr(it->id));
            //SigViewer1->WindowToView(tx - dx[1], ty - dy[1], &r_x0, &r_y0);
            orgin[0] = tx - dx[1] / zoom;
            orgin[1] = ty - dy[1] / zoom;
            orgin[2] = bmp_width/zoom;  //保持绘制的图片固定尺寸，不随图纸变大缩小
            orgin[3] = bmp_height/zoom;
            DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
            this->SigViewer1->DcSetPenColor(DC,clRed);
            this->SigViewer1->DcEllipse(DC, tx - 3 / zoom, ty - 3 / zoom, tx + 3 / zoom, ty + 3 / zoom);
        }
        else if(it->type == 2) {
            nodepen = new  TPicture;
            nodepen->LoadFromFile(p_path+"黑表笔.bmp");
            bm = nodepen->Bitmap;
            bmp_width = bm->Width;
            bmp_height = bm->Height;
            bm->Canvas->Font->Color = clBlack;
            bm->Canvas->TextOutA(dx[2], dy[2] - 22, IntToStr(it->id));
            //SigViewer1->WindowToView(tx - dx[2], ty - dy[2], &b_x0, &b_y0);
            orgin[0] = tx - dx[2] / zoom;
            orgin[1] = ty - dy[2] / zoom;
            orgin[2] = bmp_width/zoom;  //保持绘制的图片固定尺寸，不随图纸变大缩小
            orgin[3] = bmp_height/zoom;
            DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
            this->SigViewer1->DcSetPenColor(DC,clBlack);
            this->SigViewer1->DcEllipse(DC, tx - 3 / zoom, ty - 3 / zoom, tx + 3 / zoom, ty + 3 / zoom);
        }
        else {
            nodepen = new TPicture;
            nodepen->LoadFromFile(p_path+"电流钳.bmp");
            bm = nodepen->Bitmap;
            bmp_width = bm->Width;
            bmp_height = bm->Height;
            bm->Canvas->Font->Color = clBlue;
            bm->Canvas->TextOutA(dx[3], dy[3] - 22, IntToStr(it->id));
            //SigViewer1->WindowToView(tx - dx[3], ty - dy[3], &c_x0, &c_y0);
            orgin[0] = tx - dx[3] / zoom;
            orgin[1] = ty - dy[3] / zoom;
            orgin[2] = bmp_width/zoom;  //保持绘制的图片固定尺寸，不随图纸变大缩小
            orgin[3] = bmp_height/zoom;
            DrawPicture(hdc, orgin, bm->Canvas->Handle, 0, 0, bmp_width, bmp_height, RGB(255, 255, 255));
            this->SigViewer1->DcSetPenColor(DC,clYellow);
            this->SigViewer1->DcEllipse(DC, tx - 3 / zoom, ty - 3 / zoom, tx + 3 / zoom, ty + 3 / zoom);
        }
        delete nodepen;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SigViewer1MouseDown(TObject *Sender,
      Sigviewerlib_tlb::TxMouseButton Button, long X, long Y, long *Cancel) {
// 鼠标按下
    set<pair<long, long> >::iterator iter;
    long tx0, ty0;
    SigViewer1->WindowToView(X, Y, &tx0, &ty0);
    for (iter = Map_Node.begin(); iter != Map_Node.end(); ++ iter) {
        int tx = iter->first, ty = iter->second;
        if ((tx - tx0) * (tx - tx0) + (ty - ty0) * (ty - ty0) <= 36) {
            tx0 = tx, ty0 = ty;
            break;
        }
    }
    if (Button == 0) {
        if (ClickDown == 0) {
            int pos = Check_Pen(X, Y);
            vector<set<Pen>::iterator> iter_List;
            set<Pen>::iterator it;
            switch(pos) {
                case 0:
                    for (it = Pen_Node.begin(); it != Pen_Node.end(); ++ it) {
                        if (it->x == tx0 && it->y == ty0)
                            iter_List.push_back(it);
                    }
                    if (iter_List.size() >= 2) {
                        //ShowMessage("请选择表笔移动");
                        // balabala...
                        Choose_Move_Pen(iter_List);
                    }
                    else if (iter_List.size() == 1){
                        it = iter_List[0];
                        if (it->company != Department[0]) {
							ShowMessage("无法移动上级公司添加的表笔");
							break;
						}
                        ClickDown = 1;
                        if (it->type == 1)
                            using_red = it->id;
                        if (it->type == 2)
                            using_black = it->id;
                        if (it->type == 3)
                            using_clamp = it->id;
                        Partner = it->partner;
                        DB_DeletePen(it->type, it->id);
                        Pen_Node.erase(it);
                    }
                    break;
                case 1:
                    if (used_red_pen > used_black_pen) {
                        ShowMessage("尚有未配对的红表笔");
                        break;
                    }
                    else {
                        ClickDown = 1;
                        ++ used_red_pen;
                        using_red = used_red_pen;
                        Partner = 0;
                    }
                    break;
                case 2:
                    if (used_black_pen > used_red_pen) {
                        ShowMessage("尚有未配对的黑表笔");
                        break;
                    }
                    else {
                        ClickDown = 1;
                        ++ used_black_pen;
                        using_black = used_black_pen;
                        Partner = 0;
                    }
                    break;
                case 3:
                    ClickDown = 1;
                    ++ used_clamp_pen;
                    using_clamp = used_clamp_pen;
                    Partner = 0;
                    break;
            }
        }
        else if (Check_Point(tx0, ty0)){
			int tmp_company;
            ClickDown = 0;
            Pen tmppen;
            set<Pen>::iterator it;
            tmppen.x = tx0, tmppen.y = ty0;
            if (using_red) {
                tmppen.id = using_red;
                using_red = 0;
                tmppen.type = 1;
                tmppen.partner = 0;
                tmppen.company = Department[0];
                for (it = Pen_Node.begin(); it != Pen_Node.end(); ++ it)
                    if (it->id == tmppen.id && it->type == 2) {
                        tmppen.partner = tmppen.id;
                        tx0 = it->x, ty0 = it->y;
						tmp_company = it->company;
                        break;
                    }
                if (it != Pen_Node.end()) {
                    DB_DeletePen(it->type, it->id);
                    Pen_Node.erase(it);
					DB_AddPen(tmppen);
                    Pen_Node.insert(tmppen);
                    tmppen.x = tx0;
                    tmppen.y = ty0;
                    tmppen.type = 2;
					tmppen.company = tmp_company;
					DB_AddPen(tmppen);
                    Pen_Node.insert(tmppen);
                }
                else {
					DB_AddPen(tmppen);
                    Pen_Node.insert(tmppen);
                }
            }
            else if (using_black) {
                tmppen.id = using_black;
                using_black = 0;
                tmppen.type = 2;
                tmppen.partner = 0;
				tmppen.company = Department[0];
                for (it = Pen_Node.begin(); it != Pen_Node.end(); ++ it)
                    if (it->id == tmppen.id && it->type == 1) {
                        tmppen.partner = tmppen.id;
                        tx0 = it->x, ty0 = it->y;
						tmp_company = it->company;
                        break;
                    }
                if (it != Pen_Node.end()) {
					DB_DeletePen(it->type, it->id);
                    Pen_Node.erase(it);
					DB_AddPen(tmppen);
                    Pen_Node.insert(tmppen);
                    tmppen.x = tx0;
                    tmppen.y = ty0;
                    tmppen.type = 1;
					tmppen.company = tmp_company;
					DB_AddPen(tmppen);
                    Pen_Node.insert(tmppen);
                }
                else {
					DB_AddPen(tmppen);
                    Pen_Node.insert(tmppen);
                }
            }
            else if (using_clamp) {
                tmppen.id = using_clamp;
                using_clamp = 0;
                tmppen.type = 3;
                tmppen.partner = 0;
				tmppen.company = Department[0];
				DB_AddPen(tmppen);
                Pen_Node.insert(tmppen);
            }
        }
    }
    else {
        if (ClickDown == 0) {
            vector<set<Pen>::iterator> iter_List;
            set<Pen>::iterator it;
            Pen tmppen;
            for (it = Pen_Node.begin(); it != Pen_Node.end(); ++ it) {
                if (it->x == tx0 && it->y == ty0)
                    iter_List.push_back(it);
            }
            if (iter_List.size() >= 2) {
                //ShowMessage("请选择表笔删除");
                // balabala...
                Choose_Delete_Pen(iter_List);
            }
            else if (iter_List.size() == 1){
				if (iter_List[0]->company != Department[0]) {
					ShowMessage("无法移动上级公司添加的表笔");
					return;
				}
                if (iter_List[0]->partner == 0) {
                    if (iter_List[0]->type == 1)
                        -- used_red_pen;
                    if (iter_List[0]->type == 2)
                        -- used_black_pen;
                    DB_DeletePen(iter_List[0]->type, iter_List[0]->id);
                    Pen_Node.erase(iter_List[0]);
                }
                else {
                    int PTR = iter_List[0]->partner;
                    for (it = Pen_Node.begin(); it != Pen_Node.end(); ++ it)
                        if (it->partner == PTR && it->type != 3) {
							if (it->company == Department[0]) {
                                if (it->type == 1)
                                    -- used_red_pen;
                                if (it->type == 2)
                                    -- used_black_pen;
								DB_DeletePen(it->type, it->id);
								Pen_Node.erase(it);
							}
							else {
                                if (it->type == 1)
                                    -- used_red_pen;
                                if (it->type == 2)
                                    -- used_black_pen;
								tmppen.id = it->id;
                                tmppen.type = it->type;
                                tmppen.partner = 0;
                                tmppen.company = it->company;
                                tmppen.x = it->x;
                                tmppen.y = it->y;
								DB_DeletePen(it->type, it->id);
								Pen_Node.erase(it);
								DB_AddPen(tmppen);
								Pen_Node.insert(tmppen);
							}
                        }
                }
            }
        }
        else {
            if (Partner) {
                ClickDown = 0;
                using_red = using_black = using_clamp = 0;
                set<Pen>::iterator it;
                Pen tmppen;
                for (it = Pen_Node.begin(); it != Pen_Node.end(); ++ it)
                    if (it->partner == Partner) {
                        Partner = 0;
                        if (it->type == 1)
                            -- used_red_pen;
                        if (it->type == 2)
                            -- used_black_pen;
                        DB_DeletePen(it->type, it->id);
                        Pen_Node.erase(it);
                        break;
                    }

            }
            else {
                ClickDown = 0;
                if (using_red) {
                    if (using_red == used_red_pen)
                        using_red = 0, -- used_red_pen;
                    else
                        using_red = 0;
                }
                if (using_black) {
                    if (using_black == used_black_pen)
                        using_black = 0, -- used_black_pen;
                    else
                        using_black = 0;
                }
                if (using_clamp) {
                    if (using_clamp == used_clamp_pen)
                        using_clamp = 0, -- used_clamp_pen;
                    else
                        using_clamp = 0;
                }
            }
        }
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SigViewer1MouseMove(TObject *Sender, long X, long Y, long *Cancel) {
// 鼠标移动
    //*Cancel = 1;
    Coordinate.first = X, Coordinate.second = Y;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::SigViewer1MouseUp(TObject *Sender,
      Sigviewerlib_tlb::TxMouseButton Button, long X, long Y, long *Cancel) {
// 鼠标抬起
}
//---------------------------------------------------------------------------

void __fastcall TForm1::TimerTimer(TObject *Sender) {
    ::InvalidateRect((HWND)this->SigViewer1->DrawingWindow, NULL, TRUE);
}
//---------------------------------------------------------------------------

void TForm1::DrawPicture(HDC hdcDest, int Orign[4], HDC hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc,int nHeightSrc, UINT crTransparent) {
    int nXOriginDest,nYOriginDest, nWidthDest, nHeightDest;
    nXOriginDest =  Orign[0];
    nYOriginDest = Orign[1];
    nWidthDest = Orign[2];
    nHeightDest = Orign[3];

    HBITMAP hOldImageBMP, hImageBMP = CreateCompatibleBitmap(hdcDest, nWidthDest, nHeightDest);	// 创建兼容位图
	HBITMAP hOldMaskBMP, hMaskBMP = CreateBitmap(nWidthDest, nHeightDest, 1, 1, NULL);			// 创建单色掩码位图
	HDC		hImageDC = CreateCompatibleDC(hdcDest);
	HDC		hMaskDC = CreateCompatibleDC(hdcDest);
	hOldImageBMP = (HBITMAP)SelectObject(hImageDC, hImageBMP);
	hOldMaskBMP = (HBITMAP)SelectObject(hMaskDC, hMaskBMP);

	// 将源DC中的位图拷贝到临时DC中
	if (nWidthDest == nWidthSrc && nHeightDest == nHeightSrc)
		BitBlt(hImageDC, 0, 0, nWidthDest, nHeightDest, hdcSrc, nXOriginSrc, nYOriginSrc, SRCCOPY);
	else
		StretchBlt(hImageDC, 0, 0, nWidthDest, nHeightDest,
		hdcSrc, nXOriginSrc, nYOriginSrc, nWidthSrc, nHeightSrc, SRCCOPY);

	// 设置透明色
	SetBkColor(hImageDC, crTransparent);

	// 生成透明区域为白色，其它区域为黑色的掩码位图
	BitBlt(hMaskDC, 0, 0, nWidthDest, nHeightDest, hImageDC, 0, 0, SRCCOPY);

	// 生成透明区域为黑色，其它区域保持不变的位图
	SetBkColor(hImageDC, RGB(0,0,0));
	SetTextColor(hImageDC, RGB(255,255,255));
	BitBlt(hImageDC, 0, 0, nWidthDest, nHeightDest, hMaskDC, 0, 0, SRCAND);

	// 透明部分保持屏幕不变，其它部分变成黑色
	SetBkColor(hdcDest,RGB(0xff,0xff,0xff));
	SetTextColor(hdcDest,RGB(0,0,0));
	BitBlt(hdcDest, nXOriginDest, nYOriginDest, nWidthDest, nHeightDest, hMaskDC, 0, 0, SRCAND);

	// "或"运算,生成最终效果
	BitBlt(hdcDest, nXOriginDest, nYOriginDest, nWidthDest, nHeightDest, hImageDC, 0, 0, SRCPAINT);

	SelectObject(hImageDC, hOldImageBMP);
	DeleteDC(hImageDC);
	SelectObject(hMaskDC, hOldMaskBMP);
	DeleteDC(hMaskDC);
	DeleteObject(hImageBMP);
	DeleteObject(hMaskBMP);
}
//---------------------------------------------------------------------------

void TForm1::emf_Analysis() {
    // 初始化
    Map_Node.clear();
    Pen_Node.clear();
    ClickDown = 0;
    using_red = using_black = using_clamp = 0;
    used_red_pen = used_black_pen = used_clamp_pen = 0;

    String file = "1.emf";
    TMemoryStream *stream = new TMemoryStream();
    stream->LoadFromFile( file );
    stream->Position = 0;
    int pos = 0;
    stream->Seek(-4,soFromEnd);
    stream->Read( &pos, 4 );
    int HeadBeginPos = pos - 1;
    stream->Seek( HeadBeginPos, soFromBeginning );

    char header[2];
    stream->Read( header, 2 );

    int LinkPointPos = HeadBeginPos - 4;
    stream->Seek( LinkPointPos, soFromBeginning );
    stream->Read( &LinkPointPos, 4 );
    stream->Seek( LinkPointPos - 1, soFromBeginning );

    ExplainLinkPoint( stream );
}
//---------------------------------------------------------------------------

void TForm1::ExplainLinkPoint( TStream* pStream ) {
    char flag[2];
    pStream->Read( flag, 2 );
    if( flag[0] == 'C' && flag[1] == 'N' )
    {   //版本
        int m_ver = 0;
        pStream->Read( &m_ver, 4 );
        if( m_ver == 0xFFFF0000 + 1 || m_ver == 0xFFFF0000 + 2 )
        {
            //图纸ID
            int FDrawingID = 0;
            pStream->Read( &FDrawingID, 4 );
            //LogOut("图纸ID:" + AnsiString(FDrawingID) );
            //  RECT
            TRect rect;
            pStream->Read( &rect.left, 4 );
            pStream->Read( &rect.top, 4 );
            pStream->Read( &rect.right, 4 );
            pStream->Read( &rect.bottom, 4 );
            //LogOut( "RECT:" + AnsiString( rect.Left ) + " " +  AnsiString( rect.Top )+ " " + AnsiString( rect.Right )+ " " + AnsiString( rect.Bottom ) );
            //数量
            int count = 0;
            pStream->Read( &count, 4 );
            //LogOut("数量:" + AnsiString(count) );
            //
            for( int i = 0; i < count; i++ )
            {
                //LogOut("------------------------------------------------");
                //id
                int id = 0;
                pStream->Read( &id, 4 );
                //LogOut("  所属图符id:" + AnsiString(id) );
                //group
                int group = 0;
                pStream->Read( &group, 4 );
                //LogOut("  分组:" + AnsiString(group) );
                //guid
                short guid_length = 0;
                pStream->Read( &guid_length, 2 );
                //LogOut("  GUID长度:" + AnsiString(guid_length) );
                //guid
                AnsiString m_guid = "";
                m_guid.SetLength( guid_length );
                pStream->Read( m_guid.c_str(), guid_length );
                //LogOut("  GUID:" + AnsiString(guid_length) );
                //conncount
                int concount = 0;
                pStream->Read( &concount, 4 );
                //LogOut("  数据长度(个数*4):" + AnsiString(concount) );
                for (int m = 0; m < concount; m+=4)
                {
                    int m_index = 0;
                    pStream->Read( &m_index, 4 );
                    int type = 0;
                    pStream->Read( &type, 4 );

                    int x = 0;
                    pStream->Read( &x, 4 );

                    int y = 0;
                    pStream->Read( &y, 4 );
                    if (m_index < 0)
                        continue;
                    Map_Node.insert(make_pair(x, y));
                    /*LogOut("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                    LogOut("  m_index:" + AnsiString(m_index) );
                    LogOut("  type:" + AnsiString(type) );
                    LogOut("  x:" + AnsiString(x) );
                    LogOut("  y:" + AnsiString(y) );
                    */
                   // m_index >= 0 表示连接点
                }
            }     
        }
    }
}
//---------------------------------------------------------------------------

void TForm1::Choose_Move_Pen(const vector<set<Pen>::iterator> &L) {
    ExtraForm->Department = Department;
    ExtraForm->Caption = "请选择所要移动的表笔";
    ExtraForm->Move_Pen(L);
    ExtraForm->ShowModal();
    set<Pen>::iterator it;
    //ShowMessage(ExtraForm->Move_num);
    if (ExtraForm->Move_num == -1)
        return;
    it = L[ExtraForm->Move_num];
    ClickDown = 1;
    if (it->type == 1)
        using_red = it->id;
    if (it->type == 2)
        using_black = it->id;
    if (it->type == 3)
        using_clamp = it->id;
    Partner = it->partner;
    DB_DeletePen(it->type, it->id);
    Pen_Node.erase(it);
}
//---------------------------------------------------------------------------

void TForm1::Choose_Delete_Pen(const vector<set<Pen>::iterator> &L) {
	ExtraForm->Department = Department;
    ExtraForm->Caption = "请选择所要删除的表笔";
    ExtraForm->Delete_Pen(L);
    ExtraForm->ShowModal();
    set<Pen>::iterator it;
    //ShowMessage(ExtraForm->Delete_num.size());
    vector<int> v = ExtraForm->Delete_num;
    for (int i = 0; i < v.size(); ++ i) {
        int pos = v[i];
        if (L[pos]->partner == 0) {
            if (L[pos]->type == 1)
                -- used_red_pen;
            if (L[pos]->type == 2)
                -- used_black_pen;
            DB_DeletePen(L[pos]->type, L[pos]->id);
            Pen_Node.erase(L[pos]);
        }
        else {
            int PTR = L[pos]->partner;
            for (it = Pen_Node.begin(); it != Pen_Node.end();) {
                if (it->partner == PTR && it->type != 3) {
                    DB_DeletePen(it->type, it->id);
                    Pen_Node.erase(it++);
                }
                else
                    ++ it;
            }
        }
    }
}
//---------------------------------------------------------------------------

void TForm1::DB_AddPen(const Pen &u_pen) {
	CoInitialize(NULL);
    String pguid = newGUID();
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String sql = "insert into ZSK_MAP_H0000Z000K06 (PGUID, S_UDTIME, UPGUID1, UPGUID2, PEN_TYPE, PEN_ID, PEN_X, PEN_Y, PEN_PARTNER, DEPARTMENT) values('"+ pguid + "', '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "', '" + AdvStringGrid->Cells[3][AdvStringGrid->Row] + "', '" + AdvStringGrid1->Cells[4][AdvStringGrid1->Row] + "', "
                 + IntToStr(u_pen.type) + ", " + IntToStr(u_pen.id) + ", " + IntToStr(u_pen.x) + ", " + IntToStr(u_pen.y) + ", " + IntToStr(u_pen.partner) + ", " + IntToStr(Department[0]) + ")";
    DMod->ExecSql(sql, tempQuery);
}
//---------------------------------------------------------------------------

void TForm1::DB_DeletePen(int type, int id) {
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String sql = "update ZSK_MAP_H0000Z000K06 set ISDELETE = 1 where UPGUID1 = '" + AdvStringGrid->Cells[3][AdvStringGrid->Row] + "' and UPGUID2 = '" + AdvStringGrid1->Cells[4][AdvStringGrid1->Row] + "' and PEN_TYPE = " + IntToStr(type) + " and PEN_ID = " + IntToStr(id);
    DMod->ExecSql(sql + sql_Dep, tempQuery);
}
//---------------------------------------------------------------------------


void __fastcall TForm1::AdvStringGrid1ClickCell(TObject *Sender, int ARow,
      int ACol) {
    if (ARow == 0 || ACol == 0)
        return;

    AdvStringGrid2Refresh();
    String up1, up2;
    // 读取这张图数据
    Pen_Node.clear();
    used_red_pen = used_black_pen = used_clamp_pen = 0;
    using_red = using_black = using_clamp = 0;
    Partner = 0;
    ClickDown = 0;
    int row = AdvStringGrid->Row;
    up1 = AdvStringGrid->Cells[3][row];
    up2 = AdvStringGrid1->Cells[4][ARow];
    TADOQuery *AdoQ = new TADOQuery(NULL);
    String sql = "select * from ZSK_MAP_H0000Z000K06 where UPGUID1 = '" + up1 + "' and UPGUID2 = '" + up2 + "' and ISDELETE = 0";
    AdoQ->Connection = DMod->ADOConnection3;
    DMod->OpenSql(sql + sql_Dep, AdoQ);
    while (!AdoQ->Eof) {
        Pen tmppen;
        tmppen.id = AdoQ->FieldByName("PEN_ID")->AsInteger;
        tmppen.type = AdoQ->FieldByName("PEN_TYPE")->AsInteger;
        tmppen.partner = AdoQ->FieldByName("PEN_PARTNER")->AsInteger;
        tmppen.company = AdoQ->FieldByName("DEPARTMENT")->AsInteger;
        tmppen.x = AdoQ->FieldByName("PEN_X")->AsInteger;
        tmppen.y = AdoQ->FieldByName("PEN_Y")->AsInteger;
        if (tmppen.partner != 0)
            Partner ^= tmppen.id;
        if (tmppen.type == 1)
            used_red_pen = max(used_red_pen, tmppen.id);
        if (tmppen.type == 2)
            used_black_pen = max(used_black_pen, tmppen.id);
        if (tmppen.type == 3)
            used_clamp_pen = max(used_clamp_pen, tmppen.id);
		Pen_Node.insert(tmppen);
        AdoQ->Next();
    }
    delete AdoQ;
    if (Partner) {
        set<Pen>::iterator it;
        for (it = Pen_Node.begin(); it != Pen_Node.end(); ++ it) {
            if (it->id == Partner) {
                if (it->type == 1)
                    using_black = Partner;
                if (it->type == 2)
                    using_red = Partner;
                break;
            }
        }
        ClickDown = 1;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1EditCellDone(TObject *Sender,
      int ACol, int ARow) {
    // 退出触发
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1CellValidate(TObject *Sender,
      int ACol, int ARow, AnsiString &Value, bool &Valid) {
    /*SaveLog(Application->ExeName + "Delete.log", "1");
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    if (ARow == AdvStringGrid1->RowCount - 1) {
        if (Value == "")
            return;
        CoInitialize(NULL);
        String pguid = newGUID();
        // find
		String sql = "select * from ZSK_NATURE_H0000Z000K06 where NATURE = '" + Value + "' and UPGUID = '" + node[Now_Node].Data.PGUID + "' and ISDELETE = 0";
        DMod->OpenSql(sql + sql_Dep, tempQuery);
        if (!tempQuery->Eof) {
            delete tempQuery;
            Valid = false;
            ShowMessage(Value + "已经被添加进配线名称");
            Value = "";
            return;
        }
        sql = "insert into ZSK_NATURE_H0000Z000K06 (PGUID, S_UDTIME, UPGUID, NATURE, DEPARTMENT) values('" + pguid + "', '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "', '" + node[Now_Node].Data.PGUID + "', '" + Value + "', '" + IntToStr(Department[0]) + "')";
        DMod->ExecSql(sql, tempQuery);
        AdvStringGrid1->AddRow();
        AdvStringGrid1->Cells[0][ARow] = ARow;
        AdvStringGrid1->Cells[1][ARow + 1] = node[Now_Node].Data.JdText;
        AdvStringGrid1->Cells[3][ARow] = pguid;
		AdvStringGrid1->Cells[4][ARow] = Department[0];
    }
    else {
        SaveLog(Application->ExeName + "Delete.log", "2");
        if (Value == "") {
            //  delete
			SaveLog(Application->ExeName + "Delete.log", "3");
            String sql = "update ZSK_NATURE_H0000Z000K06 set ISDELETE = 1 where NATURE = '" + Now_Nature + "' and UPGUID = '" + node[Now_Node].Data.PGUID + "'";
            //ShowMessage(sql);

            DMod->ExecSql(sql + sql_Dep, tempQuery);
            SaveLog(Application->ExeName + "Delete.log", sql + sql_Dep);
            Value = AdvStringGrid1->Cells[ACol][ARow + 1];
            AdvStringGrid1->RemoveRows(ARow, 1);
            for (int i = 1; i < AdvStringGrid1->RowCount - 1; ++ i)
                AdvStringGrid1->Cells[0][i] = i;
            SaveLog(Application->ExeName + "Delete.log", "4");
        }
        else {
            // find
            String sql = "select * from ZSK_NATURE_H0000Z000K06 where NATURE = '" + Value + "' and UPGUID = '" + node[Now_Node].Data.PGUID + "' and ISDELETE = 0";
            DMod->OpenSql(sql + sql_Dep, tempQuery);
            if (!tempQuery->Eof) {
                delete tempQuery;
                Valid = false;
                ShowMessage(Value + "已经被添加进配线名称");
                Value = "";
                return;
            }
            sql = "update ZSK_NATURE_H0000Z000K06 set NATURE = '" + Value + "', S_UDTIME = '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "' where NATURE = '" + Now_Nature + "' and UPGUID = '" + node[Now_Node].Data.PGUID + "' and ISDELETE = 0";
            DMod->ExecSql(sql + sql_Dep, tempQuery);
        }
    }
    delete tempQuery;*/
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1CanEditCell(TObject *Sender,
      int ARow, int ACol, bool &CanEdit) {
    // 编辑前可触发
    /*if (ACol == 1 || ACol == 0) {
        CanEdit = false;
        return;
    }
    if (AdvStringGrid1->Cells[1][ARow] != node[Now_Node].Data.JdText) {
        CanEdit = false;
        return;
    }
	if (AdvStringGrid1->Cells[4][ARow] != IntToStr(Department[0]) && AdvStringGrid1->Cells[2][ARow] != "") {
		CanEdit = false;
		return;
	}
	
    Now_Nature= AdvStringGrid1->Cells[2][ARow]; */
    CanEdit = false;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1RowMove(TObject *Sender, int ARow,
      bool &Allow) {
    if (AdvStringGrid1->Cells[1][ARow] != node[Now_Node].Data.JdText)
        Allow = 0;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1RowMoved(TObject *Sender,
      int FromIndex, int ToIndex) {
    TADOQuery *tempQuery;
    for (int i = 1; i < AdvStringGrid1->RowCount; ++ i) {
        tempQuery = new TADOQuery(NULL);
        tempQuery->Connection = DMod->ADOConnection3;
        String sql = "update ZSK_NATURE_H0000Z000K06 set Index_ = " + IntToStr(i) + " where PGUID = '" + AdvStringGrid1->Cells[4][i] + "' and UPGUID = '" + node[Now_Node].Data.PGUID + "' and ISDELETE = 0";
        //ShowMessage(sql);
        DMod-> ExecSql(sql, tempQuery);
        AdvStringGrid1->Cells[0][i] = i;
        delete tempQuery;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1RowMoving(TObject *Sender, int ARow,
      bool &Allow) {
    if (ARow < 1 || ARow > AdvStringGrid1->RowCount - 1)
        return;
    if (AdvStringGrid1->Cells[1][ARow] == node[Now_Node].Data.JdText && AdvStringGrid1->Cells[2][ARow] != "")
        Allow = 1;
    else
        Allow = 0;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1DblClickCell(TObject *Sender,
      int ARow, int ACol)
{
    /*if (ACol != 2)
        return;
    if (AdvStringGrid1->Cells[4][ARow] == "")
        return;
    int u_id = StrToInt(AdvStringGrid1->Cells[4][ARow]);
    if (u_id != Department[0] && AdvStringGrid1->Cells[2][ARow] != "") {
		ShowMessage("无法修改上级" + Department_Name[u_id] + "的数据");
		return;
	} */
}
//---------------------------------------------------------------------------


void __fastcall TForm1::N1Click(TObject *Sender) {
	Form2->sql_Dep = sql_Dep;
    Form2->Department = IntToStr(Department[0]);
    Form2->Department_Name = Department_Name;
    Form2->Refresh();
    Form2->AdvStringGrid1SelectCell(Form2->AdvStringGrid1, 1, 1, true);
    Form2->ShowModal();

    AdvStringGrid1ClickCell(AdvStringGrid1, AdvStringGrid1->Row, AdvStringGrid1->Col);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid2CanEditCell(TObject *Sender,
      int ARow, int ACol, bool &CanEdit) {
    CanEdit = 1;
    if (AdvStringGrid1->Cells[3][AdvStringGrid1->Row] == "") {
        CanEdit = 0;
        return;
    }
    if (AdvStringGrid2->Cells[5][ARow] != IntToStr(Department[0]) && AdvStringGrid2->Cells[4][ARow] != "") {
        CanEdit = 0;
        return;
    }
    if (ACol == 0 || ACol == 1) {
        CanEdit = 0;
        return;
    }
    if (ARow == 0) {
        CanEdit = 0;
        return;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid2GetEditorType(TObject *Sender,
      int ACol, int ARow, TEditorType &AEditor) {
    if (ACol == 2) {
        String datatype;
        datatype = Data_Type[AdvStringGrid2->Cells[3][ARow]];
        //ShowMessage(datatype);
        if (datatype == "文本")
            AEditor = edNormal;
        if (datatype == "数字")
            AEditor = edNumeric;
        if (datatype == "日期")
            AEditor = edDateEditUpDown;
        if (datatype == "可选项") {
            TADOQuery * tempQuery = new TADOQuery(NULL);
            tempQuery->Connection = DMod->ADOConnection3;
            String sql = "select PROPVALUE from ZSK_LIMIT_H0000Z000K06 where UPGUID = '" + AdvStringGrid2->Cells[3][ARow] + "' and ISDELETE = 0";
            DMod->OpenSql(sql, tempQuery);
            if (tempQuery->FieldByName("PROPVALUE")->AsString == "否") {
                AEditor = edComboList;
                String str;
                TADOQuery * AdoQ = new TADOQuery(NULL);
                AdoQ->Connection = DMod->ADOConnection3;
                sql = "select COMBOSTR from ZSK_COMBOSTRLIST_H0000Z000K06 where UPGUID = '" + AdvStringGrid2->Cells[3][ARow] + "' and ISDELETE = 0";
                DMod->OpenSql(sql, AdoQ);
                while (!AdoQ->Eof) {
                    str += "," + AdoQ->FieldByName("COMBOSTR")->AsString;
                    AdoQ->Next();
                }
                this->AdvStringGrid2->Combobox->Items->CommaText = str;
                delete AdoQ;
            }
            else if (tempQuery->FieldByName("PROPVALUE")->AsString == "是") {
                String str;
                AEditor = edCustom;
                this->AdvStringGrid2->EditLink = edCheckListEdit;
                TADOQuery * AdoQ = new TADOQuery(NULL);
                AdoQ->Connection = DMod->ADOConnection3;
                sql = "select COMBOSTR from ZSK_COMBOSTRLIST_H0000Z000K06 where UPGUID = '" + AdvStringGrid2->Cells[3][ARow] + "' and ISDELETE = 0";
                DMod->OpenSql(sql, AdoQ);
                if (!AdoQ->Eof) {
                    str += AdoQ->FieldByName("COMBOSTR")->AsString;
                    AdoQ->Next();
                }
                while (!AdoQ->Eof) {
                    str += "," + AdoQ->FieldByName("COMBOSTR")->AsString;
                    AdoQ->Next();
                }
                delete AdoQ;
                edCheckListEdit->CommaText = str;         
            }
            delete tempQuery;
        }
        if (datatype == "时间")
            AEditor = edTimeEdit;
        if (datatype == "时间段")
            AEditor = edNormal;
        if (datatype == "日期+时间")
            AEditor = edNormal;
        if (datatype == "链接")
            AEditor = edNormal;
        if (datatype == "文件")
            AEditor = edNormal;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid2EditCellDone(TObject *Sender,
      int ACol, int ARow) {
    int row1 = AdvStringGrid1->Row, row2 = AdvStringGrid2->Row;
    String sql, up1, up2;
    up1 = AdvStringGrid1->Cells[4][row1];
    up2 = AdvStringGrid2->Cells[3][row2];
    if (up1 == "" || up2 == "") {
        ShowMessage("Error");
        return;
    }
    TADOQuery *AdoQ = new TADOQuery(NULL);
    sql = "select DATA from ZSK_DATA_H0000Z000K06 where UPGUID1 = '" + up1 + "' and UPGUID2 = '" + up2 + "' and ISDELETE = 0";
    AdoQ->Connection = DMod->ADOConnection3;
    DMod->OpenSql(sql + sql_Dep, AdoQ);

    if (!AdoQ->Eof) {
        sql = "update ZSK_DATA_H0000Z000K06 set DATA = '" + AdvStringGrid2->Cells[2][row2] + "', S_UDTIME = '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "' where UPGUID1 = '" + up1 + "' and UPGUID2 = '" + up2 + "' and ISDELETE = 0";
        DMod->ExecSql(sql + sql_Dep, AdoQ);
    }
    else {
        CoInitialize(NULL);
        String pguid = newGUID();
        sql = "insert into ZSK_DATA_H0000Z000K06 (PGUID, S_UDTIME, UPGUID1, UPGUID2, DATA, DEPARTMENT) values('" + pguid + "', '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "', '" + up1 + "', '" + up2 + "', '" + AdvStringGrid2->Cells[2][row2] + "', " + IntToStr(Department[0]) +")";
        DMod->ExecSql(sql, AdoQ);
    }
    delete AdoQ;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid2ComboCloseUp(TObject *Sender,
      int ARow, int ACol) {
    this->AdvStringGrid2->Combobox->Visible=false;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid2MouseMove(TObject *Sender,
      TShiftState Shift, int X, int Y) {
    int row, col;
    AdvStringGrid2->MouseToCell(X, Y, col, row);
    if (col != -1 && row != -1)
        AdvStringGrid2->Hint = AdvStringGrid2->Cells[col][row];
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid2DblClickCell(TObject *Sender,
      int ARow, int ACol) {
    /*if (ACol != 1)
        return;
    if (AdvStringGrid2->Cells[1][ARow] == "")
        return;
    int u_id = StrToInt(AdvStringGrid2->Cells[3][ARow]);
    if (u_id != Department[0]) {
		ShowMessage("无法修改上级" + Department_Name[u_id] + "的数据");
		return;
	}*/
}
//---------------------------------------------------------------------------


//数据备份
void __fastcall TForm1::N5Click(TObject *Sender) {
    String path = ExtractFilePath(Application->ExeName) + "DataBF.exe";
    ShellExecute(NULL,"open",path.c_str(),NULL,NULL,SW_SHOWNORMAL);
}
//---------------------------------------------------------------------------

//数据恢复
void __fastcall TForm1::N6Click(TObject *Sender) {
    String path = ExtractFilePath(Application->ExeName) + "DataHF.exe";
    ShellExecute(NULL,"open",path.c_str(),NULL,NULL,SW_SHOWNORMAL);
}
//---------------------------------------------------------------------------

//数据同步
void __fastcall TForm1::N7Click(TObject *Sender) {
    String path = ExtractFilePath(Application->ExeName) + "DataUp.exe";
    ShellExecute(NULL,"open",path.c_str(),NULL,NULL,SW_SHOWNORMAL);
}
//---------------------------------------------------------------------------

//IP设置
void __fastcall TForm1::IP1Click(TObject *Sender) {
    String path = ExtractFilePath(Application->ExeName) + "SetIP.exe";
    ShellExecute(NULL,"open",path.c_str(),NULL,NULL,SW_SHOWNORMAL);
}
//---------------------------------------------------------------------------

//数据类型设置
void __fastcall TForm1::N8Click(TObject *Sender) {
    String path = ExtractFilePath(Application->ExeName) + "PropDefine_pro.exe";
    ShellExecute(NULL,"open",path.c_str(),NULL,NULL,SW_SHOWNORMAL);
}
//---------------------------------------------------------------------------

//软件发布
void __fastcall TForm1::N4Click(TObject *Sender) {
    BuildSoftware->SoftwarePublish();
}
//---------------------------------------------------------------------------

//数据清除
/*void __fastcall TForm1::N9Click(TObject *Sender) {
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery -> Connection = DMod -> ADOConnection3;
    String sql = "delete * from ZSK_COMBOSTRLIST_H0000Z000K06";
    DMod -> ExecSql(sql, tempQuery);
    delete tempQuery;

    tempQuery = new TADOQuery(NULL);
    tempQuery -> Connection = DMod -> ADOConnection3;
    sql = "delete * from ZSK_DATA_H0000Z000K06";
    DMod -> ExecSql(sql, tempQuery);
    delete tempQuery;

    tempQuery = new TADOQuery(NULL);
    tempQuery -> Connection = DMod -> ADOConnection3;
    sql = "delete * from ZSK_DATATYPE_H0000Z000K06";
    DMod -> ExecSql(sql, tempQuery);
    delete tempQuery;

    tempQuery = new TADOQuery(NULL);
    tempQuery -> Connection = DMod -> ADOConnection3;
    sql = "delete * from ZSK_LIMIT_H0000Z000K06";
    DMod -> ExecSql(sql, tempQuery);
    delete tempQuery;

    tempQuery = new TADOQuery(NULL);
    tempQuery -> Connection = DMod -> ADOConnection3;
    sql = "delete * from ZSK_MAP_H0000Z000K06";
    DMod -> ExecSql(sql, tempQuery);
    delete tempQuery;

    tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    sql = "delete * from ZSK_NATURE_H0000Z000K06";
    DMod->ExecSql(sql, tempQuery);
    delete tempQuery;

    tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    sql = "delete * from ZSK_PARM_H0000Z000K06";
    DMod->ExecSql(sql, tempQuery);
    delete tempQuery;
}
//---------------------------------------------------------------------------
*/

void TForm1::InsertNature(String u_name) {
    for (int i = 1; i < AdvStringGrid1->RowCount; ++ i) {
        if (AdvStringGrid1->Cells[2][i] == u_name) {
            ShowMessage("配线名称'" + u_name + "'已存在");
            AdvStringGrid1->Row = i;
            AdvStringGrid1->Col = 2;
            return;
        }
    }
    CoInitialize(NULL);
    String pguid = newGUID();
	String Code_guid = Get26GuidText();
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String sql = "insert into ZSK_NATURE_H0000Z000K06 (PGUID, S_UDTIME, UPGUID, NATURE, DEPARTMENT, CodeGUID) values('" + pguid + "', '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "', '" + node[Now_Node].Data.PGUID + "', '" + u_name + "', '" + IntToStr(Department[0]) + "', '" + Code_guid + "')";
    DMod->ExecSql(sql, tempQuery);
    delete tempQuery;
    if (AdvStringGrid1->Cells[1][AdvStringGrid1->RowCount - 2] != "节点名称" ||
        AdvStringGrid1->Cells[1][AdvStringGrid1->RowCount - 1] != "")
        AdvStringGrid1->AddRow();
    int row = AdvStringGrid1->RowCount;
    AdvStringGrid1->Cells[0][row - 1] = row - 1;
    AdvStringGrid1->Cells[1][row - 1] = node[Now_Node].Data.JdText;
    AdvStringGrid1->Cells[2][row - 1] = u_name;
    AdvStringGrid1->Cells[3][row - 1] = node[Now_Node].Data.PGUID;
    AdvStringGrid1->Cells[4][row - 1] = pguid;
    AdvStringGrid1->Cells[5][row - 1] = Department[0];
	AdvStringGrid1->Cells[6][row - 1] = Code_guid;
    AdvStringGrid1->Col = 2;
    AdvStringGrid1->Row = AdvStringGrid1->RowCount - 1;
    AdvStringGrid1ClickCell(AdvStringGrid1, AdvStringGrid1->Row, 2);
}
//---------------------------------------------------------------------------

void TForm1::UpdateNature(String u_name) {
    for (int i = 1; i < AdvStringGrid1->RowCount; ++ i) {
        if (AdvStringGrid1->Cells[2][i] == u_name) {
            ShowMessage("配线名称'" + u_name + "'已存在");
            AdvStringGrid1->Row = i;
            AdvStringGrid1->Col = 2;
            return;
        }

    }
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String sql = "update ZSK_NATURE_H0000Z000K06 set NATURE = '" + u_name + "', S_UDTIME = '" + Now().FormatString("yyyy-MM-dd hh:mm:ss") + "' where PGUID = '" + AdvStringGrid1->Cells[4][AdvStringGrid1->Row] + "' and UPGUID = '" + node[Now_Node].Data.PGUID + "' and ISDELETE = 0";
    DMod->ExecSql(sql + sql_Dep, tempQuery);
    delete tempQuery;
    AdvStringGrid1->Cells[2][AdvStringGrid1->Row] = u_name;
    AdvStringGrid1->Col = 2;
    AdvStringGrid1ClickCell(AdvStringGrid1, AdvStringGrid1->Row, 2);
}
//---------------------------------------------------------------------------

void TForm1::DeleteNature() {
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String sql = "update ZSK_NATURE_H0000Z000K06 set ISDELETE = 1 where PGUID = '" + AdvStringGrid1->Cells[4][AdvStringGrid1->Row] + "' and UPGUID = '" + node[Now_Node].Data.PGUID + "'";
    DMod->ExecSql(sql + sql_Dep, tempQuery);
    delete tempQuery;
    AdvStringGrid1->RemoveRows(AdvStringGrid1->Row, 1);
    for (int i = 1; i < AdvStringGrid1->RowCount - 1; ++ i)
        AdvStringGrid1->Cells[0][i] = i;
    AdvStringGrid1->Col = 2;
    AdvStringGrid1->Row = 1;
    AdvStringGrid1ClickCell(AdvStringGrid1, 1, 2);
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1GetAlignment(TObject *Sender,
      int ARow, int ACol, TAlignment &HAlign, TVAlignment &VAlign) {
    if (ARow == 0 || ACol == 0 || ACol == 1) {
        HAlign = taCenter;
        VAlign = vtaCenter;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGridGetAlignment(TObject *Sender,
      int ARow, int ACol, TAlignment &HAlign, TVAlignment &VAlign) {
    HAlign = taCenter;
    VAlign = vtaCenter;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid2GetAlignment(TObject *Sender,
      int ARow, int ACol, TAlignment &HAlign, TVAlignment &VAlign) {
    if (ARow == 0 || ACol == 0) {
        HAlign = taCenter;
        VAlign = vtaCenter;
    }    
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGridResize(TObject *Sender) {
    AdvStringGrid->ColWidths[1] = (AdvStringGrid->Width - AdvStringGrid->ColWidths[0] - 4) / 3;
    AdvStringGrid->ColWidths[2] = (AdvStringGrid->Width - AdvStringGrid->ColWidths[0] - 4) / 3 * 2;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid1Resize(TObject *Sender) {
    AdvStringGrid1->ColWidths[1] = (AdvStringGrid1->Width - AdvStringGrid1->ColWidths[0] - 4) / 2;
    AdvStringGrid1->ColWidths[2] = (AdvStringGrid1->Width - AdvStringGrid1->ColWidths[0] - 4) / 2;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::AdvStringGrid2Resize(TObject *Sender) {
    AdvStringGrid2->ColWidths[1] = (AdvStringGrid2->Width - AdvStringGrid2->ColWidths[0] - 4) / 2;
    AdvStringGrid2->ColWidths[2] = (AdvStringGrid2->Width - AdvStringGrid2->ColWidths[0] - 4) / 2;
}
//---------------------------------------------------------------------------

//Add Nature
void __fastcall TForm1::A1Click(TObject *Sender) {
    Form4->Caption = "添加新的配线名称";
    Form4->Label1->Caption = "配线名称";
    Form4->Edit1->Text = "请输入配线名称";
    Form4->Edit1->SelectAll();
    Form4->Tip = 1;
    Form4->ShowModal();
}
//---------------------------------------------------------------------------

// Update Nature
void __fastcall TForm1::U1Click(TObject *Sender)
{
    int u_id = StrToInt(AdvStringGrid1->Cells[5][AdvStringGrid1->Row]);
    if (u_id != Department[0]) {
        ShowMessage("无法修改上级" + Department_Name[u_id] + "的数据");
        return;
    }
    if (AdvStringGrid1->Cells[1][AdvStringGrid1->Row] != node[Now_Node].Data.JdText) {
        ShowMessage("无法修改其他节点的数据");
        return;
    }
    Form4->Caption = "修改配线名称";
    Form4->Label1->Caption = "配线名称";
    Form4->Edit1->Text = AdvStringGrid1->Cells[2][AdvStringGrid1->Row];
    Form4->Edit1->SelectAll();
    Form4->Tip = 2;
    Form4->ShowModal();
}
//---------------------------------------------------------------------------

void __fastcall TForm1::D1Click(TObject *Sender)
{
    int u_id = StrToInt(AdvStringGrid1->Cells[5][AdvStringGrid1->Row]);
    if (u_id != Department[0]) {
        ShowMessage("无法删除上级" + Department_Name[u_id] + "的数据");
        return;
    }
    if (AdvStringGrid1->Cells[1][AdvStringGrid1->Row] != node[Now_Node].Data.JdText) {
        ShowMessage("无法删除其他节点的数据");
        return;
    }
    int msg = Application->MessageBox("是否删除该配线名称", "提示", MB_YESNO);
    if (msg == 6)
        DeleteNature();
}
//---------------------------------------------------------------------------

void TForm1::AdvStringGrid2Refresh() {
    AdvStringGrid2->Clear();
    AdvStringGrid2->RowCount = 2;
    AdvStringGrid2->ColCount = 6;
    AdvStringGrid2->FixedRows = 1;
    AdvStringGrid2->FixedCols = 1;
    AdvStringGrid2->Cells[0][0] = "序号";
    AdvStringGrid2->Cells[1][0] = "定义参数";
    AdvStringGrid2->Cells[2][0] = "定义值";
    AdvStringGrid2->ColWidths[3] = 0;
    AdvStringGrid2->ColWidths[4] = 0;
    AdvStringGrid2->ColWidths[5] = 0;
    TADOQuery *tempQuery = new TADOQuery(NULL);
    tempQuery->Connection = DMod->ADOConnection3;
    String sql = "select PGUID, PARM from ZSK_PARM_H0000Z000K06 where ISDELETE = 0";
    DMod->OpenSql(sql + " order by Index_ asc, ID asc", tempQuery);
    int cnt = 0;
    while (!tempQuery->Eof) {
        if (cnt + 1 >= AdvStringGrid2->RowCount)
            AdvStringGrid2->AddRow();
        ++ cnt;
        AdvStringGrid2->Cells[0][cnt] = cnt;
        AdvStringGrid2->Cells[1][cnt] = tempQuery->FieldByName("PARM")->AsString;
        AdvStringGrid2->Cells[3][cnt] = tempQuery->FieldByName("PGUID")->AsString;
        tempQuery->Next();
    }
    delete tempQuery;
    // read data
    String up1, up2;
    up1 = AdvStringGrid1->Cells[4][AdvStringGrid1->Row];
    if (up1 == "")
        return;
    for (int i = 1; i < AdvStringGrid2->RowCount; ++ i) {
        up2 = AdvStringGrid2->Cells[3][i];
        TADOQuery *AdoQ = new TADOQuery(NULL);
        sql = "select DATA, PGUID, DEPARTMENT from ZSK_DATA_H0000Z000K06 where UPGUID1 = '" + up1 + "' and UPGUID2 = '" + up2 + "' and ISDELETE = 0";
        AdoQ->Connection = DMod->ADOConnection3;
        DMod->OpenSql(sql + sql_Dep, AdoQ);
        if (!AdoQ->Eof) {
			AdvStringGrid2->Cells[2][i] = AdoQ->FieldByName("DATA")->AsString;
            AdvStringGrid2->Cells[4][i] = AdoQ->FieldByName("PGUID")->AsString;
			AdvStringGrid2->Cells[5][i] = AdoQ->FieldByName("DEPARTMENT")->AsString;
		}
        else {
            AdvStringGrid2->Cells[2][i] = "";
			AdvStringGrid2->Cells[4][i] = "";
            AdvStringGrid2->Cells[5][i] = "";
        }
		delete AdoQ;
    }
}
//---------------------------------------------------------------------------

void __fastcall TForm1::TreeViewCustomDrawItem(TCustomTreeView *Sender,
      TTreeNode *Node, TCustomDrawState State, bool &DefaultDraw)
{
    if (Node == TreeView->Selected) {
		TreeView->Canvas->Brush->Color = 0x00FF8000;
	}
	else {
		TreeView->Canvas->Brush->Color = clWhite;
	}
}
//---------------------------------------------------------------------------



