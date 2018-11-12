; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName ""
#define MyAppVersion ""
#define MyAppPublisher ""
#define MyAppExeName ""
#define MyAppId ""

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=Setup
;SetupIconFile=\ZBXH.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Messages]
UninstalledMost=%1 ��˳���ش����ĵ�����ɾ����

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: ""; DestDir: "{app}"; Flags: ignoreversion
Source: "\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
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
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

[registry]
Root:HKCU;Subkey:"";Flags:uninsdeletekeyifempty
Root:HKCU;Subkey:"";ValueType:string;ValueName:"InstallPath";ValueData:"{app}";Flags:uninsdeletekey
Root:HKCU;Subkey:"";ValueType:string;ValueName:"AppName";ValueData:"{#MyAppName}";Flags:uninsdeletekey

[code]
function InitializeSetup(): Boolean;
var sInstallPath: String;
var sAppName: String;
begin 
      if RegValueExists(HKEY_CURRENT_USER,'', 'InstallPath') then
               begin 
                   RegQueryStringValue(HKEY_CURRENT_USER, '', 'AppName', sAppName);
                   MsgBox('�ü�����Ѿ���װͬ���������'+sAppName+'��,����ж��Ȼ��װ,��װ���򽫹رա�',mbError,MB_OK);
                   result:=false;
               end else
               begin result:=true;
               end;
end;

procedure CurUninstallStepChanged(CurUninstallStep : TUninstallStep);
begin
     if CurUninstallStep= usUninstall then
     RegDeleteKeyIncludingSubkeys(HKEY_CURRENT_USER,'');
end;