//---------------------------------------------------------------------------
#define   NO_WIN32_LEAN_AND_MEAN
#include <shlobj.h>

#include <vcl.h>
#pragma hdrstop

#include <Filectrl.hpp>
#include <Inifiles.hpp>
#include <stdio.h>
#include <fstream>
#include <string>
#include <memory>
#include "TBuildSoftwareF.h"
#include "TProcess.h"
#include "DModUnit.h"
#include "CompanyUnit.h"

using namespace std;

const char* newGUID()
{
    static char buf[64] = {0};
    GUID guid;
    if (S_OK == ::CoCreateGuid(&guid))
    {
        _snprintf(buf, sizeof(buf),
                  "{%08X-%04X-%04x-%02X%02X-%02X%02X%02X%02X%02X%02X}",
                  guid.Data1,
                  guid.Data2,
                  guid.Data3,
                  guid.Data4[0], guid.Data4[1],
                  guid.Data4[2], guid.Data4[3],
                  guid.Data4[4], guid.Data4[5],
                  guid.Data4[6], guid.Data4[7]
                    );
    }
    return (const char*)buf;
}

string StringTostring(String str) {
    string res = str.c_str();
    return res;
}

TBuildSoftware * BuildSoftware = new TBuildSoftware();

TBuildSoftware::TBuildSoftware()
    : m_softwareInfo(NULL), m_FilePath("")
    , m_ver("1.0.0.0"), m_verBigNum("1.0"), m_softwareName("")
{
    m_softwareInfo = new TSoftwareInfo();
}

TBuildSoftware::~TBuildSoftware()
{
    if( m_softwareInfo )
    {
        delete m_softwareInfo;
        m_softwareInfo = NULL;
    }
}

const TSoftwareInfo * TBuildSoftware::BuildSoftware( AnsiString softwareName )
{
    exePath = ExtractFilePath(Application->ExeName);
    m_softwareInfo->Clear();
    m_softwareInfo->Name = softwareName;
    m_softwareName =  softwareName;
    ChoiceDirectory(); //选择目录
    bool result = true;
    if( result )
    {
        Build();   //打包
    }
    return m_softwareInfo;
}

AnsiString GetPath( AnsiString name )
{
    std::auto_ptr<TIniFile> pIniFile( new TIniFile( ExtractFilePath( Application->ExeName ) + "打包环境位置.ini" ) );
    AnsiString path = pIniFile->ReadString( "位置", name, "" );
    path = ExtractFilePath( Application->ExeName ) +path;   //相对路径
    return path;
}

AnsiString GetDefaultPath()
{
    std::auto_ptr<TIniFile> pIniFile( new TIniFile( ExtractFilePath( Application->ExeName ) + "打包环境位置.ini" ) );
    AnsiString path = pIniFile->ReadString( "默认位置", "位置", "" );
    return path;
}

//选择文件夹
void __fastcall TBuildSoftware::ChoiceDirectory()
{
    m_FilePath = "";
    AnsiString path = "";
    if( m_softwareInfo->Name != "" )
    {
        path =  GetPath( m_softwareInfo->Name );  //获取打包目录
        if( path == "" )
        {
            path = GetDefaultPath() + "\\" + m_softwareInfo->Name + "安装包制作环境";
        }
        if( !DirectoryExists( path ) )
        {
            path = GetDefaultPath();
        }
    }
    else
    {
        path = GetDefaultPath();
    }
    if( !DirectoryExists( path ) )
    {
        path = ExtractFilePath(Application->ExeName) + "安装包制作环境";
    }
    m_FilePath = path;
}

//拷贝配置文件到目录下
void __fastcall TBuildSoftware::CopyIniFile( AnsiString sourcePath, AnsiString destPath, AnsiString fileName )
{
    if( sourcePath !=  destPath )
    {
        //删除原来的
        SetFileAttributes( ( destPath + "\\" + fileName ).c_str(), FILE_ATTRIBUTE_NORMAL);
        DeleteFile( ( destPath + "\\" + fileName ).c_str() );

        //拷贝
        CopyFile( ( sourcePath + "\\" + fileName ).c_str(), ( destPath + "\\" + fileName ).c_str(), true );
    }
}

