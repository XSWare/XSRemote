using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using System.Net;
using XSLibrary.Utility.Logging;
using XSLibrary.Utility;

namespace RemoteControlAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.XSWare", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
        Logger logger;

        const string CONNECTING = "Connecting...";
        const string DISCONNECTED = "Disconnected.";

        protected override void OnCreate(Bundle savedInstanceState)
		{
            logger = new LambdaLogger((text) => RunOnUiThread(() => SetStatus(text))) { LogLevel = LogLevel.Information };

            base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            CommandCenter.OnConnect += ConnectSuccess;

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            buttonConnect.Click += OnButtonConnect;

            ResetStatus();

            OnButtonConnect(this, new EventArgs());
        }

        protected override void OnStart()
        {
            CommandCenter.ActiveLogger = logger;

            if (CommandCenter.CurrentlyConnecting)
                SetStatus(CONNECTING);

            base.OnStart();
        }

        protected override void OnRestart()
        {
            logger.Log(LogLevel.Warning, DISCONNECTED);
            base.OnRestart();
        }

        private void OnButtonConnect(object sender, EventArgs eventArgs)
        {
            if (CommandCenter.CurrentlyConnecting)
                return;

            if (CommandCenter.Connected)
            {
                StartActivity(typeof(ControlActivity));
                return;
            }

            EditText editIP = FindViewById<EditText>(Resource.Id.editIP);

            if (!IPAddress.TryParse(editIP.Text, out IPAddress ip))
            {
                SetStatus("Invalid IP format.");
                return;
            }

            CommandCenter.Connect(new IPEndPoint(ip, 443));
        }

        private void ConnectSuccess()
        {
            RunOnUiThread(() =>
            {
                if (CommandCenter.Connected)
                    StartActivity(typeof(ControlActivity));
            });
        }

        private void SetStatus(string status)
        {
            TextView labelStatus = FindViewById<TextView>(Resource.Id.labelStatus);
            labelStatus.Text = status;
        }

        private void ResetStatus()
        {
            SetStatus("");
        }
    }
}

