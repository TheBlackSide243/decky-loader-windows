using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Microsoft.Win32;

// ---------------------------------------------------------------- tema
static class Th
{
    public static readonly Color Bg = Color.FromArgb(15, 15, 17);
    public static readonly Color Panel = Color.FromArgb(28, 28, 32);
    public static readonly Color Line = Color.FromArgb(52, 52, 60);
    public static readonly Color Text = Color.FromArgb(242, 242, 246);
    public static readonly Color Sub = Color.FromArgb(150, 150, 162);
    public static readonly Color Accent = Color.FromArgb(255, 138, 20);
    public static readonly Color AccentHover = Color.FromArgb(255, 166, 66);
    public static readonly Color BtnBg = Color.FromArgb(44, 44, 52);
    public static readonly Color BtnHover = Color.FromArgb(74, 60, 44);
    public static readonly Color Good = Color.FromArgb(74, 214, 124);
    public static readonly Color Bad = Color.FromArgb(240, 96, 80);
    public static readonly Color Warn = Color.FromArgb(255, 172, 60);

    public static GraphicsPath Round(Rectangle r, int rad)
    {
        GraphicsPath p = new GraphicsPath();
        int d = rad * 2;
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }
}

// ------------------------------------------------- pulsante arrotondato
class RoundButton : Button
{
    public bool Primary = false;
    bool hover = false;

    public RoundButton()
    {
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseOverBackColor = Th.Bg;
        FlatAppearance.MouseDownBackColor = Th.Bg;
        BackColor = Th.Bg;
        Cursor = Cursors.Hand;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                 | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
    }

