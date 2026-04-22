;BrowserStartupLauncher Setup--

[Setup]
AppName=BrowserStartupLauncher
AppVerName=BrowserStartupLauncher
VersionInfoVersion=1.2.3.0
AppVersion=1.2.3.0
AppMutex=BrowserStartupLauncherSetup
;DefaultDirName=C:\BrowserStartupLauncher
DefaultDirName={code:GetProgramFiles}\BrowserStartupLauncher
Compression=lzma2
SolidCompression=yes
OutputDir=SetupOutput
OutputBaseFilename=BrowserStartupLauncherSetup_x64
AppPublisher=BrowserStartupLauncher
WizardImageStretch=no
VersionInfoDescription=BrowserStartupLauncherSetup
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
DefaultGroupName=BrowserStartupLauncher
UninstallDisplayIcon={app}\BrowserStartupLauncher.exe

[Registry]
Root: HKLM; Subkey: "Software\BrowserStartupLauncher"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\BrowserStartupLauncher"; ValueType: string; ValueName: "Path"; ValueData: "{app}\"
Root: HKLM; Subkey: "Software\BrowserStartupLauncher"; ValueType: string; ValueName: "ClientType"; ValueData: ""
Root: HKLM; Subkey: "Software\BrowserStartupLauncher"; ValueType: string; ValueName: "Version"; ValueData: "1.2.3.0"
Root: HKLM; Subkey: "Software\BrowserStartupLauncher"; ValueType: string; ValueName: "Rulefile"; ValueData: "{app}\BrowserStartupLauncher.ini"
Root: HKLM; Subkey: "Software\BrowserStartupLauncher"; ValueType: string; ValueName: "RCAPfile"; ValueData: "{app}\ResourceCap.ini"
Root: HKLM; Subkey: "Software\BrowserStartupLauncher"; ValueType: string; ValueName: "ExtensionExecfile"; ValueData: "{app}\BrowserStartupLauncher.exe"

Root: HKLM; Subkey: "Software\WOW6432Node\BrowserStartupLauncher"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\WOW6432Node\BrowserStartupLauncher"; ValueType: string; ValueName: "Path"; ValueData: "{app}\"
Root: HKLM; Subkey: "Software\WOW6432Node\BrowserStartupLauncher"; ValueType: string; ValueName: "ClientType"; ValueData: ""
Root: HKLM; Subkey: "Software\WOW6432Node\BrowserStartupLauncher"; ValueType: string; ValueName: "Version"; ValueData: "1.2.3.0"
Root: HKLM; Subkey: "Software\WOW6432Node\BrowserStartupLauncher"; ValueType: string; ValueName: "Rulefile"; ValueData: "{app}\BrowserStartupLauncher.ini"
Root: HKLM; Subkey: "Software\WOW6432Node\BrowserStartupLauncher"; ValueType: string; ValueName: "RCAPfile"; ValueData: "{app}\ResourceCap.ini"
Root: HKLM; Subkey: "Software\WOW6432Node\BrowserStartupLauncher"; ValueType: string; ValueName: "ExtensionExecfile"; ValueData: "{app}\BrowserStartupLauncher.exe"


;Edge
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Edge\NativeMessagingHosts\com.clear_code.browser_startup_launcher"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Edge\NativeMessagingHosts\com.clear_code.browser_startup_launcher"; ValueType: string; ValueData: "{app}\BrowserStartupLauncherHost\edge.json";

[Languages]
Name: jp; MessagesFile: "compiler:Languages\Japanese.isl"


[Files]
;exe
Source: "bin\Release\BrowserStartupLauncher.exe"; DestDir: "{app}\";Flags: ignoreversion;permissions:users-readexec admins-full system-full
;ini
Source: "Resources\BrowserStartupLauncher.ini"; DestDir: "{app}"; Flags: onlyifdoesntexist

;host
Source: "bin\x64\Release\BrowserStartupLauncherTalk.exe"; DestDir: "{app}\BrowserStartupLauncherHost";Flags: ignoreversion;permissions:users-readexec admins-full system-full

;edge
Source: "Resources\edge.json"; DestDir: "{app}\BrowserStartupLauncherHost";Flags: ignoreversion;permissions:users-readexec admins-full system-full

[Dirs]
Name: "{app}";Permissions: users-modify

[Run] 
Filename: "{sys}\icacls.exe";Parameters: """{app}\BrowserStartupLauncher.exe"" /inheritance:r"; Flags: runhidden shellexec
Filename: "{sys}\icacls.exe";Parameters: """{app}\BrowserStartupLauncherHost\BrowserStartupLauncherTalk.exe"" /inheritance:r"; Flags: runhidden shellexec
Filename: "{sys}\icacls.exe";Parameters: """{app}\BrowserStartupLauncherHost\edge.json"" /inheritance:r"; Flags: runhidden shellexec

[UninstallRun]

[Code]
function GetProgramFiles(Param: string): string;
  begin
    if IsWin64 then Result := ExpandConstant('{pf64}')
    else Result := ExpandConstant('{pf32}')
  end;

procedure TaskKill(FileName: String);
var
  ResultCode: Integer;
begin
    Exec(ExpandConstant('taskkill.exe'), '/f /im ' + '"' + FileName + '"', '', SW_HIDE,ewWaitUntilTerminated, ResultCode);
end;
function InitializeSetup():Boolean;
begin 
	TaskKill('msedge.exe');
	TaskKill('BrowserStartupLauncher.exe');
	Result := True; 
end; 
