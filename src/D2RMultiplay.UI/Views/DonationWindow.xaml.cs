using System;
using System.Diagnostics;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Windows.Navigation;
using D2RMultiplay.UI.Services;
using System.IO;

namespace D2RMultiplay.UI.Views
{
    public partial class DonationWindow : Window
    {
        public string LinkPayPalUrl { get; } = "https://paypal.me/squareuncle";

        public DonationWindow()
        {
            InitializeComponent();
            DataContext = this;
            ApplyLocalization();
            // Verify images exist to avoid crash (optional debug check)
            // string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images");
        }

        private void ApplyLocalization()
        {
            // Title is not bound in XAML for Window, set here
            Title = LocalizationManager.GetText("DonationTitle");
            
            TxtTitle.Text = LocalizationManager.GetText("DonationTitle");
            TxtDesc.Text = LocalizationManager.GetText("DonationDesc");
            
            LblAlipay.Text = LocalizationManager.GetText("LabelAlipay");
            LblWeChat.Text = LocalizationManager.GetText("LabelWeChat");
            LblPayPal.Text = LocalizationManager.GetText("LabelPayPal");
            
            LinkPayPal.Text = LocalizationManager.GetText("LinkPayPal");
            LinkDesc.Text = LocalizationManager.GetText("LinkDesc");
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open link: {ex.Message}");
            }
        }
    }
}
