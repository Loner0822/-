//---------------------------------------------------------------------------

#include <vcl.h>
#include <windows.h>
#include <math.h>
#include <tlhelp32.h>
#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <memory>
#include "DrawingInfo.h"
#include "TXcdTrans.h"
#include "TImageBlock.h"

#pragma hdrstop
//---------------------------------------------------------------------------
//   Important note about DLL memory management when your DLL uses the
//   static version of the RunTime Library:
//
//   If your DLL exports any functions that pass String objects (or structs/
//   classes containing nested Strings) as parameter or function results,
//   you will need to add the library MEMMGR.LIB to both the DLL project and
//   any other projects that use the DLL.  You will also need to use MEMMGR.LIB
//   if any other projects which use the DLL will be performing new or delete
//   operations on any non-TObject-derived classes which are exported from the
//   DLL. Adding MEMMGR.LIB to your project will change the DLL and its calling
//   EXE's to use the BORLNDMM.DLL as their memory manager.  In these cases,
//   the file BORLNDMM.DLL should be deployed along with your DLL.
//
//   To avoid using BORLNDMM.DLL, pass string information using "char *" or
//   ShortString parameters.
//
//   If your DLL uses the dynamic version of     the RTL, you do not need to
//   explicitly add MEMMGR.LIB as this will be done implicitly for you
//---------------------------------------------------------------------------

#pragma argsused
BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fwdreason, LPVOID lpvReserved)
{
    return 1;
}
//---------------------------------------------------------------------------

class TheIcon {
public:
    TheIcon() {
        id = 0, icon_id = 0, text = "", from = 0, to = 0;
        connect.clear();
    }
    int id, icon_id;
    String text;
    int from, to;
    vector<pair<int, int> > connect; 
};
vector<TheIcon> Icons;
String path_analysis;
HWND visioHwnd;//visio窗口句柄
TSigDrawing* drawing = new TSigDrawing();
String res_all;
//TImage *LcImage;

BOOL CALLBACK EnumWindowsProc(HWND hwnd,LPARAM lParam) {
    if (IsWindowVisible(hwnd)) {
        char s[16];
        GetWindowText(hwnd, s, 16);
        if(s[0] == 'M' && s[1] == 'i' && s[2] == 'c' && s[3] == 'r' && s[4] == 'o' &&
           s[10] == 'V' && s[11] == 'i' && s[12] == 's' && s[13] == 'i' && s[14] == 'o' ) {
            visioHwnd = hwnd;
            ::SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, Screen->Width, Screen->Height, SWP_SHOWWINDOW);
            return TRUE;
        }
    }
    return TRUE;
}
//---------------------------------------------------------------------------

BOOL CALLBACK EnumWindowsProc1(HWND hwnd,LPARAM lParam) {
    if (IsWindowVisible(hwnd)) {
        char s[16];
        GetWindowText(hwnd, s, 16);
        if(s[0] == 'M' && s[1] == 'i' && s[2] == 'c' && s[3] == 'r' && s[4] == 'o' &&
           s[10] == 'V' && s[11] == 'i' && s[12] == 's' && s[13] == 'i' && s[14] == 'o' ){
            visioHwnd = hwnd;
            //::SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, Screen->Width, Screen->Height, SWP_SHOWWINDOW);
            return TRUE;
        }
    }
    return TRUE;
}
//---------------------------------------------------------------------------

BOOL FindProcess(std::string strProc)
{
    int i=0;
    PROCESSENTRY32 pe32;
    pe32.dwSize = sizeof(PROCESSENTRY32);
    HANDLE hProcessSnap = ::CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
    if(hProcessSnap == INVALID_HANDLE_VALUE) {
        return false;
    }
    BOOL bMore = ::Process32First(hProcessSnap, &pe32);
    while(bMore) {
        //这里还是使用stricmp函数最方便，否则还要统一转换成大写或者小写
        if(0 == stricmp(strProc.c_str(), pe32.szExeFile)) {
            i += 1;  //找到
        }
        bMore = ::Process32Next(hProcessSnap, &pe32);
    }
    if(i >= 1) {
        return true;
    }
    else {
        return false;
    }
}
//---------------------------------------------------------------------------

