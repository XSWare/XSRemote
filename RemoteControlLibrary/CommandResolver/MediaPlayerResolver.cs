using RemoteShutdownLibrary;
using RemoteShutdown.Functionalty;

namespace RemoteShutdown.CommandResolving
{
    public class MediaPlayerResolver : SingleArgumentResolver
    {
        public override string KeyPhrase { get { return Commands.MEDIA; } }

        MediaPlayerHandler m_mediaHandler;

        public MediaPlayerResolver(MediaPlayerHandler mediaHandler)
        {
            m_mediaHandler = mediaHandler;
        }

        protected override bool Execute(string option, string argument)
        {
            switch (option)
            {
                case Commands.MEDIA_PLAY:
                    m_mediaHandler.StartStop();
                    return true;
                case Commands.MEDIA_PREVIOUS:
                    m_mediaHandler.Previous();
                    return true;
                case Commands.MEDIA_NEXT:
                    m_mediaHandler.Next();
                    return true;
            }

            return false;
        }
    }
}
