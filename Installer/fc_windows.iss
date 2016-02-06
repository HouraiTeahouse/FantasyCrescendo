; Setup Config file for Windows Installer
; Uses the Inno Setup program to compile a *.exe file to install the program
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{C9903768-2797-4CEB-B635-2381570C6BE9}
AppName=Fantasy Crescendo
AppVersion=1.0.0
;AppVerName=Fantasy Crescendo 1.0.0
AppPublisher=Hourai Teahouse
AppPublisherURL=http://houraiteahouse.net
AppSupportURL=http://houraiteahouse.net
AppUpdatesURL=http://houraiteahouse.net
DefaultDirName={pf}\Fantasy Crescendo
DefaultGroupName=Fantasy Crescendo
AllowNoIcons=yes
LicenseFile=D:\Users\james\Documents\Github\FantasyCrescendo\LICENSE
OutputBaseFilename=setup
Compression=lzma/ultra64
SolidCompression=yes

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
Source: "C:\Program Files (x86)\Inno Setup 5\Examples\fc.exe"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\Fantasy Crescendo"; Filename: "{app}\fc.exe"
Name: "{commondesktop}\Fantasy Crescendo"; Filename: "{app}\fc.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Fantasy Crescendo"; Filename: "{app}\fc.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\fc.exe"; Description: "{cm:LaunchProgram,Fantasy Crescendo}"; Flags: nowait postinstall skipifsilent

