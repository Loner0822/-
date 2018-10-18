//---------------------------------------------------------------------------

#ifndef TBuildSoftwareFH
#define TBuildSoftwareFH
class TSoftwareInfo
{
public:
    AnsiString Name;
    AnsiString InstallFileName;
    AnsiString VerInfo;

    TSoftwareInfo()
        : Name(""), VerInfo(""), InstallFileName("")
    {}

    void Clear()
    {
        Name = "";
        VerInfo = "";
        InstallFileName = "";
    }
};

class TBuildSoftware
{
private:
    TSoftwareInfo * m_softwareInfo;
    AnsiString m_installFileName;
    AnsiString m_softwareName;
    AnsiString m_FilePath;
    AnsiString m_SetupType;
    void __fastcall ChoiceDirectory();
    void __fastcall Build();

    //检查是否有“安装包.wse”
    int __fastcall CheckDirectory( AnsiString path, AnsiString fileName );

    //从配置文件中获取安装程序类型
    void __fastcall ReadSetupType();
    //拷贝配置文件到目录下
    void __fastcall CopyIniFile( AnsiString sourcePath, AnsiString destPath, AnsiString fileName );
    void __fastcall CopyIniFile( AnsiString sourceName, AnsiString destName );

    //拷贝打包文件
    AnsiString __fastcall CopyWiseFile( AnsiString path, AnsiString fileName );

    AnsiString m_ver;
    AnsiString m_verBigNum;
    //从配置文件中获取版本信息
    void __fastcall ReadVersionInfo();
    //修改版本号
    void __fastcall ChangeVer( AnsiString bigVerNum, AnsiString ver, AnsiString path,AnsiString SetupType);

    //打包
    bool __fastcall BuilderSetupSoftware( AnsiString path ,AnsiString SetupType);
public:
    //打包完成
    bool __fastcall AfterBuild(AnsiString unit);
private:
    //从配置文件中获取软件名称
    void __fastcall ReadSoftwareName();

   // AnsiString GetSavePath(); 暂时移入public中

    //bool CallAndWaitExternalProgram( AnsiString path, AnsiString param, PROCESS_INFORMATION & pi );


    AnsiString GetSoftwareName();
    
    AnsiString GetUnitName();

    AnsiString GenerateGuidStr();
    
public:
    TBuildSoftware();
    ~TBuildSoftware();
    AnsiString GetSavePath();
    const TSoftwareInfo * BuildSoftware( AnsiString softwareName );
    bool CallAndWaitExternalProgram( AnsiString path, AnsiString param, PROCESS_INFORMATION & pi );

    void deletefile( String patch);
    String exePath;
    int Department_Level;
    
    void SoftwarePublish();
    void Package(String unitName);
    void __fastcall CopyFolder(String srcPath, String aimPath);
    void __fastcall DeleteFolder(String srcPath);
};
extern TBuildSoftware * BuildSoftware;
//---------------------------------------------------------------------------
#endif
 