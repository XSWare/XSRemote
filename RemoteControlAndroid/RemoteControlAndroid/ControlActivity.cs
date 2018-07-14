
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using RemoteShutdowLibrary;
using System;

namespace RemoteControlAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class ControlActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_control);

            FindViewById<Button>(Resource.Id.buttonShutdown).Click += OnButtonShutdown;
            FindViewById<Button>(Resource.Id.buttonRestart).Click += OnButtonRestart;
            FindViewById<Button>(Resource.Id.buttonCancel).Click += OnButtonCancel;
            FindViewById<Button>(Resource.Id.buttonPlay).Click += OnButtonPlay;
            FindViewById<Button>(Resource.Id.buttonPrevious).Click += OnButtonPrevious;
            FindViewById<Button>(Resource.Id.buttonNext).Click += OnButtonNext;
            FindViewById<Button>(Resource.Id.buttonMute).Click += OnButtonMute;
            FindViewById<Button>(Resource.Id.buttonDisconnect).Click += OnButtonDisconnect;

            CommandCenter.OnDisconnect += HandleDisconnect;
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
            string strDelay = FindViewById<EditText>(Resource.Id.editDelay).Text;
            return int.TryParse(strDelay, out delay);
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

        private void HandleDisconnect(object sender, System.Net.EndPoint remote)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            CommandCenter.Disconnect();
            base.OnBackPressed();
        }
    }
}