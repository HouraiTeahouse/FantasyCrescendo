; Setup Config file for Windows Installer
; Note this is only for 64-bit installations
; Uses the Inno Setup program to compile a *.exe file to install the program
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define APP_NAME      'Fantasy Crescendo'
#define APP_VERSION   '0.3.0-alpha'
#define BUILD_NAME    'fc'

#define README        '..\README.md'
#define LICENSE       '..\LICENSE'

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{C9903768-2797-4CEB-B635-2381570C6BE9}
AppName={#APP_NAME}
AppVersion={#APP_VERSION}
AppVerName={#APP_NAME}
AppPublisher=Hourai Teahouse
AppPublisherURL=http://houraiteahouse.net
AppSupportURL=http://houraiteahouse.net
AppUpdatesURL=http://houraiteahouse.net
DefaultDirName={pf}\{#APP_NAME}
DefaultGroupName={#APP_NAME}
AllowNoIcons=yes
AppReadmeFile={#README}
LicenseFile={#LICENSE}
OutputBaseFilename=FC_{#ARCH}_Setup_v{#APP_VERSION}
WizardImageFile=installer_image.bmp
WizardSmallImageFile=small_installer_image.bmp
WizardImageStretch=no
Compression=lzma/ultra64
SolidCompression=yes
OutputDir=..\Build\Win{#BITS}
ArchitecturesAllowed={#ARCH_ALLOWED}
#if BITS == '64'
ArchitecturesInstallIn64BitMode={#ARCH}
#endif

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

[Dirs]
Name: "{app}"; Flags: uninsalwaysuninstall;

[Files]
; Universal Items
Source: "{#README}"; DestDir: "{app}"; Flags: ignoreversion;
Source: "{#LICENSE}"; DestDir: "{app}"; Flags: ignoreversion;
Source: "..\CREDITS.md"; DestDir: "{app}"; Flags: ignoreversion

; Architecture specfic files
Source: "..\Build\Win{#BITS}\{#BUILD_NAME}.exe"; DestDir: "{app}"; Flags: ignoreversion;      
Source: "..\Build\Win{#BITS}\{#BUILD_NAME}_Data\*.*"; DestDir: "{app}\{#BUILD_NAME}_Data"; Flags: recursesubdirs ignoreversion;

[Icons]
Name: "{group}\{#APP_NAME}"; Filename: "{app}\{#BUILD_NAME}.exe"
Name: "{commondesktop}\{#APP_NAME}"; Filename: "{app}\{#BUILD_NAME}.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#APP_NAME}"; Filename: "{app}\{#BUILD_NAME}.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#BUILD_NAME}.exe"; Description: "{cm:LaunchProgram,Fantasy Crescendo}"; Flags: nowait postinstall skipifsilent

