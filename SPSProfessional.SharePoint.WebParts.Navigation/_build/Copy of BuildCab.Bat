
@echo off
set MSBUILDPATH="C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\msbuild.exe"
set XENOCODEPATH="C:\dev\bin\xenocode\postbuild 2007 professional\xbuild.exe"
set STSADM="%CommonProgramFiles%\Microsoft Shared\web server extensions\12\BIN\stsadm.exe"
set SOLUTION="SPSProfessional.SharePoint.WebParts.Navigation"

@if "%1"=="Deploy" Goto Remove

@del *.cab
@del *.dll
@del *.wsp
@del *.xcmap

@if "%1"=="Remove" Goto Remove

@echo *** COMPILE STAGE ***
@%MSBUILDPATH% ..\SPSProfessional.SharePoint.WebParts.Navigation.csproj /t:Clean,Rebuild /P:Configuration=Release,PostBuildEvent="" /v:normal /nologo 
IF ERRORLEVEL   1 GOTO Error

@echo *** OFFUSCATE ***
@%XENOCODEPATH% xenocode.postbuild /m xenocode.xcmap /o .
IF ERRORLEVEL   1 GOTO Error

@rem *** PACKAGE SOLUTION ***
cscript ..\_Build\replace.vbs "..\makecab_var.ddf" "..\makecab.ddf" "xDIRx" "_Build\" "xDEPLOYx" "wsp"
@cd ..
@rem *** Build Package ***
@makecab.exe /F makecab.ddf

@rem *** Deployment ****
@echo off

:Remove
@cd _Build
@echo *** SYSTEM DEPLOYMENT STAGE ***
@%STSADM% -o deletewppack -name %SOLUTION%.cab

@echo --- Retracting Solution %1
@%STSADM% -o retractsolution -local -allcontenturls -name %SOLUTION%.wsp

@echo --- Uninstalling Solution %1
@%STSADM% -o deletesolution -name %SOLUTION%.wsp
@if "%1"=="Remove" Goto End

@echo +++ Adding Solution %1
@%STSADM% -o addsolution -filename %SOLUTION%.wsp
IF ERRORLEVEL   1 GOTO Error

@echo +++ Depoying Solution %1
@%STSADM% -o deploysolution -name %SOLUTION%.wsp -local -allowgacdeployment -allcontenturls -allowCasPolicies
IF ERRORLEVEL   1 GOTO Error

@echo Reseting web server
%systemroot%\system32\iisapp.vbs /a "SharePoint - 80" /r
GOTO End

:Error
@echo  === ERROR ERROR ERROR === (%ERRORLEVEL%)

:End
@echo  === A GREAT WORK === (%ERRORLEVEL%)