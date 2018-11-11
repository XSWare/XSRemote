using Android.App;
using Android.Content;

namespace RemoteControlAndroid
{
    class Configuration
    {
        const string CONFIG_FOLDER = "remotecontrol";

        public string ServerIP = "80.109.174.197";
        public string Username = "username";
        public string Password = "password";

        public void Save()
        {
            var prefs = Application.Context.GetSharedPreferences(CONFIG_FOLDER, FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("serverip", ServerIP);
            prefEditor.PutString("username", Username);
            prefEditor.PutString("password", Password);
            prefEditor.Commit();
        }

        public void Load()
        {
            var prefs = Application.Context.GetSharedPreferences(CONFIG_FOLDER, FileCreationMode.Private);
            ServerIP = prefs.GetString("serverip", "80.109.174.197");
            Username = prefs.GetString("username", "username");
            Password = prefs.GetString("password", "password");
        }
    }
}