String ChangeVsdToEmf(String vsdpath)
{
    AnsiString vsdName = ExtractFileName(vsdpath);
    vsdName = vsdName.SubString(0, vsdName.Length()-4);//去掉文件类
    String emf =  ExtractFilePath( Application->ExeName ) +"temp\\"+ vsdName+".emf";
    String xcd =  ExtractFilePath( Application->ExeName ) +"temp\\"+ vsdName+".xcd";
    //String xcd1 =  ExtractFilePath( Application->ExeName ) +"1.xcd";
    System::Variant V  = Variant::CreateObject("Visio.Application");
    //Sleep(6000);  // 等待visio打开
    try {
        const AnsiString vsl = ExtractFilePath( Application->ExeName ) + "Xaddon.vsl";
        System::Variant o = V.OlePropertyGet( "Addons" ).OleFunction( "Add", WideString(vsl) );
        o = V.OlePropertyGet("Addons").OlePropertyGet( "Item", "HLXAddon" );
        System::Variant doc = V.OlePropertyGet( "Documents" ).OleFunction( "Add", WideString(vsdpath) );

        AnsiString cmd = "9022 \"" + emf + "\" " + (512 + 4);
        o.OleProcedure( "Run", WideString(cmd) );

        cmd = "6851 0 0 \"" + xcd + "\"";
        o.OleProcedure( "Run", WideString(cmd) );

        //cmd = "6851 0 0 \"" + xcd1 + "\"";
        //o.OleProcedure( "Run", WideString(cmd) );

        doc.OlePropertySet( "Saved", true );
        doc.OleProcedure( "Close" );
        V.OleProcedure("Quit");
        V = Unassigned;
    }
    catch( Exception& ex ) {
        V.OleProcedure("Quit");
        V = Unassigned;
    }
    DeleteFile(emf + ".emf");
    if(FileExists(emf)) {
        return emf;
    }                                        
    return "";
}
//---------------------------------------------------------------------------

bool Check_Run() {
    EnumWindows((WNDENUMPROC)EnumWindowsProc1, NULL);
    if(visioHwnd  && IsWindow(visioHwnd)) {
        ShowMessage("请关闭当前桌面打开的Visio文件后继续!");
        return 1;
    }
    if( FindProcess("Visio.exe") ) {
        ShowMessage("请结束Visio进程后继续!");
        return 1;
    }
    return 0;
}
//---------------------------------------------------------------------------

void Open_Visio(String path) {
    visioHwnd = NULL;
    String vssFile = ExtractFilePath(Application->ExeName)+"vss\\49.vss";
    System::Variant V  = Variant::CreateObject("Visio.Application");
    //Sleep(6000);  // 等待visio打开

    System::Variant doc = V.OlePropertyGet("Documents").OleFunction("Open", WideString(path));
    V.OlePropertyGet( "Documents" ).OleFunction("Add", WideString(vssFile));
    
    EnumWindows((WNDENUMPROC)EnumWindowsProc, NULL);
    while (visioHwnd  && IsWindow(visioHwnd )) {
        ;
    }
}
//---------------------------------------------------------------------------

void ExplainDataBS( TStream* pStream )
{
//
    //ID
    int m_id = 0;
    pStream->Read( &m_id, 4 );  //ID
    //LogOut( "ID " + AnsiString( m_id ) );

    //类型
    int m_type = 0;
    pStream->Read( &m_type, 4 );
    //LogOut( "类型:" + AnsiString( m_type ) );

    //角度
    float m_angle = 0.0;
    pStream->Read( &m_angle, 4 );
    //LogOut( "角度:" + AnsiString( m_angle ) );
    //x
    int m_x = 0;
    pStream->Read( &m_x, 4 );
    //LogOut( "x:" + AnsiString( m_x ) );
    //y
    int m_y = 0;
    pStream->Read( &m_y, 4 );
    //LogOut( "y:" + AnsiString( m_y ) );
    //宽度
    int m_width = 0;
    pStream->Read( &m_width, 4 );
    //LogOut( "宽度:" + AnsiString( m_width ) );
    //高度
    int m_height = 0;
    pStream->Read( &m_height, 4 );
    //LogOut( "高度:" + AnsiString( m_height ) );

    //标志
    int m_flag = 0;//是否有文字
    pStream->Read( &m_flag, 4 );
    //LogOut( "havetext " + AnsiString( m_flag ) );
    if( m_flag == 1 )  //文本框
    {
        TRect rect;
        pStream->Read( &rect.left, 4 );
        pStream->Read( &rect.top, 4 );
        pStream->Read( &rect.right, 4 );
        pStream->Read( &rect.bottom, 4 );
        //LogOut( "文字位置:" + AnsiString( rect.Left ) + " " +  AnsiString( rect.Top )+ " " + AnsiString( rect.Right )+ " " + AnsiString( rect.Bottom ) );
    }

    //图形数量
    int m_count = 0;
    pStream->Read( &m_count, 4 );
    //LogOut( "图形数量:" + AnsiString( m_count ) );
    for( int i = 0; i < m_count; i++ )
    {
        TRect rect;
        pStream->Read( &rect.left, 4 );
        pStream->Read( &rect.top, 4 );
        pStream->Read( &rect.right, 4 );
        pStream->Read( &rect.bottom, 4 );
        //LogOut( "图形位置:" + AnsiString( rect.Left ) + " " +  AnsiString( rect.Top )+ " " + AnsiString( rect.Right )+ " " + AnsiString( rect.Bottom ) );
    }
}
//---------------------------------------------------------------------------

