**Download `DeckyManager.exe` below**, run it and press **Install Decky**, then restart Steam.
No installer, no admin rights, nothing else to install. Decky Loader itself is downloaded from its official GitHub releases.

**Scarica `DeckyManager.exe` qui sotto**, aprilo e premi **Installa Decky**, poi riavvia Steam.
Nessun installer, nessun permesso di amministratore, niente altro da installare. Decky Loader viene scaricato dalle sue release ufficiali su GitHub.

### What's new / Novità (v1.2.0)

- **Updates work again.** Decky is now downloaded from its official releases (v3.3.0+ ships Windows executables) instead of CI artifacts from a workflow upstream no longer runs. Each file is checked against GitHub's SHA-256 before anything is replaced.
  **Gli aggiornamenti funzionano di nuovo.** Decky ora si scarica dalle release ufficiali (da v3.3.0 includono gli exe per Windows) invece che dagli artefatti di un workflow che il progetto non usa più. Ogni file viene verificato con lo SHA-256 di GitHub prima di sostituire qualcosa.
- **No more update loop.** The old check compared against the newest commit on `main`, which never matched the downloaded build, so auto-update reinstalled Decky over and over. Versions are now compared by release tag, and auto-update runs at most once every 6 hours.
  **Niente più ciclo di aggiornamenti.** Il vecchio controllo confrontava con l'ultimo commit di `main`, che non coincideva mai con la build scaricata: l'aggiornamento automatico reinstallava Decky di continuo. Ora si confronta la versione della release e l'automatico parte al massimo ogni 6 ore.
- **Single instance.** Opening the panel while it already runs in the tray brings that window forward instead of starting a second copy that would fight over Decky.
  **Una sola istanza.** Aprire il pannello mentre è già nel tray porta in primo piano quella finestra invece di avviarne una seconda copia.
- **Stability.** The loader is stopped and waited for before files are replaced, failed updates restart the previous loader, background work no longer touches the window before it exists, and errors go to `homebrew\logs\decky-manager.log` instead of crashing the panel.
  **Stabilità.** Il loader viene fermato e atteso prima di sostituire i file, un aggiornamento fallito riavvia il loader precedente, i thread non toccano più la finestra prima che esista e gli errori finiscono in `homebrew\logs\decky-manager.log` invece di chiudere il pannello.
- ARM64 PCs get the ARM64 build of Decky automatically. / Sui PC ARM64 viene installata in automatico la build ARM64.

> Windows SmartScreen may warn about an unsigned app: *More info → Run anyway*.
> SmartScreen potrebbe avvisare che l'app non è firmata: *Ulteriori informazioni → Esegui comunque*.
