#!/bin/bash

# Very important!!
rm -rf bin
rm -rf obj

# Build PDFjet library project
dotnet build PDFjet.csproj -c release

# Loop through Example_01 to Example_50
for i in {1..50}
do
    # Check if the example number is less than 10
    if [ $i -lt 10 ]; then
        # For Example_01 to Example_09, prepend 0
        example_num="Example_0$i"
    else
        # For Example_10 to Example_20, no need to prepend 0
        example_num="Example_$i"
    fi

    # Build each Example project
    dotnet build examples/$example_num/$example_num.csproj -c release

    # Run each Example DLL
    dotnet examples/$example_num/bin/release/net8.0/$example_num.dll
done
