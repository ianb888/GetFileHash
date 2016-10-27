; To use this file as a template, the following items need to be changed...
!define PROJECT_HOME "."
!define APPNAME "GetFileHash"
!define PRODUCT_NAME "Calculate File Hash"
!define REGUNINSTKEY "{f0640747-90b0-4a16-9e26-de57e9976e59}"
!define PRODUCT_PUBLISHER "JNI Solutions"
!define PRODUCT_PUBLISHER_FULL "JNI Solutions"
!define PRODUCT_WEB_SITE "http://www.jnisolutions.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\${APPNAME}"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${REGUNINSTKEY}"
!define PRODUCT_INST_ROOT_KEY "HKLM"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define FOLDERID_UserProgramFiles "{89bea731-9b47-4dcd-be52-dc0197e47b98}"
!define KF_FLAG_CREATE 0x00008000
;!define LANG_ENGLISH "English"
!ifndef USERPROFILE
!define USERPROFILE "$%USERPROFILE%"
!endif

; The only other thing that needs to be changed are the list of .exe and .dll files
; which need to be installed/uninstalled. You'll find those items on lines 66 and 120 respectively.

SetCompressor lzma

; MUI 1.67 compatible ------
!include "${NSISDIR}\Include\MUI2.nsh"
!include "${NSISDIR}\Include\LogicLib.nsh"
!include "${NSISDIR}\Include\WinVer.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_UNABORTWARNING
!define MUI_ICON ".\JNIsolutions.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall-colorful.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\orange.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Wizard\orange-uninstall.bmp"
!define MUI_HEADERIMAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Header\orange.bmp"

; Compile the latest version of the source code
;!system 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe /m /p:FrameworkPathOverride="C:\Windows\Microsoft.NET\Framework64\v4.0.30319" /p:Configuration=Release;DeployOnBuild=True;PackageAsSingleFile=False "C:\Users\ibedson\Documents\Visual Studio 2015\Projects\${APPNAME}\${APPNAME}.sln"'

; This is probably the place where we need to insert the code to query the application version number
!system '"${PROJECT_HOME}\ShowVersion\bin\Release\ShowVersion.exe" NSIS > "$%TEMP%\${APPNAME}-Version.nsh"'
!include "$%TEMP%\${APPNAME}-Version.nsh"

VIProductVersion ${PRODUCT_VERSION}
VIAddVersionKey "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey "ProductVersion" "${PRODUCT_VERSION}"
VIAddVersionKey "Comments" "Calculate the MD5, SHA-1 and SHA-256 hashes for any file."
VIAddVersionKey "CompanyName" "${PRODUCT_PUBLISHER_FULL}"
;VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalTrademarks" "Test Application is a trademark of Fake company"
VIAddVersionKey "LegalCopyright" "© 2016 ${PRODUCT_PUBLISHER_FULL}"
VIAddVersionKey "FileDescription" "${PRODUCT_NAME}"
VIAddVersionKey "FileVersion" "${PRODUCT_VERSION}"

!define MUI_WELCOMEPAGE_TITLE "${PRODUCT_NAME}$\r$\nVersion ${PRODUCT_VERSION}"

; Installer Pages
!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

; Uninstaller Pages
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Language Files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME}"
OutFile "${APPNAME}-${PRODUCT_VERSION}-Setup.exe"
InstallDir "$PROGRAMFILES\${PRODUCT_PUBLISHER}"
InstallDirRegKey ${PRODUCT_INST_ROOT_KEY} "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails hide
ShowUnInstDetails show
BrandingText "${PRODUCT_PUBLISHER_FULL}"

RequestExecutionLevel admin

Section "MainSection" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File "${PROJECT_HOME}\${APPNAME}\bin\Release\${APPNAME}.exe"
  File "${PROJECT_HOME}\${APPNAME}\bin\Release\${APPNAME}.exe.config"
  File "${PROJECT_HOME}\${APPNAME}\bin\Release\RestSharp.dll"
  File "${PROJECT_HOME}\${APPNAME}\bin\Release\VirusTotal.NET.dll"

  ; Check if this is a new installation, or an upgrade
  ReadRegStr $R0 HKCU "${PRODUCT_DIR_REGKEY}" ""
  StrCmp $R0 "" install continue

  install:
  Push "$INSTDIR\RestSharp.dll"
  Call AddSharedDLL
  Push "$INSTDIR\VirusTotal.NET.dll"
  Call AddSharedDLL
  GoTo continue

  continue:
  CreateDirectory "$SMPROGRAMS\${PRODUCT_PUBLISHER}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_PUBLISHER}\${PRODUCT_NAME}.lnk" "$INSTDIR\${APPNAME}.exe"
  
  ; Create an entry in the SendTo location
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "SendTo"
  CreateShortCut "$0\${PRODUCT_NAME}.lnk" "$INSTDIR\${APPNAME}.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\Uninst${APPNAME}.exe"
  WriteRegStr ${PRODUCT_INST_ROOT_KEY} "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\${APPNAME}.exe"
  ; Required Values
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\Uninst${APPNAME}.exe"
  ; Optional Values (May not work on older Windows versions)
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\${APPNAME}.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER_FULL}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoModify" "1"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "NoRepair" "1"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_PUBLISHER}\${PRODUCT_NAME}.lnk" "$INSTDIR\${APPNAME}.exe"
  ; Create an Uninstall icon on the Start Menu
