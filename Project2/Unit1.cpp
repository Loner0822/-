//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit1.h"
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

void __fastcall TForm1::Button1Click(TObject *Sender)
{
    String path = ExtractFilePath(Application->ExeName);
    char* emf_path = NULL;
    char* xcd_path = NULL;
    char* vsd_path = NULL;
    path += "Analysis_LCT.dll";
    bool __stdcall (*Create_LiuCheng)(char* &, char* &, char* &);
    HINSTANCE Hdl;
	Hdl = ::LoadLibrary(path.c_str());
	if (Hdl != NULL) {
		FARPROC P;
		P = GetProcAddress(Hdl, "Create_LiuCheng");
		if (P == NULL) {
			ShowMessage("打开Create_LiuCheng()函数错误!");
		}
		else {
			Create_LiuCheng = (bool __stdcall (*)(char* &, char* &, char* &))P;
			bool flag = Create_LiuCheng(emf_path, xcd_path, vsd_path);
            String emf = emf_path, xcd = xcd_path, vsd = vsd_path;
            ShowMessage(emf);
            ShowMessage(xcd);
            ShowMessage(vsd);
		}
		::FreeLibrary(Hdl);
	}
	else {
		ShowMessage("不能载入DLL!");
	}
    delete emf_path;
    delete xcd_path;
    delete vsd_path;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button2Click(TObject *Sender)
{
    String path = ExtractFilePath(Application->ExeName);
    char* emf_path = NULL;
    char* xcd_path = NULL;
    char* vsd_path = NULL;
    bool __stdcall (*Edit_LiuCheng)(const char* &, char* &, char* &, char* &);
    HINSTANCE Hdl;
    path += "Analysis_LCT.dll";
	Hdl = ::LoadLibrary(path.c_str());
	if (Hdl != NULL) {
		FARPROC P;
		P = GetProcAddress(Hdl, "Edit_LiuCheng");
		if (P == NULL) {
			ShowMessage("打开Edit_LiuCheng()函数错误!");
		}
		else {
            String tmp = ExtractFilePath(Application->ExeName) + "vss//demo.vsd";
			Edit_LiuCheng = (bool __stdcall (*)(const char* &, char* &, char* &, char* &))P;
			bool flag = Edit_LiuCheng(tmp.c_str(), emf_path, xcd_path, vsd_path);
            String emf = emf_path, xcd = xcd_path, vsd = vsd_path;
            ShowMessage(emf);
            ShowMessage(xcd);
            ShowMessage(vsd);
		}
		::FreeLibrary(Hdl);
	}
	else {
		ShowMessage("不能载入DLL!");
	}
    delete emf_path;
    delete xcd_path;
    delete vsd_path;
}
//---------------------------------------------------------------------------

void __fastcall TForm1::Button3Click(TObject *Sender)// 解析xcd, emf 结果存入.log文件
{
    String path = ExtractFilePath(Application->ExeName);
	path += "Analysis_LCT.dll";
    String __stdcall (*Icon_Analysis)();
    HINSTANCE Hdl;
	Hdl = ::LoadLibrary(path.c_str());
	if (Hdl != NULL) {
		FARPROC P;
		P = GetProcAddress(Hdl, "Icon_Analysis");
		if (P == NULL) {
			ShowMessage("打开Icon_Analysis()函数错误!");
		}
		else {
            String tmp = ExtractFilePath(Application->ExeName) + "vss//demo.vsd";
			Icon_Analysis = (String __stdcall (*)())P;
			String flag = Icon_Analysis();
            ShowMessage(flag);
		}
		::FreeLibrary(Hdl);
	}
	else {
		ShowMessage("不能载入DLL!");
	}

}
//---------------------------------------------------------------------------


