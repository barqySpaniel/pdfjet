@echo off
REM Windows batch script for building and running PDFjet examples with Mono

REM Check if argument is provided
if "%1"=="" (
    echo Please provide an example number:
    echo run-mono.bat 33
    exit /b 1
)

REM Very important!!
REM call clean.bat

REM Compile the PDFjet library
mcs -debug -sdk:4 -warn:2 net/pdfjet/*.cs -reference:System.Drawing.dll -target:library -out:PDFjet.dll

REM Compile the specific example
mcs -debug -sdk:4 examples/Example_%1.cs -reference:PDFjet.dll

REM Move and run the example
move examples\Example_%1.exe .
mono --debug Example_%1.exe

REM Open the resulting PDF using the default PDF viewer
start Example_%1.pdf
