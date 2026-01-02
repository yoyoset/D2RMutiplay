namespace D2RMultiplay.Core.Interfaces
{
    public interface IIsolationEngine
    {
        /// <summary>
        /// Executes the "Scorched Earth" strategy.
        /// Deletes/Resets Battle.net's public product configuration to prevent directory linking.
        /// </summary>
        void CleanBattleNetConfig();

        /// <summary>
        /// Scans for running D2R processes and closes the "DiabloII Check For Other Instances" mutex.
        /// </summary>
        /// <returns>Number of handles closed.</returns>
        /// <summary>
        /// Scans for running D2R processes and closes the "DiabloII Check For Other Instances" mutex.
        /// </summary>
        /// <returns>Number of handles closed.</returns>
        int KillGameMutexes();

        /// <summary>
        /// Kills Battle.net and Agent processes to ensure a clean slate for config modification.
        /// </summary>
        void KillBattleNetProcesses();

        /// <summary>
        /// Creates a Directory Junction (Symlink) to the game client.
        /// Useful for creating "fake" paths to deceive Battle.net's folder check.
        /// </summary>
        /// <param name="sourcePath">Real Game Path (e.g. D:\D2R)</param>
        /// <param name="targetPath">Target Link Path (e.g. D:\D2R_Link1)</param>
        void CreateGameJunction(string sourcePath, string targetPath);

        /// <summary>
        /// Backups the current Battle.net config (product.db) to a unique ID slot.
        /// </summary>
        void BackupBattleNetConfig(string id);

        /// <summary>
        /// Restores a backed-up Battle.net config (product.db) from an ID slot.
        /// </summary>
        /// <returns>True if restoration was successful, False if backup not found.</returns>
        bool RestoreBattleNetConfig(string id);
    }
}
