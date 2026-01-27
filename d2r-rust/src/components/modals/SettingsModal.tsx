import { useState, useEffect } from "react";
import { Button } from "../ui/Button";
import { AppConfig, saveConfig } from "../../lib/api";
import { useTranslation } from "react-i18next";
import { Check, Palette, AlertCircle, Settings as SettingsIcon } from "lucide-react";
import { cn } from "../../lib/utils";
import { Modal, ModalContent, ModalHeader, ModalBody, ModalFooter } from '../ui/Modal';

interface SettingsModalProps {
    isOpen: boolean;
    onClose: () => void;
    config: AppConfig;
    onSave: (newConfig: AppConfig) => void;
}

const THEMES = [
    { name: 'Linear Blue', color: '#3b82f6' },
    { name: 'Violet', color: '#8b5cf6' },
    { name: 'Emerald', color: '#10b981' },
    { name: 'Amber', color: '#f59e0b' },
    { name: 'Rose', color: '#f43f5e' },
];

export function SettingsModal({ isOpen, onClose, config, onSave }: SettingsModalProps) {
    const { t } = useTranslation();
    const [themeColor, setThemeColor] = useState(config.theme_color || '#3b82f6');
    const [closeToTray, setCloseToTray] = useState(config.close_to_tray ?? true);
    const [isSaving, setIsSaving] = useState(false);

    useEffect(() => {
        setThemeColor(config.theme_color || '#3b82f6');
        setCloseToTray(config.close_to_tray ?? true);
    }, [config]);

    // Apply Live Theme Preview
    useEffect(() => {
        try {
            const hex = themeColor.replace('#', '');
            const r = parseInt(hex.substring(0, 2), 16);
            const g = parseInt(hex.substring(2, 4), 16);
            const b = parseInt(hex.substring(4, 6), 16);
            document.documentElement.style.setProperty('--color-primary', `${r} ${g} ${b}`);
        } catch (e) { }
    }, [themeColor]);

    const handleCancel = () => {
        onClose();
    };

    const handleSave = async () => {
        setIsSaving(true);
        try {
            const newConfig = {
                ...config,
                theme_color: themeColor,
                close_to_tray: closeToTray,
            };
            await saveConfig(newConfig);
            onSave(newConfig);
            onClose();
        } catch (e) {
            console.error(e);
            alert(`Failed to save: ${e}`);
        } finally {
            setIsSaving(false);
        }
    };

    if (!isOpen) return null;

    return (
        <Modal isOpen={isOpen} onClose={onClose}>
            <ModalContent>
                <ModalHeader onClose={onClose}>
                    <SettingsIcon size={18} className="text-primary" />
                    {t('settings_and_about') || "设置与关于"}
                </ModalHeader>

                <ModalBody>
                    <div className="space-y-8">
                        {/* Appearance */}
                        <div className="space-y-4">
                            <div className="flex items-center gap-2 mb-2">
                                <Palette size={14} className="text-zinc-500" />
                                <label className="text-xs font-bold text-zinc-500 uppercase tracking-widest text-[10px]">
                                    {t('appearance')}
                                </label>
                            </div>
                            <div className="flex flex-wrap gap-3">
                                {THEMES.map((theme) => (
                                    <button
                                        key={theme.color}
                                        onClick={() => setThemeColor(theme.color)}
                                        className={cn(
                                            "w-10 h-10 rounded-full flex items-center justify-center transition-all hover:scale-110",
                                            themeColor === theme.color ? "ring-2 ring-white ring-offset-2 ring-offset-zinc-950 shadow-xl" : "opacity-80 hover:opacity-100"
                                        )}
                                        style={{ backgroundColor: theme.color }}
                                    >
                                        {themeColor === theme.color && <Check size={16} className="text-white drop-shadow-md" />}
                                    </button>
                                ))}
                            </div>
                        </div>

                        <hr className="border-white/5" />

                        {/* System Tray Setting */}
                        <div className="flex items-center justify-between p-4 bg-white/5 rounded-xl border border-white/5 group hover:border-white/10 transition-all cursor-pointer"
                            onClick={() => setCloseToTray(!closeToTray)}>
                            <div className="space-y-0.5">
                                <div className="text-sm font-bold text-zinc-200">{t('setting_close_to_tray')}</div>
                                <div className="text-[11px] text-zinc-500 pr-4 leading-tight opacity-80">{t('setting_close_to_tray_desc')}</div>
                            </div>
                            <div className={cn(
                                "w-10 h-5 rounded-full relative transition-colors duration-200 shrink-0",
                                closeToTray ? "bg-primary" : "bg-zinc-800"
                            )}>
                                <div className={cn(
                                    "absolute top-1 w-3 h-3 bg-white rounded-full transition-all duration-200 shadow-sm",
                                    closeToTray ? "left-6" : "left-1"
                                )} />
                            </div>
                        </div>

                        <hr className="border-white/5" />

                        {/* About Section */}
                        <div className="space-y-4 px-1 pb-4">
                            <div className="flex items-center gap-2">
                                <AlertCircle size={14} className="text-zinc-500" />
                                <label className="text-xs font-bold text-zinc-500 uppercase tracking-widest text-[10px]">
                                    {t('about') || "关于项目"}
                                </label>
                            </div>
                            <div className="p-4 rounded-xl bg-zinc-900/50 border border-white/5 space-y-3">
                                <div className="flex justify-between items-center text-sm">
                                    <span className="text-zinc-500">{t('app_name') || "软件名称"}</span>
                                    <span className="text-zinc-300 font-bold">D2R Multiplay</span>
                                </div>
                                <div className="flex justify-between items-center text-sm">
                                    <span className="text-zinc-500">{t('version') || "当前版本"}</span>
                                    <span className="text-emerald-500 font-mono">v0.1.1</span>
                                </div>
                                <div className="flex justify-between items-center text-sm">
                                    <span className="text-zinc-500">{t('official_website') || "官方网站"}</span>
                                    <button
                                        onClick={async () => {
                                            try {
                                                const { openUrl } = await import("@tauri-apps/plugin-opener");
                                                await openUrl('https://squareuncle.com');
                                            } catch (err) {
                                                console.error("Failed to open URL:", err);
                                                window.open('https://squareuncle.com', '_blank');
                                            }
                                        }}
                                        className="text-primary hover:underline font-medium text-sm"
                                    >
                                        squareuncle.com
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </ModalBody>

                <ModalFooter>
                    <Button variant="ghost" className="text-zinc-500 px-6" onClick={handleCancel} disabled={isSaving}>
                        {t('cancel')}
                    </Button>
                    <Button variant="solid" className="px-8 bg-primary font-bold shadow-lg shadow-primary/20" onClick={handleSave} isLoading={isSaving}>
                        {t('save')}
                    </Button>
                </ModalFooter>
            </ModalContent>
        </Modal>
    );
};
