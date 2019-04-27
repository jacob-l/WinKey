rmdir /s /q bin
dotnet publish --self-contained -c Release -r win-x64
dotnet publish --self-contained -c Release -r win-x86