void ExplainDataBN( TStream* pStream )
{
    //ID
    int m_id = 0;
    pStream->Read( &m_id, 4 );  //ID
    //LogOut( "ID " + AnsiString( m_id ) );
    //GUID
    int n = 0;
    pStream->Read( &n, 2 );
    AnsiString m_guid = "";
    m_guid.SetLength( n );
    pStream->Read( m_guid.c_str(), n );
    //LogOut( "GUID:" + AnsiString( m_guid ) );
    //IconID
    int m_iconId = 0;
    pStream->Read( &m_iconId, 4 );
    //LogOut( "IconID " + AnsiString( m_iconId ) );
    //类型
    int m_type = 0;
    pStream->Read( &m_type, 4 );
    //LogOut( "类型:" + AnsiString( m_type ) );
    //角度
    float m_angle = 0.0;
    pStream->Read( &m_angle, 4 );
    //LogOut( "角度:" + AnsiString( m_angle ) );
    //x
    int m_x = 0;
    pStream->Read( &m_x, 4 );
    //LogOut( "x:" + AnsiString( m_x ) );
    //y
    int m_y = 0;
    pStream->Read( &m_y, 4 );
    //LogOut( "y:" + AnsiString( m_y ) );
    //宽度
    int m_width = 0;
    pStream->Read( &m_width, 4 );
    //LogOut( "宽度:" + AnsiString( m_width ) );
    //高度
    int m_height = 0;
    pStream->Read( &m_height, 4 );
    //LogOut( "高度:" + AnsiString( m_height ) );
    //分组
    int m_group = 0;
    pStream->Read( &m_group, 4 );
    //LogOut( "分组:" + AnsiString( m_group ) );
    //标志
    int m_flag = 0;//是否有文字
    pStream->Read( &m_flag, 4 );
    //LogOut( "havetext " + AnsiString( m_flag ) );
    if( m_flag == 1 )  //文本框
    {
        TRect rect;
        pStream->Read( &rect.left, 4 );
        pStream->Read( &rect.top, 4 );
        pStream->Read( &rect.right, 4 );
        pStream->Read( &rect.bottom, 4 );
        //LogOut( "文字位置:" + AnsiString( rect.Left ) + " " +  AnsiString( rect.Top )+ " " + AnsiString( rect.Right )+ " " + AnsiString( rect.Bottom ) );
    }
    //图形数量
    int m_count = 0;
    pStream->Read( &m_count, 4 );
    //LogOut( "图形数量:" + AnsiString( m_count ) );
    for( int i = 0; i < m_count; i++ )
    {
        TRect rect;
        pStream->Read( &rect.left, 4 );
        pStream->Read( &rect.top, 4 );
        pStream->Read( &rect.right, 4 );
        pStream->Read( &rect.bottom, 4 );
        //LogOut( "图形位置:" + AnsiString( rect.Left ) + " " +  AnsiString( rect.Top )+ " " + AnsiString( rect.Right )+ " " + AnsiString( rect.Bottom ) );
    }
}
//---------------------------------------------------------------------------

