; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "集中监测标准知识库"
#define MyAppVersion "1.0"
#define MyAppPublisher "中北信号"
#define MyAppExeName "KBS.exe"

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{3667F85E-B26D-46F1-9E76-88F544F775FE}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputBaseFilename=中北信号集中监测标准知识库安装包
SetupIconFile=D:\工作\2018.10.09(图标)\公司图标2.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Messages]
UninstalledMost=%1 已顺利地从您的电脑中删除。

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "D:\工作\集中监测标准知识库\KBS.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\工作\集中监测标准知识库\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”

[Run]
; 安装完调用
Filename: "{app}\RegOcx.exe";
Filename: "{app}\ZskAz.exe";

[UninstallRun]
; 卸载前调用
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
                   MsgBox('该计算机已经安装同类型软件《'+sAppName+'》,请先卸载然后安装,安装程序将关闭。',mbError,MB_OK);
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