void __fastcall TBuildSoftware::CopyIniFile( AnsiString sourceName, AnsiString destName )
{
    if( sourceName !=  destName )
    {
        //删除原来的
        SetFileAttributes( destName.c_str(), FILE_ATTRIBUTE_NORMAL );
        DeleteFile( destName.c_str() );

        //拷贝
        CopyFile( sourceName.c_str(), destName.c_str(), true );
    }
}

//拷贝打包文件
AnsiString __fastcall TBuildSoftware::CopyWiseFile( AnsiString path, AnsiString fileName )
{
    AnsiString copyName = "复件 " + fileName;

    //删除原来的副本文件
    SetFileAttributes( ( path + "\\" + copyName ).c_str(), FILE_ATTRIBUTE_NORMAL);
    DeleteFile( ( path + "\\" + copyName ).c_str() );

    //拷贝一份新的副本
    CopyFile( ( path + "\\" + fileName ).c_str(), ( path + "\\" + copyName ).c_str(), true );

    return ( path + "\\" + copyName );
}

const static AnsiString s_BigVerStr = "_MyBigVer_";
const static AnsiString s_VerStr = "_MyVer_";
const static AnsiString s_ExeFileName = "_MyExeFileName_";

//修改版本号
void __fastcall TBuildSoftware::ChangeVer(AnsiString bigVerNum, AnsiString ver, AnsiString path,AnsiString SetupType)
{
    std::auto_ptr <TStrings> tmpStrings( new TStringList );

    //读入并修改新的副本再保存
    tmpStrings->LoadFromFile( path );
    tmpStrings->Text = StringReplace( tmpStrings->Text, s_BigVerStr, bigVerNum, TReplaceFlags() << rfReplaceAll );
    tmpStrings->Text = StringReplace( tmpStrings->Text, s_VerStr, ver, TReplaceFlags() << rfReplaceAll );

    //修改打包文件名称
    ReadSoftwareName();
    tmpStrings->Text = StringReplace( tmpStrings->Text, s_ExeFileName, m_softwareName, TReplaceFlags() << rfReplaceAll );
    //修改AppID
    String guid = GenerateGuidStr();
    tmpStrings->Text = StringReplace( tmpStrings->Text, "{2ACBD3A3-0116-4EF4-8497-722D81BD39D3}", guid, TReplaceFlags() << rfReplaceAll );

    int index;
    if(SetupType=="Inno")
    {
        index = tmpStrings->IndexOfName( "OutputBaseFilename" );
    }
    else
    {
        index = tmpStrings->IndexOfName( "  EXE Filename" );
    }
    if( index >= 0 )
    {
        if(SetupType=="Inno")
        {
            m_softwareInfo->InstallFileName =  tmpStrings->Values["OutputBaseFilename"]+".exe";
        }
        else
        {
            m_softwareInfo->InstallFileName =  tmpStrings->Values["  EXE Filename"];
        }
    }

    FILE * pFile = fopen( ( path ).c_str(), "wb" );
    if( !pFile )
    {
        ShowMessage( "写入安装包文件出错！" );
        return;
    }
    for( int i = 0; i < tmpStrings->Count; i++ )
    {
        fwrite( tmpStrings->Strings[i].c_str(), tmpStrings->Strings[i].Length(), 1, pFile );
        fwrite("\r\n", 2, 1, pFile);
    }
    fclose(pFile);
}

