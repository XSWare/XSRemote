using RemoteShutdowLibrary;
using System;

namespace RemoteShutdown.CommandResolving
{
    class VolumeCommandResolver : CommandResolver
    {
        public override string KeyPhrase { get { return Commands.VOLUME; } }

        VolumeHandler m_volumeHandler;

        public VolumeCommandResolver(VolumeHandler volumeHandler)
        {
            m_volumeHandler = volumeHandler;
        }

        protected override bool Execute(string option, string argument)
        {
            switch (option)
            {
                case Commands.VOLUME_UP:
                    m_volumeHandler.VolUp();
                    return true;

                case Commands.VOLUME_DOWN:
                    m_volumeHandler.VolDown();
                    return true;

                case Commands.VOLUME_MUTE:
                    m_volumeHandler.Mute();
                    return true;
            }

            return false;
        }
    }
}
