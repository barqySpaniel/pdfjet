@echo off
REM Windows batch script for building and running PDFjet examples

REM Check if argument is provided
if "%1"=="" (
    echo Please provide an example number:
    echo run-dotnet.bat 33
    exit /b 1
)

REM Very important!!
REM call clean.bat

REM Build and run the specified example
dotnet build PDFjet.csproj /p:StartupObject=Example_%1
dotnet run

REM Open the resulting PDF using the default PDF viewer
start Example_%1.pdf
