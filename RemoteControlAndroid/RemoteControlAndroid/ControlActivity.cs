
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RemoteShutdowLibrary;
using System;
using System.Net;
using System.Threading;
using XSLibrary.Utility;
using XSLibrary.Utility.Logging;

namespace RemoteControlAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class ControlActivity : AppCompatActivity
    {
        Logger logger;

        Intent keepAliveService;

        EditText textDelay;
        TextView labelStatus;
        Timer timer;

        const int TimerDuration = 1000;
        const int IntervalSteps = 25;
        int intervallCount = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            logger = new LambdaLogger((text) => RunOnUiThread(() => SetStatus(text))) { LogLevel = LogLevel.Information };

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_control);

            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            textDelay = FindViewById<EditText>(Resource.Id.editDelay);
            labelStatus = FindViewById<TextView>(Resource.Id.labelStatus);

            FindViewById<Button>(Resource.Id.buttonShutdown).Click += OnButtonShutdown;
            FindViewById<Button>(Resource.Id.buttonRestart).Click += OnButtonRestart;
            FindViewById<Button>(Resource.Id.buttonCancel).Click += OnButtonCancel;
            FindViewById<Button>(Resource.Id.buttonPlay).Click += OnButtonPlay;
            FindViewById<Button>(Resource.Id.buttonPrevious).Click += OnButtonPrevious;
            FindViewById<Button>(Resource.Id.buttonNext).Click += OnButtonNext;
            FindViewById<Button>(Resource.Id.buttonMute).Click += OnButtonMute;
            FindViewById<Button>(Resource.Id.buttonDisconnect).Click += OnButtonDisconnect;

            CommandCenter.OnDisconnect += HandleDisconnect;
            CommandCenter.OnDataReceived += HandleDataReceived;

            keepAliveService = new Intent(this, typeof(KeepAliveService));
            StartService(keepAliveService);
        }

        protected override void OnStart()
        {
            CommandCenter.ActiveLogger = logger;
            textDelay.SetSelection(textDelay.SelectionEnd);
            base.OnStart();
        }

        protected override void OnResume()
        {
            if (!CommandCenter.Connected)
            {
                CleanUpConnection();
                base.OnBackPressed();
            }

            base.OnResume();
        }

        private void SetStatus(string status)
        {
            if(timer != null)
                timer.Dispose();

            labelStatus.Alpha = 1F;
            labelStatus.Text = status;
            intervallCount = IntervalSteps;
            timer = new Timer((state) => RunOnUiThread(() => FadeOutStatus()), null, TimerDuration, TimerDuration / IntervalSteps);
        }

        private void FadeOutStatus()
        {
            intervallCount--;
            if (intervallCount <= 0)
            {
                timer.Dispose();
                timer = null;
                labelStatus.Text = "";
            }
            else
            {
                labelStatus.Alpha = (float)intervallCount / IntervalSteps;
            }
        }

        private void OnButtonShutdown(object sender, EventArgs eventArgs)
        {
            SendControlWithDelay(Commands.CONTROL_SHUTDOWN);
        }

        private void OnButtonRestart(object sender, EventArgs eventArgs)
        {
            SendControlWithDelay(Commands.CONTROL_RESTART);
        }

        private void OnButtonCancel(object sender, EventArgs eventArgs)
        {
            CommandCenter.SendControlCommand(Commands.CONTROL_ABORT);
        }

        private void SendControlWithDelay(string cmd)
        {
            if (GetDelay(out int delay))
                CommandCenter.SendControlCommand(cmd + " " + delay.ToString());
        }

        private bool GetDelay(out int delay)
        {
            return int.TryParse(textDelay.Text, out delay);
        }

        private void OnButtonPlay(object sender, EventArgs eventArgs)
        {
            CommandCenter.SendMediaCommand(Commands.MEDIA_PLAY);
        }

        private void OnButtonPrevious(object sender, EventArgs eventArgs)
        {
            CommandCenter.SendMediaCommand(Commands.MEDIA_PREVIOUS);
        }

        private void OnButtonNext(object sender, EventArgs eventArgs)
        {
            CommandCenter.SendMediaCommand(Commands.MEDIA_NEXT);
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            switch (keyCode)
            {
                case Keycode.VolumeDown:
                    CommandCenter.SendVolumeCommand(Commands.VOLUME_DOWN);
                    return true;
                case Keycode.VolumeUp:
                    CommandCenter.SendVolumeCommand(Commands.VOLUME_UP);
                    return true;
            }

            return base.OnKeyDown(keyCode, e);
        }

        private void OnButtonMute(object sender, EventArgs eventArgs)
        {
            CommandCenter.SendVolumeCommand(Commands.VOLUME_MUTE);
        }

        private void OnButtonDisconnect(object sender, EventArgs eventArgs)
        {
            OnBackPressed();
        }

        private void HandleDataReceived(object sender, byte[] data, EndPoint source)
        {
            RunOnUiThread(() => SetStatus("Device received command"));
        }

        private void HandleDisconnect(object sender, EndPoint remote)
        {
            RunOnUiThread(() =>
            {
                CleanUpConnection();
                base.OnBackPressed();
            });
        }

        public override void OnBackPressed()
        {
            Disconnect();
            base.OnBackPressed();
        }

        protected override void OnDestroy()
        {
            Disconnect();
            base.OnDestroy();
        }

        private void Disconnect()
        {
            CleanUpConnection();
            StopService(keepAliveService);
            CommandCenter.Disconnect();
        }

        private void CleanUpConnection()
        {
            CommandCenter.OnDisconnect -= HandleDisconnect;
            CommandCenter.OnDataReceived -= HandleDataReceived;
        }

    }
}