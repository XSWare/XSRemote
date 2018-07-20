using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using System.Net;
using System.Threading;
using XSLibrary.Network.Connectors;
using XSLibrary.Cryptography.ConnectionCryptos;
using XSLibrary.Network.Connections;

namespace RemoteControlAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
        AccountConnector m_connector;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

            RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;

            m_connector = CreateConnector();

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            buttonConnect.Click += OnButtonConnect;

            ResetStatus();

            OnButtonConnect(this, new EventArgs());
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        private AccountConnector CreateConnector()
        {
            AccountConnector connector = new AccountConnector();
            connector.Crypto = CryptoType.RSALegacy;
            connector.TimeoutCryptoHandshake = 30000;

            return connector;
        }

        private void OnButtonConnect(object sender, EventArgs eventArgs)
        {
            if (m_connector.CurrentlyConnecting)
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

            SetStatus("Connecting...");

            m_connector.ConnectAsync(
                new IPEndPoint(ip, 443), 
                (connection) => RunOnUiThread(() => ConnectSuccess(connection)), 
                (error) => RunOnUiThread(() => ConnectFailure(error)));
        }

        private void ConnectSuccess(TCPPacketConnection connection)
        {
            CommandCenter.SetConnection(connection);

            if (CommandCenter.Connected)
            {
                StartActivity(typeof(ControlActivity));
                SetStatus("Disconnected.");
            }
        }

        private void ConnectFailure(string error)
        {
            SetStatus(error);
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

