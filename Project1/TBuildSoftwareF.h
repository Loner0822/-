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

    //����Ƿ��С���װ��.wse��
    int __fastcall CheckDirectory( AnsiString path, AnsiString fileName );

    //�������ļ��л�ȡ��װ��������
    void __fastcall ReadSetupType();
    //���������ļ���Ŀ¼��
    void __fastcall CopyIniFile( AnsiString sourcePath, AnsiString destPath, AnsiString fileName );
    void __fastcall CopyIniFile( AnsiString sourceName, AnsiString destName );

    //��������ļ�
    AnsiString __fastcall CopyWiseFile( AnsiString path, AnsiString fileName );

    AnsiString m_ver;
    AnsiString m_verBigNum;
    //�������ļ��л�ȡ�汾��Ϣ
    void __fastcall ReadVersionInfo();
    //�޸İ汾��
    void __fastcall ChangeVer( AnsiString bigVerNum, AnsiString ver, AnsiString path,AnsiString SetupType);

    //���
    bool __fastcall BuilderSetupSoftware( AnsiString path ,AnsiString SetupType);
public:
    //������
    bool __fastcall AfterBuild(AnsiString unit);
private:
    //�������ļ��л�ȡ�������
    void __fastcall ReadSoftwareName();

   // AnsiString GetSavePath(); ��ʱ����public��

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
 