//打包程序
void __fastcall TBuildSoftware::Build()
{
    if( m_FilePath == "" )
    {
        return;
    }
    int fileExist = -1;    //检查是否有“Setup.wse”
    ReadSetupType();   //安装类型
    m_SetupType="Inno";
    do
    {
        if(m_SetupType=="Inno")
        {
            fileExist = CheckDirectory( m_FilePath, "Setup.iss" );
        }
        else
        {
            fileExist = CheckDirectory( m_FilePath, "Setup.wse" );
        }
        if( fileExist == 0 )    //用户选择取消
        {
            return;
        }
    }
    while( fileExist != 1 );
    AnsiString fileName;
    if(m_SetupType=="Inno")
    {
        fileName = CopyWiseFile( m_FilePath, "Setup.iss" );  //拷贝打包文件
    }
    else
    {
        fileName = CopyWiseFile( m_FilePath, "Setup.wse" );
    }
    ReadVersionInfo();   //修改版本号
    ChangeVer( m_verBigNum, m_ver, fileName ,m_SetupType);

    BuilderSetupSoftware( fileName , m_SetupType);   //打包

    Sleep(1000);
    std::auto_ptr<TIniFile> pIniFile ( new TIniFile( m_FilePath + "\\" + "RegInfo.ini") );
    AnsiString unitName = pIniFile->ReadString( "Public", "UnitName", "" );

    AfterBuild( unitName );
}

static char arr_cDesktopPath[MAX_PATH];
AnsiString TBuildSoftware::GetSavePath()
{
    LPITEMIDLIST pidl;
    SHGetSpecialFolderLocation( NULL, CSIDL_DESKTOP, &pidl );
    SHGetPathFromIDList( pidl, arr_cDesktopPath );

    std::auto_ptr<TIniFile> pIniFile( new TIniFile( ExtractFilePath( Application->ExeName ) + "\\打包环境位置.ini" ) );
    AnsiString path = pIniFile->ReadString( "拷贝位置", "位置", "" );
    AnsiString unit = GetUnitName();
    path = AnsiString( arr_cDesktopPath ) + "\\" + unit + path;

    return path;
}

void MyCreateDir( AnsiString dirPath )
{
    AnsiString dir = dirPath + "\\";
    AnsiString dirTmp = "";
    int pos = dirPath.Pos( "\\" );
    while( pos )
    {
        AnsiString str = dir.SubString( 1, pos - 1 );
        if( dirTmp != "" )
        {
            dirTmp += "\\";
        }
        dirTmp += str;
        if( !DirectoryExists( dirTmp ) )
        {
            CreateDir( dirTmp );
        }
        dir.Delete( 1, pos );
        pos = dir.Pos( "\\" );
    }
}

//打包完成
bool __fastcall TBuildSoftware::AfterBuild( AnsiString unit )
{

    bool flag = false;
    //if( FileExists( path ) )
    {
        std::auto_ptr<TIniFile> pIniFile( new TIniFile( ExtractFilePath( Application->ExeName ) + "\\打包环境位置.ini" ) );
        AnsiString path = pIniFile->ReadString( "拷贝位置", "位置", "" );
        static char arr_cDesktopPath[MAX_PATH];
        LPITEMIDLIST pidl;
        SHGetSpecialFolderLocation( NULL, CSIDL_DESKTOP, &pidl );
        SHGetPathFromIDList( pidl, arr_cDesktopPath );
        path = AnsiString( arr_cDesktopPath ) + "\\当前生成的" +unit+"软件";
        //复制到桌面
        if( !DirectoryExists( path ) )
        {
            MyCreateDir( path );
        }
        TIniFile *ini = new TIniFile(ExtractFilePath(Application->ExeName)+"打包环境位置.ini");
        AnsiString softpath = ini->ReadString("拷贝位置", "位置", "");   //  GetPath( m_softwareInfo->Name );
        AnsiString copath = path+ "\\" + m_softwareName + ".exe";
        softpath += "\\" + m_softwareName + ".exe";
        if( FileExists( softpath ) )
        {
            AnsiString delstring = "rd\/s\/q \""+path+"\\html\\\"";
            system( delstring.c_str() );
            Sleep(1000);
            //CopyIniFile( softpath, path , m_softwareName + ".exe" );
            CopyFile(softpath.c_str(),copath.c_str(),false);
            AnsiString cpyparam = "xcopy \""+ ExtractFilePath( Application->ExeName ) + "html\\*\"";
            cpyparam = cpyparam + " \""+ path +"\\html\\\" \/s\/e\/h\/r";
            WinExec(cpyparam.c_str(),SW_HIDE);

        }

        if( FileExists( copath ) )
        {
            ShowMessage( "知识管理系统软件生成完毕" );
            //删除生成位置的安装包
            SetFileAttributes( softpath.c_str(), FILE_ATTRIBUTE_NORMAL );
            DeleteFile( softpath.c_str() );
            //桌面上打开文件夹
            ShellExecute( NULL, "open", path.c_str(), NULL, NULL, SW_SHOWNORMAL );
            flag = true;
        }
        else
        {
            ShowMessage( "文件复制失败！" );
            flag = false;
        }

        m_softwareInfo->VerInfo = m_ver;
    }
    return flag;
}

