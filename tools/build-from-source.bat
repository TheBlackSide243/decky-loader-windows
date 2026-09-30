@echo off
setlocal enabledelayedexpansion
title Decky Loader - build from source

rem ---------------------------------------------------------------------
rem  OPTIONAL. Most people never need this file: Decky Manager installs
rem  and updates Decky by downloading the official Windows build from the
rem  Decky Loader CI.
rem
rem  Use this only if you want to compile Decky Loader yourself.
rem  Requirements: Git, Node 20 + pnpm, Python 3.11 + Poetry.
rem
rem  Put this file next to DeckyManager.exe and the panel will use it for
rem  "Aggiorna ora" instead of downloading.
rem ---------------------------------------------------------------------

set "REPO=%USERPROFILE%\decky-loader"
set "HB=%USERPROFILE%\homebrew"

echo ============================================
echo   DECKY LOADER - BUILD FROM SOURCE
echo ============================================
echo.

for %%T in (git node pnpm python) do (
    where %%T >nul 2>&1 || (
        echo ERROR: %%T is not installed or not in PATH.
        echo Needed: Git, Node 20 + pnpm, Python 3.11 + Poetry.
        goto :err
    )
)

echo [1/7] Stopping Decky Loader...
taskkill /im PluginLoader.exe /f >nul 2>&1
taskkill /im PluginLoader_noconsole.exe /f >nul 2>&1
ping -n 4 127.0.0.1 >nul

if exist "%REPO%\.git" goto :pull
echo [2/7] Cloning decky-loader ^(this takes a while^)...
git clone --depth 1 https://github.com/SteamDeckHomebrew/decky-loader "%REPO%" || goto :err
goto :build

:pull
echo [2/7] Fetching latest sources...
git -C "%REPO%" fetch --depth 1 origin main || goto :err
git -C "%REPO%" reset --hard origin/main || goto :err
git -C "%REPO%" fetch --tags --depth 1 origin >nul 2>&1

:build
echo [3/7] Building the frontend...
cd /d "%REPO%\frontend" || goto :err
call pnpm i --frozen-lockfile --dangerously-allow-all-builds || goto :err
call pnpm run build || goto :err

echo [4/7] Installing Python dependencies...
cd /d "%REPO%\backend" || goto :err
python -m poetry install --no-interaction || goto :err

echo [5/7] Building the executables...
set "DECKY_NOCONSOLE="
python -m poetry run pyinstaller pyinstaller.spec -y || goto :err
set "DECKY_NOCONSOLE=1"
python -m poetry run pyinstaller pyinstaller.spec -y || goto :err
set "DECKY_NOCONSOLE="

echo [6/7] Installing into %HB%\services ...
if not exist "%HB%\services" mkdir "%HB%\services"
copy /y "%REPO%\backend\dist\PluginLoader.exe" "%HB%\services\" >nul || goto :err
copy /y "%REPO%\backend\dist\PluginLoader_noconsole.exe" "%HB%\services\" >nul || goto :err

set "SHA="
for /f "delims=" %%i in ('git -C "%REPO%" rev-parse HEAD 2^>nul') do if not defined SHA set "SHA=%%i"
set "TAG="
for /f "delims=" %%i in ('git -C "%REPO%" tag --sort^=-v:refname 2^>nul') do if not defined TAG set "TAG=%%i"
if not defined TAG set "TAG=main"
> "%HB%\services\build-info.txt" echo !TAG!^|!SHA!^|%date%

echo [7/7] Restarting Decky Loader...
start "" /d "%HB%\services" "%HB%\services\PluginLoader_noconsole.exe"

echo.
echo ============================================
echo   DONE - %TAG% (!SHA:~0,7!)
echo   Restart Steam to reload the interface.
echo ============================================
echo.
if not "%~1"=="auto" pause
exit /b 0

:err
echo.
echo Build failed - see the messages above.
echo The previously installed Decky is untouched.
echo.
pause
exit /b 1
