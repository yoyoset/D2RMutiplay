namespace D2RMultiplay.Core.Interfaces
{
    public interface IWindowsUserManager
    {
        /// <summary>
        /// Ensures a local Windows user exists. If not, creates it.
        /// </summary>
        /// <param name="username">The username (without domain).</param>
        /// <param name="password">The password.</param>
        /// <summary>
        /// Checks if a local Windows user exists.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>True if exists.</returns>
        bool UserExists(string username);

        /// <summary>
        /// Ensures a local Windows user exists. If not, creates it.
        /// </summary>
        /// <param name="username">The username (without domain).</param>
        /// <param name="password">The password.</param>
        void EnsureUserExists(string username, string password);

        /// <summary>
        /// Loads the user profile (AppData, Registry) for the specified user.
        /// This is critical for applications that rely on HKCU or AppData like Battle.net.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if successful.</returns>
        bool LoadUserProfile(string username, string password);

        /// <summary>
        /// Deletes a local Windows user.
        /// Warning: This deletes the user profile and all data.
        /// </summary>
        /// <param name="username">The username to delete.</param>
        void DeleteUser(string username);

        /// <summary>
        /// Gets a list of all local Windows user accounts.
        /// </summary>
        /// <returns>A list of usernames.</returns>
        List<string> GetLocalUsers();

        /// <summary>
        /// Validates the credentials for a local Windows user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if credentials are valid, false otherwise.</returns>
        bool ValidateCredentials(string username, string password);

        /// <summary>
        /// Launches a program as the specified user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="programPath">Absolute path to the executable.</param>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Process ID of the launched application.</returns>
        int LaunchProgramAsUser(string username, string password, string programPath, string args);
    }
}