void ExplainLinkPoint( TStream* pStream )
{
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
            Icons.clear();
            for( int i = 0; i < count; i++ )
            {
                TheIcon tmpicon;
                //LogOut("------------------------------------------------");
                //id
                int id = 0;
                pStream->Read( &id, 4 );
                //LogOut("ID " + AnsiString(id) );
                tmpicon.id = id;
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
                //LogOut("Point_Cnt " + AnsiString(concount / 4) );
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
                    //LogOut("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
                    //LogOut("m_index " + AnsiString(m_index) );
                    //LogOut("type:" + AnsiString(type) );
                    //LogOut("x " + AnsiString(x) );
                    //LogOut("y " + AnsiString(y) );
                    if (m_index < 0)
                        tmpicon.connect.push_back(make_pair(x, y));
                   // m_index >= 0 表示连接点
                }
                Icons.push_back(tmpicon);
            }
        }
    }
}
//---------------------------------------------------------------------------

void Get_Iconid_Text() {
    ifstream init;
    String path = ExtractFilePath(Application->ExeName) + "temp\\xcd.log";
    init.open(path.c_str());
    string str;
    int id, icon_id;
    string text;
    while(getline(init, str)) {
        if (str[0] == 'I' && str[1] == 'D') {
            id = 0;
            for (int i = 3; i < str.size(); ++ i)
                if ('0' <= str[i] && str[i] <= '9')
                    id = id * 10 + str[i] - '0';
            bool flag = 0;
            for (int i = 0; i < Icons.size(); ++ i) {
                if (Icons[i].id == id) {
                    flag = 1;
                    break;
                }
            }
            if (!flag) {
                TheIcon tmpicon;
                tmpicon.id = id;
                Icons.push_back(tmpicon);
            }
        }
        if (str[0] == 'i' && str[5] == 'D') {
            icon_id = 0;
            for (int i = 7; i < str.size(); ++ i)
                if ('0' <= str[i] && str[i] <= '9')
                    icon_id = icon_id * 10 + str[i] - '0';
            for (int i = 0; i < Icons.size(); ++ i)
                if (Icons[i].id == id) {
                    Icons[i].icon_id = icon_id;
                    break;
                }
        }
        if (str[0] == 't' && str[3] == 't') {
            text = str.substr(5);
            for (int i = 0; i < Icons.size(); ++ i)
            if (Icons[i].id == id) {
                Icons[i].text = text.c_str();
                //ShowMessage(Icons[i].text);
                break;
            }
        }
    }
}
//---------------------------------------------------------------------------

void Get_From_To() {
    for (int i = 0; i < Icons.size(); ++ i) {
        if (Icons[i].icon_id == 5107 || Icons[i].icon_id == 4903) {
            vector<pair<int, int> >Point;
            for (int j = 0; j < Icons.size(); ++ j) {
                for (int k = 0; k < Icons[j].connect.size(); ++ k) {
                    if (Icons[j].connect[k].first == Icons[i].id) {
                        Point.push_back(make_pair(Icons[j].id, Icons[j].connect[k].second));
                    }
                }
            }
            if (Point[0].second > Point[1].second) {
                Icons[i].from = Point[1].first;
                Icons[i].to = Point[0].first;
            }
            else {
                Icons[i].from = Point[0].first;
                Icons[i].to = Point[1].first;
            }
        }
        else
            Icons[i].from = Icons[i].to = 0;
    }
}
//---------------------------------------------------------------------------

String To_XML() {
    String xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
    xml += "<UserProperty>";
    for (int i = 0; i < Icons.size(); ++ i) {
        xml += "<id_" + IntToStr(Icons[i].id) + ">";
        xml += "<id>";
        xml += IntToStr(Icons[i].id);
        xml += "</id>";
        xml += "<icon_id>";
        xml += IntToStr(Icons[i].icon_id);
        xml += "</icon_id>";
        xml += "<text>";
        xml += Icons[i].text;
        xml += "</text>";
        xml += "<from>";
        xml += IntToStr(Icons[i].from);
        xml += "</from>";
        xml += "<to>";
        xml += IntToStr(Icons[i].to);
        xml += "</to>";
        xml += "</id_" + IntToStr(Icons[i].id) + ">";
    }
    xml += "</UserProperty>";
    //LogOut(xml);
    return xml;
}
//---------------------------------------------------------------------------

