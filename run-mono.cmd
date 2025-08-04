@echo off
REM Build and run in one step
if "%1"=="" (
    echo Please provide an example number:
    echo run-mono.bat 33
    exit /b 1
)

REM Single-command build: Compile library + example together
mcs -debug -sdk:4 -warn:2 ^
    net/pdfjet/*.cs ^
    com/pdfjet/font/*.cs ^
    examples/Example_%1.cs ^
    -reference:System.Drawing.dll ^
    -out:Example_%1.exe

if not exist "Example_%1.exe" (
    echo ERROR: Build failed - Example_%1.exe not created!
    exit /b 1
)

REM Run the example
Example_%1.exe

REM Open PDF if generated
start Example_%1.pdf
