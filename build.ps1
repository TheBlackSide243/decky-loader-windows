<#
.SYNOPSIS
    Builds DeckyManager.exe with the C# compiler that ships with Windows.

.DESCRIPTION
    No SDK, no NuGet, no Visual Studio: this uses csc.exe from the .NET
    Framework already present on every Windows install.

.PARAMETER Embed
    Also embeds the Decky Loader binaries found in src\payload so the
    resulting exe can install Decky without an internet connection.
    NOTE: those binaries are GPL-2.0 - see the licensing note in README.md
    before distributing such a build.

.PARAMETER Output
    Where to write the executable. Defaults to .\DeckyManager.exe

.EXAMPLE
    .\build.ps1
    .\build.ps1 -Embed
#>
[CmdletBinding()]
param(
    [switch]$Embed,
    [string]$Output
)

$ErrorActionPreference = 'Stop'

# $PSScriptRoot is not reliably populated while parameter defaults are being
# evaluated, so the paths are resolved here instead.
$root = $PSScriptRoot
if (-not $root) { $root = Split-Path -Parent $MyInvocation.MyCommand.Path }
if (-not $root) { $root = (Get-Location).Path }
if (-not $Output) { $Output = Join-Path $root 'DeckyManager.exe' }

$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path $csc)) {
    $csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path $csc)) {
    throw "C# compiler not found. The .NET Framework 4.x runtime is required (it ships with Windows)."
}

$src     = Join-Path $root 'src'
$icon    = Join-Path $src 'decky.ico'
$payload = Join-Path $src 'payload'
$fw      = Split-Path $csc -Parent

$refs = @(
    'System.dll'
    'System.Drawing.dll'
    'System.Windows.Forms.dll'
    'System.Core.dll'
    (Join-Path $fw 'System.IO.Compression.FileSystem.dll')
    (Join-Path $fw 'System.IO.Compression.dll')
) -join ','

$cscArgs = @(
    '-nologo'
    '-target:winexe'
    '-optimize+'
    "-win32icon:$icon"
    "-out:$Output"
    "-reference:$refs"
)

if ($Embed) {
    foreach ($f in 'PluginLoader_noconsole.exe', 'PluginLoader.exe', 'build-info.txt') {
        if (-not (Test-Path (Join-Path $payload $f))) {
            throw "-Embed requires src\payload\$f - install Decky first, then copy the files from %USERPROFILE%\homebrew\services."
        }
    }
    Write-Host 'Embedding Decky Loader binaries (GPL-2.0 - see README before sharing this build).' -ForegroundColor Yellow
    $cscArgs += "-resource:$(Join-Path $payload 'PluginLoader_noconsole.exe'),loader_noconsole"
    $cscArgs += "-resource:$(Join-Path $payload 'PluginLoader.exe'),loader_console"
    $cscArgs += "-resource:$(Join-Path $payload 'build-info.txt'),buildinfo"
}

$cscArgs += (Join-Path $src 'DeckyManager.cs')

# a running executable cannot be overwritten
Get-Process -Name ([System.IO.Path]::GetFileNameWithoutExtension($Output)) -ErrorAction SilentlyContinue |
    Stop-Process -Force -ErrorAction SilentlyContinue

Write-Host "Building $Output ..."
& $csc $cscArgs
if ($LASTEXITCODE -ne 0) { throw "Build failed (exit code $LASTEXITCODE)." }

$size = [math]::Round((Get-Item $Output).Length / 1KB, 0)
Write-Host "Done: $Output ($size KB)" -ForegroundColor Green
