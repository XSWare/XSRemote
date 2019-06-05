using System;
using System.Text;
using System.Web.UI;
using XSLibrary.Cryptography.AccountManagement;

namespace RemoteControlWebsite
{
    public partial class _Default : Page
    {
        static string dataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\RemoteControl\\";
        static FileUserBase dataBase = new FileUserBase(dataFolderPath, "accounts.txt");

        protected void OnRegisterClick(object sender, EventArgs e)
        {
            if(m_txtEmail.Text.Length <= 0)
            {
                lblStatus.Text = "Please enter an E-mail address.";
                return;
            }

            if (m_txtUser.Text.Length <= 0)
            {
                lblStatus.Text = "Please enter a username.";
                return;
            }

            if (m_txtPassword.Text.Length <= 0)
            {
                lblStatus.Text = "Please enter a password.";
                return;
            }

            AccountCreationData accountData = new AccountCreationData(m_txtUser.Text, Encoding.ASCII.GetBytes(m_txtPassword.Text), 5, m_txtEmail.Text);
            if (dataBase.AddAccount(accountData))
            {
                ClearFields();
                lblStatus.Text = "Account created successfully.";
            }
            else
                lblStatus.Text = "An account with this E-mail or username already exists.";

        }

        private void ClearFields()
        {
            m_txtEmail.Text = "";
            m_txtUser.Text = "";
            m_txtPassword.Text = "";
        }
    }
}