import { invoke as tauriInvoke } from "@tauri-apps/api/core";

/**
 * Global invoke wrapper for Tauri commands.
 */
export const invoke = tauriInvoke;

/**
 * Represents a Windows user account with game-specific metadata.
 */
export interface Account {
    /** Unique identifier for the account (UUID) */
    id: string;
    /** The Windows OS username used for isolation */
    win_user: string;
    /** Optional OS password for auto-login/creation */
    win_pass?: string;
    /** Battle.net account identifier (e.g. User#1234) */
    bnet_account: string;
    /** Avatar identifier or base64 data URI */
    avatar?: string;
    /** Personal note or character description */
    note?: string;
}

/**
 * Real-time process and system status for a specific user identity.
 */
export interface AccountStatus {
    /** Whether the Battle.net client is currently running for this user */
    bnet_active: boolean;
    /** Whether the Diablo II: Resurrected process is active */
    d2r_active: boolean;
    /** Whether the Windows user account physically exists on the system */
    exists: boolean;
}

/**
 * Universal application configuration structure.
 */
export interface AppConfig {
    /** List of managed accounts */
    accounts: Account[];
    /** ID of the last successfully launched account */
    last_active_account?: string;
    /** Hex color code for UI customization */
    theme_color?: string;
    /** Whether the window should hide to the tray on close */
    close_to_tray?: boolean;
    /** Preferred UI language (e.g. 'zh-CN', 'en') */
    language?: string;
    /** Whether debug logs are captured to the local file */
    log_enabled?: boolean;
}

/**
 * Retrieves a list of local Windows usernames.
 * @param deepScan If true, performs a more thorough system enum (may be slower)
 */
export async function getWindowsUsers(deepScan: boolean = false): Promise<string[]> {
    return await invoke('get_windows_users', { deepScan });
}

/**
 * Returns the username of the currently active Windows session.
 */
export async function getWhoami(): Promise<string> {
    return await invoke('get_whoami');
}

/**
 * Programmatically creates a new Windows user account for isolation.
 * @param username Target username
 * @param password Password for the new account
 */
export async function createWindowsUser(username: string, password: string): Promise<string> {
    return await invoke('create_windows_user', { username, password });
}

/**
 * Terminates dangling Mutex handles that prevent multi-boxing.
 * @returns Status message describing the number of handles cleared
 */
export async function killMutexes(): Promise<string> {
    try {
        return await invoke('kill_mutexes');
    } catch (e) {
        throw new Error(String(e));
    }
}

/**
 * Orchestrates the full game launch sequence for a specific account.
 * @param account The account identity to launch
 * @param gamePath Path to the D2R.exe binary
 */
export async function launchGame(account: Account, gamePath: string): Promise<string> {
    return await invoke("launch_game", {
        account,
        gamePath
    });
}

/**
 * Loads the application configuration from persistent storage.
 */
export async function getConfig(): Promise<AppConfig> {
    return await invoke("get_config");
}

/**
 * Saves the current app configuration to persistent storage.
 */
export async function saveConfig(config: AppConfig): Promise<void> {
    await invoke("save_config", { config });
}

/**
 * Batch retrieves the process status for multiple usernames.
 */
export async function getAccountsProcessStatus(usernames: string[]): Promise<Record<string, AccountStatus>> {
    return await invoke("get_accounts_process_status", { usernames });
}

/**
 * Truncates the local debug.log file.
 */
export async function clearLog(): Promise<void> {
    await invoke("clear_log_file");
}
