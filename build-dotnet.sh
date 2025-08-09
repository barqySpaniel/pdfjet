#!/bin/bash

# Very important!!
rm -rf bin
rm -rf obj

# Build PDFjet library project
dotnet build PDFjet.csproj -c release

# Function to build and run a specific example
build_and_run_example() {
    local example_num=$1
    dotnet build examples/$example_num/$example_num.csproj -c release
    dotnet examples/$example_num/bin/release/net8.0/$example_num.dll
}

# Loop through Example_01 to Example_50
for i in {1..50}
do
    if [ $i -lt 10 ]; then
        example_num="Example_0$i"
    else
        example_num="Example_$i"
    fi
    # Start building and running the example in the background
    build_and_run_example $example_num &
done

# Wait for all background processes to complete
wait
