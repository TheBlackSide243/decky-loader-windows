# Decky Manager for Windows

A tiny (~45 KB) control panel that installs, runs and keeps **[Decky Loader](https://github.com/SteamDeckHomebrew/decky-loader)** — the Steam Deck plugin system — working inside **Steam Big Picture on Windows**.

No Python, no Node, no Git, no installer: a single `.exe` that does everything.

![Decky Manager](docs/screenshot.png)

> **Note on language:** the interface is currently in **Italian**. Everything else (code, docs, issues) is in English. Pull requests adding localisation are welcome.

---

## What it does

| | |
|---|---|
| **Installs Decky** | Downloads the official Windows build straight from the Decky Loader CI and sets it up in `%USERPROFILE%\homebrew`. |
| **Enables Steam's debugger** | Creates the `.cef-enable-remote-debugging` file in your Steam folder — the switch that lets Decky attach to the Big Picture UI. |
| **Live status** | Loader, Steam, ports 8080/1337, CEF file, installed plugins — all at a glance. |
| **Start / stop** | Runs the loader with a sane working directory, or stops it. |
| **Starts with Windows** | Optional, via a Startup shortcut; can sit quietly in the system tray. |
| **Keeps itself current** | Checks for a newer Decky build on every launch and (optionally) installs it by itself. |

The orange button is always *the thing you should do next*: create the CEF file, install Decky, or update. When nothing is orange, you are good.

## Requirements

* Windows 10 or 11
* Steam
* An internet connection for install/update

.NET Framework 4.x is used, which ships with Windows — nothing to install.

## Install

1. Download `DeckyManager.exe` from the [latest release](../../releases/latest).
2. Run it and press **Installa Decky**.
3. Restart Steam completely, then open Big Picture: Decky is in the quick access menu.

It never asks for administrator rights. The only exception is the CEF file when Steam lives in a write-protected `Program Files`: the app tells you, and you can run it once as administrator just for that step.

## How it works

Steam's UI is a Chromium (CEF) application. With `.cef-enable-remote-debugging` present, Steam exposes a debugger on port **8080**. Decky's `PluginLoader.exe` connects to it, finds the `SharedJSContext` tab (the React UI behind Big Picture) and injects its frontend, served from a local web server on port **1337**. Decky Manager is simply the thing that sets all of this up and keeps it alive.

**Port 1337 is not configurable** — it is hardcoded in Decky's frontend. If another program is already listening there, Decky cannot start. The usual culprit is the **Razer Chroma SDK** service; the panel detects the conflict and names the process holding the port.

## Updating

On every launch the panel asks the Decky Loader repository for the newest successful Windows CI build and compares it with what you have installed. If the third checkbox is ticked (it is by default) the new version is downloaded and installed automatically, unless a game is running.

GitHub keeps CI artifacts for about 90 days. If the newest build's files are gone, the panel walks back through the last 20 builds until it finds one that is still downloadable.

## Building

```powershell
.\build.ps1
```

Produces `DeckyManager.exe` (~45 KB) using the C# compiler that ships with Windows — no SDK, no NuGet, no Visual Studio.

```powershell
.\build.ps1 -Embed
```

Produces a ~30 MB build with the Decky loader binaries embedded as resources, so it can install Decky **offline**. See the licensing note below before sharing such a build.

`tools/build-from-source.bat` is an optional path for contributors who would rather compile Decky Loader itself from its sources (it needs Git, Node 20 + pnpm and Python 3.11 + Poetry).

## Licensing

Decky Manager is MIT licensed (see [LICENSE](LICENSE) and [NOTICE.md](NOTICE.md)).

**Decky Loader is a separate project, licensed GPL-2.0.** This repository contains none of its code and ships none of its binaries: released builds download them from the project's own CI at install time. If you produce an embedded build with `-Embed`, you are redistributing GPL-2.0 binaries and take on the obligations that come with it — including making the corresponding source available. Keep those builds to yourself unless you are prepared to comply.

Downloads go through [nightly.link](https://nightly.link), which serves GitHub Actions artifacts without requiring a login.

## Credits

All the actual magic belongs to the [SteamDeckHomebrew](https://github.com/SteamDeckHomebrew) team, who wrote Decky Loader and its Windows support. This project only makes it easy to use on a Windows PC.

---

# Decky Manager per Windows (italiano)

> La guida completa, con tutte le opzioni e la risoluzione dei problemi, e' in [docs/GUIDA-IT.md](docs/GUIDA-IT.md).

Un pannello di controllo da ~45 KB che installa, avvia e tiene aggiornato **Decky Loader** — il sistema di plugin dello Steam Deck — dentro **Steam Big Picture su Windows**. Niente Python, Node, Git o installer: basta un solo `.exe`.

## Cosa fa

* **Installa Decky** scaricando la build ufficiale per Windows dalla CI del progetto e sistemandola in `%USERPROFILE%\homebrew`.
* **Attiva il debugger di Steam** creando il file `.cef-enable-remote-debugging` nella cartella di Steam.
* **Mostra lo stato** di loader, Steam, porte 8080 e 1337, file CEF e plugin installati.
* **Avvia e ferma** il loader, con la cartella di lavoro corretta.
* **Parte con Windows**, se vuoi, restando nella barra di sistema vicino all'orologio.
* **Si aggiorna da solo**: a ogni apertura controlla se esiste una versione più recente e, se la spunta è attiva, la installa senza chiedere nulla (ma non mentre stai giocando).

Il pulsante arancione indica sempre l'azione che serve adesso. Se nessuno è arancione, è tutto a posto.

## Installazione

1. Scarica `DeckyManager.exe` dalla [release più recente](../../releases/latest).
2. Aprilo e premi **Installa Decky**.
3. Riavvia Steam completamente e apri Big Picture: Decky è nel menu rapido.

Non chiede mai i permessi di amministratore. Unica eccezione: se Steam è in `Program Files` e la cartella è protetta in scrittura, il file CEF va creato riaprendo il programma come amministratore quella sola volta.

## Come funziona

L'interfaccia di Steam è un'applicazione Chromium. Con il file `.cef-enable-remote-debugging` presente, Steam apre un debugger sulla porta **8080**; `PluginLoader.exe` vi si collega, trova la scheda `SharedJSContext` (la UI React dietro Big Picture) e vi inietta il frontend di Decky, servito da un server locale sulla porta **1337**.

La **porta 1337 non è configurabile**, è cablata nel frontend di Decky: se un altro programma la occupa, Decky non parte. Il colpevole tipico è il servizio **Razer Chroma SDK**; il pannello se ne accorge e ti dice quale processo la sta tenendo.

## Problemi frequenti

* **Decky non compare in Big Picture** — controlla che nel pannello ci siano "Decky Loader: In esecuzione", "File CEF: Presente" e, con Steam aperto, "Debug CEF: Attivo". Poi riavvia Steam del tutto, non basta chiudere la finestra.
* **Porta 1337 occupata** — disattiva il servizio Razer Chroma SDK da `services.msc`, oppure chiudi il programma che il pannello ti indica.
* **Un plugin non funziona** — molti plugin sono scritti per SteamOS e toccano hardware o servizi di sistema (per esempio PowerTools): su Windows non possono funzionare. Vanno bene quelli di sola interfaccia, come CSS Loader, SteamGridDB o ProtonDB Badges.
* **Il pulsante "Update" dentro Decky** non funziona su Windows (è un problema noto a monte): usa "Aggiorna ora" del pannello.

## Licenze

Decky Manager è distribuito con licenza MIT. **Decky Loader è un progetto separato, con licenza GPL-2.0**: qui non c'è nulla del suo codice e le release non contengono i suoi binari, che vengono scaricati dalla sua CI al momento dell'installazione. Se costruisci una versione con `-Embed`, stai ridistribuendo binari GPL-2.0 e te ne assumi gli obblighi.
