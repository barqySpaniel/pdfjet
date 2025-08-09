@echo off
REM Very important!!
rmdir /s /q bin
rmdir /s /q obj

REM Build the PDFjet library project
dotnet build PDFjet.csproj -c release

REM Build Example_01 to Example_50 in parallel
for /L %%i in (1,1,50) do (
    REM Check if the example number is less than 10
    if %%i lss 10 (
        set example_num=Example_0%%i
    ) else (
        set example_num=Example_%%i
    )
    start /B dotnet build examples\%example_num%\%example_num%.csproj -c release
)

REM Wait for all build processes to finish
timeout /t 5 >nul

REM Run Example_01 to Example_50
for /L %%i in (1,1,50) do (
    REM Check if the example number is less than 10
    if %%i lss 10 (
        set example_num=Example_0%%i
    ) else (
        set example_num=Example_%%i
    )
    dotnet examples\%example_num%\bin\release\net8.0\%example_num%.dll
)
