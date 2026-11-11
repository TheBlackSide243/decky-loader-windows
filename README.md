**English** | [Italiano](README.it.md)

# Decky Manager for Windows

[![Latest release](https://img.shields.io/github/v/release/TheBlackSide243/decky-loader-windows?label=release&color=ff8a14)](https://github.com/TheBlackSide243/decky-loader-windows/releases/latest)
[![Build](https://img.shields.io/github/actions/workflow/status/TheBlackSide243/decky-loader-windows/build.yml?branch=main)](https://github.com/TheBlackSide243/decky-loader-windows/actions/workflows/build.yml)
[![Downloads](https://img.shields.io/github/downloads/TheBlackSide243/decky-loader-windows/total?color=4ad67c)](https://github.com/TheBlackSide243/decky-loader-windows/releases)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue)](LICENSE)
![Windows 10/11](https://img.shields.io/badge/Windows-10%20%7C%2011-0078d4)

A tiny (~70 KB) control panel that installs, runs and keeps **[Decky Loader](https://github.com/SteamDeckHomebrew/decky-loader)** — the Steam Deck plugin system — working inside **Steam Big Picture on Windows**.

No Python, no Node, no Git, no installer: a single `.exe` that does everything.

[**⬇ Download the latest release**](../../releases/latest)

![Decky Manager](docs/screenshot-en.png)

> The interface is available in **English** and **Italian**. It follows your Windows language, and you can switch any time with **ITA | ENG** next to the title.

---

## What it does

| | |
|---|---|
| **Installs Decky** | Downloads the official Windows build straight from the Decky Loader releases and sets it up in `%USERPROFILE%\homebrew`. |
| **Enables Steam's debugger** | Creates the `.cef-enable-remote-debugging` file in your Steam folder — the switch that lets Decky attach to the Big Picture UI. |
| **Live status** | Loader, Steam, ports 8080/1337, CEF file, installed plugins — all at a glance. |
| **Start / stop** | Runs the loader with a proper working directory, or stops it. |
| **Starts with Windows** | Optional, via a Startup shortcut; can sit quietly in the system tray. |
| **Keeps itself current** | Checks for a newer Decky release on every launch and (optionally) installs it by itself. |

The orange button is always *the thing you should do next*: create the CEF file, install Decky, or update. When nothing is orange, you are good.

## Requirements

* Windows 10 or 11
* Steam
* An internet connection for install and updates

It uses the .NET Framework 4.x that ships with Windows — nothing to install.

## Install

1. Download `DeckyManager.exe` from the [latest release](../../releases/latest).
2. Run it and press **Install Decky**.
3. Restart Steam completely, then open Big Picture: Decky is in the quick access menu.

It never asks for administrator rights. The only exception is the CEF file when Steam lives in a write-protected `Program Files`: the app tells you, and you can run it once as administrator just for that step.

> Windows SmartScreen may warn about an unrecognised app, because the exe is not code-signed. Click *More info → Run anyway*. The source is all here, and every release is built from it by GitHub Actions.

## The three options

* **Start Decky automatically when the PC boots** — adds a shortcut to the Windows Startup folder.
* **Start minimised to the system tray** — at boot the panel itself starts hidden near the clock and launches the loader. Left-click the icon to open it; right-click for a menu. On Windows 11 new icons land in the *hidden icons* flyout under the `^` arrow: drag it onto the taskbar to keep it visible.
* **Update Decky automatically when a new version is out** — on by default. On every launch, if a newer build exists it is downloaded and installed silently, with a notification on the tray icon. It never does this while a game is running.

## Window size

The panel adapts to your screen: on a 1440p or 4K monitor it opens larger, and it always fits inside the screen. It is DPI-aware per monitor, so it stays sharp at any Windows scaling and redraws itself when you drag it to a monitor with a different scale.

To make it bigger or smaller, drag any edge of the window: the whole content scales with it and the size is remembered. **Ctrl +** and **Ctrl −** do the same from the keyboard, **Ctrl 0** goes back to the automatic size.
## How it works

Steam's UI is a Chromium (CEF) application. With `.cef-enable-remote-debugging` present, Steam exposes a debugger on port **8080**. Decky's `PluginLoader.exe` connects to it, finds the `SharedJSContext` tab (the React UI behind Big Picture) and injects its frontend, served from a local web server on port **1337**. Decky Manager is simply the thing that sets all of this up and keeps it alive.

**Port 1337 is not configurable** — it is hardcoded in Decky's frontend. If another program is already listening there, Decky cannot start. The usual culprit is the **Razer Chroma SDK** service; the panel detects the conflict and names the process holding the port.

## Updating

On every launch the panel asks GitHub for the latest **official Decky Loader release** and compares its version with the one you have installed (`build-info.txt`).

Since v3.3.0 every Decky release ships ready-made Windows executables (`PluginLoader-win.exe` and `PluginLoader-win-noconsole.exe`, plus ARM64 builds, which the panel picks automatically on ARM PCs). The panel downloads them straight from the release, checks the SHA-256 GitHub publishes for each file, and only then stops the loader, swaps the files and starts it again. If anything fails, the installed version is left untouched and the loader is restarted.

Automatic updates run at most once every 6 hours, never while a game is running, and only one panel can be open at a time — opening it again just brings the existing window forward. Errors are written to `%USERPROFILE%\homebrew\logs\decky-manager.log`.

## Troubleshooting

* **Decky doesn't show up in Big Picture** — the panel should read *Decky Loader: Running*, *CEF file in Steam: Present* and, with Steam open, *CEF debug: Active*. Then restart Steam completely (quit it, don't just close the window).
* **Port 1337 busy** — stop the program the panel names; for Razer Chroma SDK, disable the service in `services.msc`.
* **A plugin doesn't work** — many plugins are written for SteamOS and touch hardware or system services (PowerTools, for example): they cannot work on Windows. UI-only plugins such as CSS Loader, SteamGridDB or ProtonDB Badges work fine.
* **The "Update" button inside Decky does nothing** — a known upstream issue on Windows. Use *Update now* in the panel instead.

## Building

```powershell
.\build.ps1
```

Produces `DeckyManager.exe` (~70 KB) using the C# compiler that ships with Windows — no SDK, no NuGet, no Visual Studio.

```powershell
.\build.ps1 -Embed
```

Produces a ~30 MB build with the Decky loader binaries embedded, so it can install Decky **offline**. Read the licensing note below before sharing such a build.

`tools/build-from-source.bat` is an optional path for people who would rather compile Decky Loader itself from its sources (it needs Git, Node 20 + pnpm and Python 3.11 + Poetry).

## Licensing

Decky Manager is MIT licensed (see [LICENSE](LICENSE) and [NOTICE.md](NOTICE.md)).

**Decky Loader is a separate project, licensed GPL-2.0.** This repository contains none of its code and releases ship none of its binaries: they are downloaded from the project's own GitHub releases at install time. If you produce an embedded build with `-Embed`, you are redistributing GPL-2.0 binaries and take on the obligations that come with it — including making the corresponding source available.

## Credits

All the actual magic belongs to the [SteamDeckHomebrew](https://github.com/SteamDeckHomebrew) team, who wrote Decky Loader and its Windows support. This project only makes it easy to use on a Windows PC.
