@echo off

set MSBUILDPATH="C:\WINDOWS\Microsoft.NET\Framework\v3.5\msbuild.exe"
@%MSBUILDPATH% /t:%1 /p:Build=%2 buildcab.msbuild