using System;
using System.Runtime.InteropServices;

namespace D2RMultiplay.Modules.ModuleA_AccountManager
{
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROFILEINFO
        {
            public int dwSize;
            public int dwFlags;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpUserName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpProfilePath;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDefaultPath;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpServerName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpPolicyPath;
            public IntPtr hProfile;
        }

        public const int LOGON_WITH_PROFILE = 0x00000001;
        public const int LOGON_NETCREDENTIALS_ONLY = 0x00000002;
        public const int CREATE_NEW_CONSOLE = 0x00000010;
        public const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessWithLogonW(
            string lpUsername,
            string? lpDomain,
            string lpPassword,
            int dwLogonFlags,
            string? lpApplicationName,
            string lpCommandLine,
            int dwCreationFlags,
            IntPtr lpEnvironment,
            string? lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LoadUserProfile(IntPtr hToken, ref PROFILEINFO lpProfileInfo);
        
        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool UnloadUserProfile(IntPtr hToken, IntPtr hProfile);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        
        // Logon Types
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        // NetUserEnum
        [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int NetUserEnum(
            string? servername,
            int level,
            int filter,
            out IntPtr bufptr,
            int prefmaxlen,
            out int entriesread,
            out int totalentries,
            ref int resume_handle);

        [DllImport("netapi32.dll")]
        public static extern int NetApiBufferFree(IntPtr Buffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_INFO_0
        {
            public string usri0_name;
        }

        public const int FILTER_NORMAL_ACCOUNT = 0x0002;
    }
}