//检查是否有“Setup.wse”
int __fastcall TBuildSoftware::CheckDirectory( AnsiString path, AnsiString fileName )
{
    if( !FileExists( path + "\\" + fileName ) )
    {
        ShowMessage( "没有在所选目录下检查到安装盘制作文件。"+path + "\\" + fileName );
        return 0;
    }
    return 1;
}

bool TBuildSoftware::CallAndWaitExternalProgram( AnsiString path, AnsiString param, PROCESS_INFORMATION & pi )
{
    if( path == "" )
    {
        return false;
    }
    ZeroMemory( &pi, sizeof(pi) );
    STARTUPINFO si;
    ZeroMemory( &si, sizeof(si) );

    si.cb = sizeof(STARTUPINFO);
    si.lpReserved = NULL;
    si.lpReserved2 = NULL;
    si.cbReserved2 = 0;
    si.lpDesktop = NULL;
    si.dwFlags = 0;
    si.dwFlags = STARTF_USESHOWWINDOW;
    si.wShowWindow = SW_HIDE;

    AnsiString tmp = path + " " + param;
    char pbuf[1024];
    strcpy( pbuf, tmp.c_str() );
    if( !CreateProcess( NULL, pbuf, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi ) )
	{
		return false;
	}
    else
    {
        WaitForSingleObject( pi.hProcess, INFINITE );
	    return true;
    }
}

//打包
bool __fastcall TBuildSoftware::BuilderSetupSoftware( AnsiString path ,AnsiString SetupType)
{
    //调用打包
    bool result = true;
    unsigned long ret = -1;
    PROCESS_INFORMATION pi;
    m_SetupType = SetupType;
    if(m_SetupType=="Inno")
    {
        AnsiString Path = exePath + "Inno Setup\\Compil32.exe";
        SetCurrentDir(Path);
        Path = "\"" + Path + "\"";
        path = "\"" + path + "\"";
        result = CallAndWaitExternalProgram( Path, " /cc " + path , pi );
    }
    else
    {
        result = CallAndWaitExternalProgram( "wise32.exe", " /c \"" + path + "\"", pi );
    }

    if( !result )
    {
        ShowMessage( "没有找到打包程序。" );
    }
    GetExitCodeProcess( pi.hProcess, &ret );

    return result;
}

//从配置文件中获取软件名称
void __fastcall TBuildSoftware::ReadSoftwareName()
{
    m_softwareName = GetUnitName();
    m_softwareName += GetSoftwareName();
}

AnsiString TBuildSoftware::GetSoftwareName()
{
    m_FilePath = ExtractFilePath( Application->ExeName );
    std::auto_ptr<TIniFile> pIniFile ( new TIniFile( m_FilePath + "\\" + "RegInfo.ini") );
    AnsiString name = pIniFile->ReadString( "Public", "AppName", "" );
    return name;
}

AnsiString TBuildSoftware::GetUnitName()
{
    //m_FilePath = ExtractFilePath( Application->ExeName ) + "安装包制作环境\\信号虚拟世界（工区）安装包制作环境";
    m_FilePath = ExtractFilePath( Application->ExeName );
    std::auto_ptr<TIniFile> pIniFile ( new TIniFile( m_FilePath + "\\" + "RegInfo.ini") );
    AnsiString unitName = pIniFile->ReadString( "Public", "UnitName", "" );
    return unitName;
}

