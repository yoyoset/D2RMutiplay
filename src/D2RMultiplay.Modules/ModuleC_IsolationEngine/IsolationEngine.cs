using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using D2RMultiplay.Core.Interfaces;

namespace D2RMultiplay.Modules.ModuleC_IsolationEngine
{
    public class IsolationEngine : IIsolationEngine
    {
        private const string BATTLE_NET_AGENT_DB = @"C:\ProgramData\Battle.net\Agent\product.db";
        // Note: There might be other files like 'Battle.net.config' but product.db is the main index for "installed games".
        
        public void CleanBattleNetConfig()
        {
            Console.WriteLine("[IsolationEngine] Executing Scorched Earth on Battle.net config...");

            // 1. Ensure Agent is dead (optional but recommended to release file locks)
            // Ideally LaunchController handles closing Bnet, but we can do a safety check here.
            // KillProcess("Agent"); 

            // 2. Delete product.db
            try
            {
                if (File.Exists(BATTLE_NET_AGENT_DB))
                {
                    File.Delete(BATTLE_NET_AGENT_DB);
                    Console.WriteLine($"[IsolationEngine] Deleted {BATTLE_NET_AGENT_DB}");
                }
                else
                {
                    Console.WriteLine($"[IsolationEngine] {BATTLE_NET_AGENT_DB} not found. access safe.");
                }

                // Consider also clearing backup files if they exist? e.g. .db.temp
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IsolationEngine] Failed to delete config: {ex.Message}");
                // Non-critical? If we fail, battle.net might pick up old path. 
                // We should probably throw or log error.
            }
        }

        public int KillGameMutexes()
        {
            int count = 0;
            Console.WriteLine("[IsolationEngine] Scanning for D2R mutexes...");
            
            // This requires low-level system calls (NtQuerySystemInformation).
            // Delegating to HandleKiller helper.
            count = HandleKiller.CloseAllD2RMutexes();

            Console.WriteLine($"[IsolationEngine] Closed {count} mutex handles.");
            return count;
        }

        public void KillBattleNetProcesses()
        {
            Console.WriteLine("[IsolationEngine] Killing Battle.net and Agent processes...");
            KillProcess("Battle.net");
            KillProcess("Agent");
        }

        private void KillProcess(string name)
        {
            foreach (var p in Process.GetProcessesByName(name))
            {
                try 
                { 
                    string pName = p.ProcessName;
                    int pId = p.Id;
                    p.Kill(); 
                    p.WaitForExit(1000); 
                    Console.WriteLine($"[IsolationEngine] Killed {pName} (PID: {pId})");
                } 
                catch (Exception ex) 
                {
                     Console.WriteLine($"[IsolationEngine] Failed to kill {name}: {ex.Message}");
                }
            }
        }

        public void CreateGameJunction(string sourcePath, string targetPath)
        {
            Console.WriteLine($"[IsolationEngine] Creating Junction: {targetPath} -> {sourcePath}");
            
            if (!Directory.Exists(sourcePath)) throw new DirectoryNotFoundException($"Source path not found: {sourcePath}");
            if (Directory.Exists(targetPath)) throw new Exception($"Target path already exists: {targetPath}");

            // Mklink is a cmd internal command
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c mklink /J \"{targetPath}\" \"{sourcePath}\"";
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
                    throw new Exception($"Mklink failed. ExitCode: {p.ExitCode}\nError: {error}\nOutput: {output}");
                }
                Console.WriteLine("[IsolationEngine] Junction created successfully.");
            }
        }

        public void BackupBattleNetConfig(string id)
        {
            try 
            {
                if (!File.Exists(BATTLE_NET_AGENT_DB))
                {
                    Console.WriteLine($"[IsolationEngine] Cannot backup, source DB missing: {BATTLE_NET_AGENT_DB}");
                    return;
                }

                string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
                if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);

                string destPath = Path.Combine(backupDir, $"config_{id}.db");
                File.Copy(BATTLE_NET_AGENT_DB, destPath, true); // Overwrite
                Console.WriteLine($"[IsolationEngine] Backed up config to {destPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IsolationEngine] Backup Error: {ex.Message}");
                throw; // Rethrow to UI
            }
        }

        public bool RestoreBattleNetConfig(string id)
        {
            try
            {
                string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
                string sourcePath = Path.Combine(backupDir, $"config_{id}.db");

                if (!File.Exists(sourcePath))
                {
                    Console.WriteLine($"[IsolationEngine] No backup found for id: {id}");
                    return false;
                }

                // Ensure parent exists
                string destDir = Path.GetDirectoryName(BATTLE_NET_AGENT_DB);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

                File.Copy(sourcePath, BATTLE_NET_AGENT_DB, true); // Overwrite
                Console.WriteLine($"[IsolationEngine] Restored config from {sourcePath}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IsolationEngine] Restore Error: {ex.Message}");
                return false;
            }
        }
    }
}
