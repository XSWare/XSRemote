using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;

namespace RemoteControlAndroid
{
    [Service(Name = "com.xsware.KeepAliveService", Enabled = true)]
    class KeepAliveService : Service
    {
        const int TimerWait = 10000;
        Timer timer;
        bool started = false;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (!started)
            {
                timer = new Timer(HandleTimerCallback, null, 0, TimerWait);
                started = true;
            }

            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            timer.Dispose();
            timer = null;
            started = false;

            base.OnDestroy();
        }

        void HandleTimerCallback(object state)
        {
            CommandCenter.SendKeepAlive();
        }
    }
}