;  CreateShortCut "$SMPROGRAMS\${PRODUCT_PUBLISHER}\Uninstall ${PRODUCT_NAME}.lnk" "$INSTDIR\Uninst${APPNAME}.exe"
SectionEnd

Function .onInit
;  Default $Instdir (UserProgramFiles is %LOCALAPPDATA%\Programs by default, so we use that as our default)
;  StrCpy $0 "$LOCALAPPDATA\Veolia"
  UserInfo::GetAccountType
  pop $0
  ${If} $0 != "admin"
    MessageBox mb_iconstop "Administrator rights are required to install this software!"
    SetErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
    Quit
  ${EndIf}
  
;  Make sure we don't overwrite $Instdir if specified on the command line or from InstallDirRegKey
  ${If} $INSTDIR == ""
    ${If} ${IsNT}
      ;Win7 has a per-user ProgramFiles known folder and this could be a non-default location?
      System::Call 'Shell32::SHGetKnownFolderPath(g "${FOLDERID_UserProgramFiles}",i ${KF_FLAG_CREATE},i0,*i.r2)i.r1'
      ${If} $1 == 0
        System::Call '*$2(&w${NSIS_MAX_STRLEN} .r1)'
        StrCpy $0 $1
        System::Call 'Ole32::CoTaskMemFree(ir2)'
      ${EndIf}
    ${Else}
      ;Everyone is admin on Win9x, so falling back to $ProgramFiles is ok
      ${IfThen} $LocalAppData == "" ${|} StrCpy $0 $ProgramFiles ${|}
    ${EndIf}
    StrCpy $INSTDIR "$0\${APPNAME}"
  ${EndIf}
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

; AddSharedDLL
;
; Increments a shared DLLs reference count.
; Use by passing one item on the stack (the full path of the DLL).
;
; Usage:
;   Push $SYSDIR\myDll.dll
;   Call AddSharedDLL
;
Function AddSharedDLL
  Exch $R1
  Push $R0
  ReadRegDword $R0 HKCU Software\Microsoft\Windows\CurrentVersion\SharedDLLs $R1
  IntOp $R0 $R0 + 1
  WriteRegDWORD HKCU Software\Microsoft\Windows\CurrentVersion\SharedDLLs $R1 $R0
  Pop $R0
  Pop $R1
FunctionEnd

; un.RemoveSharedDLL
;
; Decrements a shared DLLs reference count, and removes if necessary.
; Use by passing one item on the stack (the full path of the DLL).
; Note: for use in the main installer (not the uninstaller), rename the
; function to RemoveSharedDLL.
;
; Usage:
;   Push $SYSDIR\myDll.dll
;   Call un.RemoveSharedDLL
;
Function un.RemoveSharedDLL
  Exch $R1
  Push $R0
  ReadRegDword $R0 HKCU Software\Microsoft\Windows\CurrentVersion\SharedDLLs $R1
  StrCmp $R0 "" remove
    IntOp $R0 $R0 - 1
    IntCmp $R0 0 rk rk uk
    rk:
      DeleteRegValue HKCU Software\Microsoft\Windows\CurrentVersion\SharedDLLs $R1
    goto remove
    uk:
      WriteRegDWORD HKCU Software\Microsoft\Windows\CurrentVersion\SharedDLLs $R1 $R0
    Goto noremove
  remove:
    Delete /REBOOTOK $R1
  noremove:
  Pop $R0
  Pop $R1
FunctionEnd

Section Uninstall
  Delete "$INSTDIR\${APPNAME}.exe"
  Delete "$INSTDIR\${APPNAME}.exe.config"
  Delete "$INSTDIR\Uninst${APPNAME}.exe"

  ; Make the appropriate registry changes when we remove an application which uses
  ; a shared DLL. If this is the last application that uses the DLL, then we can
  ; also remove it as well.
  Push "$INSTDIR\RestSharp.dll"
  Call un.RemoveSharedDLL
  Push "$INSTDIR\VirusTotal.NET.dll"
  Call un.RemoveSharedDLL

  Delete "$SMPROGRAMS\${PRODUCT_PUBLISHER}\${PRODUCT_NAME}.lnk"
  Delete "$SMPROGRAMS\${PRODUCT_PUBLISHER}\Uninstall ${PRODUCT_NAME}.lnk"

  ; Delete the entry in the SendTo location
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "SendTo"
  Delete "$0\${PRODUCT_NAME}.lnk"

  RMDir "$SMPROGRAMS\${PRODUCT_PUBLISHER}"
  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey ${PRODUCT_INST_ROOT_KEY} "${PRODUCT_DIR_REGKEY}"
  DeleteRegKey ${PRODUCT_INST_ROOT_KEY} "Software\${PRODUCT_PUBLISHER}\${PRODUCT_NAME}"
  SetAutoClose true
SectionEnd