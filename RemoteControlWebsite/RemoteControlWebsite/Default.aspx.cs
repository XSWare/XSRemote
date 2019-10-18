using RemoteShutdown;
using System;
using System.Text;
using System.Web.UI;
using XSLibrary.Cryptography.AccountManagement;

namespace RemoteControlWebsite
{
    public partial class _Default : Page
    {
        static IUserDataBase dataBase = new LocalSQLUserBase(CommonPaths.DATABASE_FILEPATH, CommonPaths.DATABASE_SERVER_STRING);

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

        protected void OnDownloadAndroidClick(object sender, EventArgs e)
        {
            DownloadFile("XSRemote.apk");
        }

        protected void OnDownloadWindowsClick(object sender, EventArgs e)
        {
            DownloadFile("XSRemote_windows.zip");
        }

        private void DownloadFile(string filename)
        {
            Response.ContentType = "application/octect-stream";
            Response.AppendHeader("content-disposition", "filename=" + filename);
            Response.TransmitFile(Server.MapPath("~/DownloadableContent/" + filename));
            Response.End();
        }
    }
}