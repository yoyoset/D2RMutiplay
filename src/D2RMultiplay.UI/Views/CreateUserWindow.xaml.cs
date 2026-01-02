using System.Windows;
using D2RMultiplay.UI.Services;

namespace D2RMultiplay.UI.Views
{
    public partial class CreateUserWindow : Window
    {
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public string BattleTag { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        public CreateUserWindow()
        {
            InitializeComponent();
            ApplyLocalization();
        }

        private void ApplyLocalization()
        {
            Title = LocalizationManager.GetText("CreateUserTitle");
            TitleText.Text = LocalizationManager.GetText("CreateUserTitle");
            LabelUser.Text = LocalizationManager.GetText("LabelUser");
            LabelPass.Text = LocalizationManager.GetText("LabelPassword");
            LabelBattleTag.Text = LocalizationManager.GetText("LabelBattleTag");
            LabelNote.Text = LocalizationManager.GetText("LabelNote");
            BtnCreate.Content = LocalizationManager.GetText("BtnCreate");
            BtnCancel.Content = LocalizationManager.GetText("BtnCancel");
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            Username = TxtUsername.Text;
            Password = TxtPassword.Password;
            BattleTag = TxtBattleTag.Text;
            Description = TxtNote.Text;

            if (string.IsNullOrWhiteSpace(Username))
            {
                MessageBox.Show(LocalizationManager.GetText("MsgEnterUsername"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show(LocalizationManager.GetText("MsgEnterPassword"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
