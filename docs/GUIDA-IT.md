# Guida completa (italiano)

Manuale d'uso di Decky Manager. Per la panoramica breve vedi il [README](../README.it.md).

## Come funziona, in breve

L'interfaccia di Steam è un'applicazione Chromium. Se nella cartella di Steam
esiste un file vuoto chiamato `.cef-enable-remote-debugging`, Steam apre un
debugger sulla porta **8080**. `PluginLoader.exe` (il cuore di Decky) vi si
collega, individua la scheda `SharedJSContext` — cioè l'interfaccia React che
sta dietro a Big Picture — e vi inietta il proprio frontend, servito da un
piccolo server locale sulla porta **1337**.

Decky Manager non fa altro che preparare tutto questo e tenerlo in piedi.
Se Steam si riavvia, il loader si riaggancia da solo: l'ordine di avvio fra
Steam e loader è indifferente.

## Le tre spunte

**Avvia Decky automaticamente all'accensione del PC**
Crea una scorciatoia nella cartella Esecuzione automatica di Windows. Non usa
la chiave di registro `Run` e non chiede permessi di amministratore.

**Avvia ridotto a icona nella barra di sistema**
All'accensione parte il pannello stesso, che resta nascosto vicino
all'orologio e avvia lui il loader. Un clic sull'icona riapre la finestra, il
tasto destro dà un menu con *Apri pannello*, *Avvia/Ferma Decky* e *Chiudi
pannello*. Chiudendo la finestra con la X non esce: torna nella barra.

Windows 11 mette le icone nuove nel riquadro delle **icone nascoste**, quello
sotto la freccetta `^`. Per averla sempre in vista, trascinala fuori dal
riquadro sulla barra, oppure accendila da *Impostazioni → Personalizzazione →
Barra delle applicazioni → Altre icone nell'area di notifica*.

**Aggiorna Decky da solo quando esce una versione nuova**
È accesa di serie. A ogni apertura del pannello (quindi anche a ogni
accensione, se hai attivato l'avvio automatico) controlla se esiste una
versione più recente e, se c'è, la scarica e la installa senza chiedere nulla,
avvisando con un fumetto sull'icona. Non lo fa mentre è in corso un gioco:
aspetta la volta successiva. La scelta si ricorda in
`%USERPROFILE%\homebrew\settings\decky-manager.ini` e vale solo per quel PC.

## Dove finiscono i file

```
%USERPROFILE%\homebrew\
  services\            gli eseguibili di Decky in uso
  services\build-info.txt   versione e commit installati
  plugins\  themes\  settings\  logs\  data\

<cartella di Steam>\.cef-enable-remote-debugging
  file vuoto che attiva il debugger di Steam
```

La cartella di Steam viene letta dal registro, quindi funziona anche se Steam
non è nel percorso classico.

## Aggiornamenti

Il pannello chiede a GitHub qual è l'ultima build Windows andata a buon fine
nella CI di Decky Loader e la confronta con quella installata, leggendo
`build-info.txt`. Se serve, scarica l'archivio degli artefatti tramite
[nightly.link](https://nightly.link) — che li serve senza bisogno di account —
lo estrae, sostituisce gli eseguibili e riavvia il loader.

GitHub conserva gli artefatti circa 90 giorni. Se quelli della build più
recente non ci sono più, il pannello scorre a ritroso le ultime venti build
finché ne trova una ancora scaricabile.

Se accanto all'exe metti `build-from-source.bat` (lo trovi in `tools/`) e hai
Git, Node e Python installati, il pulsante "Aggiorna ora" compila invece Decky
dai sorgenti anziché scaricarlo.

## Se qualcosa non va

**Decky non compare in Big Picture.**
Nel pannello devono essere a posto tre righe: *Decky Loader: In esecuzione*,
*File CEF in Steam: Presente* e, con Steam aperto, *Debug CEF: Attivo*. Poi
riavvia Steam completamente — uscire davvero, non chiudere solo la finestra.

**"Server Decky (porta 1337): Occupata da un altro programma".**
La porta 1337 è cablata nel frontend di Decky e non si può cambiare. Il
colpevole più comune è il servizio **Razer Chroma SDK**: fermalo e
disabilitalo da `services.msc`. Per scoprire chi la occupa:

```
netstat -ano | findstr :1337
tasklist /fi "pid eq NUMERO"
```

**Il file CEF non si crea.**
Succede se Steam è in `Program Files` e la cartella è protetta in scrittura.
Chiudi Steam e riprova, oppure riapri il pannello come amministratore solo per
quella volta: è l'unico caso in cui servono permessi elevati.

**Un plugin non funziona.**
Molti plugin sono scritti per SteamOS e toccano hardware o servizi di sistema
(per esempio PowerTools, che regola TDP e ventole): su Windows non possono
funzionare. Vanno bene quelli di sola interfaccia, come CSS Loader,
SteamGridDB, ProtonDB Badges, Non-Steam Badges o Speed Test.

**Il pulsante "Update" dentro l'interfaccia di Decky non funziona.**
È un problema noto a monte su Windows: l'updater di Decky scrive nella
cartella di lavoro del processo, che avviandolo dal sistema è una cartella
protetta. Usa "Aggiorna ora" del pannello.

## Disinstallazione completa

1. Nel pannello togli le spunte e premi **Ferma Decky**.
2. Elimina `%USERPROFILE%\homebrew`.
3. Elimina `.cef-enable-remote-debugging` dalla cartella di Steam.
4. Riavvia Steam.
