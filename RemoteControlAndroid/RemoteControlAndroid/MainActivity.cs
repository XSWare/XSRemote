using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using System.Net;
using XSLibrary.Utility.Logging;
using XSLibrary.Utility;
using XSLibrary.Network;

namespace RemoteControlAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.XSWare", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
        Logger logger;
        Configuration configuration = new Configuration();
        EditText editServerIP;
        EditText editUsername;
        EditText editPassword;

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

            editServerIP = FindViewById<EditText>(Resource.Id.editIP);
            editUsername = FindViewById<EditText>(Resource.Id.editUser);
            editPassword = FindViewById<EditText>(Resource.Id.editPassword);

            ResetStatus();

            configuration.Load();

            editServerIP.Text = configuration.ServerIP;
            editUsername.Text = configuration.Username;
            editPassword.Text = configuration.Password;

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

            string serverIP = editServerIP.Text;

            IPEndPoint destination;
            try { destination = AddressResolver.Resolve(serverIP, 22223); }
            catch (Exception ex)
            { 
                SetStatus(ex.Message);
                return;
            }

            string username = editUsername.Text;
            if(username.Contains(" "))
            {
                SetStatus("Username must not contain space character.");
                return;
            }

            string password = editPassword.Text;
            if (password.Contains(" "))
            {
                SetStatus("Password must not contain space character.");
                return;
            }

            configuration.ServerIP = serverIP;
            configuration.Username = username;
            configuration.Password = password;
            configuration.Save();
            CommandCenter.Connect(destination, username, password);
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

