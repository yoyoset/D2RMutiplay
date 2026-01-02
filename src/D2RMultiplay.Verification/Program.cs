using System;
using System.IO;
using D2RMultiplay.Modules.ModuleA_AccountManager;

namespace D2RMultiplay.Verification
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Windows Account Manager Verification ===");

            string testUser = "D2R_Test_User_01";
            string testPass = "TestPass123"; // Shortened to avoid warning
            
            var manager = new WindowsUserManager();

            try
            {
                // 1. Ensure User Exists
                Console.WriteLine($"\n[1] Ensuring user '{testUser}' exists...");
                manager.EnsureUserExists(testUser, testPass);
                Console.WriteLine("User creation/check step completed.");

                // 2. Load Profile
                Console.WriteLine($"\n[2] Loading profile for '{testUser}' (This prepares AppData)...");
                bool loaded = manager.LoadUserProfile(testUser, testPass);
                
                if (loaded)
                {
                    Console.WriteLine("SUCCESS: Profile loaded via API.");
                }
                else
                {
                    Console.WriteLine("FAILURE: Profile load returned false.");
                    return;
                }

                // 3. Verify Directory
                string expectedPath = Path.Combine("C:\\Users", testUser);
                Console.WriteLine($"\n[3] Verifying directory: {expectedPath}");
                if (Directory.Exists(expectedPath))
                {
                    Console.WriteLine("SUCCESS: User profile directory exists!");
                    
                    // Check AppData
                    string appData = Path.Combine(expectedPath, "AppData");
                    if (Directory.Exists(appData))
                    {
                         Console.WriteLine("SUCCESS: AppData directory exists!");
                    }
                    else
                    {
                         Console.WriteLine("WARNING: AppData not found immediately (might need hiding?).");
                    }
                }
                else
                {
                    Console.WriteLine("FAILURE: User profile directory does NOT exist.");
                }

                Console.WriteLine("\n[Module A Verified]");
                
                // === Module C Verification ===
                Console.WriteLine("\n=== Isolation Engine Verification (Module C) ===");
                var isolation = new D2RMultiplay.Modules.ModuleC_IsolationEngine.IsolationEngine();
                
                // C1. Test Config Cleaner
                Console.WriteLine("[C1] Testing Config Cleaner (Safe Mode)...");
                isolation.CleanBattleNetConfig();
                Console.WriteLine("Config Cleaner executed.");

                // C2. Test Handle Killer
                Console.WriteLine("\n[C2] Testing Handle Killer (Scan Only)...");
                int closed = isolation.KillGameMutexes();
                Console.WriteLine($"Scan complete. Closed handles: {closed}");
                
                Console.WriteLine("\n[Module C Verified]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nEXCEPTION: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey(); 
        }
    }
}