//从配置文件中获取版本信息
void __fastcall TBuildSoftware::ReadVersionInfo()
{
    std::auto_ptr<TIniFile> pIniFile ( new TIniFile( m_FilePath + "\\" + "VerInfo.ini") );
    m_verBigNum = pIniFile->ReadString( "Version", "BigNum", "" );
    m_ver = " " + pIniFile->ReadString( "Version", "Num", "" );
}
//从配置文件中获取安装程序类型
void __fastcall TBuildSoftware::ReadSetupType()
{
    std::auto_ptr<TIniFile> pIniFile ( new TIniFile( m_FilePath + "\\" + "VerInfo.ini") );
    m_SetupType = pIniFile->ReadString( "Version", "SetUpType", "" );
}
//---------------------------------------------------------------------------
AnsiString TBuildSoftware::GenerateGuidStr()
{
    AnsiString str;
    GUID uid;
    CoCreateGuid(&uid);
    str = Sysutils::GUIDToString(uid);

    return str;
}
void TBuildSoftware::deletefile( String patch)
{
    if( patch != "")
    {
      SetFileAttributes( patch.c_str(), FILE_ATTRIBUTE_NORMAL );
      DeleteFile( patch.c_str() );
    }
}

void TBuildSoftware::SoftwarePublish() {
    // 读取当前公司信息
    String softwarename = this->GetSoftwareName();
    String unitname = this->GetUnitName();

    // 找到下属公司
    TADOQuery *AdoQ = new TADOQuery(NULL);
    if (unitname == "中北信号") {
        String sql = "select 名称 from BD_AU_单位字典表 where 单位编号 = 1";
        AdoQ->Connection = DMod->ADOConnection4;
        DMod->OpenSql(sql, AdoQ);
    }
    else {
        TADOQuery *tempQuery = new TADOQuery(NULL);
        String sql = "select 单位编号 from BD_AU_单位字典表 where 名称 = '" + unitname + "'";
        tempQuery->Connection = DMod->ADOConnection4;
        DMod->OpenSql(sql, tempQuery);
        if (tempQuery -> Eof) {
            ShowMessage("单位名称有误,请检查RegInfo.ini文件");
            delete tempQuery;
            return;
        }
        int u_id = tempQuery->FieldByName("单位编号")->AsInteger;
        delete tempQuery;
        sql = "select 名称 from BD_AU_单位字典表 where 上级单位 = " + IntToStr(u_id);
        AdoQ->Connection = DMod->ADOConnection4;
        DMod->OpenSql(sql, AdoQ);
    }

    // 发布
    Form3->ComboBox1->Clear();
    while(!AdoQ->Eof) {
        String u_name = AdoQ->FieldByName("名称")->AsString;
        Form3->ComboBox1->Items->Add(u_name);
        AdoQ->Next();
    }
    delete AdoQ;
    Form3->ShowModal();
}

