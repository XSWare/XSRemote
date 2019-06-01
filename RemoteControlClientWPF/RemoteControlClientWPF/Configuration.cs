using System.Configuration;
using XSLibrary.Cryptography;

namespace RemoteControlClientWPF
{
    class LoginConfiguration
    {
        const string KEY_SERVER = "server";
        const string KEY_USER = "user";
        const string KEY_PASSWORD = "password";
        const string KEY_STORE_PASSWORD = "storepassword";
        const string KEY_AUTOLOGIN = "autologin";

        const string TRUE = "true";
        const string FALSE = "false";

        public string Server { get; set; } = "";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public bool StorePassword { get; set; } = false;
        public bool AutoLogin { get; set; } = false;

        public void StoreConfig()
        {
            StoreValue(KEY_SERVER, Server);
            StoreValue(KEY_USER, User);

            StoreBool(KEY_STORE_PASSWORD, StorePassword);

            if (StorePassword)
                StoreValue(KEY_PASSWORD, PasswordStorage.Encrypt(Password));
            else
                StoreValue(KEY_PASSWORD, "");

            StoreBool(KEY_AUTOLOGIN, AutoLogin);
        }

        public void LoadConfig()
        {
            Server = LoadValue(KEY_SERVER);
            if (Server == null)
                Server = "80.109.174.197";

            User = LoadValue(KEY_USER);

            StorePassword = LoadBool(KEY_STORE_PASSWORD);
            if (StorePassword)
            {
                string temp;
                if (PasswordStorage.Decrypt(LoadValue(KEY_PASSWORD), out temp))
                    Password = temp;
                else
                    Password = "";
            }
            else
                Password = "";

            AutoLogin = LoadBool(KEY_AUTOLOGIN);
        }

        private void StoreValue(string key, string value)
        {
            var roaming = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = roaming.FilePath;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            var setting = config.AppSettings.Settings;
            setting.Remove(key);
            setting.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        private string LoadValue(string key)
        {
            var roaming = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = roaming.FilePath;

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            var pair = config.AppSettings.Settings[key];
            return pair != null ? pair.Value : null;
        }

        private void StoreBool(string key, bool value)
        {
            StoreValue(key, value ? TRUE : FALSE);
        }

        private bool LoadBool(string key)
        {
            return LoadValue(key) == TRUE;
        }
    }
}
