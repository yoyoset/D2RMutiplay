using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace D2RMultiplay.Modules.ModuleC_IsolationEngine
{
    internal static class HandleKiller
    {
        // Mutex name to find
        private const string D2R_MUTEX_NAME = "DiabloII Check For Other Instances";

        public static int CloseAllD2RMutexes()
        {
            int closedCount = 0;
            
            // 1. Identify target PIDs (Only D2R)
            var targetPids = new HashSet<int>();
            try 
            {
                foreach(var p in Process.GetProcessesByName("D2R")) targetPids.Add(p.Id);
                foreach(var p in Process.GetProcessesByName("DiabloII")) targetPids.Add(p.Id);
            }
            catch {} // Ignore race conditions getting PIDs

            if (targetPids.Count == 0) return 0; // No game running

            // 2. Get ALL system handles once (Fast, low-level)
            var allHandles = GetSystemHandles();

            // 3. Filter only handles belonging to our target PIDs
            // We optimize by not even looking at handles from other processes
            var d2rHandles = new List<SYSTEM_HANDLE_ENTRY>();
            foreach(var h in allHandles)
            {
                if(targetPids.Contains(h.ProcessId))
                {
                    d2rHandles.Add(h);
                }
            }

            foreach (var entry in d2rHandles)
            {
                // 4. Check Handle Name
                // Note: GetHandleName requires OpenProcess/DuplicateHandle which is "heavy"
                // But now we ONLY do it for D2R processes, not random system ones.
                string? name = GetHandleName(entry.ProcessId, (IntPtr)entry.HandleValue);
                if (!string.IsNullOrEmpty(name) && name.Contains(D2R_MUTEX_NAME, StringComparison.OrdinalIgnoreCase))
                {
                     Console.WriteLine($"[HandleKiller] Found D2R Mutex! PID: {entry.ProcessId}, Handle: {entry.HandleValue:X}");
                     if (CloseRemoteHandle(entry.ProcessId, (IntPtr)entry.HandleValue))
                     {
                         closedCount++;
                     }
                }
            }
            return closedCount;
        }

        // --- Low Level Win32 Logic ---
        
        [DllImport("ntdll.dll")]
        private static extern uint NtQuerySystemInformation(int SystemInformationClass, IntPtr SystemInformation, int SystemInformationLength, out int ReturnLength);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr lpTargetHandle, int dwDesiredAccess, bool bInheritHandle, int dwOptions);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("ntdll.dll")]
        private static extern uint NtQueryObject(IntPtr Handle, int ObjectInformationClass, IntPtr ObjectInformation, int ObjectInformationLength, out int ReturnLength);

        private const int SystemHandleInformation = 16;
        private const int ObjectNameInformation = 1;

        private const int PROCESS_DUP_HANDLE = 0x0040;
        private const int DUPLICATE_CLOSE_SOURCE = 0x1;

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_HANDLE_ENTRY
        {
            public ushort ProcessId;
            public ushort CreatorBackTraceIndex;
            public byte ObjectTypeIndex;
            public byte HandleAttributes;
            public ushort HandleValue;
            public IntPtr Object;
            public uint GrantedAccess;
        }

        private static List<SYSTEM_HANDLE_ENTRY> GetSystemHandles()
        {
            List<SYSTEM_HANDLE_ENTRY> handles = new List<SYSTEM_HANDLE_ENTRY>();
            int size = 0x10000;
            IntPtr rootPtr = Marshal.AllocHGlobal(size); 
            IntPtr ptr = rootPtr;
            int needed = 0;

            try
            {
                // Loop until buffer is large enough
                while (NtQuerySystemInformation(SystemHandleInformation, ptr, size, out needed) == 0xC0000004) // STATUS_INFO_LENGTH_MISMATCH
                {
                    Marshal.FreeHGlobal(rootPtr);
                    size = needed == 0 ? size * 2 : needed;
                    rootPtr = Marshal.AllocHGlobal(size);
                    ptr = rootPtr;
                }

                long handleCount = 0;
                handleCount = Marshal.ReadInt32(ptr);
                
                int offset = IntPtr.Size == 8 ? 8 : 4; 
                ptr = (IntPtr)((long)ptr + offset);
                int structSize = Marshal.SizeOf<SYSTEM_HANDLE_ENTRY>(); 

                for (int i = 0; i < handleCount; i++)
                {
                    SYSTEM_HANDLE_ENTRY entry = Marshal.PtrToStructure<SYSTEM_HANDLE_ENTRY>(ptr);
                    handles.Add(entry);
                    ptr = (IntPtr)((long)ptr + structSize);
                }
            }
            catch (Exception)
            {
                 // Ignore errors during enumeration
            }
            finally
            {
                Marshal.FreeHGlobal(rootPtr);
            }
            return handles;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        private static string? GetHandleName(int pid, IntPtr handleVal)
        {
            IntPtr hProcess = OpenProcess(PROCESS_DUP_HANDLE, false, pid);
            if (hProcess == IntPtr.Zero) return null;

            IntPtr hDup = IntPtr.Zero;
            try
            {
                if (!DuplicateHandle(hProcess, handleVal, Process.GetCurrentProcess().Handle, out hDup, 0, false, 0x2))
                {
                    return null;
                }

                int length = 0x2000;
                IntPtr ptr = Marshal.AllocHGlobal(length);
                int returnLength;
                
                try
                {
                    uint status = NtQueryObject(hDup, ObjectNameInformation, ptr, length, out returnLength);
                    
                    if (status == 0) // STATUS_SUCCESS
                    {
                        var info = Marshal.PtrToStructure<UNICODE_STRING>(ptr);
                        if (info.Buffer != IntPtr.Zero && info.Length > 0)
                        {
                            return Marshal.PtrToStringUni(info.Buffer, info.Length / 2);
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
            catch
            {
                // Ignore
            }
            finally
            {
                if (hDup != IntPtr.Zero) CloseHandle(hDup);
                CloseHandle(hProcess);
            }
            return null;
        }

        private static bool CloseRemoteHandle(int pid, IntPtr handleVal)
        {
            IntPtr hProcess = OpenProcess(PROCESS_DUP_HANDLE, false, pid);
            if (hProcess == IntPtr.Zero) return false;

            IntPtr hDup;
            // DUPLICATE_CLOSE_SOURCE = 1
            bool result = DuplicateHandle(hProcess, handleVal, IntPtr.Zero, out hDup, 0, false, DUPLICATE_CLOSE_SOURCE);
            
            CloseHandle(hProcess);
            return result;
        }
    }
}
