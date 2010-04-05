; Installer for VPNKeepAlive
; Ben Scott / Belfry Images
; http://blog.belfryimages.com.au
; http://github.com/belfryimages/VPNKeepAlive
; This script requires the KillProc plugin for NSIS: http://nsis.sourceforge.net/KillProc_plug-in

Name "VPNKeepAlive"
OutFile "VPNKeepAlive_Installer.exe"
RequestExecutionLevel admin

InstallDir "$PROGRAMFILES\Belfry Images VPNKeepAlive\"

Page directory
Page instfiles

Section ""
	; Prompt to kill running app
	MessageBox MB_Ok "Please close VPNKeepAlive before continuing"
	StrCpy $0 "VPNKeepAlive.exe"
	KillProc::KillProcesses
	Sleep 2000
	
	; Copy files
	SetOutPath "$INSTDIR"
	File "VPNKeepAlive\bin\Release\VPNKeepAlive.exe"
	File "README.txt"
	
	; Write the uninstaller
	WriteUninstaller "$INSTDIR\Uninstall.exe"
	
	; Create shortcuts
	MessageBox MB_YESNO "Do you want to create a folder in the start menu?" IDNO skipstartmenu
		CreateDirectory "$SMPROGRAMS\Belfry Images VPNKeepAlive"
		CreateShortcut "$SMPROGRAMS\Belfry Images VPNKeepAlive\VPNKeepAlive.lnk" "$INSTDIR\VPNKeepAlive.exe"
		CreateShortcut "$SMPROGRAMS\Belfry Images VPNKeepAlive\README.lnk" "$INSTDIR\README.txt"
		CreateShortcut "$SMPROGRAMS\Belfry Images VPNKeepAlive\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
	skipstartmenu:
	
	; Prompt to install startup shortcut
	MessageBox MB_YESNO "Do you want to VPNKeepAlive to start automatically?" IDNO skipstartupshortcut
		CreateShortcut "$SMPROGRAMS\Startup\VPNKeepAlive.lnk" "$INSTDIR\VPNKeepAlive.exe"
	skipstartupshortcut:
	
	; Prompt to start now
	MessageBox MB_YESNO "Do you want to start VPNKeepAlive now?" IDNO skipstartnow
		Exec "$PROGRAMFILES\Belfry Images VPNKeepAlive\VPNKeepAlive.exe"
	skipstartnow:
SectionEnd

Section "Uninstall"
	MessageBox MB_YESNO "VPNKeepAlive will be shut down. Do you want to continue?" IDNO abort
		StrCpy $0 "VPNKeepAlive.exe"
		KillProc::KillProcesses
		Sleep 2000
		
		Delete "$INSTDIR\VPNKeepAlive.exe"
		Delete "$INSTDIR\README.txt"
		Delete "$INSTDIR\Uninstall.exe"
		RMDir "$INSTDIR"
		Delete "$SMPROGRAMS\Belfry Images VPNKeepAlive\VPNKeepAlive.lnk"
		Delete "$SMPROGRAMS\Belfry Images VPNKeepAlive\README.lnk"
		Delete "$SMPROGRAMS\Belfry Images VPNKeepAlive\Uninstall.lnk"
		RMDir "$SMPROGRAMS\Belfry Images VPNKeepAlive"
	abort:
SectionEnd