;RepostConfirmationCanceler Setup--

[Setup]
AppName=RepostConfirmationCanceler
AppVerName=RepostConfirmationCanceler
VersionInfoVersion=1.1.0.0
AppVersion=1.1.0.0
AppMutex=RepostConfirmationCancelerSetup
;DefaultDirName=C:\RepostConfirmationCanceler
DefaultDirName={code:GetProgramFiles}\RepostConfirmationCanceler
Compression=lzma2
SolidCompression=yes
OutputDir=SetupOutput
OutputBaseFilename=RepostConfirmationCancelerSetup_x64
AppPublisher=RepostConfirmationCanceler
WizardImageStretch=no
VersionInfoDescription=RepostConfirmationCancelerSetup
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
DefaultGroupName=RepostConfirmationCanceler
UninstallDisplayIcon={app}\RepostConfirmationCanceler.exe

[Registry]
Root: HKLM; Subkey: "Software\RepostConfirmationCanceler"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\RepostConfirmationCanceler"; ValueType: string; ValueName: "Path"; ValueData: "{app}\"
Root: HKLM; Subkey: "Software\RepostConfirmationCanceler"; ValueType: string; ValueName: "ClientType"; ValueData: ""
Root: HKLM; Subkey: "Software\RepostConfirmationCanceler"; ValueType: string; ValueName: "Version"; ValueData: "1.1.0.0"
Root: HKLM; Subkey: "Software\RepostConfirmationCanceler"; ValueType: string; ValueName: "Rulefile"; ValueData: "{app}\RepostConfirmationCanceler.ini"
Root: HKLM; Subkey: "Software\RepostConfirmationCanceler"; ValueType: string; ValueName: "RCAPfile"; ValueData: "{app}\ResourceCap.ini"
Root: HKLM; Subkey: "Software\RepostConfirmationCanceler"; ValueType: string; ValueName: "ExtensionExecfile"; ValueData: "{app}\RepostConfirmationCanceler.exe"

Root: HKLM; Subkey: "Software\WOW6432Node\RepostConfirmationCanceler"; Flags: uninsdeletekey
Root: HKLM; Subkey: "Software\WOW6432Node\RepostConfirmationCanceler"; ValueType: string; ValueName: "Path"; ValueData: "{app}\"
Root: HKLM; Subkey: "Software\WOW6432Node\RepostConfirmationCanceler"; ValueType: string; ValueName: "ClientType"; ValueData: ""
Root: HKLM; Subkey: "Software\WOW6432Node\RepostConfirmationCanceler"; ValueType: string; ValueName: "Version"; ValueData: "1.1.0.0"
Root: HKLM; Subkey: "Software\WOW6432Node\RepostConfirmationCanceler"; ValueType: string; ValueName: "Rulefile"; ValueData: "{app}\RepostConfirmationCanceler.ini"
Root: HKLM; Subkey: "Software\WOW6432Node\RepostConfirmationCanceler"; ValueType: string; ValueName: "RCAPfile"; ValueData: "{app}\ResourceCap.ini"
Root: HKLM; Subkey: "Software\WOW6432Node\RepostConfirmationCanceler"; ValueType: string; ValueName: "ExtensionExecfile"; ValueData: "{app}\RepostConfirmationCanceler.exe"


;Edge
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Edge\NativeMessagingHosts\com.clear_code.repost_confirmation_canceler"; Flags: uninsdeletekey
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Edge\NativeMessagingHosts\com.clear_code.repost_confirmation_canceler"; ValueType: string; ValueData: "{app}\RepostConfirmationCancelerHost\edge.json";

[Languages]
Name: jp; MessagesFile: "compiler:Languages\Japanese.isl"


[Files]
;exe
Source: "bin\Release\RepostConfirmationCanceler.exe"; DestDir: "{app}\";Flags: ignoreversion;permissions:users-readexec admins-full system-full
;ini
Source: "Resources\RepostConfirmationCanceler.ini"; DestDir: "{app}"; Flags: onlyifdoesntexist

;host
Source: "bin\x64\Release\RepostConfirmationCancelerTalk.exe"; DestDir: "{app}\RepostConfirmationCancelerHost";Flags: ignoreversion;permissions:users-readexec admins-full system-full

;edge
Source: "Resources\edge.json"; DestDir: "{app}\RepostConfirmationCancelerHost";Flags: ignoreversion;permissions:users-readexec admins-full system-full

[Dirs]
Name: "{app}";Permissions: users-modify

[Run] 
Filename: "{sys}\icacls.exe";Parameters: """{app}\RepostConfirmationCanceler.exe"" /inheritance:r"; Flags: runhidden shellexec
Filename: "{sys}\icacls.exe";Parameters: """{app}\RepostConfirmationCancelerHost\RepostConfirmationCancelerTalk.exe"" /inheritance:r"; Flags: runhidden shellexec
Filename: "{sys}\icacls.exe";Parameters: """{app}\RepostConfirmationCancelerHost\edge.json"" /inheritance:r"; Flags: runhidden shellexec

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
	TaskKill('RepostConfirmationCanceler.exe');
	Result := True; 
end; 
