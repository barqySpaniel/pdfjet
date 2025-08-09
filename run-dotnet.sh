#!/bin/bash

# Check if an argument is provided (example number)
if [ -z "$1" ]; then
    echo "Usage: $0 <example_number>"
    exit 1
fi

# Very important!!
rm -rf bin
rm -rf obj

# Build the PDFjet library
dotnet build PDFjet.csproj -c release

# Check if the example number is less than 10
if [ $1 -lt 10 ]; then
    # dotnet build examples/Example_0$1/Example_0$1.csproj -c release
    # dotnet examples/Example_0$1/bin/release/net8.0/Example_0$1.dll
    echo "hello"
else
    dotnet build examples/Example_$1/Example_$1.csproj -c release
    dotnet examples/Example_$1/bin/release/net8.0/Example_$1.dll
fi

# Run the specified Example DLL
# dotnet examples/$EXAMPLE/bin/release/net8.0/$EXAMPLE.dll

# Open the generated PDF for the specified Example
# evince Example_01.pdf
