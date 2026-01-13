using System.Collections.Generic;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using D2RMultiplay.UI.Services;

namespace D2RMultiplay.UI.Views
{
    public partial class UserSelectionWindow : Window
    {
        public string SelectedUsername { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public UserSelectionWindow(List<string> users)
        {
            InitializeComponent();
            ApplyLocalization();

            // Mark Current User
            string currentUser = System.Environment.UserName;
            var displayList = new List<string>();
            string suffix = LocalizationManager.GetText("SuffixCurrent");

            foreach(var u in users)
            {
                if(u.Equals(currentUser, System.StringComparison.OrdinalIgnoreCase))
                {
                    displayList.Add(u + suffix);
                }
                else
                {
                    displayList.Add(u);
                }
            }

            CmbUsers.ItemsSource = displayList;
            
            // Auto-select current if present
            int currentIdx = displayList.FindIndex(u => u.StartsWith(currentUser, System.StringComparison.OrdinalIgnoreCase));
            if (currentIdx >= 0) CmbUsers.SelectedIndex = currentIdx;
            else if (users.Count > 0) CmbUsers.SelectedIndex = 0;

            TxtPassword.Focus();
        }

        private void ApplyLocalization()
        {
            Title = LocalizationManager.GetText("LinkUserTitle");
            TitleText.Text = LocalizationManager.GetText("SelectUserTitle");
            LabelUser.Text = LocalizationManager.GetText("LabelSelectUser");
            LabelPass.Text = LocalizationManager.GetText("LabelVerifyPass");
            BtnLink.Content = LocalizationManager.GetText("BtnLink");
            BtnCancel.Content = LocalizationManager.GetText("BtnCancel");
        }

        private void BtnLink_Click(object sender, RoutedEventArgs e)
        {
            string rawSelection = CmbUsers.Text; // Use Text for editable support
            
            // Strip Suffix "(...)" if present
            // We need to be careful as suffix changes with language.
            // Simplified logic: Just take everything before the first " (" if it exists.
            int parenIndex = rawSelection.LastIndexOf(" (");
            if (parenIndex > 0)
            {
                SelectedUsername = rawSelection.Substring(0, parenIndex);
            }
            else
            {
                SelectedUsername = rawSelection;
            }

            Password = TxtPassword.Password;

            if (string.IsNullOrEmpty(SelectedUsername))
            {
                MessageBox.Show(LocalizationManager.GetText("MsgSelectUser"), "Error");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
