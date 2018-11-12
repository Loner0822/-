; �ű��� Inno Setup �ű��� ���ɣ�
; �йش��� Inno Setup �ű��ļ�����ϸ��������İ����ĵ���

#define MyAppName "���������ݱ�����ָ�"
#define MyAppVersion "1.00.00"
#define MyAppExeName "Server.exe"

[Setup]
; ע: AppId��ֵΪ������ʶ��Ӧ�ó���
; ��ҪΪ������װ����ʹ����ͬ��AppIdֵ��
; (�����µ�GUID����� ����|��IDE������GUID��)
AppId={{37F3859D-5C22-4AEC-822D-4D3065B02322}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
DefaultDirName={pf}\{#MyAppName}  
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=���������ݱ�����ָ���װ��
SetupIconFile=D:\����\2018.10.09(ͼ��)\��˾ͼ��2.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Messages]
UninstalledMost=%1 ��˳���ش����ĵ�����ɾ����

[Files]Source: "D:\����\�������ϴ�_����\Server(���)\Server.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\����\�������ϴ�_����\Server(���)\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; ע��: ��Ҫ���κι���ϵͳ�ļ���ʹ�á�Flags: ignoreversion��

[UninstallDelete]
Name: {app}; Type: filesandordirs

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "Server"; ValueData: "{app}\{#MyAppExeName}"; Flags: uninsdeletevalue
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}";Flags: nowait
