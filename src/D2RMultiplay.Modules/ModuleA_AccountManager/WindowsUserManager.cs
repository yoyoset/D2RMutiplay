using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using D2RMultiplay.Core.Interfaces;

namespace D2RMultiplay.Modules.ModuleA_AccountManager
{
    public class WindowsUserManager : IWindowsUserManager
    {
        public bool UserExists(string username)
        {
            try
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = "net";
                    p.StartInfo.Arguments = $"user {username}";
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();
                    
                    bool exited = p.WaitForExit(2000); 
                    if (!exited) { p.Kill(); return false; }
                    return p.ExitCode == 0;
                }
            }
            catch { return false; }
        }

        public void EnsureUserExists(string username, string password)
        {
            if (UserExists(username))
            {
                return;
            }

            CreateUser(username, password);
        }

        public bool LoadUserProfile(string username)
        {
            Console.WriteLine($"[WindowsUserManager] Attempting to load profile for {username}...");
            
            // We need the password to logon. 
            // NOTE: In a real app we'd retrieve this from secure storage. 
            // For this interface signature, if we don't have password, we can't LogonUser.
            // I will assume for the Prototype that the upper layer passes password or we store it.
            // Wait, the interface only asked for username. 
            // I need to update the interface or store the password temporarily.
            // For now, I'll update the interface/implementation to require password for LoadUserProfile as well 
            // OR finding a way to lookup password. 
            // Let's assume for this specific method, we might fail if we don't have password.
            // Actually, the PRD says "Programmatically load user profile". It requires a Token. Token requires Password.
            // I will update the logic to accept password in LoadUserProfile or manage it internally.
            
            // Re-reading interface: bool LoadUserProfile(string username).
            // This is a disconnect. I should probably instantiate this Manager with a secure vault reference, 
            // or pass password. 
            // For the Prototype verification step, I will overload or change the internal logic.
            // Let's change the implementation to throw NotSupported since we lack password, 
            // OR I will fix the interface in the next step to include password.
            // Better: I will implement a helper `LoadUserProfile(username, password)` and update interface later.
            return false; 
        }

        public bool LoadUserProfile(string username, string password)
        {
             IntPtr hToken = IntPtr.Zero;
             IntPtr hProfile = IntPtr.Zero;

             try
             {
                 bool logonResult = NativeMethods.LogonUser(
                     username, 
                     ".", 
                     password, 
                     NativeMethods.LOGON32_LOGON_INTERACTIVE, 
                     NativeMethods.LOGON32_PROVIDER_DEFAULT, 
                     out hToken);

                 if (!logonResult)
                 {
                     int error = Marshal.GetLastWin32Error();
                     Console.WriteLine($"[WindowsUserManager] LogonUser failed. Error: {error}");
                     return false;
                 }

                 NativeMethods.PROFILEINFO profileInfo = new NativeMethods.PROFILEINFO();
                 profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                 profileInfo.lpUserName = username;

                 bool loadResult = NativeMethods.LoadUserProfile(hToken, ref profileInfo);
                 if (!loadResult)
                 {
                     int error = Marshal.GetLastWin32Error();
                     Console.WriteLine($"[WindowsUserManager] LoadUserProfile failed. Error: {error}");
                     NativeMethods.CloseHandle(hToken);
                     return false;
                 }

                 hProfile = profileInfo.hProfile;
                 Console.WriteLine($"[WindowsUserManager] Profile loaded successfully for {username}.");
                 
                 // Check if directory exists now
                 string profilePath = $"C:\\Users\\{username}"; // Simple heuristic
                 if (Directory.Exists(profilePath))
                 {
                     Console.WriteLine($"[WindowsUserManager] User directory verified at {profilePath}");
                 }

                 // Unload immediately for initialization check? Or keep it?
                 // Usually for "Initialization" we can just Unload. 
                 // The directory should persist.
                 NativeMethods.UnloadUserProfile(hToken, hProfile);
                 NativeMethods.CloseHandle(hToken);

                 return true;
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"[WindowsUserManager] Exception: {ex.Message}");
                 if (hToken != IntPtr.Zero) NativeMethods.CloseHandle(hToken);
                 return false;
             }
        }

        public int LaunchProgramAsUser(string username, string password, string programPath, string args)
        {
             NativeMethods.STARTUPINFO si = new NativeMethods.STARTUPINFO();
             si.cb = Marshal.SizeOf(si);
             si.lpDesktop = ""; 
             
             NativeMethods.PROCESS_INFORMATION pi = new NativeMethods.PROCESS_INFORMATION();

             // Flags: 
             // LOGON_WITH_PROFILE (1) is usually recommended so AppData works. 
             // Although we manually loaded it, using this flag ensures environment variables are set.
             int logonFlags = NativeMethods.LOGON_WITH_PROFILE; 

             string commandLine = $"\"{programPath}\" {args}";

             Console.WriteLine($"[WindowsUserManager] Launching {commandLine} as {username}...");

             bool result = NativeMethods.CreateProcessWithLogonW(
                 username,
                 ".",
                 password,
                 logonFlags,
                 null, // Application Name
                 commandLine, // Command Line (Application Name usually null if CmdLine has it)
                 NativeMethods.CREATE_NEW_CONSOLE | NativeMethods.CREATE_UNICODE_ENVIRONMENT,
                 IntPtr.Zero,
                 Path.GetDirectoryName(programPath),
                 ref si,
                 out pi);

             if (!result)
             {
                 int error = Marshal.GetLastWin32Error();
                 throw new Exception($"CreateProcessWithLogonW failed with error code {error}");
             }

             NativeMethods.CloseHandle(pi.hThread);
             NativeMethods.CloseHandle(pi.hProcess);

             return pi.dwProcessId;
        }

        public void DeleteUser(string username)
        {
            Console.WriteLine($"[WindowsUserManager] Deleting user {username}...");
            if (!UserExists(username))
            {
                Console.WriteLine($"[WindowsUserManager] User {username} does not exist.");
                return;
            }

            using (Process p = new Process())
            {
                p.StartInfo.FileName = "net";
                p.StartInfo.Arguments = $"user {username} /delete";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                     // Access is denied check
                    if (error.Contains("Access is denied") || output.Contains("Access is denied") || error.Contains("拒绝访问") || output.Contains("拒绝访问"))
                    {
                         throw new UnauthorizedAccessException("Administrator privileges are required to delete Windows users.");
                    }
                    throw new Exception($"Failed to delete user {username}. ExitCode: {p.ExitCode}\nError: {error}");
                }
                
                Console.WriteLine($"[WindowsUserManager] Deleted user {username}.");
            }
        }
        private void CreateUser(string username, string password)
        {
            Console.WriteLine($"[WindowsUserManager] Creating user {username}...");
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "net";
                p.StartInfo.Arguments = $"user {username} {password} /add /fullname:\"{username}\" /passwordchg:no /Y";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true; // Still hidden
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                
                var outputBuilder = new System.Text.StringBuilder();
                var errorBuilder = new System.Text.StringBuilder();

                p.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
                p.ErrorDataReceived += (s, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                bool exited = p.WaitForExit(10000); // 10s timeout
                if (!exited)
                {
                    try { p.Kill(); } catch { }
                    throw new TimeoutException($"Create user timed out.\nPartial Output: {outputBuilder}\nPartial Error: {errorBuilder}");
                }

                if (p.ExitCode != 0)
                {
                    string err = errorBuilder.ToString();
                    string outStr = outputBuilder.ToString();

                    // Access is denied (5)
                    if (err.Contains("Access is denied") || outStr.Contains("Access is denied") || err.Contains("拒绝访问") || outStr.Contains("拒绝访问"))
                    {
                         throw new UnauthorizedAccessException("Administrator privileges are required to create Windows users. Please run as Admin.");
                    }
                    throw new Exception($"Failed to create user {username}. ExitCode: {p.ExitCode}.\nError: {err}\nOutput: {outStr}");
                }
                
                Console.WriteLine($"[WindowsUserManager] Created user {username}");
            }
        }


        public List<string> GetLocalUsers()
        {
            var users = new List<string>();
            int entriesRead;
            int totalEntries;
            int resumeHandle = 0;
            IntPtr bufPtr = IntPtr.Zero;

            try 
            {
                int nStatus = NativeMethods.NetUserEnum(
                    null, 
                    0, 
                    NativeMethods.FILTER_NORMAL_ACCOUNT, 
                    out bufPtr, 
                    -1, 
                    out entriesRead, 
                    out totalEntries, 
                    ref resumeHandle);

                if (nStatus == 0)
                {
                    IntPtr iter = bufPtr;
                    for (int i = 0; i < entriesRead; i++)
                    {
                        var user = (NativeMethods.USER_INFO_0)Marshal.PtrToStructure(iter, typeof(NativeMethods.USER_INFO_0))!;
                        
                        // Filter standard built-in accounts if needed, but NetUserEnum mostly returns what users see in compmgmt
                        // Let's filter some common system ones just in case
                        string name = user.usri0_name;
                        if (name != "Administrator" && name != "Guest" && name != "DefaultAccount" && name != "WDAGUtilityAccount")
                        {
                            users.Add(name);
                        }

                        iter = (IntPtr)((long)iter + Marshal.SizeOf(typeof(NativeMethods.USER_INFO_0)));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WindowsUserManager] NetUserEnum failed: {ex.Message}");
            }
            finally
            {
                if (bufPtr != IntPtr.Zero)
                    NativeMethods.NetApiBufferFree(bufPtr);
            }

            return users;
        }

        public bool ValidateCredentials(string username, string password)
        {
             IntPtr hToken = IntPtr.Zero;
             try
             {
                 return NativeMethods.LogonUser(
                     username, 
                     ".", 
                     password, 
                     NativeMethods.LOGON32_LOGON_INTERACTIVE, 
                     NativeMethods.LOGON32_PROVIDER_DEFAULT, 
                     out hToken);
             }
             catch 
             {
                 return false;
             }
             finally
             {
                 if (hToken != IntPtr.Zero) NativeMethods.CloseHandle(hToken);
             }
        }
    }
}
