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
using D2RMultiplay.Modules.ModuleA_AccountManager;
using D2RMultiplay.Modules.ModuleC_IsolationEngine;
using D2RMultiplay.UI.Utilities;
using D2RMultiplay.UI.Views;
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
        public bool IsChinese { get; private set; } = true;
        public string LangButtonText => IsChinese ? "English" : "中文";
        public string WindowTitle => IsChinese ? "D2R 多开工具 (v0.4.1 Secure)" : "D2R Multi-Open Tool (v0.4.1 Secure)";
        
        // Group Headers
        public string GroupUserMgmt => IsChinese ? "1. Windows 用户与映射管理" : "1. User & Mapping Management";
        public string GroupLaunchOps => IsChinese ? "2. 启动操作区" : "2. Launch Operations";
        
        // User Mgmt UI
        public string LabelInputUser => IsChinese ? "Windows 用户名:" : "Windows Username:";
        public string LabelInputPass => IsChinese ? "密码 (用于自动登录):" : "Password (for auto-login):";
        public string LabelInputBattleTag => IsChinese ? "战网账号 (别名):" : "BattleTag (Alias):";
        public string LabelInputNote => IsChinese ? "备注 (Note):" : "Note:";
        public string BtnCreateNew => IsChinese ? "新建 Windows 用户" : "Create Windows User";
        public string BtnLinkExisting => IsChinese ? "关联现有用户 (Link)" : "Link Existing User";
        public string BtnUpdate => IsChinese ? "保存修改 (Save)" : "Save Changes";

        public string BtnPickPath => IsChinese ? "浏览..." : "Browse...";
        public string BtnCreateMirror => IsChinese ? "创建镜像 (Mirror)" : "Create Mirror";
        public string BtnDeleteSysUser => IsChinese ? "删除系统用户 (Delete User)" : "Delete System User";
        
        // Launch UI
        public string LabelCurrentAccount => IsChinese ? "当前选中账号:" : "Current Account:";
        public string LabelGamePath => IsChinese ? "游戏路径 (Game Path):" : "Game Path:";
        public string LabelPathHint => IsChinese 
            ? "说明: 本工具仅记录路径，不会自动生成。请手动为每个账号指定不同的游戏文件夹(或使用镜像功能生成)。" 
            : "Note: Tool records path only. Manually select a unique folder per account (or use Mirror).";
        public ObservableCollection<string> AvailableLanguages { get; } = new ObservableCollection<string> { "English", "简体中文" };

        private string _selectedLanguage = "简体中文"; // Default matches IsChinese = true
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    IsChinese = (_selectedLanguage == "简体中文");
                    OnPropertyChanged();
                    UpdateAllLocalization();
                }
            }
        }

        public string LabelLanguage => IsChinese ? "语言选择:" : "Language:";

        public string LabelManualLoginWarning => IsChinese 
            ? "⚠️ 新建用户必读:\n1. [必须] 手动切换到该用户登录一次 Windows (初始化环境)。\n2. [建议] 在该用户下登录一次战网客户端 (确保无异常)。\n3. [异常] 若一键启动卡在登录页 (请关闭战网并重试)。" 
            : "⚠️ New User Setup:\n1. [REQUIRED] Log into Windows manually (Initialize Environment).\n2. [SUGGESTED] Log into Battle.net Client once (Ensure no anomalies).\n3. [TROUBLESHOOT] If login freezes (Close Battle.net and retry).";

        // Admin Status Checks
        public bool IsAdmin => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        public string LabelAdminStatus => IsChinese 
            ? (IsAdmin ? "🛡️ 已获管理员权限 (Admin)" : "⚠️ 未获管理员权限 (限制模式)")
            : (IsAdmin ? "🛡️ Administrator Access" : "⚠️ Restricted Mode (No Admin)");
        
        public Brush ColorAdminStatus => IsAdmin ? Brushes.Green : Brushes.Red;

        public string LabelCopyright => IsChinese ? "By 方砖叔 with Antigravity" : "By SquareUncle & Antigravity";

        public string BtnLaunchAuto => IsChinese ? "一键启动 (清理+启动)" : "One-Click Launch (Clean+Start)";
        public string BtnLaunchDirect => IsChinese ? "直接启动 (仅启动)" : "Direct Launch (Just Start)";
        public string LaunchHint => IsChinese 
            ? "* 若战网登录跳转时卡死，请先通过 Windows 登录该用户一次，设置默认浏览器。" 
            : "* If browser launch freezes, log in to this Windows User once to set default browser.";
        public string BtnDelete => IsChinese ? "从列表移除" : "Remove from List";
        public string BtnSave => IsChinese ? "保存路径" : "Save Path";
        
        // Manual Tools Strings
        public string GroupManual => IsChinese ? "手动工具 (调试用)" : "Manual Tools (Debug)";
        public string BtnKillBnet => IsChinese ? "清理战网 (Kill Bnet)" : "Kill Battle.net";
        public string BtnCleanConfig => IsChinese ? "删档案 (Del Config)" : "Del Config";
        public string BtnKillMutex => IsChinese ? "杀句柄 (Kill Mutex)" : "Kill Mutex"; 
        public string BtnSnapshotConfig => IsChinese ? "抓取配置 (Snapshot)" : "Snapshot Config";
        // Junction moved to main UI

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
        public ICommand ToggleLangCommand { get; }
        
        // Launch Commands
        public ICommand OneClickLaunchCommand { get; }
        public ICommand DirectLaunchCommand { get; }
        
        // Manual Commands
        public ICommand KillBnetCommand { get; }
        public ICommand CleanConfigCommand { get; }
        public ICommand KillMutexCommand { get; }
        public ICommand SnapshotConfigCommand { get; }


        public MainViewModel()
        {
            _userManager = new WindowsUserManager();
            _isolationEngine = new IsolationEngine();
            Accounts = new ObservableCollection<Account>();

            LoadAccounts();
            LoadSettings(); // Load Last User info

            // Part 1: User Mgmt
            CreateNewUserCommand = new RelayCommand(CreateNewUser);
            LinkExistingUserCommand = new RelayCommand(LinkExistingUser);
            UpdateUserCommand = new RelayCommand(UpdateUser, _ => SelectedAccount != null);
            LoadForEditCommand = new RelayCommand(LoadForEdit, _ => SelectedAccount != null);

            PickPathCommand = new RelayCommand(PickGamePath, _ => SelectedAccount != null);
            CreateMirrorCommand = new RelayCommand(CreateMirrorPath, _ => SelectedAccount != null);
            SaveCommand = new RelayCommand(SaveAccounts);
            DeleteCommand = new RelayCommand(DeleteAccount, _ => SelectedAccount != null);
            DeleteSystemUserCommand = new RelayCommand(DeleteSystemUser, _ => !string.IsNullOrEmpty(InputUsername));
            
            // Part 2: Launch
            OneClickLaunchCommand = new RelayCommand(OneClickLaunch, CanLaunchGame);
            DirectLaunchCommand = new RelayCommand(DirectLaunch, CanLaunchGame);
            
            // Manual
            KillBnetCommand = new RelayCommand(KillBnet);
            CleanConfigCommand = new RelayCommand(CleanConfig);
            KillMutexCommand = new RelayCommand(KillMutex);
            SnapshotConfigCommand = new RelayCommand(SnapshotConfig, _ => SelectedAccount != null);

            
            ToggleLangCommand = new RelayCommand(ToggleLanguage);

            StatusMessage = IsChinese ? "就绪。请先在左侧管理用户。" : "Ready. Manage users on the left first.";
        }



        private bool CanCreateOrLink(object? parameter)
        {
            return !string.IsNullOrWhiteSpace(InputUsername);
        }



        private void LoadForEdit(object? parameter)
        {
            if (SelectedAccount == null) return;
            // Populate inputs from selection
            InputUsername = SelectedAccount.Username;
            InputPassword = SelectedAccount.Password;
            InputBattleTag = SelectedAccount.BattleTag;
            InputNote = SelectedAccount.Note;
            
            StatusMessage = IsChinese ? "已加载信息到上方输入框，修改后点击[更新信息]。" : "Loaded info. Modify and click [Update Info].";
        }

        private void UpdateUser(object? parameter)
        {
            if (SelectedAccount == null) return;
            
            // Update the model
            SelectedAccount.Username = InputUsername;
            SelectedAccount.Password = InputPassword;
            SelectedAccount.BattleTag = InputBattleTag;
            SelectedAccount.Note = InputNote;
            
            // Trigger UI refresh
            int index = Accounts.IndexOf(SelectedAccount);
            if (index != -1) {
                var temp = SelectedAccount;
                Accounts.RemoveAt(index);
                Accounts.Insert(index, temp);
                SelectedAccount = temp;
            }
            
            SaveAccounts();
            StatusMessage = IsChinese ? "用户信息已更新。" : "User info updated.";
            
            // Clear inputs checking? User might want to keep them. Let's keep them.
        }

        private void CheckSelectedUserStatus()
        {
            if (SelectedAccount == null) return;
            
            bool exists = _userManager.UserExists(SelectedAccount.Username);
            if (!exists)
            {
                StatusMessage = IsChinese 
                    ? $"警告: 用户 {SelectedAccount.Username} 在系统中不存在 (Ghost User)。请新建。" 
                    : $"Warning: User {SelectedAccount.Username} not found (Ghost User). Please create.";
            }
            else
            {
                StatusMessage = IsChinese 
                    ? $"就绪: 用户 {SelectedAccount.Username} 有效。" 
                    : $"Ready: User {SelectedAccount.Username} verified.";
            }
        }

        private void ToggleLanguage(object? parameter)
        {
            IsChinese = !IsChinese;
            // Update SelectedLanguage to match
            SelectedLanguage = IsChinese ? "简体中文" : "English";
        }

        private void UpdateAllLocalization()
        {
            OnPropertyChanged(string.Empty);
            // Specifically notify these as they are computed properties
            OnPropertyChanged(nameof(LabelAdminStatus));
            OnPropertyChanged(nameof(ColorAdminStatus));
        }
        
        private void CreateNewUser(object? parameter)
        {
            var dialog = new CreateUserWindow(IsChinese);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            if (dialog.ShowDialog() == true)
            {
                string user = dialog.Username;
                string pass = dialog.Password;
                string btag = dialog.BattleTag; // New Field
                string desc = dialog.Description;
                string note = desc; // Use description as note initially

                // 2. Logic: Create User via Module A
                bool success = false;
                try
                {
                    _userManager.EnsureUserExists(user, pass);
                    success = true;
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error creating user: {ex.Message}";
                    success = false;
                }
                
                if (success)
                {
                    // 3. Create Account Model
                    var newAccount = new Account
                    {
                        Username = user,
                        Password = pass, // In real app, encrypt this
                        BattleTag = btag, // Set from dialog
                        Note = note,
                        GamePath = "" 
                    };

                    Accounts.Add(newAccount);
                    SaveAccounts(null); // Auto-save

                    string msg = IsChinese
                    ? $"用户 {user} 创建成功!\n\n1. **必须步骤**: 请立即切换 Windows 用户到 '{user}' 登录一次 (以初始化桌面)。\n2. **建议步骤**: 在该用户下打开战网客户端登录一次。\n\n提示: 如果首次一键启动时卡在登录界面，请关闭战网并重试。"
                    : $"User {user} Created!\n\n1. **REQUIRED**: Log out & Log in as '{user}' once (to init desktop).\n2. **SUGGESTED**: Open Battle.net and log in once.\n\nTip: If first launch freezes, close Battle.net and try again.";
                    
                    MessageBox.Show(msg, "Important: First Run Setup");
                    StatusMessage = IsChinese ? $"用户 {user} 已创建 (需手动登录初始化)。" : $"User {user} created (Manual Login Required).";
                    SelectedAccount = newAccount; // Auto-select
                }
                else
                {
                    StatusMessage = IsChinese ? $"创建用户 {user} 失败。" : $"Failed to create user {user}.";
                    System.Windows.MessageBox.Show(StatusMessage, "Error");
                }
            }
        }

        private void LinkExistingUser(object? parameter)
        {
            try
            {
                // 1. Get List of Windows Users
                // 1. Get List of Windows Users
                var users = _userManager.GetLocalUsers();
                
                // 2. Open Dialog
                var dialog = new UserSelectionWindow(users, IsChinese);
                dialog.Owner = System.Windows.Application.Current.MainWindow;

                if (dialog.ShowDialog() == true)
                {
                    string selectedUser = dialog.SelectedUsername;
                    string password = dialog.Password;
                    
                    // Check if already linked
                    if (Accounts.Any(a => a.Username == selectedUser))
                    {
                        StatusMessage = IsChinese ? "该用户已在列表中。" : "User already in list.";
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
                    StatusMessage = IsChinese ? $"用户 {selectedUser} 已关联。" : $"User {selectedUser} linked.";
                    SelectedAccount = newAccount;
                }
            }
            catch (Exception ex) { PositionError(ex); }
        }

        private void AddAccountToList(string u, string p, string tag, string n)
        {
            var acc = new Account
            {
                Username = u,
                Password = p,
                BattleTag = tag, // Set Tag
                Note = n,
                GamePath = @"C:\Program Files (x86)\Diablo II Resurrected\D2R.exe" // Default
            };
            Accounts.Add(acc);
            SelectedAccount = acc;
            SaveAccounts();
            
            // Clear inputs
            InputUsername = "";
            InputPassword = "";
            InputBattleTag = "";
            InputNote = "";
        }

        private void PickGamePath(object? parameter)
        {
            if (SelectedAccount == null) return;
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "D2R.exe|D2R.exe|All Files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string newPath = openFileDialog.FileName;

                // Duplicate Check
                var duplicate = Accounts.FirstOrDefault(a => 
                    a != SelectedAccount && 
                    string.Equals(a.GamePath, newPath, StringComparison.OrdinalIgnoreCase));

                if (duplicate != null)
                {
                    string msg = IsChinese 
                        ? $"警告: 该路径已被用户 '{duplicate.Username}' 使用。\n多开需要不同路径(或镜像)。确定要重复使用吗?" 
                        : $"Warning: Path used by '{duplicate.Username}'.\nMulti-boxing requires unique paths. Reuse?";
                    
                    if (MessageBox.Show(msg, "Duplicate Path", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                SelectedAccount.GamePath = newPath;
                OnPropertyChanged(nameof(SelectedAccount)); // Refresh UI
                SaveAccounts();
            }
        }

        // --- Launch Logic ---

        private void OneClickLaunch(object? parameter)
        {
            if (SelectedAccount == null) return;
            SaveAccounts();

            try
            {
                StatusMessage = IsChinese ? "一键启动中..." : "Launching...";

                // 1. Kill Bnet
                _isolationEngine.KillBattleNetProcesses();

                // 2. CONFIG STRATEGY: AUTO-BACKUP & RESTORE
                
                // A. Auto-Backup Previous User (Save the session!)
                if (!string.IsNullOrEmpty(_settings.LastLaunchedUsername) && _settings.LastLaunchedUsername != SelectedAccount.Username)
                {
                     // If we are switching users, SAVE the config of the previous guy before we overwrite it.
                     // This assumes product.db currently belongs to LastLaunchedUsername.
                     try { _isolationEngine.BackupBattleNetConfig(_settings.LastLaunchedUsername); } catch { /* Ignore backup errors */ }
                }
                else if (!string.IsNullOrEmpty(_settings.LastLaunchedUsername) && _settings.LastLaunchedUsername == SelectedAccount.Username)
                {
                     // Same user launching again. Should we backup? 
                     // Yes, in case they changed paths in the last session. Always keep latest.
                     try { _isolationEngine.BackupBattleNetConfig(_settings.LastLaunchedUsername); } catch { /* Ignore */ }
                }

                
                // B. Restore Current User
                bool restored = _isolationEngine.RestoreBattleNetConfig(SelectedAccount.Username);
                if (restored)
                {
                     StatusMessage = IsChinese ? "已恢复专属配置 (含路径)..." : "Restored specific config (checking paths)...";
                }
                else
                {
                     // C. If Restore Fails (First Run or New User) -> FORCE CLEAN.
                     // User must locate their UNIQUE game path. We cannot inherit paths in multiboxing.
                     _isolationEngine.CleanBattleNetConfig();
                     
                     StatusMessage = IsChinese 
                        ? "未找到快照，已清理配置。启动后请手动【定位游戏】到该账号的专属路径(镜像)。" 
                        : "No snapshot. Config cleaned. Manually [Locate Game] to unique path/mirror after launch.";
                }
                
                // D. Update Last Launched User
                _settings.LastLaunchedUsername = SelectedAccount.Username;
                SaveSettings();

                // 3. Kill Mutex
                _isolationEngine.KillGameMutexes();

                // 4. Launch Battle.net (Shadow User)
                LaunchBattleNet(SelectedAccount);
            }
            catch (Exception ex) { StatusMessage = $"Launch Error: {ex.Message}"; }
        }

        private void DirectLaunch(object? parameter)
        {
             if (SelectedAccount == null) return;
             try 
             {
                 StatusMessage = IsChinese ? "直接启动 (仅拉起战网)..." : "Direct Launching (Bnet only)...";
                 // User Request: Direct Launch is same as One-Click but without cleanup steps.
                 // Both must launch Battle.net, not D2R.exe directly.
                 LaunchBattleNet(SelectedAccount);
             }
             catch(Exception ex) { StatusMessage = $"Launch Error: {ex.Message}"; }
        }

        private void LaunchBattleNet(Account acc)
        {
            VerifyUser(acc.Username);
            
            // SECURITY CHECK: Password Requirement
            bool isCurrentUser = acc.Username.Equals(Environment.UserName, StringComparison.OrdinalIgnoreCase);

            if (!isCurrentUser && string.IsNullOrEmpty(acc.Password))
            {
               string err = IsChinese 
                   ? "无法启动: Windows 用户必须设置密码。\n\n技术原因: Windows 的隔离运行机制 (CreateProcessWithLogonW) 强制要求目标用户必须有密码才能调用。\n\n解决办法: 请在 Windows 设置中为该用户设置一个密码，然后在本工具中更新。" 
                   : "Launch Failed: Windows User MUST have a password.\n\nReason: Windows security policies require a password for process isolation features.\n\nFix: Set a password for this user in Windows, then update it here.";
               throw new Exception(err);
            }

            // Try to find Battle.net
            string bnetPath = @"C:\Program Files (x86)\Battle.net\Battle.net.exe";
            if (!File.Exists(bnetPath))
            {
                // Fallback: check typically used drives/paths or just fail
                throw new FileNotFoundException(IsChinese ? "未找到 Battle.net.exe (默认路径)" : "Battle.net.exe not found!");
            }

            int pid = 0;
            if (isCurrentUser)
            {
                // Direct Launch for Current User (No Password Needed)
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
                // Shadow Launch
                pid = _userManager.LaunchProgramAsUser(
                    acc.Username,
                    acc.Password,
                    bnetPath,
                    "--exec=\"launch D2R\""); 
            }
            
            StatusMessage = IsChinese 
                ? $"已启动战网 (PID: {pid})。{(isCurrentUser ? "[当前用户模式]" : "[隔离模式]")}" 
                : $"Launched Battle.net (PID: {pid}). {(isCurrentUser ? "[Current User]" : "[Shadow User]")}";
        }

        private void LaunchD2RDirect(Account acc)
        {
            VerifyUser(acc.Username);
            int pid = _userManager.LaunchProgramAsUser(
                acc.Username,
                acc.Password,
                acc.GamePath,
                "-launch -uid europa");
            StatusMessage = $"D2R Direct Launch PID: {pid}";
        }

        private void VerifyUser(string username)
        {
             if (!_userManager.UserExists(username))
            {
                throw new Exception(IsChinese ? $"用户 {username} 不存在! 请先在上方新建。" : $"User {username} missing!");
            }
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
                CheckSelectedUserStatus(); // Refresh status on save too
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
                MessageBox.Show(IsChinese ? "已强制关闭战网及 Agent 进程。" : "Killed Battle.net & Agent processes.", "Success");
                StatusMessage = "Killed Bnet";
            }
            catch(Exception e){ PositionError(e); }
        }

        private void CleanConfig(object? p) 
        { 
            try
            {
                _isolationEngine.CleanBattleNetConfig();
                MessageBox.Show(IsChinese ? "已删除 product.db 配置文件。" : "Deleted product.db config.", "Success");
                StatusMessage = "Cleaned Config";
            }
            catch(Exception e){ PositionError(e); }
        }

        private void CreateMirrorPath(object? parameter)
        {
            if (SelectedAccount == null) return;

            // 1. Prompt for Source D2R.exe
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = IsChinese ? "请选择原始 D2R.exe (源文件)" : "Select Source D2R.exe (Original)";
            dialog.Filter = "D2R.exe|D2R.exe";
            
            if (dialog.ShowDialog() != true) return;

            string sourcePath = dialog.FileName;
            if (!File.Exists(sourcePath)) return;

            string sourceDir = Path.GetDirectoryName(sourcePath);
            string exeName = Path.GetFileName(sourcePath);
            
            // 2. Generate Mirror Name: D2R_Clone_{Username}
            string safeUsername = SelectedAccount.Username;
            foreach (char c in Path.GetInvalidFileNameChars()) safeUsername = safeUsername.Replace(c, '_');
            
            string mirrorDirName = $"D2R_Clone_{safeUsername}";
            string targetDir = Path.Combine(Path.GetDirectoryName(sourceDir), mirrorDirName); // Create sibling folder

            // 3. Logic: Create Junction
            bool success = false;
            try 
            {
                _isolationEngine.CreateGameJunction(sourceDir, targetDir);
                success = true;
            }
            catch (Exception ex)
            {
                 StatusMessage = $"Error creating junction: {ex.Message}";
                 success = false;
                 MessageBox.Show(StatusMessage, "Error");
            }

            if (success)
            {
                string newExePath = Path.Combine(targetDir, exeName);
                SelectedAccount.GamePath = newExePath; // Auto-update model (Point to Mirror)
                SaveAccounts(null); // Save
                OnPropertyChanged(nameof(SelectedAccount));
                
                string msg = IsChinese 
                    ? $"镜像创建成功!\n\n路径已更新为: {newExePath}\n\n该账号现已隔离。" 
                    : $"Mirror Created!\n\nPath updated to: {newExePath}\n\nAccount is now isolated.";
                MessageBox.Show(msg, "Success");
                StatusMessage = $"Mirror created: {mirrorDirName}";
            }
        }
        private void KillMutex(object? p) 
        { 
            try
            {
                int count = _isolationEngine.KillGameMutexes();
                string msg = IsChinese 
                    ? $"句柄清理完成。共关闭了 {count} 个互斥体。" 
                    : $"Mutex cleanup done. Closed {count} handles.";
                MessageBox.Show(msg, "Success");
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
                string msg = IsChinese 
                    ? $"配置抓取成功!\n当前战网配置已保存为 '{SelectedAccount.Username}' 的专属快照。\n下次一键启动时将自动恢复此配置 (包含游戏路径)。" 
                    : $"Config Snapshot Saved!\nCurrent Bnet config saved for '{SelectedAccount.Username}'.\nIt will be auto-restored on next launch (including game paths).";
                MessageBox.Show(msg, "Success");
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

                // Strong Warning
                string title = IsChinese ? "危险操作确认 (High Risk)" : "High Risk Confirmation";
                string msg = IsChinese 
                    ? $"警告！即将执行不可逆操作：\n\n彻底删除 Windows 用户 '{userToDelete}'\n\n1. 该用户的所有文档、存档、配置将被永久抹除。\n2. 此操作无法撤销。\n\n您确定要继续吗？" 
                    : $"WARNING! Irreversible Action:\n\nPermanently deleting system user '{userToDelete}'\n\n1. All documents, saves, and configs for this user will be wiped.\n2. This cannot be undone.\n\nAre you sure?";

                if (MessageBox.Show(
                    msg,
                    title,
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Stop,
                    MessageBoxResult.No) == MessageBoxResult.Yes) // Default to No
                {
                     _userManager.DeleteUser(userToDelete);
                     
                     // Also remove from App List if exists
                     var acc = Accounts.FirstOrDefault(a => a.Username.Equals(userToDelete, StringComparison.OrdinalIgnoreCase));
                     if (acc != null)
                     {
                         Accounts.Remove(acc);
                         if (SelectedAccount == acc) SelectedAccount = null;
                         SaveAccounts();
                     }

                     StatusMessage = IsChinese ? $"用户 {userToDelete} 已从系统的列表中删除。" : $"User {userToDelete} deleted from system & list.";
                     MessageBox.Show(StatusMessage, "Success");
                }
            }
            catch(Exception ex) { PositionError(ex); }
        }

        // --- Settings Management ---
        public class AppSettings
        {
            public string LastLaunchedUsername { get; set; } = "";
        }
        
        private AppSettings _settings = new AppSettings();
        private const string SETTINGS_FILE = "settings.json";

        private void LoadSettings()
        {
            try {
                if (File.Exists(SETTINGS_FILE)) 
                    _settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SETTINGS_FILE)) ?? new AppSettings();
            } catch {}
        }

        private void SaveSettings()
        {
            try {
                File.WriteAllText(SETTINGS_FILE, JsonSerializer.Serialize(_settings));
            } catch {}
        }

        private void PositionError(Exception e)
        {
            StatusMessage = $"Error: {e.Message}";
            MessageBox.Show(e.Message, "Error");
        }
    }
}
