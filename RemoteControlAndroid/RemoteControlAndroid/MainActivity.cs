using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using System.Net;
using System.Net.Sockets;
using XSLibrary.Network.Connections;
using RemoteShutdownLibrary;

namespace RemoteControlAndroid
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
        private Socket socket;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

            Button buttonConnect = FindViewById<Button>(Resource.Id.buttonConnect);
            buttonConnect.Click += Connect;
        }

		public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        private void Connect(object sender, EventArgs eventArgs)
        {
            EditText editIP = FindViewById<EditText>(Resource.Id.editIP);

            if (!IPAddress.TryParse(editIP.Text, out IPAddress ip))
                return;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try { socket.Connect(ip, 22223); }
            catch { return; }

            TCPPacketConnection connection = new TCPPacketConnection(socket);

            connection.Send(TransmissionConverter.ConvertStringToByte("volume up"));
            connection.Send(TransmissionConverter.ConvertStringToByte("volume down"));
            connection.Disconnect();
            //ECDsaOpenSsl ssl = new ECDsaOpenSsl(NamedCurves.nistP521);

            //ECDsaCng ECDSA = new ECDsaCng(CngKey.Create(CngAlgorithm.ECDiffieHellmanP521));
        }
    }
}

