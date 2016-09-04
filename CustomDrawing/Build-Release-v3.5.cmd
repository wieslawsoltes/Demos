@echo off
set PATH=C:\WINDOWS\Microsoft.NET\Framework\v3.5;%PATH%

MSBuild CustomDrawing.sln /m /t:Build /p:Configuration=Release;TargetFrameworkVersion=v3.5

pause
