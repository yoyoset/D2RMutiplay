import React, { useState, useRef, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Globe, Check, ChevronDown } from 'lucide-react';
import { cn } from '../../lib/utils';

const LANGUAGES = [
    { code: 'zh-CN', label: '简体中文' },
    { code: 'en', label: 'English' },
    { code: 'zh-TW', label: '繁體中文' },
    { code: 'ja', label: '日本語' },
    { code: 'ko', label: '한국어' }
];

export const LanguageSelector: React.FC = () => {
    const { i18n } = useTranslation();
    const [isOpen, setIsOpen] = useState(false);
    const containerRef = useRef<HTMLDivElement>(null);

    const currentLanguage = LANGUAGES.find(l => l.code === i18n.language) || LANGUAGES[0];

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleSelect = (code: string) => {
        i18n.changeLanguage(code);
        setIsOpen(false);
    };

    return (
        <div className="relative" ref={containerRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className={cn(
                    "flex items-center gap-2 px-3 py-1.5 rounded-lg transition-all duration-300",
                    "bg-zinc-900/40 border border-white/5 hover:border-primary/30 hover:bg-zinc-800/60",
                    isOpen ? "border-primary/40 bg-zinc-800/80 ring-1 ring-primary/20" : ""
                )}
            >
                <Globe size={14} className={cn("transition-colors", isOpen ? "text-primary" : "text-zinc-500")} />
                <span className="text-[11px] font-black uppercase tracking-tight text-zinc-300">
                    {currentLanguage.label}
                </span>
                <ChevronDown size={12} className={cn("text-zinc-600 transition-transform duration-300", isOpen ? "rotate-180 text-primary" : "")} />
            </button>

            {isOpen && (
                <div className="absolute right-0 mt-2 w-40 overflow-hidden rounded-xl border border-white/10 bg-zinc-950/90 backdrop-blur-xl shadow-2xl animate-in fade-in zoom-in-95 duration-200 z-[100]">
                    <div className="py-1">
                        {LANGUAGES.map((lang) => (
                            <button
                                key={lang.code}
                                onClick={() => handleSelect(lang.code)}
                                className={cn(
                                    "w-full flex items-center justify-between px-4 py-2.5 text-[11px] font-bold transition-all",
                                    i18n.language === lang.code
                                        ? "text-primary bg-primary/5"
                                        : "text-zinc-400 hover:text-white hover:bg-white/5"
                                )}
                            >
                                <span>{lang.label}</span>
                                {i18n.language === lang.code && (
                                    <Check size={12} className="text-primary" />
                                )}
                            </button>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};
