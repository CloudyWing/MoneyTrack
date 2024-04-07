using System.Web.Configuration;

namespace CloudyWing.MoneyTrack.Infrastructure.Application {
    /// <summary>
    /// This class manages the application settings and provides methods to retrieve the values of those settings.
    /// </summary>
    public class ApplicationSettingManager {
        private static ApplicationSettingManager manager = null;

        private ApplicationSettingManager() { }

        /// <summary>
        /// Gets the singleton instance of the ApplicationSettingManager.
        /// </summary>
        public static ApplicationSettingManager Instance {
            get {
                manager = manager ?? new ApplicationSettingManager();

                return manager;
            }
        }

        public bool VerifyUser(string userId, string password) {
            return GetValue("UserId") == userId
                && GetValue("Password") == password;
        }

        /// <summary>
        /// Retrieves the value of the specified key from the application settings.
        /// </summary>
        /// <param name="key">The key of the setting to retrieve.</param>
        /// <returns>The value of the specified setting.</returns>
        public string GetValue(string key) {
            return WebConfigurationManager.AppSettings[key];
        }
    }
}
