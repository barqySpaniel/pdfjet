# Very important!!
rm -rf bin
rm -rf obj

dotnet build PDFjet.csproj -c release

dotnet build examples/Example_01/Example_01.csproj -c release
dotnet build examples/Example_02/Example_02.csproj -c release

dotnet examples/Example_01/bin/release/net8.0/Example_01.dll
dotnet examples/Example_02/bin/release/net8.0/Example_02.dll

evince Example_02.pdf
