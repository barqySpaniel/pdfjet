# Very important!!
rm -rf bin
rm -rf obj

dotnet build PDFjet.csproj -c release
dotnet build examples/Example_01/Example_01.csproj -c release

./examples/Example_01/bin/release/net8.0/Example_01

evince Example_01.pdf
