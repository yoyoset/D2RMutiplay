using System.Collections.Generic;
using System.Windows;

namespace D2RMultiplay.UI.Views
{
    public partial class UserSelectionWindow : Window
    {
        public string SelectedUsername { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public UserSelectionWindow(List<string> users, bool isChinese = false)
        {
            InitializeComponent();
            
            // Mark Current User
            string currentUser = System.Environment.UserName;
            var displayList = new List<string>();
            foreach(var u in users)
            {
                if(u.Equals(currentUser, System.StringComparison.OrdinalIgnoreCase))
                {
                    displayList.Add(u + (isChinese ? " (当前用户)" : " (Current)"));
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

            if (isChinese)
            {
                Title = "关联现有用户";
                TitleText.Text = "选择 Windows 用户";
                LabelUser.Text = "选择 Windows 用户:";
                LabelPass.Text = "密码 (用于验证):";
                BtnLink.Content = "关联用户";
                BtnCancel.Content = "取消";
            }
        }

        private void BtnLink_Click(object sender, RoutedEventArgs e)
        {
            string rawSelection = CmbUsers.Text; // Use Text for editable support
            
            // Strip Suffix "(...)" if present
            int parenIndex = rawSelection.IndexOf(" (");
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
                MessageBox.Show(Title == "关联现有用户" ? "请输入或选择一个用户。" : "Please select or enter a user.", "Error");
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
