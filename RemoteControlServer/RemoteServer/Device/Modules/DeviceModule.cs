namespace RemoteServer.Device.Modules
{
    abstract class DeviceModule
    {
        public abstract bool IsModuleTargeted(string modulePhrase);
        public abstract string TranslateCommand(string command);
    }
}
