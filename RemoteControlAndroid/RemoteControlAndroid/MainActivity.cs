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

namespace RemoteControlAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
        private TCPPacketConnection connection;
        volatile bool connecting = false;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            buttonConnect.Click += OnButtonConnect;

            Button buttonDisonnect = FindViewById<Button>(Resource.Id.buttonDisconnect);
            buttonDisonnect.Click += OnButtonDisconnect;

            ResetStatus();
        }

		public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        private void OnButtonConnect(object sender, EventArgs eventArgs)
        {
            if (connecting || connection != null)
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

            Task.Run(() => SetStatus(Connect(new IPEndPoint(ip, 22223))));
        }

        private string Connect(EndPoint remote)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try { socket.Connect(remote); }
                catch { return "Failed to connect."; }

                connection = new TCPPacketConnection(socket);
                if (!connection.InitializeCrypto(new RSALegacyCrypto(true)))
                {
                    connection = null;
                    return "Handshake failed.";
                }

                connection.InitializeReceiving();

                connection.Send(TransmissionConverter.ConvertStringToByte("volume up"));
                connection.Send(TransmissionConverter.ConvertStringToByte("volume down"));

                return "Connected.";
            }
            finally
            {
                connecting = false;
            }

            return "Failure!";
        }

        private void OnButtonDisconnect(object sender, EventArgs eventArgs)
        {
            if (connection != null)
            {
                connection.Disconnect();
                connection = null;
                SetStatus("Disconnected.");
            }
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

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            switch (keyCode)
            {
                case Keycode.VolumeDown:
                    SendVolumeCommand("down");
                    return true;
                case Keycode.VolumeUp:
                    SendVolumeCommand("up");
                    return true;
            }

            return base.OnKeyDown(keyCode, e);
        }

        private void SendVolumeCommand(string cmd)
        {
            SendCommand("volume " + cmd);
        }

        private void SendCommand(string cmd)
        {
            if (connection == null)
                return;

            connection.Send(TransmissionConverter.ConvertStringToByte(cmd));
        }
    }
}

