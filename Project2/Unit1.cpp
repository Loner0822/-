//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit1.h"
#include "DrawingInfo.h"
#include "meta.h"
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma link "SigViewerLib_OCX"
#pragma resource "*.dfm"
TForm1 *Form1;
int Mapid = 0;
TSigDrawing* drawing = new TSigDrawing();
//---------------------------------------------------------------------------
__fastcall TForm1::TForm1(TComponent* Owner)
    : TForm(Owner)
{
}
//---------------------------------------------------------------------------

HINSTANCE Hdl;
bool __stdcall (*Create_LiuCheng)(char* &, char* &, char* &);
bool __stdcall (*Edit_LiuCheng)(const char* &, char* &, char* &, char* &);
bool __stdcall (*Icon_XML)(const char* &, const char* &, const char* &, char* &);
bool __stdcall (*Icon_Type)(const char* &, const char* &, const char* &, const int &, int &, char* &);
bool __stdcall (*Coordinate_Icon)(const char* &, const char* &, const char* &, const int &, const int &, int &, int &, char* &);

void __fastcall TForm1::FormCreate(TObject *Sender)
{
    String path = ExtractFilePath(Application->ExeName);
    path += "Analysis_LCT.dll";

	Hdl = ::LoadLibrary(path.c_str());
	if (Hdl != NULL) {
		FARPROC P;
		//Create
		P = GetProcAddress(Hdl, "Create_LiuCheng");
		if (P == NULL) {
			ShowMessage("打开Create_LiuCheng()函数错误!");
		}
		else {
			Create_LiuCheng = (bool __stdcall (*)(char* &, char* &, char* &))P;
		}
		//Edit 
        P = GetProcAddress(Hdl, "Edit_LiuCheng");
		if (P == NULL) {
			ShowMessage("打开Edit_LiuCheng()函数错误!");
		}
		else {
			Edit_LiuCheng = (bool __stdcall (*)(const char* &, char* &, char* &, char* &))P;
		}
		// XML
        P = GetProcAddress(Hdl, "Icon_XML");
		if (P == NULL) {
			ShowMessage("打开Icon_XML()函数错误!");
		}
		else {
			Icon_XML = (bool __stdcall(*)(const char* &, const char* &, const char* &, char* &))P;
		}
		// Type
		P = GetProcAddress(Hdl, "Icon_Type");
		if (P == NULL) {
			ShowMessage("打开Icon_Type()函数错误!");
		}
		else {
			Icon_Type = (bool __stdcall (*)(const char* &, const char* &, const char* &, const int &, int &, char* &))P;
		}
		// Coordinate
		P = GetProcAddress(Hdl, "Coordinate_Icon");
		if (P == NULL) {
			ShowMessage("打开Coordinate_Icon()函数错误!");
		}
		else {
			Coordinate_Icon = (bool __stdcall (*)(const char* &, const char* &, const char* &, const int &, const int &, int &, int &, char* &))P;
		}
	}
	else {
		ShowMessage("不能载入DLL!");
	}
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button1Click(TObject *Sender)
{
    char* emf_path = NULL;
    char* xcd_path = NULL;
    char* vsd_path = NULL;
    bool flag = Create_LiuCheng(emf_path, xcd_path, vsd_path);
    String emf = emf_path, xcd = xcd_path, vsd = vsd_path;
    ShowMessage(emf);
    ShowMessage(xcd);
    ShowMessage(vsd);
    delete emf_path;
    emf_path = NULL;
    delete xcd_path;
    xcd_path = NULL;
    delete vsd_path;
    vsd_path = NULL;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button2Click(TObject *Sender)
{
    char* emf_path = NULL;
    char* xcd_path = NULL;
    char* vsd_path = NULL;
    String tmp = ExtractFilePath(Application->ExeName) + "vss//demo.vsd";
    bool flag = Edit_LiuCheng(tmp.c_str(), emf_path, xcd_path, vsd_path);
    ShowMessage(emf_path);
    ShowMessage(xcd_path);
    ShowMessage(vsd_path);
    delete emf_path;
    emf_path = NULL;
    delete xcd_path;
    xcd_path = NULL;
    delete vsd_path;
    vsd_path = NULL;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button3Click(TObject *Sender)// 解析xcd, emf 结果存入.log文件
{
    char* xml = NULL;
    String xcd = ExtractFilePath(Application->ExeName) + "temp//demo.xcd";
    bool flag = Icon_XML("", "", xcd.c_str(), xml);
    ShowMessage(xml);
    delete xml;
    xml = NULL;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button4Click(TObject *Sender) //传入：图符ID，返回：图符类型ID，图符内容
{
    int ID = 81, Icon_ID;
    char* Text;
    String vsd = ExtractFilePath(Application->ExeName) + "temp//demo.vsd";
    bool flag = Icon_Type(vsd.c_str(), "", "", ID, Icon_ID, Text);
    ShowMessage(Icon_ID);
    ShowMessage(Text);
    delete Text;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button5Click(TObject *Sender)
{
    int X = 235, Y = 13;
    int ID = 0, Icon_ID = 0;
    char *Text = NULL;
    String vsd = ExtractFilePath(Application->ExeName) + "temp//demo.vsd";
    bool flag = Coordinate_Icon(vsd.c_str(), "", "", X, Y, ID, Icon_ID, Text);
    ShowMessage("ID: " + IntToStr(ID) + ", Icon_ID: " + IntToStr(Icon_ID) + ", Text: " + String(Text));
    delete Text;
    Text = NULL;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::FormClose(TObject *Sender, TCloseAction &Action)
{
    ::FreeLibrary(Hdl);
}
//---------------------------------------------------------------------------

/*void __fastcall TForm1::Button6Click(TObject *Sender)
{
    SigViewer1->Init( L"123", L"" );
    SigViewer1->ShowToolbar = false;
    String emfpath = ExtractFilePath( Application->ExeName )+"temp\\demo.emf"  ;
    this->SigViewer1->OpenDrawingFile(++Mapid,WideString(emfpath),L"") ;
    TFileStream* fs = new TFileStream( emfpath, fmOpenRead	|  fmShareDenyNone );
    tmpdraw->LoadFromStream( fs );
    delete fs;
}
//---------------------------------------------------------------------------
*/
/*
void __fastcall TForm1::SigViewer1MouseDown(TObject *Sender,
      TxMouseButton Button, long X, long Y, long *Cancel)
{
    //
    long mapx, mapy;
    this->SigViewer1->WindowToView(X, Y, &mapx, &mapy);
    //ShowMessage(IntToStr(mapx) + ", " + IntToStr(mapy));
    //bool flag = Coordinate_Icon(mapx, mapy, ID, Icon_ID, Text);

}
//---------------------------------------------------------------------------
*/

