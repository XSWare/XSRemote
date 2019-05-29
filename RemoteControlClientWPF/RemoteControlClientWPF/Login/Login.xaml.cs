using System.Windows.Controls;

namespace RemoteControlClientWPF
{
    public partial class Login : UserControl
    {
        string Server
        {
            get { return m_txtServer.Text; }
            set { m_txtServer.Text = value; }
        }

        string Username
        {
            get { return m_txtUser.Text; }
            set { m_txtUser.Text = value; }
        }

        string Password
        {
            get { return m_txtPassword.Text; }
            set { m_txtPassword.Text = value; }
        }

        public Login()
        {
            InitializeComponent();
        }
    }
}
