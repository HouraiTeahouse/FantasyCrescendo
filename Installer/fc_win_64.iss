; Setup Config file for Windows Installer
; Note this is only for 64-bit installations
; Uses the Inno Setup program to compile a *.exe file to install the program
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{C9903768-2797-4CEB-B635-2381570C6BE9}
AppName=Fantasy Crescendo
AppVersion=1.0.0
AppVerName=Fantasy Crescendo
AppPublisher=Hourai Teahouse
AppPublisherURL=http://houraiteahouse.net
AppSupportURL=http://houraiteahouse.net
AppUpdatesURL=http://houraiteahouse.net
DefaultDirName={pf}\Fantasy Crescendo
DefaultGroupName=Fantasy Crescendo
AllowNoIcons=yes
LicenseFile=..\LICENSE
OutputBaseFilename=FC_x86_Setup
Compression=lzma/ultra64
SolidCompression=yes
OutputDir=..\Build\Win64
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
; Universal Items
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion;
Source: "..\LICENSE"; DestDir: "{app}"; Flags: ignoreversion;
Source: "..\CREDITS.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\CONTRIBUTING.md"; DestDir: "{app}"; Flags: ignoreversion

; 64-bit install
Source: "..\Build\Win64\fc.exe"; DestDir: "{app}"; Flags: ignoreversion;      
Source: "..\Build\Win64\fc_Data\*.*"; DestDir: "{app}\fc_Data"; Flags: recursesubdirs ignoreversion;

[Icons]
Name: "{group}\Fantasy Crescendo"; Filename: "{app}\fc.exe"
Name: "{commondesktop}\Fantasy Crescendo"; Filename: "{app}\fc.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Fantasy Crescendo"; Filename: "{app}\fc.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\fc.exe"; Description: "{cm:LaunchProgram,Fantasy Crescendo}"; Flags: nowait postinstall skipifsilent

