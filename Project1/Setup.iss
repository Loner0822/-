; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "���м���׼֪ʶ��"
#define MyAppVersion "1.0"
#define MyAppPublisher "�б��ź�"
#define MyAppExeName "KBSTree.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
AppId={{4D9CE8D5-A5B3-4770-9E1F-A49BCA9AA37D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=Setup
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
Source: "D:\����\���м���׼֪ʶ��(���)\KBSTree.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\����\���м���׼֪ʶ��(���)\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[Run]
; ��װ�����
Filename: "{app}\ControlUnit\RegOcx.exe";
Filename: "{app}\Backup\ZskAz.exe";

[UninstallRun]
; ж��ǰ����
Filename: "{app}\Backup\kill.bat";Flags:RunHidden SkipIfDoesntExist;
Filename: "{app}\Backup\ZskXz.exe";Flags:RunHidden SkipIfDoesntExist;

[UninstallDelete]
Name: {app}; Type: filesandordirs


[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent

