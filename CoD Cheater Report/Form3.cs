using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System;

public partial class MainForm : Form
{
    private const string APP_NAME = "CoD Cheater Report";
    private const string ICON_PNG_URL = "https://raw.githubusercontent.com/HarleyTG-O/cod-report/main/Images/logo.png";
    private string TEMP_DIR;
    private string ICON_PNG_PATH;
    private string ICON_ICO_PATH;
    private System.Windows.Forms.TextBox txtLog;

    public MainForm()
    {
        InitializeComponent();
        InitDirectories();
    }

    private void InitializeComponent()
    {
        this.txtLog = new System.Windows.Forms.TextBox();
        this.SuspendLayout();
        // 
        // txtLog
        // 
        this.txtLog.Location = new System.Drawing.Point(12, 12);
        this.txtLog.Multiline = true;
        this.txtLog.Name = "txtLog";
        this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtLog.Size = new System.Drawing.Size(776, 426);
        this.txtLog.TabIndex = 0;
        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.txtLog);
        this.Name = "MainForm";
        this.Text = "CoD Cheater Report Installer";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void InitDirectories()
    {
        TEMP_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoD_Cheater_Report", "temp");
        ICON_PNG_PATH = Path.Combine(TEMP_DIR, "app_icon.png");
        ICON_ICO_PATH = Path.Combine(TEMP_DIR, "app_icon.ico");

        Directory.CreateDirectory(TEMP_DIR);
        Log("Directories Initialized.");
    }

    private void Log(string message)
    {
        string timestamp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
        txtLog.AppendText(timestamp + message + Environment.NewLine);
    }

    private bool DownloadIcon()
    {
        try
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(ICON_PNG_URL, ICON_PNG_PATH);
                Log("[✓] PNG Icon Downloaded.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Log("[✗] Failed to Download Icon: " + ex.Message);
            return false;
        }
    }

    private bool ConvertPngToIco()
    {
        try
        {
            using (var img = new Bitmap(ICON_PNG_PATH))
            {
                img.Save(ICON_ICO_PATH, System.Drawing.Imaging.ImageFormat.Icon);
                Log("[✓] Converted PNG to ICO.");
                return true;
            }
        }
        catch (Exception ex)
        {
            Log("[✗] Failed to Convert to ICO: " + ex.Message);
            return false;
        }
    }

    private void CreateShortcut()
    {
        if (DownloadIcon() && ConvertPngToIco())
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string shortcutPath = Path.Combine(desktop, $"{APP_NAME}.lnk");

            dynamic shell = Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));
            dynamic shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = @"C:\Program Files\YourBrowser\browser.exe"; // Example browser path
            shortcut.WorkingDirectory = Path.GetDirectoryName(@"C:\Program Files\YourBrowser\browser.exe");
            shortcut.Arguments = $"--app=https://cod.cheater-report.online/";
            shortcut.IconLocation = ICON_ICO_PATH;
            shortcut.Save();

            Log("[✓] Shortcut Created on Desktop.");
        }
    }

    private void RemoveShortcut()
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string shortcutPath = Path.Combine(desktop, $"{APP_NAME}.lnk");

        try
        {
            if (File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
                Log("[✓] Shortcut Removed from Desktop.");
            }
            else
            {
                Log("[✗] Shortcut not found.");
            }
        }
        catch (Exception ex)
        {
            Log("[✗] Failed to Remove Shortcut: " + ex.Message);
        }
    }

    private void Uninstall()
    {
        try
        {
            RemoveShortcut();
            Directory.Delete(TEMP_DIR, true);
            Log("[✓] Uninstallation Complete.");
        }
        catch (Exception ex)
        {
            Log("[✗] Uninstallation Failed: " + ex.Message);
        }
    }

    private void btnInstall_Click(object sender, EventArgs e)
    {
        Log("Installation Started...");
        CreateShortcut();
        Log("[✓] Installation Complete!");
    }

    private void btnUninstall_Click(object sender, EventArgs e)
    {
        Log("Uninstallation Started...");
        Uninstall();
    }
}
