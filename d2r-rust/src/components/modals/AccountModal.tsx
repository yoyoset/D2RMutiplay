import { useState, useEffect } from "react";
import { Button } from "../ui/Button";
import { Account, AppConfig, saveConfig, getWindowsUsers, createWindowsUser, getWhoami } from "../../lib/api";
import { useTranslation } from "react-i18next";
import { UserRound, Sparkles, AlertCircle, ChevronLeft, Check } from "lucide-react";
import { cn } from "../../lib/utils";
import { Modal, ModalContent, ModalHeader, ModalBody, ModalFooter } from '../ui/Modal';

import { invoke } from "@tauri-apps/api/core";
import { CLASS_AVATARS, ClassAvatar } from "../ui/Avatars";

interface AccountModalProps {
    isOpen: boolean;
    onClose: () => void;
    config: AppConfig;
    onSave: (newConfig: AppConfig) => void;
    editingAccount?: Account;
}

// Remote duplicate definition of CLASS_AVATARS and ClassAvatar

export function AccountModal({ isOpen, onClose, config, onSave, editingAccount }: AccountModalProps) {
    const { t } = useTranslation();

    // Form State
    const [winUser, setWinUser] = useState("");
    const [winPass, setWinPass] = useState("");
    const [bnetAccount, setBnetAccount] = useState("");
    const [note, setNote] = useState("");
    const [avatar, setAvatar] = useState<string | undefined>(undefined);

    // UI state
    const [isManualInput, setIsManualInput] = useState(false);
    const [osUsers, setOsUsers] = useState<string[]>([]);
    const [isCreatingNew, setIsCreatingNew] = useState(false);
    const [isSaving, setIsSaving] = useState(false);
    const [isScanning, setIsScanning] = useState(false);
    const [hasScannedDeep, setHasScannedDeep] = useState(false);
    const [currentUser, setCurrentUser] = useState("");

    // Modals & Feedback
    const [feedback, setFeedback] = useState<{ type: 'success' | 'error', message: string, showSwitch?: boolean } | null>(null);
    const [showSuccessView, setShowSuccessView] = useState(false);

    const handleDiscovery = async (deep: boolean = false) => {
        setIsScanning(true);
        try {
            const [users, whoami] = await Promise.all([
                getWindowsUsers(deep),
                getWhoami()
            ]);
            setCurrentUser(whoami);
            if (deep) setHasScannedDeep(true);

            // Deduplicate case-insensitively
            const combined = [whoami, ...users];
            const unique = combined.reduce((acc: string[], curr) => {
                if (!acc.some(u => u.toLowerCase() === curr.toLowerCase()) && curr.trim() !== "") {
                    acc.push(curr);
                }
                return acc;
            }, []);

            setOsUsers(unique);
        } catch (e) {
            console.error(e);
        } finally {
            setIsScanning(false);
        }
    };

    useEffect(() => {
        if (isOpen && osUsers.length === 0) {
            handleDiscovery(false);
        }
    }, [isOpen]);

    useEffect(() => {
        if (isOpen) {
            setFeedback(null);
            setShowSuccessView(false);
            if (editingAccount) {
                setWinUser(editingAccount.win_user);
                setWinPass(editingAccount.win_pass || "");
                setBnetAccount(editingAccount.bnet_account || "");
                setNote(editingAccount.note || "");
                setAvatar(editingAccount.avatar);
                setIsCreatingNew(false);
                setIsManualInput(editingAccount.win_user.includes("\\"));
            } else {
                setWinUser("");
                setWinPass("");
                setBnetAccount("");
                setNote("");
                setAvatar(undefined);
                setIsCreatingNew(false);
                setIsManualInput(false);
            }
        }
    }, [editingAccount, isOpen]);


    const handleSave = async () => {
        if (!winUser.trim()) return;
        setIsSaving(true);
        setFeedback(null);
        try {
            if (isCreatingNew) {
                await createWindowsUser(winUser, winPass);
                // After creating user, we continue to save it to our config (Auto-Bind)
            }

            const newId = editingAccount ? editingAccount.id : crypto.randomUUID();
            const newAccount: Account = {
                id: newId,
                win_user: winUser,
                win_pass: winPass || undefined,
                bnet_account: bnetAccount,
                note: note || undefined,
                avatar: avatar,
            };

            let newAccounts = [...config.accounts];
            if (editingAccount) {
                newAccounts = newAccounts.map(a => a.id === editingAccount.id ? newAccount : a);
            } else {
                newAccounts.push(newAccount);
            }

            const newConfig = { ...config, accounts: newAccounts };
            await saveConfig(newConfig);
            onSave(newConfig);

            if (isCreatingNew) {
                setFeedback({
                    type: 'success',
                    message: t('account_created_success') || "Account Created!",
                    showSwitch: true
                });
                setShowSuccessView(true);
                setIsCreatingNew(false);
            } else {
                onClose();
            }
        } catch (e) {
            console.error(e);
            const errStr = String(e);
            await invoke('log_frontend_error', { message: errStr });
            setFeedback({ type: 'error', message: errStr });
        } finally {
            setIsSaving(false);
        }
    };

    if (!isOpen) return null;

    const isHost = winUser.toLowerCase() === currentUser.toLowerCase();

    return (
        <Modal isOpen={isOpen} onClose={onClose}>
            <ModalContent>
                <ModalHeader onClose={onClose}>
                    <UserRound size={16} />
                    {showSuccessView ? t('success') : (editingAccount ? t('edit_account') : t('add_account'))}
                </ModalHeader>

                <ModalBody>
                    {showSuccessView ? (
                        <div className="flex flex-col items-center justify-center py-10 space-y-6 animate-in fade-in zoom-in-95 duration-300">
                            <div className="w-16 h-16 rounded-full bg-emerald-500/10 border border-emerald-500/20 flex items-center justify-center text-emerald-500 shadow-lg shadow-emerald-500/10">
                                <Check size={32} strokeWidth={3} />
                            </div>
                            <div className="text-center space-y-2">
                                <h3 className="text-xl font-bold text-whiteTracking-tight">
                                    {t('account_created_success')}
                                </h3>
                                <p className="text-sm text-zinc-500 max-w-[280px] leading-relaxed">
                                    {t('account_created_switch_prompt')}
                                </p>
                            </div>
                            <div className="flex flex-col w-full gap-3 pt-4">
                                <Button
                                    size="md"
                                    variant="solid"
                                    className="bg-emerald-600 hover:bg-emerald-500 text-white font-bold h-11"
                                    onClick={() => invoke('trigger_switch_user')}
                                >
                                    {t('trigger_switch_user') || "立即切换用户 (Switch)"}
                                </Button>
                                <Button
                                    size="md"
                                    variant="ghost"
                                    className="text-zinc-500 hover:text-white"
                                    onClick={onClose}
                                >
                                    {t('close') || "稍后"}
                                </Button>
                            </div>
                        </div>
                    ) : (
                        <div className="space-y-6">
                            {/* Windows User Section */}
                            <div className="space-y-1.5 px-0.5">
                                <div className="flex justify-between items-center">
                                    <label className="text-sm font-medium text-zinc-300">
                                        {t('win_user_binding')}
                                    </label>
                                    {!editingAccount && (
                                        <button
                                            onClick={() => { setIsCreatingNew(!isCreatingNew); setIsManualInput(true); }}
                                            className={cn(
                                                "text-[11px] px-2.5 py-1.5 rounded-lg transition-all shadow-sm",
                                                isCreatingNew
                                                    ? "text-zinc-500 hover:text-white underline underline-offset-4 decoration-zinc-800"
                                                    : "bg-emerald-600 inline-flex items-center gap-1.5 text-white hover:bg-emerald-500 font-bold"
                                            )}
                                        >
                                            {isCreatingNew ? t('use_existing_user') : (
                                                <>
                                                    <span className="text-[10px]">✨</span>
                                                    {t('create_new_win_user')}
                                                </>
                                            )}
                                        </button>
                                    )}
                                </div>
                                <div className="flex items-center gap-4 h-4">
                                    {!isManualInput && !isCreatingNew ? (
                                        <button
                                            onClick={() => setIsManualInput(true)}
                                            className="text-[10px] text-zinc-500 hover:text-zinc-300 transition-colors underline underline-offset-4 decoration-zinc-800"
                                        >
                                            {t('domain_work_account_link') || "Domain login"}
                                        </button>
                                    ) : (
                                        !editingAccount && (
                                            <button
                                                onClick={() => { setIsManualInput(false); setIsCreatingNew(false); }}
                                                className="text-[10px] text-zinc-500 hover:text-zinc-300 transition-colors flex items-center gap-1"
                                            >
                                                <ChevronLeft size={10} /> {t('back_to_list')}
                                            </button>
                                        )
                                    )}
                                </div>
                            </div>

                            {isManualInput ? (
                                <div className="space-y-4 animate-in slide-in-from-top-1">
                                    <input
                                        type="text"
                                        value={winUser}
                                        onChange={(e) => setWinUser(e.target.value)}
                                        className="w-full bg-black/50 border border-zinc-700/50 rounded-lg px-3 py-2 text-sm text-gray-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary/20 transition-all font-medium"
                                        placeholder={t('win_username')}
                                    />
                                    <input
                                        type="password"
                                        value={winPass}
                                        onChange={(e) => setWinPass(e.target.value)}
                                        className="w-full bg-black/50 border border-zinc-700/50 rounded-lg px-3 py-2 text-sm text-gray-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary/20 transition-all font-medium"
                                        placeholder={t('win_password')}
                                    />
                                </div>
                            ) : (
                                <div className="space-y-3">
                                    <div className="relative group flex gap-3">
                                        <div className="relative flex-1">
                                            <div className="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-500 pointer-events-none">
                                                <UserRound size={16} />
                                            </div>
                                            <select
                                                value={winUser}
                                                onChange={(e) => setWinUser(e.target.value)}
                                                className="w-full h-10 bg-black/50 border border-zinc-700/50 rounded-lg pl-10 pr-3 text-sm text-gray-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary/20 appearance-none cursor-pointer transition-all"
                                            >
                                                <option value="" disabled>{t('select_win_user')}</option>
                                                {osUsers.map(u => (
                                                    <option key={u} value={u}>
                                                        {u} {u.toLowerCase() === currentUser.toLowerCase() ? `(${t('host_current')})` : ''}
                                                    </option>
                                                ))}
                                            </select>
                                        </div>
                                        {!hasScannedDeep && (
                                            <Button
                                                size="sm"
                                                variant="ghost"
                                                onClick={() => handleDiscovery(true)}
                                                isLoading={isScanning}
                                                className="h-10 px-4 border border-zinc-700/50 bg-black/20 hover:bg-black/40 text-xs"
                                            >
                                                <Sparkles size={12} className="mr-1.5 text-zinc-400" />
                                                {t('scan_users')}
                                            </Button>
                                        )}
                                    </div>

                                    {winUser && (
                                        <div className="animate-in slide-in-from-top-1">
                                            <input
                                                type="password"
                                                value={winPass}
                                                onChange={(e) => setWinPass(e.target.value)}
                                                className="w-full bg-black/50 border border-zinc-700/50 rounded-lg px-3 py-2 text-sm text-gray-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary/20 transition-all font-medium"
                                                placeholder={t('win_password')}
                                            />
                                        </div>
                                    )}
                                </div>
                            )}

                            {winUser && (
                                <div className="flex items-start gap-1.5 px-0.5 text-[10px] text-amber-500/80 leading-tight">
                                    <AlertCircle size={12} className="mt-0.5 shrink-0 opacity-70" />
                                    <span>
                                        {t('pin_warning') || "Real Login Password required. NO PIN/Hello."}
                                        {isHost && (
                                            <span className="block mt-0.5 text-emerald-500 font-bold">
                                                {t('host_no_pass_hint') || "Current User [Host] doesn't need password."}
                                            </span>
                                        )}
                                    </span>
                                </div>
                            )}

                            {feedback && !showSuccessView && (
                                <div className={cn(
                                    "p-3 rounded-lg border animate-in fade-in slide-in-from-bottom-2 mt-4",
                                    feedback.type === 'success' ? "bg-emerald-500/10 border-emerald-500/20 text-emerald-400" : "bg-rose-500/10 border-rose-500/20 text-rose-400"
                                )}>
                                    <div className="flex items-center gap-2 mb-1">
                                        {feedback.type === 'success' ? <Check size={14} /> : <AlertCircle size={14} />}
                                        <span className="text-sm font-bold">{feedback.type === 'success' ? t('success') : t('error')}</span>
                                    </div>
                                    <div className="text-xs opacity-90 mb-2">{feedback.message}</div>
                                </div>
                            )}

                            {/* Avatar & Info Grouping */}
                            <div className="space-y-4 pt-2 border-t border-white/5">
                                {/* Class Avatar Selection */}
                                <div className="space-y-2.5">
                                    <label className="text-sm font-medium text-zinc-400">
                                        {t('avatar')}
                                    </label>
                                    <div className="flex flex-wrap gap-2.5">
                                        {Object.keys(CLASS_AVATARS).map(cls => (
                                            <button
                                                key={cls}
                                                type="button"
                                                onClick={() => setAvatar(cls)}
                                                className={cn(
                                                    "relative transition-all duration-200 outline-none rounded-lg",
                                                    avatar === cls ? "ring-2 ring-primary ring-offset-2 ring-offset-zinc-900 scale-105 z-10" : "hover:scale-105 opacity-50 hover:opacity-100"
                                                )}
                                            >
                                                <ClassAvatar cls={cls} size="sm" />
                                            </button>
                                        ))}
                                        <label className={cn(
                                            "w-7 h-7 rounded-lg border flex items-center justify-center cursor-pointer transition-all bg-black/40 hover:bg-black/60",
                                            avatar?.startsWith('data:') ? "border-primary text-primary" : "border-white/10 text-zinc-600 hover:text-zinc-300"
                                        )}>
                                            <input type="file" className="hidden" accept="image/*" onChange={(e) => {
                                                const file = e.target.files?.[0];
                                                if (file) {
                                                    const reader = new FileReader();
                                                    reader.onloadend = () => setAvatar(reader.result as string);
                                                    reader.readAsDataURL(file);
                                                }
                                            }} />
                                            <Sparkles size={12} />
                                        </label>
                                    </div>
                                </div>

                                {/* Bnet Account & Note (Vertical Stack) */}
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div className="space-y-1.5">
                                        <label className="text-sm font-medium text-zinc-400">
                                            {t('bnet_account')}
                                        </label>
                                        <input
                                            type="text"
                                            value={bnetAccount}
                                            onChange={(e) => setBnetAccount(e.target.value)}
                                            className="w-full bg-black/50 border border-zinc-700/50 rounded-lg px-3 py-2 text-sm text-gray-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary/20 font-medium transition-all"
                                            placeholder="Bnet ID..."
                                        />
                                    </div>
                                    <div className="space-y-1.5">
                                        <label className="text-sm font-medium text-zinc-400">
                                            {t('note')}
                                        </label>
                                        <input
                                            type="text"
                                            value={note}
                                            onChange={(e) => setNote(e.target.value)}
                                            className="w-full bg-black/50 border border-zinc-700/50 rounded-lg px-3 py-2 text-sm text-gray-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary/20 transition-all font-medium"
                                            placeholder={t('note_placeholder') || "Note..."}
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                    )}
                </ModalBody>

                {!showSuccessView && (
                    <ModalFooter>
                        <Button variant="ghost" size="sm" onClick={onClose} disabled={isSaving}>
                            {t('cancel')}
                        </Button>
                        <Button onClick={handleSave} size="sm" isLoading={isSaving} variant="solid" className="px-6 font-bold">
                            {t('save')}
                        </Button>
                    </ModalFooter>
                )}
            </ModalContent>
        </Modal>
    );
};
