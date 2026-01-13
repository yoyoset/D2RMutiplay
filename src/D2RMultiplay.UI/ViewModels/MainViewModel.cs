using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32; // For OpenFileDialog
using D2RMultiplay.Core.Models;
using D2RMultiplay.Core.Interfaces;
using System.Security.Principal;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush; // Resolve ambiguity with System.Drawing
using Brushes = System.Windows.Media.Brushes; // Resolve ambiguity
using OpenFileDialog = Microsoft.Win32.OpenFileDialog; // Resolve ambiguity
using Path = System.IO.Path;
using D2RMultiplay.Modules.ModuleA_AccountManager;
using D2RMultiplay.Modules.ModuleC_IsolationEngine;
using D2RMultiplay.UI.Utilities;
using D2RMultiplay.UI.Views;
using D2RMultiplay.UI.Services;
using System.Windows.Media;


namespace D2RMultiplay.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWindowsUserManager _userManager;
        private readonly IIsolationEngine _isolationEngine;
        private const string ACCOUNTS_FILE = "accounts.json";

        public ObservableCollection<Account> Accounts { get; set; }

        private Account? _selectedAccount;
        public Account? SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                OnPropertyChanged();
                
                // Auto-load inputs on selection
                if (_selectedAccount != null)
                {
                    InputUsername = _selectedAccount.Username;
                    InputPassword = _selectedAccount.Password;
                    InputBattleTag = _selectedAccount.BattleTag;
                    InputNote = _selectedAccount.Note;
                }

                CheckSelectedUserStatus();
            }
        }

        private string _statusMessage = "Ready";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // Inputs
        private string _inputUsername = "";
        public string InputUsername { get => _inputUsername; set { _inputUsername = value; OnPropertyChanged(); } }
        
        private string _inputPassword = "";
        public string InputPassword { get => _inputPassword; set { _inputPassword = value; OnPropertyChanged(); } }

        private string _inputBattleTag = "";
        public string InputBattleTag { get => _inputBattleTag; set { _inputBattleTag = value; OnPropertyChanged(); } }

         private string _inputNote = "";
        public string InputNote { get => _inputNote; set { _inputNote = value; OnPropertyChanged(); } }


        // Localization
        private string _selectedLanguage = "English"; // Default: English
        public ObservableCollection<string> AvailableLanguages => new ObservableCollection<string>(LocalizationManager.SupportedLanguages);

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    LocalizationManager.CurrentLanguage = _selectedLanguage;
                    OnPropertyChanged();
                    UpdateAllLocalization();
                    // Auto status update on switch
                    StatusMessage = LocalizationManager.GetText("StatusReady", _selectedLanguage);
                    SaveSettings(); // Auto-save language choice
                }
            }
        }

        // Computed Properties calling LocalizationManager
        public string WindowTitle => LocalizationManager.GetText("WindowTitle", _selectedLanguage);
        public string LangButtonText => LocalizationManager.GetText("LangButton", _selectedLanguage);
        
        public string GroupUserMgmt => LocalizationManager.GetText("GroupUserMgmt", _selectedLanguage);
        public string GroupLaunchOps => LocalizationManager.GetText("GroupLaunchOps", _selectedLanguage);
        
        public string LabelInputUser => LocalizationManager.GetText("LabelInputUser", _selectedLanguage);
        public string LabelInputPass => LocalizationManager.GetText("LabelInputPass", _selectedLanguage);
        public string HintNoPassword => LocalizationManager.GetText("HintNoPassword", _selectedLanguage);
        public string LabelInputBattleTag => LocalizationManager.GetText("LabelInputBattleTag", _selectedLanguage);
        public string LabelInputNote => LocalizationManager.GetText("LabelInputNote", _selectedLanguage);
        
        public string BtnCreateNew => LocalizationManager.GetText("BtnCreateNew", _selectedLanguage);
        public string BtnLinkExisting => LocalizationManager.GetText("BtnLinkExisting", _selectedLanguage);
        public string BtnUpdate => LocalizationManager.GetText("BtnUpdate", _selectedLanguage);

        public string BtnPickPath => LocalizationManager.GetText("BtnPickPath", _selectedLanguage);
        public string BtnCreateMirror => LocalizationManager.GetText("BtnCreateMirror", _selectedLanguage);
        public string BtnDeleteSysUser => LocalizationManager.GetText("BtnDeleteSysUser", _selectedLanguage);
        
        public string LabelCurrentAccount => LocalizationManager.GetText("LabelCurrentAccount", _selectedLanguage);
        public string LabelGamePath => LocalizationManager.GetText("LabelGamePath", _selectedLanguage);
        public string LabelPathHint => LocalizationManager.GetText("LabelPathHint", _selectedLanguage);
        public string LabelLanguage => LocalizationManager.GetText("LabelLanguage", _selectedLanguage);

        public string LabelAdminStatus => IsAdmin 
            ? LocalizationManager.GetText("LabelAdminStatus_Yes", _selectedLanguage)
            : LocalizationManager.GetText("LabelAdminStatus_No", _selectedLanguage);
        
        public Brush ColorAdminStatus => IsAdmin ? Brushes.Green : Brushes.Red;
        public bool IsAdmin => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public string LabelCopyright => "By SquareUncle & Antigravity";
        public string AppVersion => $"v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3)}";

        public string BtnLaunchAuto => LocalizationManager.GetText("BtnLaunchAuto", _selectedLanguage);
        public string BtnLaunchDirect => LocalizationManager.GetText("BtnLaunchDirect", _selectedLanguage);
        public string LaunchHint => LocalizationManager.GetText("LaunchHint", _selectedLanguage);
        public string LabelManualLoginWarning => LocalizationManager.GetText("LaunchHint", _selectedLanguage);
        
        public string BtnDelete => LocalizationManager.GetText("BtnDelete", _selectedLanguage);
        public string BtnSave => LocalizationManager.GetText("BtnSave", _selectedLanguage);
        
        public string GroupManual => LocalizationManager.GetText("GroupManual", _selectedLanguage);
        public string BtnKillBnet => LocalizationManager.GetText("BtnKillBnet", _selectedLanguage);
        public string BtnCleanConfig => LocalizationManager.GetText("BtnCleanConfig", _selectedLanguage);
        public string BtnKillMutex => LocalizationManager.GetText("BtnKillMutex", _selectedLanguage); 
        public string BtnSnapshotConfig => LocalizationManager.GetText("BtnSnapshotConfig", _selectedLanguage);
        public string BtnSupport => LocalizationManager.GetText("BtnSupport", _selectedLanguage);

        private string _currentTheme = "Dark";
        public string CurrentTheme
        {
            get => _currentTheme;
            set { _currentTheme = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand CreateNewUserCommand { get; }
        public ICommand LinkExistingUserCommand { get; }
        public ICommand UpdateUserCommand { get; }
        public ICommand LoadForEditCommand { get; }
        public ICommand PickPathCommand { get; }
        public ICommand CreateMirrorCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand DeleteSystemUserCommand { get; }
        // public ICommand ToggleLangCommand { get; } // Removed in favor of ComboBox binding
        
        public ICommand OneClickLaunchCommand { get; }
        public ICommand DirectLaunchCommand { get; }
        
        public ICommand KillBnetCommand { get; }
        public ICommand CleanConfigCommand { get; }
        public ICommand KillMutexCommand { get; }
        public ICommand SnapshotConfigCommand { get; }
        public ICommand OpenDonationCommand { get; }
        public ICommand ToggleThemeCommand { get; }

        public MainViewModel()
        {
            _userManager = new WindowsUserManager();
            _isolationEngine = new IsolationEngine();
            Accounts = new ObservableCollection<Account>();

            LoadAccounts();
            LoadSettings(); 

            CreateNewUserCommand = new RelayCommand(CreateNewUser);
            LinkExistingUserCommand = new RelayCommand(LinkExistingUser);
            UpdateUserCommand = new RelayCommand(UpdateUser, _ => SelectedAccount != null);
            LoadForEditCommand = new RelayCommand(LoadForEdit, _ => SelectedAccount != null);

            PickPathCommand = new RelayCommand(PickGamePath, _ => SelectedAccount != null);
            CreateMirrorCommand = new RelayCommand(CreateMirrorPath, _ => SelectedAccount != null);
            SaveCommand = new RelayCommand(SaveAccounts);
            DeleteCommand = new RelayCommand(DeleteAccount, _ => SelectedAccount != null);
            DeleteSystemUserCommand = new RelayCommand(DeleteSystemUser, _ => !string.IsNullOrEmpty(InputUsername));
            
            OneClickLaunchCommand = new RelayCommand(OneClickLaunch, CanLaunchGame);
            DirectLaunchCommand = new RelayCommand(DirectLaunch, CanLaunchGame);
            
            KillBnetCommand = new RelayCommand(KillBnet);
            CleanConfigCommand = new RelayCommand(CleanConfig);
            KillMutexCommand = new RelayCommand(KillMutex);
            SnapshotConfigCommand = new RelayCommand(SnapshotConfig, _ => SelectedAccount != null);
            OpenDonationCommand = new RelayCommand(OpenDonation);
            ToggleThemeCommand = new RelayCommand(_ => ToggleTheme());

            // Set Initial Status
            StatusMessage = LocalizationManager.GetText("StatusReady", _selectedLanguage);
        }

        private bool CanCreateOrLink(object? parameter) => !string.IsNullOrWhiteSpace(InputUsername);

        private void LoadForEdit(object? parameter)
        {
            if (SelectedAccount == null) return;
            InputUsername = SelectedAccount.Username;
            InputPassword = SelectedAccount.Password;
            InputBattleTag = SelectedAccount.BattleTag;
            InputNote = SelectedAccount.Note;
            
            StatusMessage = _selectedLanguage == "简体中文" ? "已加载信息到上方输入框。" : "Loaded info to inputs.";
        }

        private void UpdateUser(object? parameter)
        {
            if (SelectedAccount == null) return;
            
            SelectedAccount.Username = InputUsername;
            SelectedAccount.Password = InputPassword;
            SelectedAccount.BattleTag = InputBattleTag;
            SelectedAccount.Note = InputNote;
            
            int index = Accounts.IndexOf(SelectedAccount);
            if (index != -1) {
                var temp = SelectedAccount;
                Accounts.RemoveAt(index);
                Accounts.Insert(index, temp);
                SelectedAccount = temp;
            }
            
            SaveAccounts();
            StatusMessage = "User info updated.";
        }

        private void CheckSelectedUserStatus()
        {
            if (SelectedAccount == null) return;
            
            bool exists = _userManager.UserExists(SelectedAccount.Username);
            if (!exists)
            {
               StatusMessage = $"Warning: User {SelectedAccount.Username} not found (Ghost User).";
            }
            else
            {
               StatusMessage = $"Ready: User {SelectedAccount.Username} verified.";
            }
        }

        private void UpdateAllLocalization()
        {
            // Refresh all properties
            OnPropertyChanged(string.Empty);
        }
        
        private void CreateNewUser(object? parameter)
        {
            // Note: CreateUserWindow needs update to support Lang code if we want it localized too.
            // For now, passing bool IsChinese logic might be broken.
            // Assuming CreateUserWindow refactor later.
            var dialog = new CreateUserWindow(); 
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                string user = dialog.Username;
                string pass = dialog.Password;
                string btag = dialog.BattleTag;
                string desc = dialog.Description;
                string note = desc;

                try
                {
                    _userManager.EnsureUserExists(user, pass);
                    var newAccount = new Account
                    {
                        Username = user,
                        Password = pass,
                        BattleTag = btag,
                        Note = note,
                        GamePath = "" 
                    };

                    Accounts.Add(newAccount);
                    SaveAccounts(null);

                    string msg = string.Format(LocalizationManager.GetText("MsgUserCreatedBody"), user);
                    MessageBox.Show(msg, LocalizationManager.GetText("TitleFirstRun"));
                    StatusMessage = $"User {user} created.";
                    SelectedAccount = newAccount;
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error creating user: {ex.Message}";
                    MessageBox.Show(StatusMessage, "Error");
                }
            }
        }

        private void LinkExistingUser(object? parameter)
        {
            try
            {
                var users = _userManager.GetLocalUsers();
                
                // Note: UserSelectionWindow needs update too
                var dialog = new UserSelectionWindow(users);
                dialog.Owner = System.Windows.Application.Current.MainWindow;

                if (dialog.ShowDialog() == true)
                {
                    string selectedUser = dialog.SelectedUsername;
                    string password = dialog.Password;
                    
                    if (Accounts.Any(a => a.Username == selectedUser))
                    {
                        StatusMessage = "User already in list.";
                        return;
                    }

                    var newAccount = new Account
                    {
                        Username = selectedUser,
                        Password = password,
                        Note = "Linked User",
                        GamePath = ""
                    };

                    Accounts.Add(newAccount);
                    SaveAccounts(null);
                    StatusMessage = $"User {selectedUser} linked.";
                    SelectedAccount = newAccount;
                }
            }
            catch (Exception ex) { PositionError(ex); }
        }

        private void PickGamePath(object? parameter)
        {
            if (SelectedAccount == null) return;
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "D2R.exe|D2R.exe|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string newPath = openFileDialog.FileName;
                SelectedAccount.GamePath = newPath;
                OnPropertyChanged(nameof(SelectedAccount));
                SaveAccounts();
            }
        }

        private void OneClickLaunch(object? parameter)
        {
            if (SelectedAccount == null) return;
            SaveAccounts();

            try
            {
                StatusMessage = "Launching...";
                _isolationEngine.KillBattleNetProcesses();

                // Snapshot Strategy
                if (!string.IsNullOrEmpty(_settings.LastLaunchedUsername))
                {
                     try { _isolationEngine.BackupBattleNetConfig(_settings.LastLaunchedUsername); } catch { }
                }

                bool restored = _isolationEngine.RestoreBattleNetConfig(SelectedAccount.Username);
                if (!restored)
                {
                     _isolationEngine.CleanBattleNetConfig();
                     StatusMessage = "Config Cleaned (No Snapshot).";
                }
                
                _settings.LastLaunchedUsername = SelectedAccount.Username;
                SaveSettings();

                _isolationEngine.KillGameMutexes();
                LaunchBattleNet(SelectedAccount);
            }
            catch (Exception ex) { StatusMessage = $"Launch Error: {ex.Message}"; }
        }

        private void DirectLaunch(object? parameter)
        {
             if (SelectedAccount == null) return;
             try 
             {
                 StatusMessage = "Direct Launching...";
                 LaunchBattleNet(SelectedAccount);
             }
             catch(Exception ex) { StatusMessage = $"Launch Error: {ex.Message}"; }
        }

        private void LaunchBattleNet(Account acc)
        {
            VerifyUser(acc.Username);
            bool isCurrentUser = acc.Username.Equals(Environment.UserName, StringComparison.OrdinalIgnoreCase);

            if (!isCurrentUser && string.IsNullOrEmpty(acc.Password))
            {
               throw new Exception("Windows User MUST have a password for isolation.");
            }

            string bnetPath = @"C:\Program Files (x86)\Battle.net\Battle.net.exe";
            if (!File.Exists(bnetPath)) throw new FileNotFoundException("Battle.net.exe not found!");

            int pid = 0;
            if (isCurrentUser)
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = bnetPath,
                    Arguments = "--exec=\"launch D2R\"",
                    UseShellExecute = false
                };
                var p = System.Diagnostics.Process.Start(psi);
                if (p != null) pid = p.Id;
            }
            else
            {
                pid = _userManager.LaunchProgramAsUser(acc.Username, acc.Password, bnetPath, "--exec=\"launch D2R\""); 
            }
            
            StatusMessage = $"Launched Battle.net (PID: {pid}).";
        }

        private void VerifyUser(string username)
        {
             if (!_userManager.UserExists(username)) throw new Exception($"User {username} missing!");
        }
        
        private void LoadAccounts()
        {
            try { if (File.Exists(ACCOUNTS_FILE)) { 
                var list = JsonSerializer.Deserialize<List<Account>>(File.ReadAllText(ACCOUNTS_FILE));
                if(list!=null) foreach(var a in list) Accounts.Add(a);
            }} catch {}
        }
        private void SaveAccounts(object? parameter = null)
        {
             try { 
                File.WriteAllText(ACCOUNTS_FILE, JsonSerializer.Serialize(Accounts, new JsonSerializerOptions{WriteIndented=true}));
                CheckSelectedUserStatus(); 
             } catch {}
        }
        private void DeleteAccount(object? parameter)
        {
            if(SelectedAccount != null) { 
                Accounts.Remove(SelectedAccount); 
                SelectedAccount = null; 
                SaveAccounts(); 
                StatusMessage = "Account removed from list.";
            }
        }
        private bool CanLaunchGame(object? param) => SelectedAccount != null && !string.IsNullOrWhiteSpace(SelectedAccount.Username);

        private void KillBnet(object? p) 
        { 
            try
            {
                _isolationEngine.KillBattleNetProcesses();
                StatusMessage = "Killed Bnet";
                MessageBox.Show("Killed Battle.net & Agent processes.", "Success");
            }
            catch(Exception e){ PositionError(e); }
        }

        private void CleanConfig(object? p) 
        { 
            try
            {
                _isolationEngine.CleanBattleNetConfig();
                StatusMessage = "Cleaned Config";
                MessageBox.Show("Deleted product.db config.", "Success");
            }
            catch(Exception e){ PositionError(e); }
        }

        private void CreateMirrorPath(object? parameter)
        {
            if (SelectedAccount == null) return;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Source D2R.exe";
            dialog.Filter = "D2R.exe|D2R.exe";
            
            if (dialog.ShowDialog() != true) return;

            string sourcePath = dialog.FileName;
            string? sourceDir = Path.GetDirectoryName(sourcePath);
            if (string.IsNullOrEmpty(sourceDir)) return;
            string exeName = Path.GetFileName(sourcePath);
            string safeUsername = SelectedAccount.Username;
            foreach (char c in Path.GetInvalidFileNameChars()) safeUsername = safeUsername.Replace(c, '_');
            
            string mirrorDirName = $"D2R_Clone_{safeUsername}";
            string targetDir = Path.Combine(Path.GetDirectoryName(sourceDir), mirrorDirName);

            try 
            {
                _isolationEngine.CreateGameJunction(sourceDir, targetDir);
                string newExePath = Path.Combine(targetDir, exeName);
                SelectedAccount.GamePath = newExePath; 
                SaveAccounts(null); 
                OnPropertyChanged(nameof(SelectedAccount));
                MessageBox.Show($"Mirror Created!\nPath: {newExePath}", "Success");
            }
            catch (Exception ex)
            {
                 StatusMessage = $"Error: {ex.Message}";
                 MessageBox.Show(StatusMessage, "Error");
            }
        }
        private void KillMutex(object? p) 
        { 
            try
            {
                int count = _isolationEngine.KillGameMutexes();
                MessageBox.Show($"Closed {count} handles.", "Success");
                StatusMessage = $"Killed Mutex ({count})";
            }
            catch(Exception e){ PositionError(e); }
        }

        private void SnapshotConfig(object? p)
        {
            if (SelectedAccount == null) return;
            try
            {
                _isolationEngine.BackupBattleNetConfig(SelectedAccount.Username);
                MessageBox.Show($"Config Snapshot Saved for {SelectedAccount.Username}", "Success");
                StatusMessage = $"Config Snapshot saved for {SelectedAccount.Username}";
            }
            catch(Exception e){ PositionError(e); }
        }

        private void DeleteSystemUser(object? p)
        {
            try
            {
                var userToDelete = InputUsername;
                if (string.IsNullOrWhiteSpace(userToDelete)) return;

                // SAFETY CHECK 1: Prevent deleting current user
                if (userToDelete.Equals(Environment.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        $"Operation BLOCKED: Cannot delete the currently logged-in user '{userToDelete}'.",
                        "Safety Block",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                bool isAdmin = false;
                try {
                     // Check if user is in Admin group using PrincipalContext or check against well-known SIDs
                     // Simplified check: if it's "Administrator" or similar. 
                     // For robust check, we'd need DirectoryEntry but sticking to basic logic for now or just treat all deletions as high risk.
                     // Let's rely on the Double Confirmation for ALL deletions that are not the current user, or specifically flag if possible.
                     // Given the user request: "Delete Admin user needs strong confirmation". 
                     // We will implement Double Confirmation for *ALL* system user deletions to be safe, as checking "IsAdmin" for another user locally is complex without AD references.
                     isAdmin = true; // Treat all deletions as high risk requiring double confirmation as per request "Delete Admin needs verify".
                } catch { }

                // CONFIRMATION 1
                if (MessageBox.Show(
                    $"WARNING! Irreversible Action:\nPermanently deleting Windows user '{userToDelete}' and their data.\n\nAre you sure?",
                    "Delete User Confirmation (1/2)",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) != MessageBoxResult.Yes) 
                {
                    return;
                }

                // SAFETY CHECK 2: Secondary Confirmation for critical action
                if (MessageBox.Show(
                    $"FINAL WARNING:\n\nYou are about to DELETE user '{userToDelete}'.\nThis cannot be undone.\n\nType 'Yes' to confirm? (Just kidding, click Yes).",
                    "Final Confirmation (2/2)",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Stop,
                    MessageBoxResult.No) != MessageBoxResult.Yes)
                {
                    return;
                }

                _userManager.DeleteUser(userToDelete);
                var acc = Accounts.FirstOrDefault(a => a.Username.Equals(userToDelete, StringComparison.OrdinalIgnoreCase));
                if (acc != null)
                {
                    Accounts.Remove(acc);
                    if (SelectedAccount == acc) SelectedAccount = null;
                    SaveAccounts();
                }
                StatusMessage = $"User {userToDelete} deleted.";
                MessageBox.Show(StatusMessage, "Success");
            }
            catch(Exception ex) { PositionError(ex); }
        }

        public class AppSettings
        {
            public string LastLaunchedUsername { get; set; } = "";
            public string Theme { get; set; } = "Dark"; // Default to Dark
            public string Language { get; set; } = "English";
        }
        
        private AppSettings _settings = new AppSettings();
        private const string SETTINGS_FILE = "settings.json";

        private void LoadSettings()
        {
            try {
                if (File.Exists(SETTINGS_FILE)) 
                    _settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SETTINGS_FILE)) ?? new AppSettings();
            } catch {}
            
            // Apply Saved Theme
            ThemeManager.ApplyTheme(_settings.Theme);
            CurrentTheme = _settings.Theme;

            // Apply Saved Language
            // Note: Since _selectedLanguage initializes to "English", we need to force an update if saved is different
            if (_selectedLanguage != _settings.Language)
            {
                SelectedLanguage = _settings.Language;
            }
        }

        private void SaveSettings()
        {
            try {
                _settings.Theme = CurrentTheme; 
                _settings.Language = SelectedLanguage;
                File.WriteAllText(SETTINGS_FILE, JsonSerializer.Serialize(_settings));
            } catch {}
        }
        private void OpenDonation(object? parameter)
        {
            var win = new DonationWindow();
            win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

        private void ToggleTheme()
        {
            var newTheme = CurrentTheme == "Dark" ? "Light" : "Dark";
            ThemeManager.ApplyTheme(newTheme);
            CurrentTheme = newTheme;
            SaveSettings();
        }

        private void PositionError(Exception e)
        {
            StatusMessage = $"Error: {e.Message}";
            MessageBox.Show(e.Message, "Error");
        }
    }
}