bool Icon_Analysis(String vsd, String emf, String xcd) {
    String t_vsd = "", t_emf = "", t_xcd = "";
    path_analysis = vsd + emf + xcd;
    if (path_analysis == "") {
        ShowMessage("请给出路径!");
        return 0;
    }
    if (emf != "") {
        t_emf = ExtractFilePath(Application->ExeName) + "temp\\demo.emf";
        ::CopyFile(emf.c_str(), t_emf.c_str(), false);
    }
    if (xcd != "") {
        t_xcd = ExtractFilePath(Application->ExeName) + "temp\\demo.xcd";
        ::CopyFile(xcd.c_str(), t_xcd.c_str(), false);
    }
    if (vsd != "") {
        t_vsd = ExtractFilePath(Application->ExeName) + "temp\\demo.vsd";
        ::CopyFile(vsd.c_str(), t_vsd.c_str(), false);
        t_emf = ChangeVsdToEmf(t_vsd);
        t_xcd = t_vsd.SubString(0, t_vsd.Length() - 4) + ".xcd";
    }
    
    if (FileExists(ExtractFilePath(Application->ExeName) + "temp\\emf.log"))
        DeleteFile(ExtractFilePath(Application->ExeName) + "temp\\emf.log");
    if (FileExists(ExtractFilePath(Application->ExeName) + "temp\\xcd.log"))
        DeleteFile(ExtractFilePath(Application->ExeName) + "temp\\xcd.log");

    // emf 解析
    if (t_emf != "") {
        TFileStream* fs = new TFileStream(t_emf, fmOpenRead | fmShareDenyNone);
        drawing->LoadFromStream(fs);
        delete fs;
        
        TMemoryStream *stream = new TMemoryStream();
        stream->LoadFromFile(t_emf);
        stream->Position = 0;
        int pos = 0;
        //最后四位 代表数据开始位置
        //seek实现指针移动
        //            偏移字节数  移动后指针距离数据结束
        stream->Seek(-4, soFromEnd);
        stream->Read(&pos, 4);
        int HeadBeginPos = pos - 1;
        stream->Seek(HeadBeginPos, soFromBeginning);
        char header[2];
        stream->Read(header, 2);

        int LinkPointPos = HeadBeginPos - 4;

        //---解析连接点-------------------
        stream->Seek( LinkPointPos, soFromBeginning );
        stream->Read( &LinkPointPos, 4 );
        stream->Seek( LinkPointPos - 1, soFromBeginning );
        ExplainLinkPoint( stream );
        delete stream;
        stream = NULL;
        Get_From_To();
    }

    // xcd 解析
    if (t_xcd != "") {
        char *xcdmap = NULL;
        int length = 0;
        LoadFile(t_xcd, xcdmap, length);
        xcdTranslater = new TXcdTrans();
        xcdTranslater->ParsFieldInfo(xcdmap, length);
        delete  xcdTranslater;
        xcdTranslater = NULL;
        delete xcdmap;
        xcdmap = NULL;
        Get_Iconid_Text();
    }

    if (FileExists(ExtractFilePath(Application->ExeName) + "temp\\xcd.log"))
        DeleteFile(ExtractFilePath(Application->ExeName) + "temp\\xcd.log");
    for (vector<TheIcon>::iterator it = Icons.begin(); it != Icons.end();) {
        if (it->icon_id == 0)
            it = Icons.erase(it);
        else
            ++ it;
    }
    return 1;
}

extern "C" __declspec(dllexport) bool __stdcall Create_LiuCheng(char* &emf_path, char* &xcd_path, char* &vsd_path) {
    if (FileExists(ExtractFilePath(Application->ExeName) + "temp\\icon.log"))
        DeleteFile(ExtractFilePath(Application->ExeName) + "temp\\icon.log");
    if (Check_Run()) {
        return 0;
    }
    String dFile;
    String s = ExtractFilePath(Application->ExeName)+"vss\\demo.vsd";
    dFile = ExtractFilePath(Application->ExeName)+"temp\\demo.vsd";
    ::CopyFile(s.c_str(), dFile.c_str(), false);  //复制一份空白visio
    Open_Visio(dFile);
    String emf = ChangeVsdToEmf(dFile);
    String xcd = dFile.SubString(0, dFile.Length() - 4) + ".xcd";

    res_all = emf;
    emf_path = res_all.c_str();

    res_all = xcd;
    xcd_path = res_all.c_str();

    res_all = dFile;
    vsd_path = res_all.c_str();
    return 1;
}
//---------------------------------------------------------------------------

