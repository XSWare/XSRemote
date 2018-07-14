using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using System.Net;
using System.Net.Sockets;
using XSLibrary.Network.Connections;
using XSLibrary.Cryptography.ConnectionCryptos;
using RemoteShutdownLibrary;
using System.Threading.Tasks;
using Android.Runtime;
using Android.Content;

namespace RemoteControlAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
        volatile bool connecting = false;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            buttonConnect.Click += OnButtonConnect;
        }

        protected override void OnStart()
        {
            ResetStatus();
            base.OnStart();
        }

        private void OnButtonConnect(object sender, EventArgs eventArgs)
        {
            if(CommandCenter.Connected)
            {
                StartActivity(typeof(ControlActivity));
                return;
            }

            if (connecting)
                return;

            connecting = true;

            EditText editIP = FindViewById<EditText>(Resource.Id.editIP);

            if (!IPAddress.TryParse(editIP.Text, out IPAddress ip))
            {
                SetStatus("Invalid IP format.");
                connecting = false;
                return;
            }

            SetStatus("Connecting...");

            Task.Run(() => CommandCenter.Instance.Connect(new IPEndPoint(ip, 22223))).Wait();

            if(CommandCenter.Connected)
                StartActivity(typeof(ControlActivity));
            else
                SetStatus(CommandCenter.Instance.LastError);

            connecting = false;
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

