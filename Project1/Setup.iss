; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "集中监测标准知识库"
#define MyAppVersion "1.0"
#define MyAppPublisher "中北信号"
#define MyAppExeName "KBSTree.exe"

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{4D9CE8D5-A5B3-4770-9E1F-A49BCA9AA37D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputBaseFilename=Setup
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
Source: "D:\工作\集中监测标准知识库(打包)\KBSTree.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\工作\集中监测标准知识库(打包)\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”

[Run]
; 安装完调用
Filename: "{app}\ControlUnit\RegOcx.exe";
Filename: "{app}\Backup\ZskAz.exe";

[UninstallRun]
; 卸载前调用
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

