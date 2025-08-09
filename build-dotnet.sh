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
        example_num="Example_0$i"
    else
        example_num="Example_$i"
    fi
    dotnet build examples/$example_num/$example_num.csproj -c release &
done
wait

# Run Example_01 to Example_50
for i in {1..50}
do
    if [ $i -lt 10 ]; then
        example_num="Example_0$i"
    else
        example_num="Example_$i"
    fi
    dotnet examples/$example_num/bin/release/net8.0/$example_num.dll
done