    protected override void OnMouseEnter(EventArgs e) { hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { hover = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent != null ? Parent.BackColor : Th.Bg);

        Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
        using (GraphicsPath path = Th.Round(r, 9))
        {
            Color fill, border, txt;
            if (!Enabled)
            {
                fill = Color.FromArgb(32, 32, 38);
                border = Color.FromArgb(48, 48, 56);
                txt = Color.FromArgb(96, 96, 106);
            }
            else if (Primary)
            {
                fill = hover ? Th.AccentHover : Th.Accent;
                border = fill;
                txt = Color.FromArgb(24, 18, 8);
            }
            else
            {
                fill = hover ? Th.BtnHover : Th.BtnBg;
                border = hover ? Th.Accent : Color.FromArgb(78, 78, 92);
                txt = Th.Text;
            }
            using (SolidBrush b = new SolidBrush(fill)) g.FillPath(b, path);
            using (Pen p = new Pen(border, 1)) g.DrawPath(p, path);
            TextRenderer.DrawText(g, Text, Font, r, txt,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}

// ------------------------------------------- casella di spunta a tema
class ThemedCheck : CheckBox
{
    public ThemedCheck()
    {
        FlatStyle = FlatStyle.Flat;
        BackColor = Th.Bg;
        ForeColor = Th.Text;
        Cursor = Cursors.Hand;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint
                 | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnMouseEnter(EventArgs e) { Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { Invalidate(); base.OnMouseLeave(e); }
    protected override void OnCheckedChanged(EventArgs e) { Invalidate(); base.OnCheckedChanged(e); }
    protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Parent != null ? Parent.BackColor : Th.Bg);

        int side = 19;
        int top = (Height - side) / 2;
        Rectangle box = new Rectangle(1, top, side, side);
        using (GraphicsPath path = Th.Round(box, 5))
        {
            using (SolidBrush b = new SolidBrush(Checked
                ? (Enabled ? Th.Accent : Color.FromArgb(122, 82, 30))
                : Color.FromArgb(38, 38, 44)))
                g.FillPath(b, path);
            using (Pen p = new Pen(Checked ? Th.Accent : Color.FromArgb(88, 88, 100), 1))
                g.DrawPath(p, path);
        }
        if (Checked)
        {
            using (Pen p = new Pen(Color.FromArgb(24, 18, 8), 2.2f))
            {
                p.StartCap = LineCap.Round; p.EndCap = LineCap.Round;
                g.DrawLines(p, new Point[] {
                    new Point(box.X + 5, box.Y + 10),
                    new Point(box.X + 8, box.Y + 13),
                    new Point(box.X + 14, box.Y + 6) });
            }
        }
        Rectangle txt = new Rectangle(side + 10, 0, Width - side - 10, Height);
        TextRenderer.DrawText(g, Text, Font, txt, Enabled ? ForeColor : Color.FromArgb(104, 104, 114),
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
    }

    public override Size GetPreferredSize(Size proposed)
    {
        Size t = TextRenderer.MeasureText(Text, Font);
        return new Size(t.Width + 19 + 16, Math.Max(t.Height, 21) + 6);
    }
}

// ------------------------------------------------------- card arrotondata
class Card : Panel
{
    public Card()
    {
        BackColor = Th.Bg;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                 | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
        using (GraphicsPath path = Th.Round(r, 10))
        {
            using (SolidBrush b = new SolidBrush(Th.Panel)) g.FillPath(b, path);
            using (Pen p = new Pen(Th.Line, 1)) g.DrawPath(p, path);
        }
        base.OnPaint(e);
    }
}

// ------------------------------------------------------------ lingua
// Testi dell'interfaccia: la chiave e' il testo italiano, cosi' qualsiasi
// stringa non ancora tradotta ricade semplicemente sull'italiano.
static class L
{
    public static bool En = false;

    static readonly Dictionary<string, string> en = new Dictionary<string, string>
    {
        { "per Steam Big Picture su Windows", "for Steam Big Picture on Windows" },
        { "STATO", "STATUS" },
        { "Debug CEF (porta 8080)", "CEF debug (port 8080)" },
        { "Server Decky (porta 1337)", "Decky server (port 1337)" },
        { "File CEF in Steam", "CEF file in Steam" },
        { "Plugin installati", "Installed plugins" },
        { "VERSIONE", "VERSION" },
        { "Installata", "Installed" },
        { "Aggiornamenti", "Updates" },
        { "Avvia Decky automaticamente all'accensione del PC", "Start Decky automatically when the PC boots" },
        { "Avvia ridotto a icona nella barra di sistema", "Start minimised to the system tray" },
        { "All'accensione parte anche questo pannello, nascosto vicino\nall'orologio: un clic sull'icona lo riapre.", "At boot this panel starts too, hidden near\nthe clock: click the icon to open it again." },
        { "Aggiorna Decky da solo quando esce una versione nuova", "Update Decky automatically when a new version is out" },
        { "Al momento dell'apertura, se esiste una versione piu' recente\nviene scaricata e installata da sola.\nNon lo fa mentre e' in corso un gioco.", "When the panel opens, a newer version is\ndownloaded and installed automatically.\nNever while a game is running." },
        { "Installa Decky", "Install Decky" },
        { "Installa Decky e prepara il PC.\nUsalo alla prima installazione o per reinstallare.", "Installs Decky and prepares the PC.\nUse it for the first install or to reinstall." },
        { "Crea file CEF", "Create CEF file" },
        { "Primo passo su un PC nuovo: abilita il debug CEF di Steam.\nSi disattiva da solo quando il file e' gia' presente.", "First step on a new PC: enables Steam's CEF debugger.\nDisabled automatically once the file exists." },
        { "Avvia Decky", "Start Decky" },
        { "Aggiorna ora", "Update now" },
        { "Controlla aggiornamenti", "Check for updates" },
        { "Apri cartella log", "Open log folder" },
        { "Dopo un aggiornamento riavvia Steam per ricaricare l'interfaccia.", "After an update, restart Steam to reload the interface." },
        { "Apri pannello", "Open panel" },
        { "Ferma Decky", "Stop Decky" },
        { "Chiudi pannello (Decky resta attivo)", "Close panel (Decky keeps running)" },
        { "Risorsa mancante nell'eseguibile: ", "Resource missing from the executable: " },
        { "versione ignota", "unknown version" },
        { "In esecuzione", "Running" },
        { "Fermo", "Stopped" },
        { "NON INSTALLATO", "NOT INSTALLED" },
        { "Chiuso", "Closed" },
        { "Attivo", "Active" },
        { "Non attivo", "Inactive" },
        { "in attesa di Steam", "waiting for Steam" },
        { "Occupata da un altro programma", "Used by another program" },
        { "Libera", "Free" },
        { "Presente", "Present" },
        { "MANCANTE", "MISSING" },
        { "Ricompila Decky dai sorgenti presenti su questo PC.", "Rebuilds Decky from the sources on this PC." },
        { "Scarica e installa l'ultima versione ufficiale di Decky da GitHub.\nNon serve nient'altro: basta la connessione a internet.", "Downloads and installs the latest official Decky build from GitHub.\nNothing else needed: just an internet connection." },
        { "nessuno", "none" },
        { ")  -  compilata il ", ")  -  built on " },
        { "sconosciuta  -  exe del ", "unknown  -  exe dated " },
        { "  -  installata il ", "  -  installed on " },
        { "controllo in corso...", "checking..." },
        { "controllo fallito (nessuna rete?)", "check failed (no network?)" },
        { "limite di richieste GitHub raggiunto: riprova tra un'ora", "GitHub request limit reached: try again in an hour" },
        { "ultima versione disponibile: ", "latest version available: " },
        { "AGGIORNAMENTO DISPONIBILE (", "UPDATE AVAILABLE (" },
        { "aggiornato all'ultima versione", "up to date" },
        { "Non trovo:\n", "Not found:\n" },
        { "La porta 1337 e' occupata da un altro programma\n", "Port 1337 is used by another program\n" },
        { "(spesso il servizio Razer Chroma SDK).\nDecky non puo' partire finche' non lo chiudi.", "(often the Razer Chroma SDK service).\nDecky cannot start until you close it." },
        { "Porta occupata", "Port in use" },
        { "Errore", "Error" },
        { "Decky risulta gia' installato.\n\n", "Decky is already installed.\n\n" },
        { "Scarico da GitHub l'ultima versione ufficiale di Decky per Windows\n(circa 30 MB) e la installo. Procedere?", "I will download the latest official Decky build for Windows from GitHub\n(about 30 MB) and install it. Continue?" },
        { " installato in:\n", " installed in:\n" },
        { "Installazione non riuscita:\n", "Installation failed:\n" },
        { "Decky risulta gia' installato.\nVuoi reinstallare la versione contenuta in questo programma?", "Decky is already installed.\nReinstall the version bundled with this program?" },
        { "Reinstalla Decky", "Reinstall Decky" },
        { "Decky installato in:\n", "Decky installed in:\n" },
        { "\nNon trovo la cartella di Steam: apri Steam almeno una volta.", "\nSteam folder not found: open Steam at least once." },
        { "\nDebug CEF di Steam: gia' attivo.", "\nSteam CEF debugger: already enabled." },
        { "\nDebug CEF di Steam: attivato.", "\nSteam CEF debugger: enabled." },
        { "\nATTENZIONE: non ho potuto creare il file CEF nella cartella di Steam.\n", "\nWARNING: could not create the CEF file in the Steam folder.\n" },
        { "Chiudi Steam e premi 'Crea file CEF', oppure riapri questo\n", "Close Steam and press 'Create CEF file', or reopen this\n" },
        { "programma come amministratore.", "program as administrator." },
        { "\nDecky avviato.", "\nDecky started." },
        { "\nLa porta 1337 e' occupata: chiudi il programma che la usa.", "\nPort 1337 is in use: close the program using it." },
        { "\n\nOra riavvia Steam completamente.", "\n\nNow restart Steam completely." },
        { "Installazione completata", "Installation complete" },
        { "Non trovo la cartella di Steam.", "Steam folder not found." },
        { "File creato in:\n", "File created in:\n" },
        { "\n\nRiavvia Steam completamente.", "\n\nRestart Steam completely." },
        { "Fatto", "Done" },
        { "Accesso negato.\nRiapri questo programma come amministratore, oppure crea a mano\n", "Access denied.\nReopen this program as administrator, or manually create\n" },
        { "un file vuoto chiamato .cef-enable-remote-debugging in:\n", "an empty file named .cef-enable-remote-debugging in:\n" },
        { "Servono i permessi", "Permission required" },
        { "Verranno scaricati gli aggiornamenti e Decky verra' ricompilato.\n", "The latest sources will be downloaded and Decky rebuilt.\n" },
        { "Servono alcuni minuti e il loader si chiude temporaneamente.\n\nProcedere?", "It takes a few minutes and the loader stops meanwhile.\n\nContinue?" },
        { "Aggiorna Decky", "Update Decky" },
        { "Aggiornamento...", "Updating..." },
        { "Anche questo pannello e' stato rigenerato con la versione nuova\nincorporata dentro.\n\nLo riavvio adesso per usarla?", "This panel was rebuilt too, with the new version\nembedded.\n\nRestart it now to use it?" },
        { "Pannello aggiornato", "Panel updated" },
        { "scarico la build ufficiale... ", "downloading the official build... " },
        { "cerco l'ultima build ufficiale...", "looking for the latest official build..." },
        { "L'ultima release di Decky non contiene la versione per Windows.", "The latest Decky release has no Windows build." },
        { "Il file scaricato non e' un eseguibile valido.", "The downloaded file is not a valid executable." },
        { "Il file scaricato e' danneggiato (controllo SHA-256 fallito).", "The downloaded file is corrupted (SHA-256 check failed)." },
        { "installo...", "installing..." },
        { "Scarico da GitHub l'ultima versione ufficiale di Decky per Windows\n(circa 30 MB) e la installo al posto di quella attuale.\n\nIl loader si chiude per un momento. Procedere?", "I will download the latest official Decky build for Windows from GitHub\n(about 30 MB) and install it over the current one.\n\nThe loader stops for a moment. Continue?" },
        { "Decky aggiornato", "Decky updated" },
        { "Installata la versione ", "Installed version " },
        { ". Riavvia Steam per usarla.", ". Restart Steam to use it." },
        { "Decky aggiornato a ", "Decky updated to " },
        { ".\n\nRiavvia Steam per ricaricare l'interfaccia.", ".\n\nRestart Steam to reload the interface." },
        { "Aggiornamento completato", "Update complete" },
        { "aggiornamento automatico non riuscito", "automatic update failed" },
        { "Aggiornamento non riuscito:\n", "Update failed:\n" },
    };

    public static string T(string it)
    {
        string e;
        return (En && en.TryGetValue(it, out e)) ? e : it;
    }
}

public class MainForm : Form
{
    static readonly string Home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    static readonly string ServicesDir = Path.Combine(Home, "homebrew\\services");
    static readonly string ExeNoConsole = Path.Combine(ServicesDir, "PluginLoader_noconsole.exe");
    static readonly string LogsDir = Path.Combine(Home, "homebrew\\logs");
    static readonly string PluginsDir = Path.Combine(Home, "homebrew\\plugins");
    static readonly string BuildInfo = Path.Combine(ServicesDir, "build-info.txt");
    static readonly string SettingsFile = Path.Combine(Home, "homebrew\\settings\\decky-manager.ini");
    static readonly string StartupLnk = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Decky Loader.lnk");
    const string RunKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    const string RunValue = "DeckyLoader";
    static readonly string AppDir = Path.GetDirectoryName(Application.ExecutablePath);
    // Catturato all'avvio. Durante l'aggiornamento il file in esecuzione viene
    // rinominato e Application.ExecutablePath, letto dopo, seguirebbe il file
    // rinominato; qui resta il nome originale, qualunque sia (i browser salvano
    // spesso "DeckyManager (1).exe").
    static readonly string SelfPath = Application.ExecutablePath;

    Label lblLoader, lblSteam, lblCef8080, lblPort1337, lblCefFile, lblPlugins, lblVersion, lblUpdate;
    ThemedCheck chkAutostart;
    ThemedCheck chkTray;
    ThemedCheck chkAuto;
    volatile bool updating = false;
    int checking = 0;                              // 1 mentre un controllo e' in corso
    DateTime lastAutoAttempt = DateTime.MinValue;  // ultimo aggiornamento automatico tentato
    NotifyIcon tray;
    ToolStripMenuItem miToggleLoader;
    bool startHidden = false;
    bool trayEnabled = false;
    RoundButton btnStartStop, btnUpdate, btnFixCef, btnInstall;
    ToolTip tips = new ToolTip();
    List<Card> cards = new List<Card>();
    bool suppressToggle = false;

    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    // Istanza singola: una seconda copia segnala questo evento alla prima, che
    // si mostra, e poi si chiude; cosi' due pannelli non aggiornano e fermano
    // Decky insieme.
    const string ShowEventName = "Local\\DeckyManager.Show";
    static EventWaitHandle showEvent;

    void ListenForShowRequests()
    {
        if (showEvent == null) return;
        Thread t = new Thread(delegate()
        {
            try
            {
                while (true)
                {
                    showEvent.WaitOne();
                    if (IsDisposed) return;
                    UI(delegate { OnTrayOpen(this, EventArgs.Empty); });
                }
            }
            catch { }
        });
        t.IsBackground = true;
        t.Start();
    }

    public MainForm(bool minimized)
    {
        startHidden = minimized;
        Text = "Decky Loader Manager";
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f);
        BackColor = Th.Bg;
        ForeColor = Th.Text;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

        FlowLayoutPanel root = new FlowLayoutPanel();
        root.FlowDirection = FlowDirection.TopDown;
        root.WrapContents = false;
        root.AutoSize = true;
        root.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        root.BackColor = Th.Bg;
        root.Padding = new Padding(20, 18, 20, 18);
        Controls.Add(root);

        // ---- intestazione: icona + titolo
        FlowLayoutPanel head = new FlowLayoutPanel();
        head.FlowDirection = FlowDirection.LeftToRight;
        head.WrapContents = false;
        head.AutoSize = true;
        head.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        head.BackColor = Th.Bg;
        head.Margin = new Padding(0, 0, 0, 16);
        root.Controls.Add(head);

        try
        {
            PictureBox pic = new PictureBox();
            pic.Image = new Icon(Icon.ExtractAssociatedIcon(Application.ExecutablePath), 40, 40).ToBitmap();
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            pic.Size = new Size(40, 40);
            pic.BackColor = Th.Bg;
            pic.Margin = new Padding(0, 2, 12, 0);
            head.Controls.Add(pic);
        }
        catch { }

        FlowLayoutPanel titles = new FlowLayoutPanel();
        titles.FlowDirection = FlowDirection.TopDown;
        titles.WrapContents = false;
        titles.AutoSize = true;
        titles.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        titles.BackColor = Th.Bg;
        titles.Margin = new Padding(0);
        head.Controls.Add(titles);

        Label t1 = new Label();
        t1.Text = "DECKY LOADER";
        t1.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
        t1.ForeColor = Th.Accent;
        t1.AutoSize = true;
        t1.BackColor = Color.Transparent;
        t1.Margin = new Padding(0);
        FlowLayoutPanel titleRow = new FlowLayoutPanel();
        titleRow.FlowDirection = FlowDirection.LeftToRight;
        titleRow.WrapContents = false;
        titleRow.AutoSize = true;
        titleRow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        titleRow.BackColor = Th.Bg;
        titleRow.Margin = new Padding(0);
        titles.Controls.Add(titleRow);
        titleRow.Controls.Add(t1);
        titleRow.Controls.Add(LinguaLink("ITA", "it"));
        Label sep = new Label();
        sep.Text = "|";
        sep.AutoSize = true;
        sep.BackColor = Color.Transparent;
        sep.ForeColor = Th.Line;
        sep.Font = new Font("Segoe UI", 8.5f);
        sep.Margin = new Padding(0, 11, 0, 0);
        titleRow.Controls.Add(sep);
        titleRow.Controls.Add(LinguaLink("ENG", "en"));

        Label t2 = new Label();
        t2.Text = L.T("per Steam Big Picture su Windows");
        t2.ForeColor = Th.Sub;
        t2.AutoSize = true;
        t2.BackColor = Color.Transparent;
        t2.Margin = new Padding(2, 0, 0, 0);
        titles.Controls.Add(t2);

        // ---- card stato
        TableLayoutPanel st = AddCard(root, L.T("STATO"));
        lblLoader   = AddRow(st, "Decky Loader");
        lblSteam    = AddRow(st, "Steam");
        lblCef8080  = AddRow(st, L.T("Debug CEF (porta 8080)"));
        lblPort1337 = AddRow(st, L.T("Server Decky (porta 1337)"));
        lblCefFile  = AddRow(st, L.T("File CEF in Steam"));
        lblPlugins  = AddRow(st, L.T("Plugin installati"));

        // ---- card versione
        TableLayoutPanel vt = AddCard(root, L.T("VERSIONE"));
        lblVersion = AddRow(vt, L.T("Installata"));
        lblUpdate  = AddRow(vt, L.T("Aggiornamenti"));

        chkAutostart = new ThemedCheck();
        chkAutostart.Text = L.T("Avvia Decky automaticamente all'accensione del PC");
        chkAutostart.AutoSize = true;
        chkAutostart.Margin = new Padding(4, 16, 0, 2);
        chkAutostart.CheckedChanged += new EventHandler(OnAutostartToggled);
        root.Controls.Add(chkAutostart);

        chkTray = new ThemedCheck();
        chkTray.Text = L.T("Avvia ridotto a icona nella barra di sistema");
        chkTray.AutoSize = true;
        chkTray.Margin = new Padding(28, 0, 0, 6);
        chkTray.CheckedChanged += new EventHandler(OnTrayOptionToggled);
        tips.SetToolTip(chkTray, L.T("All'accensione parte anche questo pannello, nascosto vicino\nall'orologio: un clic sull'icona lo riapre."));
        root.Controls.Add(chkTray);

        chkAuto = new ThemedCheck();
        chkAuto.Text = L.T("Aggiorna Decky da solo quando esce una versione nuova");
        chkAuto.AutoSize = true;
        chkAuto.Margin = new Padding(4, 0, 0, 12);
        chkAuto.CheckedChanged += new EventHandler(OnAutoUpdateToggled);
        tips.SetToolTip(chkAuto, L.T("Al momento dell'apertura, se esiste una versione piu' recente\nviene scaricata e installata da sola.\nNon lo fa mentre e' in corso un gioco."));
        root.Controls.Add(chkAuto);

        FlowLayoutPanel row1 = AddButtonRow(root);
        btnInstall = AddButton(row1, L.T("Installa Decky"), new EventHandler(OnInstall), false);
        tips.SetToolTip(btnInstall, L.T("Installa Decky e prepara il PC.\nUsalo alla prima installazione o per reinstallare."));
        btnFixCef = AddButton(row1, L.T("Crea file CEF"), new EventHandler(OnFixCef), false);
        tips.SetToolTip(btnFixCef, L.T("Primo passo su un PC nuovo: abilita il debug CEF di Steam.\nSi disattiva da solo quando il file e' gia' presente."));
        btnStartStop = AddButton(row1, L.T("Avvia Decky"), new EventHandler(OnStartStop), false);

        FlowLayoutPanel row2 = AddButtonRow(root);
        btnUpdate = AddButton(row2, L.T("Aggiorna ora"), new EventHandler(OnUpdate), false);
        AddButton(row2, L.T("Controlla aggiornamenti"), new EventHandler(OnRefresh), false);
        AddButton(row2, L.T("Apri cartella log"), new EventHandler(OnLogs), false);

        Label hint = new Label();
        hint.Text = L.T("Dopo un aggiornamento riavvia Steam per ricaricare l'interfaccia.");
        hint.ForeColor = Th.Sub;
        hint.BackColor = Color.Transparent;
        hint.AutoSize = true;
        hint.Margin = new Padding(4, 14, 0, 0);
        root.Controls.Add(hint);

        SelfHealAutostart();
        RefreshStatus();
        // il controllo aggiornamenti parte in OnShown: qui la finestra non ha
        // ancora un handle e il thread toccherebbe i controlli dal thread sbagliato
    }

    // Se l'avvio automatico lancia questo pannello ma la cartella e' stata
    // spostata, la scorciatoia punterebbe a un file inesistente: la riallinea.
    void SelfHealAutostart()
    {
        try
        {
            if (!AutostartEnabled() || !AutostartIsTrayMode()) return;
            string target = ShortcutTarget(StartupLnk);
            if (string.Equals(target, SelfPath, StringComparison.OrdinalIgnoreCase)) return;
            // se punta a un'altra copia che esiste ancora, e' una scelta dell'utente:
            // si interviene solo quando il file e' sparito (cartella spostata o rinominata)
            if (target.Length > 0 && File.Exists(target)) return;
            CreateShortcut(StartupLnk, SelfPath, AppDir, "--minimized");
        }
        catch { }
    }

    void UpdateTray()
    {
        bool want = startHidden || chkTray.Checked;
        if (want && tray == null) SetupTray();
        else if (!want && tray != null)
        {
            tray.Visible = false;
            tray.Dispose();
            tray = null;
            miToggleLoader = null;
        }
        trayEnabled = (tray != null);
    }

    void SetupTray()
    {
        tray = new NotifyIcon();
        try { tray.Icon = Icon; } catch { }
        tray.Text = "Decky Loader";
        ContextMenuStrip menu = new ContextMenuStrip();
        ToolStripMenuItem miOpen = new ToolStripMenuItem(L.T("Apri pannello"));
        miOpen.Click += new EventHandler(OnTrayOpen);
        miToggleLoader = new ToolStripMenuItem(
            LoaderProcesses().Length > 0 ? L.T("Ferma Decky") : L.T("Avvia Decky"));
        miToggleLoader.Click += new EventHandler(OnStartStop);
        ToolStripMenuItem miExit = new ToolStripMenuItem(L.T("Chiudi pannello (Decky resta attivo)"));
        miExit.Click += new EventHandler(OnTrayExit);
        menu.Items.Add(miOpen);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(miToggleLoader);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(miExit);
        tray.ContextMenuStrip = menu;
        tray.DoubleClick += new EventHandler(OnTrayOpen);
        tray.MouseClick += new MouseEventHandler(OnTrayClick);
        tray.Visible = true;
    }

    void OnTrayClick(object s, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left) OnTrayOpen(s, EventArgs.Empty);
    }

    void OnTrayOpen(object s, EventArgs e)
    {
        Show();
        ShowInTaskbar = true;
        WindowState = FormWindowState.Normal;
        Activate();
        RefreshStatus();
        StartThread(CheckUpdatesWorker);
    }

    void OnTrayExit(object s, EventArgs e)
    {
        trayEnabled = false;
        startHidden = false;
        if (tray != null) { tray.Visible = false; tray.Dispose(); tray = null; }
        Close();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (startHidden) { Hide(); ShowInTaskbar = false; }
        ListenForShowRequests();
        StartThread(CheckUpdatesWorker);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (trayEnabled && tray != null && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            ShowInTaskbar = false;
            return;
        }
        if (tray != null) { tray.Visible = false; tray.Dispose(); tray = null; }
        base.OnFormClosing(e);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        try
        {
            int on = 1;
            if (DwmSetWindowAttribute(Handle, 20, ref on, 4) != 0)
                DwmSetWindowAttribute(Handle, 19, ref on, 4);
        }
        catch { }
    }

    Label LinguaLink(string testo, string codice)
    {
        bool attiva = (L.En ? "en" : "it") == codice;
        Label l = new Label();
        l.Text = testo;
        l.AutoSize = true;
        l.BackColor = Color.Transparent;
        l.Font = new Font("Segoe UI", 8.5f, attiva ? FontStyle.Bold : FontStyle.Regular);
        l.ForeColor = attiva ? Th.Accent : Th.Sub;
        l.Margin = new Padding(codice == "it" ? 16 : 0, 11, 0, 0);
        if (!attiva)
        {
            l.Cursor = Cursors.Hand;
            l.MouseEnter += delegate { l.ForeColor = Th.Text; };
            l.MouseLeave += delegate { l.ForeColor = Th.Sub; };
            l.Click += delegate { CambiaLingua(codice); };
        }
        return l;
    }

    // La lingua si applica alla costruzione della finestra: si salva e si riapre.
    void CambiaLingua(string codice)
    {
        WriteSetting("Language", codice);
        try
        {
            LiberaIstanza();
            Process.Start(new ProcessStartInfo(SelfPath) { WorkingDirectory = AppDir, UseShellExecute = true });
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, L.T("Errore")); return; }
        if (tray != null) { tray.Visible = false; tray.Dispose(); tray = null; }
        trayEnabled = false;
        Application.Exit();
    }

    TableLayoutPanel AddCard(FlowLayoutPanel parent, string header)
    {
        Label h = new Label();
        h.Text = header;
        h.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        h.ForeColor = Th.Sub;
        h.AutoSize = true;
        h.BackColor = Color.Transparent;
        h.Margin = new Padding(4, 0, 0, 6);
        parent.Controls.Add(h);

        Card card = new Card();
        card.AutoSize = true;
        card.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        card.Margin = new Padding(0, 0, 0, 12);
        parent.Controls.Add(card);
        cards.Add(card);

        TableLayoutPanel t = new TableLayoutPanel();
        t.ColumnCount = 2;
        t.AutoSize = true;
        t.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        t.BackColor = Color.Transparent;
        t.Padding = new Padding(14, 12, 18, 12);
        card.Controls.Add(t);
        return t;
    }

    void EqualizeCards()
    {
        int max = 0;
        foreach (Card c in cards)
        {
            c.MinimumSize = Size.Empty;
            int w = c.PreferredSize.Width;
            if (w > max) max = w;
        }
        foreach (Card c in cards) c.MinimumSize = new Size(max, 0);
    }

    Label AddRow(TableLayoutPanel t, string name)
    {
        Label k = new Label();
        k.Text = name;
        k.ForeColor = Th.Sub;
        k.BackColor = Color.Transparent;
        k.AutoSize = true;
        k.Margin = new Padding(0, 4, 26, 4);
        k.MinimumSize = new Size(165, 0);

        Label v = new Label();
        v.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        v.ForeColor = Th.Text;
        v.BackColor = Color.Transparent;
        v.AutoSize = true;
        v.Margin = new Padding(0, 4, 0, 4);
        v.MinimumSize = new Size(240, 0);

        int r = t.RowCount;
        t.Controls.Add(k, 0, r);
        t.Controls.Add(v, 1, r);
        t.RowCount = r + 1;
        return v;
    }

    FlowLayoutPanel AddButtonRow(FlowLayoutPanel parent)
    {
        FlowLayoutPanel f = new FlowLayoutPanel();
        f.FlowDirection = FlowDirection.LeftToRight;
        f.WrapContents = false;
        f.AutoSize = true;
        f.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        f.BackColor = Th.Bg;
        f.Margin = new Padding(2, 0, 0, 8);
        parent.Controls.Add(f);
        return f;
    }

    RoundButton AddButton(FlowLayoutPanel row, string text, EventHandler h, bool primary)
    {
        RoundButton b = new RoundButton();
        b.Text = text;
        b.Primary = primary;
        b.AutoSize = true;
        b.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        b.Padding = new Padding(16, 9, 16, 9);
        b.Margin = new Padding(0, 0, 10, 0);
        b.Click += h;
        row.Controls.Add(b);
        return b;
    }

    void Set(Label l, bool ok, string text) { l.Text = text; l.ForeColor = ok ? Th.Good : Th.Bad; }
    void SetNeutral(Label l, string text) { l.Text = text; l.ForeColor = Th.Text; }

    static Process[] LoaderProcesses()
    {
        Process[] a = Process.GetProcessesByName("PluginLoader_noconsole");
        Process[] b = Process.GetProcessesByName("PluginLoader");
        Process[] all = new Process[a.Length + b.Length];
        a.CopyTo(all, 0);
        b.CopyTo(all, a.Length);
        return all;
    }

    static bool PortInUse(int port)
    {
        try
        {
            IPEndPoint[] eps = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            foreach (IPEndPoint ep in eps) if (ep.Port == port) return true;
        }
        catch { }
        return false;
    }

    static string SteamPath()
    {
        try
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam");
            if (k != null)
            {
                object v = k.GetValue("SteamPath");
                if (v != null)
                {
                    string p = v.ToString().Replace('/', '\\');
                    if (Directory.Exists(p)) return p;
                }
            }
        }
        catch { }
        string fallback = "C:\\Program Files (x86)\\Steam";
        return Directory.Exists(fallback) ? fallback : null;
    }