void TBuildSoftware::Package(String unitName) {
    this->exePath = ExtractFilePath(Application->ExeName);
    this->m_softwareName = unitName;
    this->m_FilePath = this->exePath + "Package";
    // 复制到Package目录下
    CopyFolder(this->exePath, this->m_FilePath);

    // 修改 Backup.ini, RegInfo.ini
    TADOQuery *AdoQ = new TADOQuery(NULL);
    String sql = "select 单位编号, 单位级别 from BD_AU_单位字典表 where 名称 = '" + unitName + "'";
    AdoQ->Connection = DMod->ADOConnection4;
    DMod->OpenSql(sql, AdoQ);
    String unitlevel = AdoQ->FieldByName("单位级别")->AsString;
    int unitID = AdoQ->FieldByName("单位编号")->AsInteger;
    auto_ptr<TIniFile> pIniFile ( new TIniFile( this->m_FilePath + "\\" + "RegInfo.ini") );
    m_verBigNum = pIniFile->ReadString("版本号", "VerNum", "1.0");
    pIniFile->DeleteKey("Public", "UnitLevel");
    pIniFile->DeleteKey("Public", "UnitName");
    pIniFile->DeleteKey("Public", "UnitID");
    pIniFile->WriteString("Public", "UnitLevel", unitlevel);
    pIniFile->WriteString("Public", "UnitName", unitName);
    pIniFile->WriteString("Public", "UnitID", unitID);

    // 修改Raw.iss并存入Setup.iss
    string file_path = StringTostring(this->m_FilePath) + "\\Raw.iss";
    string out_path = StringTostring(this->m_FilePath) + "\\Setup.iss";
    string str;
    string::size_type pos = 0;
    ifstream instream;
    ofstream outstream;
    instream.open(file_path.c_str());
    if (!instream) {
        ShowMessage("找不到Raw.iss文件");
        return;
    }
    outstream.open(out_path.c_str());
    while (getline(instream, str)) {
        pos = str.find("MY_APP_NAME");
        if (pos != string::npos)
            str.replace(pos, 11, StringTostring(unitName) + "集中监测标准知识库");
        pos = str.find("MY_APP_VERSION");
        if (pos != string::npos)
            str.replace(pos, 14, StringTostring(m_verBigNum));
        pos = str.find("MY_APP_PUBLISHER");
        if (pos != string::npos)
            str.replace(pos, 16, StringTostring(unitName));
        pos = str.find("MY_APP_EXE_NAME");
        if (pos != string::npos)
            str.replace(pos, 15, "KBS.exe");
        CoInitialize(NULL);
        string appid = newGUID();
        pos = str.find("APP_ID");
        if (pos != string::npos)
            str.replace(pos, 6, "{" + appid);
        pos = str.find("SOURCE_EXE_PATH");
        if (pos != string::npos)
            str.replace(pos, 15, StringTostring(m_FilePath) + "\\KBS.exe");
        pos = str.find("SOURCE_PATH");
        if (pos != string::npos)
            str.replace(pos, 11, StringTostring(m_FilePath));
        outstream << str <<endl;
    }
    instream.close();
    outstream.close();

    //调用Setup.iss
    bool result = true;
    unsigned long ret = -1;
    PROCESS_INFORMATION pi;
    AnsiString Path = exePath + "Inno Setup\\Compil32.exe";
    AnsiString path = m_FilePath + "\\Setup.iss";
    SetCurrentDir(Path);
    Path = "\"" + Path + "\"";
    path = "\"" + path + "\"";
    result = CallAndWaitExternalProgram( Path, " /cc " + path , pi );
    GetExitCodeProcess( pi.hProcess, &ret );
    CopyFile((this->m_FilePath + "\\Output\\Setup.exe").c_str(), (this->exePath + "\\Output\\Setup.exe").c_str(), 0);
    DeleteFolder(this->m_FilePath);
    RemoveDir(this->m_FilePath);
}

void __fastcall TBuildSoftware::CopyFolder(String srcPath, String aimPath) {
    TSearchRec sr;
    if (!DirectoryExists(srcPath)) {
        return ;
    }
    if (!DirectoryExists(aimPath)) {
   		ForceDirectories(aimPath);
    }
    if (FindFirst(srcPath + "//*.*", faAnyFile, sr) == 0) {
        do {
            try {
                if ((sr.Attr & faDirectory) != 0) {
                    //folder
                    if (sr.Name != "." && sr.Name != ".." && sr.Name != "Package" && sr.Name != "Output") {
                        CopyFolder(srcPath+"//"+sr.Name, aimPath+"//"+sr.Name);
                    }
                }
                else {
                    //file
                    CopyFile((srcPath + "//" + sr.Name).c_str(),
                             (aimPath + "//" + sr.Name).c_str(),
                              0);
                }
            }catch(...){}
        } while (FindNext(sr) == 0);
        FindClose(sr);
    }
}

void __fastcall TBuildSoftware::DeleteFolder(String srcPath) {
    TSearchRec sr;
    if (!DirectoryExists(srcPath)) {
        return ;
    }
    if (FindFirst(srcPath + "//*.*", faAnyFile, sr) == 0) {
        do {
            try {
                if ((sr.Attr & faDirectory) != 0) {
                    //folder
                    if (sr.Name != "." && sr.Name != "..") {
                        DeleteFolder(srcPath+"//"+sr.Name);
                        RemoveDir(srcPath+"//"+sr.Name);
                    }
                }
                else {
                    //file
                    DeleteFile((srcPath + "//" + sr.Name).c_str());
                }
            }catch(...){}
        } while (FindNext(sr) == 0);
        FindClose(sr);
    }
}

//---------------------------------------------------------------------------
#pragma package(smart_init)
