import { useState, useEffect, useCallback } from "react";
import { getConfig, launchGame, saveConfig, AppConfig, Account, getWindowsUsers, getWhoami } from "./lib/api";
import { Settings, LayoutGrid, Users, Wrench, Heart, ShieldAlert, Check, X, ShieldCheck } from "lucide-react";
import { SettingsModal } from "./components/modals/SettingsModal";
import { AccountModal } from "./components/modals/AccountModal";
import { DonateModal } from "./components/modals/DonateModal";
import { useTranslation } from "react-i18next";
import Dashboard from "./components/views/Dashboard";
import AccountManager from "./components/views/AccountManager";
import ManualTools from "./components/views/ManualTools";
import { useStatusStore } from "./store/useStatusStore";
import { cn } from "./lib/utils";
import { getCurrentWindow } from "@tauri-apps/api/window";
import { Button } from "./components/ui/Button";
import { LanguageSelector } from "./components/ui/LanguageSelector";

type View = 'dashboard' | 'accounts' | 'manual';

function App() {
  const { t } = useTranslation();
  const [config, setConfig] = useState<AppConfig>({ accounts: [] });
  const [isLoaded, setIsLoaded] = useState(false);
  const [missingAccounts, setMissingAccounts] = useState<Account[]>([]);
  const [currentUser, setCurrentUser] = useState<string>("");

  const loadConfig = async () => {
    try {
      const cfg = await getConfig();
      setConfig(prev => {
        if (JSON.stringify(prev) === JSON.stringify(cfg)) return prev;
        return cfg;
      });
      const me = await getWhoami();
      setCurrentUser(me);
      return cfg;
    } catch (e) {
      console.error(e);
      return null;
    }
  };

  const verifyAccounts = async (accounts: Account[]) => {
    if (accounts.length === 0) return;
    try {
      const osUsers = await getWindowsUsers(false);
      const missing = accounts.filter(acc => {
        const parts = acc.win_user.split('\\');
        const normalized = parts[parts.length - 1].toLowerCase();
        return !osUsers.some(u => u.toLowerCase() === normalized);
      });
      if (missing.length > 0) {
        setMissingAccounts(missing);
      }
    } catch (e) {
      console.error("Verification failed", e);
    }
  };

  const { startPolling } = useStatusStore();

  useEffect(() => {
    const init = async () => {
      const cfg = await loadConfig();
      if (cfg) {
        await verifyAccounts(cfg.accounts);
      }
      setIsLoaded(true);
      // Wait a tiny bit for LCP and theme to settle
      setTimeout(() => {
        getCurrentWindow().show().catch(console.error);
        getCurrentWindow().setFocus().catch(console.error);
      }, 50);
    };
    init();
  }, []);

  useEffect(() => {
    if (config.accounts.length === 0) return;
    const usernames = config.accounts.map(a => a.win_user);
    const stop = startPolling(usernames);
    return () => stop();
  }, [config.accounts, startPolling]);

  const [currentView, setCurrentView] = useState<View>('dashboard');

  // Dashboard State
  const [selectedAccountId, setSelectedAccountId] = useState<string | null>(null);
  const [isLaunching, setIsLaunching] = useState(false);

  // Modals
  const [isSettingsOpen, setIsSettingsOpen] = useState(false);
  const [isAccountModalOpen, setIsAccountModalOpen] = useState(false);
  const [isDonateOpen, setIsDonateOpen] = useState(false);
  const [editingAccount, setEditingAccount] = useState<Account | undefined>(undefined);

  // Apply Theme Color
  useEffect(() => {
    if (config.theme_color) {
      const hex = config.theme_color.replace('#', '');
      const r = parseInt(hex.substring(0, 2), 16);
      const g = parseInt(hex.substring(2, 4), 16);
      const b = parseInt(hex.substring(4, 6), 16);
      document.documentElement.style.setProperty('--color-primary', `${r} ${g} ${b}`);
      document.documentElement.style.setProperty('--primary-rgb', `${r}, ${g}, ${b}`);
    }
  }, [config.theme_color]);


  const handleLaunch = useCallback(async () => {
    if (!selectedAccountId) return;
    const account = config.accounts.find(a => a.id === selectedAccountId);
    if (!account) return;

    try {
      setIsLaunching(true);
      await launchGame(account, "");
    } catch (e) {
      alert(`Launch failed: ${e}`);
    } finally {
      setIsLaunching(false);
    }
  }, [selectedAccountId, config.accounts]);

  const handleAddAccount = useCallback(() => {
    setEditingAccount(undefined);
    setIsAccountModalOpen(true);
  }, []);

  const handleEditAccount = useCallback((acc: Account) => {
    setEditingAccount(acc);
    setIsAccountModalOpen(true);
  }, []);

  const handleDeleteAccount = useCallback(async (id: string) => {
    const newAccounts = config.accounts.filter(a => a.id !== id);
    const newConfig = { ...config, accounts: newAccounts };
    setConfig(newConfig);
    try {
      await saveConfig(newConfig);
    } catch (e) { console.error(e); }
  }, [config]);

  const handleReorder = useCallback(async (newAccounts: Account[]) => {
    const newConfig = { ...config, accounts: newAccounts };
    setConfig(newConfig);
    try {
      await saveConfig(newConfig);
    } catch (e) {
      console.error("Failed to save order", e);
    }
  }, [config]);

  const syncMissingAccounts = async () => {
    const missingIds = new Set(missingAccounts.map(a => a.id));
    const newAccounts = config.accounts.filter(a => !missingIds.has(a.id));
    const newConfig = { ...config, accounts: newAccounts };
    setConfig(newConfig);
    await saveConfig(newConfig);
    setMissingAccounts([]);
  };

  if (!isLoaded) return null;

  return (
    <div className="h-screen overflow-hidden bg-transparent text-gray-200 flex flex-col font-sans selection:bg-gold/30 selection:text-black">
      <SettingsModal
        isOpen={isSettingsOpen}
        onClose={() => setIsSettingsOpen(false)}
        config={config}
        onSave={setConfig}
      />
      <AccountModal
        isOpen={isAccountModalOpen}
        onClose={() => setIsAccountModalOpen(false)}
        config={config}
        onSave={setConfig}
        editingAccount={editingAccount}
      />
      <DonateModal
        isOpen={isDonateOpen}
        onClose={() => setIsDonateOpen(false)}
      />

      {/* Startup Conflict Overlay */}
      {missingAccounts.length > 0 && (
        <div className="fixed inset-0 z-[100] flex items-center justify-center p-6 bg-black/60 backdrop-blur-md animate-in fade-in duration-300">
          <div className="bg-zinc-900 border border-white/10 rounded-2xl p-6 max-w-md w-full shadow-2xl scale-in-center overflow-hidden relative">
            <div className="absolute top-0 left-0 w-full h-1 bg-amber-500/30"></div>
            <div className="flex items-center gap-4 mb-3 text-amber-500">
              <ShieldAlert size={28} />
              <h3 className="text-xl font-bold tracking-tight uppercase">{t('user_not_found')}</h3>
            </div>
            <p className="text-zinc-400 text-sm mb-6 leading-relaxed">
              {t('missing_users_desc')}
              <div className="mt-3 flex flex-wrap gap-2">
                {missingAccounts.map(a => (
                  <span key={a.id} className="px-2 py-1 rounded bg-black/40 border border-white/5 text-[10px] text-zinc-500 font-bold uppercase">{a.win_user}</span>
                ))}
              </div>
            </p>
            <div className="flex gap-3 mt-8">
              <Button
                variant="ghost"
                className="flex-1 text-zinc-500 hover:text-white"
                onClick={() => setMissingAccounts([])}
              >
                <X size={14} className="mr-2" />
                {t('ignore')}
              </Button>
              <Button
                variant="solid"
                className="flex-1 bg-amber-600 hover:bg-amber-500 text-white font-bold"
                onClick={syncMissingAccounts}
              >
                <Check size={14} className="mr-2" />
                {t('sync_delete')}
              </Button>
            </div>
          </div>
        </div>
      )}

      <header className="flex-shrink-0 h-16 linear-glass border-b border-white/5 flex items-center justify-between px-6 z-50">
        <div className="flex items-center gap-8">
          <div className="flex items-center gap-3 font-sans">
            <div className="w-8 h-8 rounded-lg bg-primary/10 flex items-center justify-center border border-primary/20 shadow-[0_0_15px_rgba(var(--primary-rgb),0.2)]">
              <LayoutGrid size={18} className="text-primary" />
            </div>
            <div className="flex flex-col">
              <h1 className="text-sm font-bold tracking-tight text-white uppercase leading-tight">D2R Multiplay</h1>
              <span className="text-[9px] text-zinc-500 font-medium tracking-[0.2em] leading-none">COMMANDER v0.1.3</span>
            </div>
          </div>

          <nav className="flex items-center bg-zinc-900/40 p-1 rounded-full border border-white/5">
            <button
              onClick={() => setCurrentView('dashboard')}
              className={cn(
                "flex items-center gap-2 px-4 py-1.5 rounded-full text-xs font-bold transition-all",
                currentView === 'dashboard' ? "bg-primary text-white shadow-lg shadow-primary/20" : "text-zinc-500 hover:text-zinc-200"
              )}
            >
              <LayoutGrid size={13} />
              <span>{t('dashboard')}</span>
            </button>
            <button
              onClick={() => setCurrentView('accounts')}
              className={cn(
                "flex items-center gap-2 px-4 py-1.5 rounded-full text-xs font-bold transition-all",
                currentView === 'accounts' ? "bg-primary text-white shadow-lg shadow-primary/20" : "text-zinc-500 hover:text-zinc-200"
              )}
            >
              <Users size={13} />
              <span>{t('accounts')}</span>
            </button>
            <button
              onClick={() => setCurrentView('manual')}
              className={cn(
                "flex items-center gap-2 px-4 py-1.5 rounded-full text-xs font-bold transition-all",
                currentView === 'manual' ? "bg-primary text-white shadow-lg shadow-primary/20" : "text-zinc-500 hover:text-zinc-200"
              )}
            >
              <Wrench size={13} />
              <span>{t('manual_tools')}</span>
            </button>
          </nav>
        </div>

        <div className="flex items-center gap-4 text-zinc-400">
          {/* Identity Display */}
          <div className="hidden lg:flex items-center gap-3 px-3.5 py-1.5 rounded-full bg-primary/5 border border-primary/10 mr-1 group/id cursor-default">
            <ShieldCheck size={14} className="text-primary/40 group-hover/id:text-primary/80 transition-colors" />
            <div className="flex flex-col">
              <span className="text-[9px] uppercase tracking-tighter text-zinc-600 font-black leading-none">{t('current_admin')}</span>
              <span className="text-[11px] text-primary/70 font-mono font-bold leading-tight tracking-tight">{currentUser}</span>
            </div>
          </div>

          <LanguageSelector />

          <button
            onClick={() => setIsDonateOpen(true)}
            className="group flex items-center gap-2 px-5 py-2 rounded-full bg-primary/10 border border-primary/20 text-primary hover:bg-primary/20 hover:border-primary/40 transition-all font-black shadow-[0_0_20px_rgba(var(--primary-rgb),0.1)] hover:shadow-[0_0_30px_rgba(var(--primary-rgb),0.3)]"
          >
            <Heart size={14} className="text-primary group-hover:scale-125 transition-transform" fill="currentColor" fillOpacity={0.2} />
            <span className="text-[11px] tracking-widest uppercase">{t('donate')}</span>
          </button>

          <button onClick={() => setIsSettingsOpen(true)} className="text-zinc-500 hover:text-zinc-200 transition-colors">
            <Settings size={18} />
          </button>
        </div>
      </header>

      <main className="flex-1 relative overflow-hidden min-h-0">
        {currentView === 'dashboard' && (
          <Dashboard
            accounts={config.accounts}
            selectedAccountId={selectedAccountId}
            onSelectAccount={setSelectedAccountId}
            onLaunch={handleLaunch}
            isLaunching={isLaunching}
            onReorder={handleReorder}
            onEdit={handleEditAccount}
            currentUser={currentUser}
          />
        )}
        {currentView === 'accounts' && (
          <AccountManager
            accounts={config.accounts}
            onAdd={handleAddAccount}
            onEdit={handleEditAccount}
            onDelete={handleDeleteAccount}
          />
        )}
        {currentView === 'manual' && (
          <ManualTools
            accounts={config.accounts}
            selectedAccountId={selectedAccountId}
          />
        )}
      </main>
    </div>
  );
}

export default App;
