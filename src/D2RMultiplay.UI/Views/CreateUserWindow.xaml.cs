using System.Windows;

namespace D2RMultiplay.UI.Views
{
    public partial class CreateUserWindow : Window
    {
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public string BattleTag { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public CreateUserWindow(bool isChinese)
        {
            InitializeComponent();
            
            if (isChinese)
            {
                Title = "新建 Windows 用户";
                TitleText.Text = "新建 Windows 用户";
                LabelUser.Text = "用户名:";
                LabelPass.Text = "密码:";
                LabelBattleTag.Text = "战网账号 (别名):";
                LabelNote.Text = "备注 (Note):"; // Align with MainViewModel
                BtnCreate.Content = "创建";
                BtnCancel.Content = "取消";
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            Username = TxtUsername.Text;
            Password = TxtPassword.Password;
            BattleTag = TxtBattleTag.Text;
            Description = TxtNote.Text;

            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show(Title == "新建 Windows 用户" ? "请输入用户名。" : "Please enter a username.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show(Title == "新建 Windows 用户" ? "请输入密码。" : "Please enter a password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
