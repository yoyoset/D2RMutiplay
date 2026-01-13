using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace D2RMultiplay.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private System.Windows.Forms.NotifyIcon? _notifyIcon;

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new D2RMultiplay.UI.ViewModels.MainViewModel();

        InitializeTrayIcon();
    }

    private void InitializeTrayIcon()
    {
        _notifyIcon = new System.Windows.Forms.NotifyIcon
        {
            Icon = new System.Drawing.Icon("app.ico"),
            Visible = false,
            Text = "D2R Multiplay"
        };

        _notifyIcon.DoubleClick += (s, args) => ShowWindow();
        
        // Add context menu for tray icon
        var contextMenu = new System.Windows.Forms.ContextMenuStrip();
        contextMenu.Items.Add("Show", null, (s, args) => ShowWindow());
        contextMenu.Items.Add("Exit", null, (s, args) => Close());
        _notifyIcon.ContextMenuStrip = contextMenu;
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide();
            if (_notifyIcon != null) _notifyIcon.Visible = true;
        }
        base.OnStateChanged(e);
    }

    private void ShowWindow()
    {
        Show();
        WindowState = WindowState.Normal;
        if (_notifyIcon != null) _notifyIcon.Visible = false;
        Activate();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_notifyIcon != null)
        {
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
        base.OnClosed(e);
    }
}