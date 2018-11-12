; 脚本由 Inno Setup 脚本向导 生成！
; 有关创建 Inno Setup 脚本文件的详细资料请查阅帮助文档！

#define MyAppName "服务器数据备份与恢复"
#define MyAppVersion "1.00.00"
#define MyAppExeName "Server.exe"

[Setup]
; 注: AppId的值为单独标识该应用程序。
; 不要为其他安装程序使用相同的AppId值。
; (生成新的GUID，点击 工具|在IDE中生成GUID。)
AppId={{37F3859D-5C22-4AEC-822D-4D3065B02322}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
DefaultDirName={pf}\{#MyAppName}  
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=服务器数据备份与恢复安装包
SetupIconFile=D:\工作\2018.10.09(图标)\公司图标2.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Default.isl"

[Messages]
UninstalledMost=%1 已顺利地从您的电脑中删除。

[Files]Source: "D:\工作\服务器上传_下载\Server(打包)\Server.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "D:\工作\服务器上传_下载\Server(打包)\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; 注意: 不要在任何共享系统文件上使用“Flags: ignoreversion”

[UninstallDelete]
Name: {app}; Type: filesandordirs

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "Server"; ValueData: "{app}\{#MyAppExeName}"; Flags: uninsdeletevalue
[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}";Flags: nowait
