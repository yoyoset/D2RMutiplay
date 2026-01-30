import { create } from 'zustand';
import { AccountStatus, getAccountsProcessStatus } from '../lib/api';

interface StatusState {
    statuses: Record<string, AccountStatus>;
    isPolling: boolean;
    updateStatuses: (usernames: string[]) => Promise<void>;
    startPolling: (usernames: string[]) => () => void;
}

export const useStatusStore = create<StatusState>((set, get) => ({
    statuses: {},
    isPolling: false,
    updateStatuses: async (usernames: string[]) => {
        if (usernames.length === 0) return;
        try {
            const newStatuses = await getAccountsProcessStatus(usernames);

            // Deep compare before setting state to prevent unnecessary re-renders
            const prev = get().statuses;
            const keys = Object.keys(newStatuses);

            const isSame = keys.length === Object.keys(prev).length && keys.every(k => {
                const p = prev[k];
                const s = newStatuses[k];
                return p && s &&
                    p.exists === s.exists &&
                    p.bnet_active === s.bnet_active &&
                    p.d2r_active === s.d2r_active;
            });

            if (!isSame) {
                set({ statuses: newStatuses });
            }
        } catch (e) {
            console.error("Store polling error:", e);
        }
    },
    startPolling: (usernames: string[]) => {
        const interval = setInterval(() => {
            get().updateStatuses(usernames);
        }, 10000); // 10 seconds to reduce frequency

        // Initial fetch
        get().updateStatuses(usernames);

        return () => clearInterval(interval);
    }
}));
