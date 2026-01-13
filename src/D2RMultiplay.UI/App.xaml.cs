using System.Configuration;
using System.Data;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;

namespace D2RMultiplay.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        string errorMsg = $"Crash at {DateTime.Now}: {e.Exception.Message}\nStack Trace: {e.Exception.StackTrace}";
        if (e.Exception.InnerException != null)
        {
            errorMsg += $"\nInner: {e.Exception.InnerException.Message}";
        }
        System.IO.File.WriteAllText("crash_log.txt", errorMsg);
        MessageBox.Show($"Application crashed: {e.Exception.Message}", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true; // Prevent immediate termination to allow message box
        Shutdown();
    }
}

