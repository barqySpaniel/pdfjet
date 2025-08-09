# Very important!!
rm -rf bin
rm -rf obj

dotnet build PDFjet.csproj -c release

dotnet build examples/Example_01/Example_01.csproj -c release
dotnet build examples/Example_02/Example_02.csproj -c release
dotnet build examples/Example_03/Example_03.csproj -c release
dotnet build examples/Example_04/Example_04.csproj -c release
dotnet build examples/Example_05/Example_05.csproj -c release
dotnet build examples/Example_06/Example_06.csproj -c release
dotnet build examples/Example_07/Example_07.csproj -c release
dotnet build examples/Example_08/Example_08.csproj -c release
dotnet build examples/Example_09/Example_09.csproj -c release
dotnet build examples/Example_10/Example_10.csproj -c release

dotnet examples/Example_01/bin/release/net8.0/Example_01.dll
dotnet examples/Example_02/bin/release/net8.0/Example_02.dll
dotnet examples/Example_03/bin/release/net8.0/Example_03.dll
dotnet examples/Example_04/bin/release/net8.0/Example_04.dll
dotnet examples/Example_05/bin/release/net8.0/Example_05.dll
dotnet examples/Example_06/bin/release/net8.0/Example_06.dll
dotnet examples/Example_07/bin/release/net8.0/Example_07.dll
dotnet examples/Example_08/bin/release/net8.0/Example_08.dll
dotnet examples/Example_09/bin/release/net8.0/Example_09.dll
dotnet examples/Example_10/bin/release/net8.0/Example_10.dll

# evince Example_02.pdf
