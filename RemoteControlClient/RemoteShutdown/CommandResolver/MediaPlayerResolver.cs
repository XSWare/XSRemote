namespace RemoteShutdown.CommandResolving
{
    class MediaPlayerResolver : CommandResolver
    {
        MediaPlayerHandler MediaHandler { get; set; } = new MediaPlayerHandler();

        public override string KeyPhrase { get { return "media"; } }

        protected override bool Execute(string option, string argument)
        {
            switch (option)
            {
                case "play":
                    MediaHandler.StartStop();
                    return true;
                case "previous":
                    MediaHandler.Previous();
                    return true;
                case "next":
                    MediaHandler.Next();
                    return true;
            }

            return false;
        }
    }
}