extern "C" __declspec(dllexport) bool __stdcall Edit_LiuCheng(const char* &get_vsd_path, char* &emf_path, char* &xcd_path, char* &vsd_path) {
    if (FileExists(ExtractFilePath(Application->ExeName) + "temp\\icon.log"))
        DeleteFile(ExtractFilePath(Application->ExeName) + "temp\\icon.log");
    if (Check_Run()) {
        return 0;
    }
    String dFile;
    String s = get_vsd_path;
    dFile = ExtractFilePath(Application->ExeName)+"temp\\demo.vsd";
    ::CopyFile(s.c_str(), dFile.c_str(), false);  //复制一份空白visio
    Open_Visio(dFile);
    String emf = ChangeVsdToEmf(dFile);
    String xcd = dFile.SubString(0, dFile.Length() - 4) + ".xcd";

    res_all = emf;
    emf_path = res_all.c_str();

    res_all = xcd;
    xcd_path = res_all.c_str();

    res_all = dFile;
    vsd_path = res_all.c_str();
    return 1;
}
//---------------------------------------------------------------------------

extern "C" __declspec(dllexport) bool __stdcall Icon_XML(const char* &vsd_path, const char* &emf_path, const char* &xcd_path, char* &xml_text) {
    bool flag = 0;
    String vsd = vsd_path, emf = emf_path, xcd = xcd_path;
    if (vsd + emf + xcd != path_analysis) {
        flag = Icon_Analysis(vsd_path, emf_path, xcd_path);
        path_analysis = vsd + emf + xcd;
    }
    if (!flag && vsd + emf + xcd != path_analysis)
        return 0;
    res_all = To_XML();
    xml_text = res_all.c_str();
    return 1;
}
//---------------------------------------------------------------------------

extern "C" __declspec(dllexport) bool __stdcall Icon_Type(const char* &vsd_path, const char* &emf_path, const char* &xcd_path, const int &id, int &icon_id, char* &text) {
    bool flag = 0;
    String vsd = vsd_path, emf = emf_path, xcd = xcd_path;
    if (vsd + emf + xcd != path_analysis) {
        flag = Icon_Analysis(vsd_path, emf_path, xcd_path);
        path_analysis = vsd + emf + xcd;
    }
    if (!flag && vsd + emf + xcd != path_analysis)
        return 0;
    for (int i = 0; i < Icons.size(); ++ i) {
        if (Icons[i].id == id) {
            icon_id = Icons[i].icon_id;
            res_all = Icons[i].text;
            text = res_all.c_str();
            return 1;
        }
    }
    icon_id = 0;
    text = NULL;
    ShowMessage("未找到该ID的图符");
    return 0;
}
//---------------------------------------------------------------------------

extern "C" __declspec(dllexport) bool __stdcall Coordinate_Icon(const char* &vsd_path, const char* &emf_path, const char* &xcd_path, const int &X, const int &Y, int &id, int &icon_id, char* &text) {
    bool flag = 0;
    String vsd = vsd_path, emf = emf_path, xcd = xcd_path;
    if (vsd + emf + xcd != path_analysis) {
        flag = Icon_Analysis(vsd_path, emf_path, xcd_path);
        path_analysis = vsd + emf + xcd;
    }
    if (!flag && vsd + emf + xcd != path_analysis)
        return 0;
    const TShapeInfo* shape = drawing->HitTest( X, Y );
    if(shape) {
        icon_id = shape->GetIconId();
        id = shape->GetId();
        for (int i = 0; i < Icons.size(); ++ i) {
            if (id == Icons[i].id) {

                res_all = Icons[i].text;
                text = res_all.c_str();
                return 1;
            }
        }
        ShowMessage("对应坐标无图符!");
        return 0;
    }
    else {
        ShowMessage("对应坐标无图符!");
        return 0;
    }
    return 0;
}
//---------------------------------------------------------------------------
