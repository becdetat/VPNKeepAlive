using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VPNKeepAlive.Properties;

namespace VPNKeepAlive
{
    public partial class SettingsForm : Form
    {
        bool loading = false;
        bool modified = false;

        public SettingsForm()
        {
            InitializeComponent();
            LoadControl();
            RefreshControl();
        }

        private void connectionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                modified = true;
                RefreshControl();
            }
        }

        private void usernameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                modified = true;
                RefreshControl();
            }
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                modified = true;
                RefreshControl();
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Connection = connectionTextBox.Text;
            Settings.Default.Username = usernameTextBox.Text;
            Settings.Default.Password = passwordTextBox.Text;
            Settings.Default.ServerName = serverNameTextBox.Text;
            Settings.Default.PingDelay = (int)pingDelayNumericUpDown.Value;
            Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        void LoadControl()
        {
            loading = true;
            connectionTextBox.Text = Settings.Default.Connection;
            usernameTextBox.Text = Settings.Default.Username;
            passwordTextBox.Text = Settings.Default.Password;
            serverNameTextBox.Text = Settings.Default.ServerName;
            pingDelayNumericUpDown.Value = Settings.Default.PingDelay;
            loading = false;
        }

        void RefreshControl()
        {
            okButton.Enabled = 
                modified
                && connectionTextBox.Text != ""
                && usernameTextBox.Text != ""
                && passwordTextBox.Text != ""
                && serverNameTextBox.Text != ""
                && pingDelayNumericUpDown.Value > 0;
        }

        private void pingDelayNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                modified = true;
                RefreshControl();
            }
        }

        private void serverNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                modified = true;
                RefreshControl();
            }
        }
    }
}
