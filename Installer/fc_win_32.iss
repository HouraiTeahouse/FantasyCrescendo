; Setup Config file for Windows Installer
; Note this is only for 32-bit installations
; Uses the Inno Setup program to compile a *.exe file to install the program
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define ARCH            'x86'
#define BITS            '32'
#define ARCH_ALLOWED    'x86 x64'

#include "fc_win_universal.iss"
