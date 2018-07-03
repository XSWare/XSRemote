using System;

namespace RemoteShutdown.CommandResolving
{
    class VolumeCommandResolver : CommandResolver
    {
        public override string KeyPhrase { get { return "volume"; } }

        VolumeHandler m_volumeHandler;

        public VolumeCommandResolver(VolumeHandler volumeHandler)
        {
            m_volumeHandler = volumeHandler;
        }

        protected override bool Execute(string option, string argument)
        {
            switch (option)
            {
                case "up":
                    m_volumeHandler.VolUp();
                    return true;

                case "down":
                    m_volumeHandler.VolDown();
                    return true;

                case "mute":
                    m_volumeHandler.Mute();
                    return true;
            }

            return false;
        }
    }
}
