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

dotnet build examples/Example_11/Example_11.csproj -c release
dotnet build examples/Example_12/Example_12.csproj -c release
dotnet build examples/Example_13/Example_13.csproj -c release
dotnet build examples/Example_14/Example_14.csproj -c release
dotnet build examples/Example_15/Example_15.csproj -c release
dotnet build examples/Example_16/Example_16.csproj -c release
dotnet build examples/Example_17/Example_17.csproj -c release
dotnet build examples/Example_18/Example_18.csproj -c release
dotnet build examples/Example_19/Example_19.csproj -c release
dotnet build examples/Example_20/Example_20.csproj -c release

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

dotnet examples/Example_11/bin/release/net8.0/Example_11.dll
dotnet examples/Example_12/bin/release/net8.0/Example_12.dll
dotnet examples/Example_13/bin/release/net8.0/Example_13.dll
dotnet examples/Example_14/bin/release/net8.0/Example_14.dll
dotnet examples/Example_15/bin/release/net8.0/Example_15.dll
dotnet examples/Example_16/bin/release/net8.0/Example_16.dll
dotnet examples/Example_17/bin/release/net8.0/Example_17.dll
dotnet examples/Example_18/bin/release/net8.0/Example_18.dll
dotnet examples/Example_19/bin/release/net8.0/Example_19.dll
dotnet examples/Example_20/bin/release/net8.0/Example_20.dll

# evince Example_02.pdf
