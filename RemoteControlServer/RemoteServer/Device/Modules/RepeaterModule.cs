namespace RemoteServer.Device.Modules
{
    class RepeaterModule : DeviceModule
    {
        public override bool IsModuleTargeted(string modulePhrase)
        {
            return true;
        }

        public override string TranslateCommand(string command)
        {
            return command;
        }
    }
}
