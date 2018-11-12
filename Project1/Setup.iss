; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "���м���׼֪ʶ��"
#define MyAppVersion "1.0"
#define MyAppPublisher "�б��ź�"
#define MyAppExeName "KBS.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
AppId={{3667F85E-B26D-46F1-9E76-88F544F775FE}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=�б��źż��м���׼֪ʶ�ⰲװ��
SetupIconFile=D:\����\2018.10.09(ͼ��)\��˾ͼ��2.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Messages]
UninstalledMost=%1 ��˳���ش����ĵ�����ɾ����

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "D:\����\���м���׼֪ʶ��\KBS.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\����\���м���׼֪ʶ��\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[Run]
; ��װ�����
Filename: "{app}\RegOcx.exe";
Filename: "{app}\ZskAz.exe";

[UninstallRun]
; ж��ǰ����
Filename: "{app}\kill.bat";Flags:RunHidden SkipIfDoesntExist;
Filename: "{app}\ZskXz.exe";Flags:RunHidden SkipIfDoesntExist;

[UninstallDelete]
Name: {app}; Type: filesandordirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

[registry]
Root:HKCU;Subkey:"JZJCBZZSK0";Flags:uninsdeletekeyifempty
Root:HKCU;Subkey:"JZJCBZZSK0";ValueType:string;ValueName:"InstallPath";ValueData:"{app}";Flags:uninsdeletekey
Root:HKCU;Subkey:"JZJCBZZSK0";ValueType:string;ValueName:"AppName";ValueData:"{#MyAppName}";Flags:uninsdeletekey

[code]
function InitializeSetup(): Boolean;
var sInstallPath: String;
var sAppName: String;
begin 
      if RegValueExists(HKEY_CURRENT_USER,'JZJCBZZSK0', 'InstallPath') then
               begin 
                   RegQueryStringValue(HKEY_CURRENT_USER, 'JZJCBZZSK0', 'AppName', sAppName);
                   MsgBox('�ü�����Ѿ���װͬ���������'+sAppName+'��,����ж��Ȼ��װ,��װ���򽫹رա�',mbError,MB_OK);
                   result:=false;
               end else
               begin result:=true;
               end;
end;

procedure CurUninstallStepChanged(CurUninstallStep : TUninstallStep);
begin
     if CurUninstallStep= usUninstall then
     RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER,'JZJCBZZSK0');
end;