#!/bin/bash

# Very important!!
rm -rf bin
rm -rf obj

# Build PDFjet library project
dotnet build PDFjet.csproj -c release

# Loop through Example_01 to Example_50
for i in {1..50}
do
    # Build each Example project
    dotnet build examples/Example_0$i/Example_0$i.csproj -c release

    # Run each Example DLL
    dotnet examples/Example_0$i/bin/release/net8.0/Example_0$i.dll
done
