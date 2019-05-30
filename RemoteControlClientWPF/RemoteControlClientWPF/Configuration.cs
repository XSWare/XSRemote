namespace RemoteControlClientWPF
{
    class LoginConfiguration
    {
        public string Server { get; set; } = "";
        public string User { get; set; } = "";
        public string Password { get; set; } = "";
        public bool StorePassword { get; set; } = false;
        public bool AutoLogin { get; set; } = false;

        public void LoadConfig()
        {

        }

        public void StoreConfig()
        {

        }
    }
}
