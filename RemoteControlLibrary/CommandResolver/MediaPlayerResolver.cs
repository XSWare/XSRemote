using RemoteShutdownLibrary;
using RemoteShutdown.Functionalty;

namespace RemoteShutdown.CommandResolving
{
    public class MediaPlayerResolver : CommandResolver
    {
        MediaPlayerHandler MediaHandler { get; set; } = new MediaPlayerHandler();

        public override string KeyPhrase { get { return Commands.MEDIA; } }

        protected override bool Execute(string option, string argument)
        {
            switch (option)
            {
                case Commands.MEDIA_PLAY:
                    MediaHandler.StartStop();
                    return true;
                case Commands.MEDIA_PREVIOUS:
                    MediaHandler.Previous();
                    return true;
                case Commands.MEDIA_NEXT:
                    MediaHandler.Next();
                    return true;
            }

            return false;
        }
    }
}
