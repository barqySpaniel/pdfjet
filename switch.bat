@echo off
REM This script switches between Java versions on Windows

REM Define the paths to your Java installations
set JAVA_8=C:\Program Files\Java\jdk1.8.0_251
set JAVA_11=C:\Program Files\Java\jdk-11.0.10
set JAVA_17=C:\Program Files\Java\jdk-17

@echo off
REM This script switches between Java versions on Windows

REM Define the paths to your Java installations
set JAVA_8=C:\Program Files\Eclipse Adoptium\jdk-8.0.462.8-hotspot
set JAVA_11=C:\Program Files\Eclipse Adoptium\jdk-11.0.28.6-hotspot
set JAVA_17=C:\Program Files\Eclipse Adoptium\jdk-17.0.16.8-hotspot
set JAVA_21=C:\Program Files\Eclipse Adoptium\jdk-21.0.8.9-hotspot

REM Prompt for the Java version choice
echo Select Java version to switch to:
echo 1. Java 8
echo 2. Java 11
echo 3. Java 17
echo 4. Java 21
set /p choice=Enter your choice (1, 2, 3 or 4):

REM Set the JAVA_HOME and update the PATH based on the choice
if "%choice%"=="1" (
    set JAVA_HOME=%JAVA_8%
) else if "%choice%"=="2" (
    set JAVA_HOME=%JAVA_11%
) else if "%choice%"=="3" (
    set JAVA_HOME=%JAVA_17%
) else if "%choice%"=="4" (
    set JAVA_HOME=%JAVA_21%
) else (
    echo Invalid choice. Exiting.
    exit /b
)

REM Update the PATH to include the chosen Java version
set PATH=%JAVA_HOME%\bin;%PATH%

echo ""
REM Print the new Java version to confirm
echo Java version switched to:
"%JAVA_HOME%\bin\java" -version
