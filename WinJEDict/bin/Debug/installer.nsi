; example2.nsi
;
; This script is based on example1.nsi, but it remember the directory, 
; has uninstall support and (optionally) installs start menu shortcuts.
;
; It will install example2.nsi into a directory that the user selects,

;--------------------------------

; The name of the installer
Name "WinEngJa"

; The file to write
OutFile "WinEngJaSetup.exe"

; The default installation directory
InstallDir $PROGRAMFILES\WinEngJa

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\WinEngJa" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "WinEngJa (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  ; Install files
  SetOutPath "$INSTDIR\Inputs"
  File "Inputs\jmdict.sqlite"  

  SetOutPath $INSTDIR  
  File "WinEngJa.exe"
  File "WinEngJa.pdb"
  File "WinEngJa.exe.config"
  File "System.Data.SQLite.dll"
  File "System.Data.SQLite.xml"
  

  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\WinEngJa "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinEngJa" "DisplayName" "WinEngJa v0.1"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinEngJa" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinEngJa" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinEngJa" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\WinEngJa"
  CreateShortCut "$SMPROGRAMS\WinEngJa\Uninstall WinEngJa.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\WinEngJa\WinEngJa.lnk" "$INSTDIR\WinEngJa.exe" "" "$INSTDIR\WinEngJa.exe" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\WinEngJa"
  DeleteRegKey HKLM SOFTWARE\WinEngJa

  ; Remove files and uninstaller
  Delete "$INSTDIR\*.*"
  Delete "$INSTDIR\Inputs\*.*"
  RMDIR "$INSTDIR\Inputs"

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\WinEngJa\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\WinEngJa"
  RMDir "$INSTDIR"

SectionEnd
