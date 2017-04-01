cd ConsoleApp1
dotnet restore
dotnet publish -c Release -r win7-x64 -o bin/win7-x64
dotnet publish -c Release -r win7-x86 -o bin/win7-x86
dotnet publish -c Release -r win10-x64 -o bin/win10-x64
dotnet publish -c Release -r win10-x86 -o bin/win10-x86
dotnet publish -c Release -r ubuntu.14.04-x64 -o bin/ubuntu.14.04-x64
dotnet publish -c Release -r ubuntu.16.04-x64 -o bin/ubuntu.16.04-x64
dotnet publish -c Release -r ubuntu.16.10-x64 -o bin/ubuntu.16.10-x64
dotnet publish -c Release -r osx.10.12-x64 -o bin/osx.10.12-x64
dotnet publish -c Release -r win10-x64 -o bin/win10-x64
dotnet publish -c Release -r win10-x86 -o bin/win10-x86
cd ..
cd ConsoleApp2
dotnet restore
dotnet publish -c Release -r win7-x64 -o bin/win7-x64
dotnet publish -c Release -r win7-x86 -o bin/win7-x86
dotnet publish -c Release -r win10-x64 -o bin/win10-x64
dotnet publish -c Release -r win10-x86 -o bin/win10-x86
dotnet publish -c Release -r ubuntu.14.04-x64 -o bin/ubuntu.14.04-x64
dotnet publish -c Release -r ubuntu.16.04-x64 -o bin/ubuntu.16.04-x64
dotnet publish -c Release -r ubuntu.16.10-x64 -o bin/ubuntu.16.10-x64
dotnet publish -c Release -r osx.10.12-x64 -o bin/osx.10.12-x64
cd ..