    static string CefFilePath()
    {
        string s = SteamPath();
        return s == null ? null : Path.Combine(s, ".cef-enable-remote-debugging");
    }

    static string ShortcutTarget(string lnkPath)
    {
        try
        {
            if (!File.Exists(lnkPath)) return "";
            Type t = Type.GetTypeFromProgID("WScript.Shell");
            object shell = Activator.CreateInstance(t);
            object lnk = t.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { lnkPath });
            object v = lnk.GetType().InvokeMember("TargetPath", BindingFlags.GetProperty, null, lnk, null);
            return v == null ? "" : v.ToString();
        }
        catch { return ""; }
    }

    static bool AutostartIsTrayMode()
    {
        return ShortcutTarget(StartupLnk).ToLower().EndsWith("deckymanager.exe");
    }

    // Steam parte da solo all'accensione, quindi non basta "Steam aperto":
    // si guarda se c'e' davvero un gioco in esecuzione.
    static bool GiocoInCorso()
    {
        try
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey("Software" + Path.DirectorySeparatorChar + "Valve" + Path.DirectorySeparatorChar + "Steam");
            if (k != null)
            {
                object v = k.GetValue("RunningAppID");
                if (v != null && Convert.ToInt32(v) != 0) return true;
            }
        }
        catch { }
        return false;
    }

    static string ReadSetting(string key, string def)
    {
        try
        {
            if (File.Exists(SettingsFile))
                foreach (string l in File.ReadAllLines(SettingsFile))
                    if (l.StartsWith(key + "="))
                        return l.Substring(key.Length + 1).Trim();
        }
        catch { }
        return def;
    }

    static void WriteSetting(string key, string val)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsFile));
            List<string> righe = new List<string>();
            bool trovata = false;
            if (File.Exists(SettingsFile))
                foreach (string l in File.ReadAllLines(SettingsFile))
                {
                    if (l.StartsWith(key + "=")) { righe.Add(key + "=" + val); trovata = true; }
                    else if (l.Trim().Length > 0) righe.Add(l);
                }
            if (!trovata) righe.Add(key + "=" + val);
            File.WriteAllLines(SettingsFile, righe.ToArray());
        }
        catch { }
    }

    // acceso di serie: cosi' resta aggiornato da solo
    static bool ReadAutoUpdate() { return ReadSetting("AutoUpdate", "1") == "1"; }
    static void WriteAutoUpdate(bool v) { WriteSetting("AutoUpdate", v ? "1" : "0"); }

    void OnAutoUpdateToggled(object s, EventArgs e)
    {
        if (suppressToggle) return;
        WriteAutoUpdate(chkAuto.Checked);
    }

    static bool AutostartEnabled()
    {
        if (File.Exists(StartupLnk)) return true;
        try
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RunKey);
            if (k != null && k.GetValue(RunValue) != null) return true;
        }
        catch { }
        return false;
    }

    static bool HasResource(string name)
    {
        try
        {
            using (Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
                return st != null;
        }
        catch { return false; }
    }

    static void ExtractResource(string name, string destPath)
    {
        using (Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
        {
            if (st == null) throw new Exception(L.T("Risorsa mancante nell'eseguibile: ") + name);
            using (FileStream f = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                byte[] buf = new byte[81920];
                int n;
                while ((n = st.Read(buf, 0, buf.Length)) > 0) f.Write(buf, 0, n);
            }
        }
    }

    static string EmbeddedBuildInfo()
    {
        try
        {
            using (Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream("buildinfo"))
            {
                if (st == null) return "";
                using (StreamReader sr = new StreamReader(st)) return sr.ReadToEnd().Trim();
            }
        }
        catch { return ""; }
    }

    static string Short(string sha)
    {
        return (sha != null && sha.Length >= 7) ? sha.Substring(0, 7) : sha;
    }

    static string[] ReadBuildInfo()
    {
        try
        {
            string raw = File.Exists(BuildInfo) ? File.ReadAllText(BuildInfo).Trim() : EmbeddedBuildInfo();
            if (raw.Length == 0) return null;
            string[] parts = raw.Split('|');
            if (parts.Length < 2) return null;
            if (parts[0].Length == 0) parts[0] = L.T("versione ignota");
            return parts;
        }
        catch { return null; }
    }

    void RefreshStatus()
    {
        bool running = LoaderProcesses().Length > 0;
        bool installed = LoaderInstalled();
        if (running) Set(lblLoader, true, L.T("In esecuzione"));
        else if (installed) Set(lblLoader, false, L.T("Fermo"));
        else Set(lblLoader, false, L.T("NON INSTALLATO"));
        btnStartStop.Text = running ? L.T("Ferma Decky") : L.T("Avvia Decky");
        btnStartStop.Enabled = installed;
        btnInstall.Primary = !installed;
        btnInstall.Invalidate();
        if (miToggleLoader != null) miToggleLoader.Text = running ? L.T("Ferma Decky") : L.T("Avvia Decky");

        bool steam = Process.GetProcessesByName("steam").Length > 0;
        Set(lblSteam, steam, steam ? L.T("In esecuzione") : L.T("Chiuso"));

        bool cef = PortInUse(8080);
        if (cef) Set(lblCef8080, true, L.T("Attivo"));
        else if (steam) Set(lblCef8080, false, L.T("Non attivo"));
        else SetNeutral(lblCef8080, L.T("in attesa di Steam"));

        bool p1337 = PortInUse(1337);
        if (running && p1337) Set(lblPort1337, true, L.T("Attivo"));
        else if (!running && p1337) Set(lblPort1337, false, L.T("Occupata da un altro programma"));
        else SetNeutral(lblPort1337, L.T("Libera"));

        string cefFile = CefFilePath();
        bool hasCef = cefFile != null && File.Exists(cefFile);
        Set(lblCefFile, hasCef, hasCef ? L.T("Presente") : L.T("MANCANTE"));
        btnFixCef.Enabled = !hasCef;
        btnFixCef.Primary = !hasCef;
        bool canBuild = CanBuild();
        btnUpdate.Enabled = true;
        tips.SetToolTip(btnUpdate, canBuild
            ? L.T("Ricompila Decky dai sorgenti presenti su questo PC.")
            : L.T("Scarica e installa l'ultima versione ufficiale di Decky da GitHub.\nNon serve nient'altro: basta la connessione a internet."));
        btnFixCef.Invalidate();

        try
        {
            string[] dirs = Directory.Exists(PluginsDir) ? Directory.GetDirectories(PluginsDir) : new string[0];
            if (dirs.Length == 0) SetNeutral(lblPlugins, L.T("nessuno"));
            else
            {
                string names = "";
                for (int i = 0; i < dirs.Length; i++)
                    names += (i > 0 ? ", " : "") + Path.GetFileName(dirs[i]);
                SetNeutral(lblPlugins, dirs.Length.ToString());
                tips.SetToolTip(lblPlugins, names);
            }
        }
        catch { SetNeutral(lblPlugins, "?"); }

        suppressToggle = true;
        chkAutostart.Checked = AutostartEnabled();
        chkTray.Checked = AutostartIsTrayMode();
        chkTray.Enabled = chkAutostart.Checked;
        chkAuto.Checked = ReadAutoUpdate();
        suppressToggle = false;

        UpdateTray();

        string date = File.Exists(ExeNoConsole)
            ? File.GetLastWriteTime(ExeNoConsole).ToString("dd/MM/yyyy") : "?";
        string[] bi = ReadBuildInfo();
        if (bi != null && bi[1].Length >= 7) SetNeutral(lblVersion, bi[0] + " (" + Short(bi[1]) + L.T(")  -  compilata il ") + date);
        else if (bi != null) SetNeutral(lblVersion, bi[0] + L.T("  -  installata il ") + date);
        else SetNeutral(lblVersion, L.T("sconosciuta  -  exe del ") + date);

        EqualizeCards();
    }

    static int gitCache = -1;
    static bool GitAvailable()
    {
        if (gitCache >= 0) return gitCache == 1;
        try
        {
            ProcessStartInfo si = new ProcessStartInfo("git", "--version");
            si.UseShellExecute = false;
            si.RedirectStandardOutput = true;
            si.RedirectStandardError = true;
            si.CreateNoWindow = true;
            Process p = Process.Start(si);
            p.StandardOutput.ReadToEnd();
            p.WaitForExit(8000);
            gitCache = 1;
        }
        catch { gitCache = 0; }
        return gitCache == 1;
    }

    // Script opzionale che compila Decky dai sorgenti (tools\build-from-source.bat
    // messo accanto all'exe). Se non c'e', si usa la release ufficiale.
    static string BuildScript()
    {
        string f = Path.Combine(AppDir, "build-from-source.bat");
        return File.Exists(f) ? f : null;
    }

    static bool CanBuild()
    {
        return BuildScript() != null && GitAvailable();
    }

    static readonly string LogFile = Path.Combine(LogsDir, "decky-manager.log");

    static void Log(string msg)
    {
        try
        {
            Directory.CreateDirectory(LogsDir);
            File.AppendAllText(LogFile, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + msg + Environment.NewLine);
        }
        catch { }
    }

    // Esegue sul thread dell'interfaccia; non fa nulla se la finestra non esiste
    // (ancora o piu'), invece di lanciare eccezioni dal thread in background.
    void UI(MethodInvoker m)
    {
        try
        {
            if (IsDisposed || !IsHandleCreated) return;
            if (InvokeRequired) Invoke(m); else m();
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException ex) { Log("UI: " + ex.Message); }
    }

    void StartThread(ThreadStart work)
    {
        Thread t = new Thread(delegate()
        {
            try { work(); }
            catch (Exception ex) { Log("errore nel thread: " + ex); }
        });
        t.IsBackground = true;
        t.Start();
    }

    void CheckUpdatesWorker()
    {
        // un solo controllo alla volta (apertura dal tray + pulsante, ecc.)
        if (Interlocked.Exchange(ref checking, 1) == 1) return;
        try { CheckUpdatesCore(); }
        finally { Interlocked.Exchange(ref checking, 0); }
    }

    // Confronta la versione installata (build-info.txt) con l'ultima release
    // ufficiale di Decky. Dopo un aggiornamento le due coincidono per forza,
    // quindi l'aggiornamento automatico non puo' piu' ripetersi all'infinito.
    void CheckUpdatesCore()
    {
        if (updating) return;
        SetUpdateLabel(L.T("controllo in corso..."), Th.Sub);

        ReleaseInfo rel;
        try { rel = LatestRelease(); }
        catch (WebException ex)
        {
            HttpWebResponse r = ex.Response as HttpWebResponse;
            bool limite = r != null && ((int)r.StatusCode == 403 || (int)r.StatusCode == 429);
            Log("controllo aggiornamenti: " + ex.Message);
            SetUpdateLabel(limite ? L.T("limite di richieste GitHub raggiunto: riprova tra un'ora")
                                  : L.T("controllo fallito (nessuna rete?)"), Th.Sub);
            return;
        }
        catch (Exception ex)
        {
            Log("controllo aggiornamenti: " + ex.Message);
            SetUpdateLabel(L.T("controllo fallito (nessuna rete?)"), Th.Sub);
            return;
        }

        // non installato: il pulsante "Installa Decky" e' gia' quello evidenziato
        if (!LoaderInstalled()) { SetUpdateLabel(L.T("ultima versione disponibile: ") + rel.Tag, Th.Sub); return; }

        string[] built = ReadBuildInfo();
        string local = built != null ? built[0] : "";
        if (IsUpToDate(local, rel.Tag))
        {
            SetUpdateLabel(L.T("aggiornato all'ultima versione") + " (" + rel.Tag + ")", Th.Good);
            return;
        }

        SetUpdateLabel(L.T("AGGIORNAMENTO DISPONIBILE (") + rel.Tag + ")", Th.Warn, true);

        // aggiornamento automatico: solo se richiesto, non mentre si gioca e al
        // massimo un tentativo ogni 6 ore, cosi' un errore non diventa un ciclo
        if (ReadAutoUpdate() && !updating && !GiocoInCorso()
            && (DateTime.Now - lastAutoAttempt).TotalHours >= 6)
        {
            lastAutoAttempt = DateTime.Now;
            UI(delegate { OnlineUpdate(true); });
        }
    }

    // "v3.3.1" -> {3, 3, 1, 1}; "v3.3.1-pre1" -> {3, 3, 1, 0}; null se non e' una versione
    static int[] ParseVersion(string tag)
    {
        Match m = Regex.Match((tag ?? "").Trim(), "^v?(\\d+)\\.(\\d+)\\.(\\d+)(-\\S+)?$");
        if (!m.Success) return null;
        return new int[] {
            int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value),
            int.Parse(m.Groups[3].Value), m.Groups[4].Success ? 0 : 1 };
    }

    // true se la versione installata e' uguale o piu' recente dell'ultima release
    // (una build compilata dai sorgenti puo' essere piu' avanti: non va sovrascritta)
    static bool IsUpToDate(string local, string latest)
    {
        if (string.Equals((local ?? "").Trim(), latest, StringComparison.OrdinalIgnoreCase)) return true;
        int[] a = ParseVersion(local), b = ParseVersion(latest);
        if (a == null || b == null) return false;
        for (int i = 0; i < a.Length; i++)
            if (a[i] != b[i]) return a[i] > b[i];
        return true;
    }

    void SetUpdateLabel(string text, Color c) { SetUpdateLabel(text, c, false); }

    void SetUpdateLabel(string text, Color c, bool needsUpdate)
    {
        UI(delegate
        {
            btnUpdate.Primary = needsUpdate;
            btnUpdate.Invalidate();
            lblUpdate.Text = text;
            lblUpdate.ForeColor = c;
            EqualizeCards();
        });
    }

    void OnRefresh(object s, EventArgs e) { RefreshStatus(); StartThread(CheckUpdatesWorker); }

    void OnStartStop(object s, EventArgs e)
    {
        Process[] procs = LoaderProcesses();
        if (procs.Length > 0) StopLoader();
        else
        {
            if (!File.Exists(ExeNoConsole)) { MessageBox.Show(this, L.T("Non trovo:\n") + ExeNoConsole, "Decky"); return; }
            if (PortInUse(1337))
            {
                MessageBox.Show(this, L.T("La porta 1337 e' occupata da un altro programma\n") +
                    L.T("(spesso il servizio Razer Chroma SDK).\nDecky non puo' partire finche' non lo chiudi."),
                    L.T("Porta occupata"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                ProcessStartInfo si = new ProcessStartInfo(ExeNoConsole);
                si.WorkingDirectory = ServicesDir;
                si.UseShellExecute = false;
                Process.Start(si);
                Thread.Sleep(2500);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, L.T("Errore")); }
        }
        RefreshStatus();
    }

    void OnAutostartToggled(object s, EventArgs e)
    {
        if (suppressToggle) return;
        try
        {
            if (chkAutostart.Checked)
            {
                ApplyAutostart();
                RemoveRunKey();
                if (LoaderProcesses().Length == 0 && !PortInUse(1337))
                {
                    ProcessStartInfo si = new ProcessStartInfo(ExeNoConsole);
                    si.WorkingDirectory = ServicesDir;
                    si.UseShellExecute = false;
                    Process.Start(si);
                    Thread.Sleep(2000);
                }
            }
            else
            {
                if (File.Exists(StartupLnk)) File.Delete(StartupLnk);
                RemoveRunKey();
            }
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, L.T("Errore")); }
        RefreshStatus();
    }

    void ApplyAutostart()
    {
        if (chkTray.Checked)
            CreateShortcut(StartupLnk, SelfPath, AppDir, "--minimized");
        else
            CreateShortcut(StartupLnk, ExeNoConsole, ServicesDir, "");
    }

    void OnTrayOptionToggled(object s, EventArgs e)
    {
        if (suppressToggle) return;
        try { if (chkAutostart.Checked) ApplyAutostart(); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, L.T("Errore")); }
        RefreshStatus();
        if (tray != null) tray.Visible = true;
    }

    static void RemoveRunKey()
    {
        try
        {
            RegistryKey k = Registry.CurrentUser.OpenSubKey(RunKey, true);
            if (k != null && k.GetValue(RunValue) != null) k.DeleteValue(RunValue, false);
        }
        catch { }
    }

    static void CreateShortcut(string lnkPath, string target, string workDir, string args)
    {
        Type t = Type.GetTypeFromProgID("WScript.Shell");
        object shell = Activator.CreateInstance(t);
        object lnk = t.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { lnkPath });
        Type lt = lnk.GetType();
        lt.InvokeMember("TargetPath", BindingFlags.SetProperty, null, lnk, new object[] { target });
        lt.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, lnk, new object[] { workDir });
        lt.InvokeMember("Arguments", BindingFlags.SetProperty, null, lnk, new object[] { args });
        lt.InvokeMember("Description", BindingFlags.SetProperty, null, lnk, new object[] { "Decky Loader" });
        lt.InvokeMember("Save", BindingFlags.InvokeMethod, null, lnk, null);
    }

    static void CleanOldExe()
    {
        Thread t = new Thread(delegate()
        {
            // il processo precedente puo' essere ancora vivo e tenere il file
            // bloccato: si riprova per una ventina di secondi.
            for (int i = 0; i < 15; i++)
            {
                bool rimasti = false;
                try
                {
                    foreach (string f in Directory.GetFiles(AppDir, "DeckyManager.old*.exe"))
                    {
                        try { File.Delete(f); }
                        catch { rimasti = true; }
                    }
                }
                catch { }
                if (!rimasti) return;
                Thread.Sleep(2000);
            }
        });
        t.IsBackground = true;
        t.Start();
    }

    static bool LoaderInstalled()
    {
        return File.Exists(ExeNoConsole);
    }

    void OnInstall(object sender, EventArgs e)
    {
        bool already = LoaderInstalled();
        bool incorporato = HasResource("loader_noconsole");

        if (!incorporato)
        {
            // Build "leggera": i binari di Decky non sono inclusi, si prendono
            // dalla CI ufficiale del progetto.
            if (MessageBox.Show(this,
                (already ? L.T("Decky risulta gia' installato.\n\n") : "") +
                L.T("Scarico da GitHub l'ultima versione ufficiale di Decky per Windows\n(circa 30 MB) e la installo. Procedere?"),
                L.T("Installa Decky"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            if (updating) return;
            updating = true;
            btnInstall.Enabled = false;
            btnUpdate.Enabled = false;
            StartThread(delegate
            {
                try
                {
                    string tag = InstallaReleaseUfficiale();
                    UI(delegate
                    {
                        updating = false;
                        btnInstall.Enabled = true;
                        btnUpdate.Enabled = true;
                        CompletaInstallazione("Decky " + tag + L.T(" installato in:\n") + ServicesDir + "\n");
                    });
                }
                catch (Exception ex)
                {
                    Log("installazione: " + ex);
                    UI(delegate
                    {
                        updating = false;
                        btnInstall.Enabled = true;
                        btnUpdate.Enabled = true;
                        RefreshStatus();
                        MessageBox.Show(this, L.T("Installazione non riuscita:\n") + ex.Message, L.T("Errore"),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
            return;
        }

        if (already && MessageBox.Show(this,
            L.T("Decky risulta gia' installato.\nVuoi reinstallare la versione contenuta in questo programma?"),
            L.T("Reinstalla Decky"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        string esito = "";
        try
        {
            StopLoader();

            string hb = Path.Combine(Home, "homebrew");
            string[] dirs = new string[] { "services", "plugins", "settings", "logs", "data", "themes" };
            foreach (string d in dirs) Directory.CreateDirectory(Path.Combine(hb, d));

            ExtractResource("loader_noconsole", ExeNoConsole);
            ExtractResource("loader_console", Path.Combine(ServicesDir, "PluginLoader.exe"));
            string bi = EmbeddedBuildInfo();
            if (bi.Length > 0) File.WriteAllText(BuildInfo, bi);
            esito += L.T("Decky installato in:\n") + ServicesDir + "\n";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, L.T("Installazione non riuscita:\n") + ex.Message, L.T("Errore"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            RefreshStatus();
            return;
        }

        CompletaInstallazione(esito);
    }

    // Passi comuni dopo aver messo i loader al loro posto: file CEF, avvio, esito.
    void CompletaInstallazione(string esito)
    {
        string cef = CefFilePath();
        if (cef == null) esito += L.T("\nNon trovo la cartella di Steam: apri Steam almeno una volta.");
        else if (File.Exists(cef)) esito += L.T("\nDebug CEF di Steam: gia' attivo.");
        else
        {
            try { File.WriteAllText(cef, ""); esito += L.T("\nDebug CEF di Steam: attivato."); }
            catch { esito += L.T("\nATTENZIONE: non ho potuto creare il file CEF nella cartella di Steam.\n") +
                             L.T("Chiudi Steam e premi 'Crea file CEF', oppure riapri questo\n") +
                             L.T("programma come amministratore."); }
        }

        if (!PortInUse(1337))
        {
            try
            {
                ProcessStartInfo si = new ProcessStartInfo(ExeNoConsole);
                si.WorkingDirectory = ServicesDir;
                si.UseShellExecute = false;
                Process.Start(si);
                Thread.Sleep(2500);
                esito += L.T("\nDecky avviato.");
            }
            catch { }
        }
        else esito += L.T("\nLa porta 1337 e' occupata: chiudi il programma che la usa.");

        esito += L.T("\n\nOra riavvia Steam completamente.");
        RefreshStatus();
        StartThread(CheckUpdatesWorker);
        MessageBox.Show(this, esito, L.T("Installazione completata"), MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    void OnFixCef(object s, EventArgs e)
    {
        string f = CefFilePath();
        if (f == null) { MessageBox.Show(this, L.T("Non trovo la cartella di Steam."), "Decky"); return; }
        try
        {
            File.WriteAllText(f, "");
            MessageBox.Show(this, L.T("File creato in:\n") + f + L.T("\n\nRiavvia Steam completamente."), L.T("Fatto"));
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show(this, L.T("Accesso negato.\nRiapri questo programma come amministratore, oppure crea a mano\n") +
                L.T("un file vuoto chiamato .cef-enable-remote-debugging in:\n") + Path.GetDirectoryName(f),
                L.T("Servono i permessi"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, L.T("Errore")); }
        RefreshStatus();
    }

    void OnLogs(object s, EventArgs e)
    {
        try
        {
            if (!Directory.Exists(LogsDir)) Directory.CreateDirectory(LogsDir);
            Process.Start("explorer.exe", "\"" + LogsDir + "\"");
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, L.T("Errore")); }
    }

    void OnUpdate(object s, EventArgs e)
    {
        if (updating) return;
        string bat = BuildScript();
        if (!CanBuild() || bat == null) { OnlineUpdate(); return; }
        if (MessageBox.Show(this,
            L.T("Verranno scaricati gli aggiornamenti e Decky verra' ricompilato.\n") +
            L.T("Servono alcuni minuti e il loader si chiude temporaneamente.\n\nProcedere?"),
            L.T("Aggiorna Decky"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        updating = true;
        btnUpdate.Enabled = false;
        btnUpdate.Text = L.T("Aggiornamento...");
        DateTime selfBefore = DateTime.MinValue;
        try { selfBefore = File.GetLastWriteTime(SelfPath); } catch { }
        StartThread(delegate
        {
            try
            {
                ProcessStartInfo si = new ProcessStartInfo("cmd.exe", "/c \"\"" + bat + "\" auto\"");
                si.WorkingDirectory = AppDir;
                si.UseShellExecute = true;
                Process p = Process.Start(si);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                UI(delegate { MessageBox.Show(this, ex.Message, L.T("Errore")); });
            }
            UI(delegate
            {
                updating = false;
                btnUpdate.Enabled = true;
                btnUpdate.Text = L.T("Aggiorna ora");
                RefreshStatus();
                DateTime selfAfter = DateTime.MinValue;
                try { selfAfter = File.GetLastWriteTime(SelfPath); } catch { }
                if (selfBefore != DateTime.MinValue && selfAfter > selfBefore)
                {
                    if (MessageBox.Show(this,
                        L.T("Anche questo pannello e' stato rigenerato con la versione nuova\nincorporata dentro.\n\nLo riavvio adesso per usarla?"),
                        L.T("Pannello aggiornato"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        try
                        {
                            LiberaIstanza();
                            Process.Start(new ProcessStartInfo(SelfPath) { WorkingDirectory = AppDir, UseShellExecute = true });
                            if (tray != null) { tray.Visible = false; tray.Dispose(); tray = null; }
                            trayEnabled = false;
                            Application.Exit();
                            return;
                        }
                        catch (Exception ex2) { MessageBox.Show(this, ex2.Message, L.T("Errore")); }
                    }
                }
            });
            CheckUpdatesWorker();
        });
    }

    [DllImport("user32.dll")]
    static extern bool SetProcessDPIAware();

    static string HttpGet(string url)
    {
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.UserAgent = "DeckyManager";
        req.Timeout = 20000;
        using (WebResponse res = req.GetResponse())
        using (StreamReader sr = new StreamReader(res.GetResponseStream()))
            return sr.ReadToEnd();
    }

    void DownloadFile(string url, string dest)
    {
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        req.UserAgent = "DeckyManager";
        req.Timeout = 30000;
        req.ReadWriteTimeout = 120000;
        using (WebResponse res = req.GetResponse())
        using (Stream input = res.GetResponseStream())
        using (FileStream output = new FileStream(dest, FileMode.Create, FileAccess.Write))
        {
            long total = res.ContentLength;
            byte[] buf = new byte[65536];
            long done = 0;
            int last = -1, n;
            while ((n = input.Read(buf, 0, buf.Length)) > 0)
            {
                output.Write(buf, 0, n);
                done += n;
                if (total > 0)
                {
                    int pct = (int)(done * 100 / total);
                    if (pct != last) { last = pct; SetUpdateLabel(L.T("scarico la build ufficiale... ") + pct + "%", Th.Sub); }
                }
            }
        }
    }

    // ---------------------------------------- release ufficiali di Decky
    // Da v3.3.0 Decky pubblica gli exe per Windows come asset delle release
    // GitHub: niente artefatti CI che scadono, niente servizi esterni, e il
    // nome della versione installata coincide con quello della release.
    class ReleaseInfo
    {
        public string Tag = "";
        public string UrlNoConsole = "", UrlConsole = "";
        public string DigestNoConsole = "", DigestConsole = "";
    }

    static bool IsArm64()
    {
        string a = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
        if (string.IsNullOrEmpty(a)) a = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
        return a != null && a.ToUpperInvariant() == "ARM64";
    }

    static string Str(Dictionary<string, object> d, string key)
    {
        object v;
        return (d != null && d.TryGetValue(key, out v) && v != null) ? Convert.ToString(v) : "";
    }

    static ReleaseInfo LatestRelease()
    {
        string json = HttpGet("https://api.github.com/repos/SteamDeckHomebrew/decky-loader/releases/latest");
        JavaScriptSerializer js = new JavaScriptSerializer();
        js.MaxJsonLength = int.MaxValue;
        Dictionary<string, object> rel = js.Deserialize<Dictionary<string, object>>(json);

        ReleaseInfo r = new ReleaseInfo();
        r.Tag = Str(rel, "tag_name");
        string prefix = IsArm64() ? "PluginLoader-win-arm64" : "PluginLoader-win";
        object assets;
        if (rel.TryGetValue("assets", out assets) && assets is IEnumerable)
        {
            foreach (object o in (IEnumerable)assets)
            {
                Dictionary<string, object> a = o as Dictionary<string, object>;
                string name = Str(a, "name");
                if (name == prefix + "-noconsole.exe")
                {
                    r.UrlNoConsole = Str(a, "browser_download_url");
                    r.DigestNoConsole = Str(a, "digest");
                }
                else if (name == prefix + ".exe")
                {
                    r.UrlConsole = Str(a, "browser_download_url");
                    r.DigestConsole = Str(a, "digest");
                }
            }
        }
        if (r.Tag == "" || r.UrlNoConsole == "")
            throw new Exception(L.T("L'ultima release di Decky non contiene la versione per Windows."));
        return r;
    }

    // Scarica e controlla che sia un exe integro (intestazione MZ + SHA-256
    // pubblicato da GitHub, quando presente).
    void DownloadVerified(string url, string dest, string digest)
    {
        DownloadFile(url, dest);
        using (FileStream f = File.OpenRead(dest))
        {
            if (f.Length < 1024 * 1024 || f.ReadByte() != 'M' || f.ReadByte() != 'Z')
                throw new Exception(L.T("Il file scaricato non e' un eseguibile valido."));
            if (digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
            {
                f.Position = 0;
                byte[] h;
                using (SHA256 sha = SHA256.Create()) h = sha.ComputeHash(f);
                string hex = BitConverter.ToString(h).Replace("-", "").ToLowerInvariant();
                if (hex != digest.Substring(7).ToLowerInvariant())
                    throw new Exception(L.T("Il file scaricato e' danneggiato (controllo SHA-256 fallito)."));
            }
        }
    }

    // Chiude il loader e aspetta davvero che i processi siano usciti, altrimenti
    // gli exe restano bloccati e la copia fallisce.
    static void StopLoader()
    {
        foreach (Process p in LoaderProcesses()) { try { p.Kill(); } catch { } }
        foreach (Process p in LoaderProcesses()) { try { p.WaitForExit(10000); } catch { } }
        Thread.Sleep(500);
    }

    static void CopyWithRetry(string src, string dst)
    {
        for (int i = 0; ; i++)
        {
            try { File.Copy(src, dst, true); return; }
            catch (Exception ex)
            {
                if (!(ex is IOException || ex is UnauthorizedAccessException) || i >= 9) throw;
                Thread.Sleep(1000);
                StopLoader();
            }
        }
    }

    // Scarica l'ultima release ufficiale e la installa in homebrew\services.
    // Usato sia dall'aggiornamento sia dalla prima installazione quando l'exe
    // non porta i binari incorporati. Restituisce il nome della versione.
    string InstallaReleaseUfficiale()
    {
        string tmp = null;
        try
        {
            SetUpdateLabel(L.T("cerco l'ultima build ufficiale..."), Th.Sub);
            ReleaseInfo rel = LatestRelease();

            tmp = Path.Combine(Path.GetTempPath(), "decky-dl-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tmp);
            string srcNo = Path.Combine(tmp, "PluginLoader_noconsole.exe");
            string srcCo = Path.Combine(tmp, "PluginLoader.exe");
            // tutto scaricato e verificato prima di toccare il loader in uso
            DownloadVerified(rel.UrlNoConsole, srcNo, rel.DigestNoConsole);
            bool haConsole = false;
            if (rel.UrlConsole != "")
            {
                try { DownloadVerified(rel.UrlConsole, srcCo, rel.DigestConsole); haConsole = true; }
                catch (Exception ex) { Log("PluginLoader.exe non scaricato: " + ex.Message); }
            }

            SetUpdateLabel(L.T("installo..."), Th.Sub);
            StopLoader();
            Directory.CreateDirectory(ServicesDir);
            CopyWithRetry(srcNo, ExeNoConsole);
            if (haConsole)
            {
                try { CopyWithRetry(srcCo, Path.Combine(ServicesDir, "PluginLoader.exe")); }
                catch (Exception ex) { Log("PluginLoader.exe non copiato: " + ex.Message); }
            }
            File.WriteAllText(BuildInfo, rel.Tag + "||" + DateTime.Now.ToString("dd/MM/yyyy"));
            Log("installata " + rel.Tag);
            return rel.Tag;
        }
        finally
        {
            if (tmp != null) { try { Directory.Delete(tmp, true); } catch { } }
        }
    }

    void OnlineUpdate() { OnlineUpdate(false); }

    void OnlineUpdate(bool silenzioso)
    {
        if (updating) return;
        if (!silenzioso && MessageBox.Show(this,
            L.T("Scarico da GitHub l'ultima versione ufficiale di Decky per Windows\n(circa 30 MB) e la installo al posto di quella attuale.\n\nIl loader si chiude per un momento. Procedere?"),
            L.T("Aggiorna Decky"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        updating = true;
        btnUpdate.Enabled = false;
        btnUpdate.Text = L.T("Aggiornamento...");
        StartThread(delegate
        {
            try
            {
                string tag = InstallaReleaseUfficiale();
                StartLoaderIfNeeded();
                Thread.Sleep(2500);

                UI(delegate
                {
                    updating = false;
                    btnUpdate.Enabled = true;
                    btnUpdate.Text = L.T("Aggiorna ora");
                    RefreshStatus();
                    if (silenzioso)
                    {
                        if (tray != null)
                        {
                            tray.BalloonTipTitle = L.T("Decky aggiornato");
                            tray.BalloonTipText = L.T("Installata la versione ") + tag + L.T(". Riavvia Steam per usarla.");
                            tray.ShowBalloonTip(8000);
                        }
                    }
                    else MessageBox.Show(this,
                        L.T("Decky aggiornato a ") + tag + L.T(".\n\nRiavvia Steam per ricaricare l'interfaccia."),
                        L.T("Aggiornamento completato"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
                CheckUpdatesWorker();
            }
            catch (Exception ex)
            {
                Log("aggiornamento: " + ex);
                // se si e' fermato il loader prima dell'errore, lo si riavvia com'era
                StartLoaderIfNeeded();
                UI(delegate
                {
                    updating = false;
                    btnUpdate.Enabled = true;
                    btnUpdate.Text = L.T("Aggiorna ora");
                    RefreshStatus();
                    if (silenzioso) SetUpdateLabel(L.T("aggiornamento automatico non riuscito"), Th.Warn, true);
                    else MessageBox.Show(this, L.T("Aggiornamento non riuscito:\n") + ex.Message,
                        L.T("Errore"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                });
            }
        });
    }

    static void StartLoaderIfNeeded()
    {
        try
        {
            if (LoaderProcesses().Length > 0) return;
            if (!File.Exists(ExeNoConsole)) return;
            if (PortInUse(1337)) return;
            ProcessStartInfo si = new ProcessStartInfo(ExeNoConsole);
            si.WorkingDirectory = ServicesDir;
            si.UseShellExecute = false;
            Process.Start(si);
        }
        catch { }
    }

    [STAThread]
    public static void Main(string[] args)
    {
        try { if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware(); } catch { }
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        bool minimized = false;
        foreach (string a in args)
            if (a == "--minimized" || a == "-m") minimized = true;

        bool primo;
        unico = new Mutex(true, "Local\\DeckyManager.SingleInstance", out primo);
        showEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ShowEventName);
        if (!primo)
        {
            // gia' aperto (magari nascosto nel tray): lo si porta davanti
            if (!minimized) { try { showEvent.Set(); } catch { } }
            return;
        }

        // un errore imprevisto finisce nel log invece di chiudere il pannello
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += delegate(object s, ThreadExceptionEventArgs e) { Log("errore: " + e.Exception); };
        AppDomain.CurrentDomain.UnhandledException += delegate(object s, UnhandledExceptionEventArgs e) { Log("errore fatale: " + e.ExceptionObject); };

        CleanOldExe();

        // avviato dal boot: fa partire il loader al posto della vecchia scorciatoia
        if (minimized) StartLoaderIfNeeded();

        // lingua: scelta salvata, altrimenti quella di Windows
        string lingua = ReadSetting("Language", "auto");
        if (lingua != "it" && lingua != "en")
            lingua = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "it" ? "it" : "en";
        L.En = (lingua == "en");

        Application.Run(new MainForm(minimized));
        LiberaIstanza();
    }

    static Mutex unico;

    // Da chiamare prima di rilanciare se stessi (cambio lingua, pannello
    // aggiornato), altrimenti la nuova copia si troverebbe bloccata.
    static void LiberaIstanza()
    {
        try { if (unico != null) { unico.ReleaseMutex(); unico.Close(); } }
        catch { }
        unico = null;
    }
}
