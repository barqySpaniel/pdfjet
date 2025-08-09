#!/bin/bash

# Very important!!
rm -rf bin
rm -rf obj

# Build the PDFjet library project
dotnet build PDFjet.csproj -c release

# Build Example_01 to Example_50
for i in {1..50}
do
    if [ $i -lt 10 ]; then
        dotnet build examples/Example_0$i/Example_0$i.csproj -c release &
    else
        dotnet build examples/Example_$i/Example_$i.csproj -c release &
    fi
done
wait

# Run Example_01 to Example_50
for i in {1..50}
do
    if [ $i -lt 10 ]; then
        dotnet examples/Example_0$i/bin/release/net8.0/Example_0$i.dll
    else
        dotnet examples/Example_$i/bin/release/net8.0/Example_$i.dll
    fi
done
