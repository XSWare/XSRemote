using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using XSLibrary.Network.Connections;

namespace RemoteControlClientWPF
{
    public partial class MainWindow : Window
    {
        Login m_login;

        TCPPacketConnection Connection { get; set; }
        NotifyIcon m_notifyIcon;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTrayIcon();

            m_login = new Login();
            m_login.SuccessfullyConnected += OnLogin;
            m_login.LoginFailed += OnLoginFailed;
            OpenLogin();

            if (m_login.AutoLogin)
                m_login.Connect();

        }

        private void InitializeTrayIcon()
        {
            m_notifyIcon = new NotifyIcon();
            using (Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/;component/LogoSmall.ico")).Stream)
                m_notifyIcon.Icon = new Icon(iconStream);

            m_notifyIcon.MouseDoubleClick += new MouseEventHandler(MyNotifyIcon_MouseDoubleClick);
        }

        private void OpenLogin()
        {
            Content = m_login;
            Dispatcher.Invoke(() => m_login.SetFocus(), DispatcherPriority.Background);
        }

        private void OnLogin(object sender, TCPPacketConnection connection)
        {
            Content = new Control(connection);
            Connection = connection;
            connection.OnDisconnect.Event += OnDisconnect;
        }

        private void OnLoginFailed(object sender)
        {
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
        }

        private void OnDisconnect(object sender, EndPoint remote)
        {
            Dispatcher.Invoke(() => OpenLogin());
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TCPPacketConnection connection = Connection;
            if(connection != null)
            {
                Connection = null;
                connection.OnDisconnect.Event -= OnDisconnect;
                connection.Disconnect();
            }
        }

        void MyNotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
                m_notifyIcon.Visible = true;
            }
            else
            {
                m_notifyIcon.Visible = false;
                ShowInTaskbar = true;

                Activate();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // initially minimized
            if(m_login.AutoLogin)
                WindowState = WindowState.Minimized;
        }
    }
}
