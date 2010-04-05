using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using VPNKeepAlive.Properties;
using System.Net.NetworkInformation;
using System.Drawing;
using System.Reflection;

namespace VPNKeepAlive
{
    public class VPNKeepAliveAppContext : ApplicationContext
    {
        bool connected = false;
        bool pingFailed = false;

        IContainer components;
        NotifyIcon notifyIcon;
        ContextMenu contextMenu;
        MenuItem settingsMenuItem;
        MenuItem exitMenuItem;
        MenuItem connectMenuItem;
        MenuItem reconnectMenuItem;
        Timer timer;
        Icon disconnectedIcon;
        Icon connectedIcon;
        Icon errorIcon;

        public VPNKeepAliveAppContext()
        {
            InitializeContext();
        }

        void InitializeContext()
        {
            components = new Container();
            notifyIcon = new NotifyIcon(this.components);
            contextMenu = new ContextMenu();
            connectMenuItem = new MenuItem();
            reconnectMenuItem = new MenuItem();
            settingsMenuItem = new MenuItem();
            exitMenuItem = new MenuItem();
            timer = new Timer(this.components);

            // Load icons:
            disconnectedIcon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("VPNKeepAlive.server_delete.ico"));
            connectedIcon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("VPNKeepAlive.server_go.ico"));
            errorIcon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("VPNKeepAlive.server_error.ico"));

            // notifyIcon
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Icon = disconnectedIcon;
            notifyIcon.Visible = true;
            notifyIcon.Text = "Not connected";
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);

            // contextMenu
            contextMenu.MenuItems.Add(connectMenuItem);
            contextMenu.MenuItems.Add(reconnectMenuItem);
            contextMenu.MenuItems.Add(new MenuItem
            {
                Text = "-",
                Index = 2
            });
            contextMenu.MenuItems.Add(settingsMenuItem);
            contextMenu.MenuItems.Add(exitMenuItem);
            contextMenu.Popup += new EventHandler(contextMenu_Popup);

            // connectMenuItem
            connectMenuItem.Index = 0;
            connectMenuItem.Text = "&Connect";
            connectMenuItem.Click += new EventHandler(connectMenuItem_Click);

            // reconnectMenuItem
            reconnectMenuItem.Index = 1;
            reconnectMenuItem.Text = "&Reconnect";
            reconnectMenuItem.Click += new EventHandler(reconnectMenuItem_Click);

            // settingsMenuItem
            settingsMenuItem.Index = 3;
            settingsMenuItem.Text = "&Settings";
            settingsMenuItem.Click += new EventHandler(settingsMenuItem_Click);

            // exitMenuItem
            exitMenuItem.Index = 4;
            exitMenuItem.Text = "E&xit";
            exitMenuItem.Click += new EventHandler(exitMenuItem_Click);

            // timer
            timer.Tick += new EventHandler((o, e) => DoPing());
            timer.Enabled = false;

            // this
            this.ThreadExit += new EventHandler(VPNKeepAliveAppContext_ThreadExit);
        }

        void VPNKeepAliveAppContext_ThreadExit(object sender, EventArgs e)
        {
            this.notifyIcon.Visible = false;
        }

        void contextMenu_Popup(object sender, EventArgs e)
        {
            reconnectMenuItem.Visible = connected;
        }

        void reconnectMenuItem_Click(object sender, EventArgs e)
        {
            Disconnect();
            Connect();
        }

        void connectMenuItem_Click(object sender, EventArgs e)
        {
            if (connected) Disconnect(); else Connect();
        }

        void settingsMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        void exitMenuItem_Click(object sender, EventArgs e)
        {
            if (connected) Disconnect();
            this.ExitThread();
        }

        void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (connected)
            {
                Disconnect();
                if (pingFailed)
                {
                    Connect();
                }
            }
            else
            {
                Connect();
            }
        }


        void Connect()
        {
            this.notifyIcon.Icon = disconnectedIcon;

            var args = string.Format("{0} {1} {2}",
                Settings.Default.Connection,
                Settings.Default.Username,
                Settings.Default.Password
                );
            Process
                .Start("rasdial", args)
                .WaitForExit();
            connected = true;
            RefreshControl();

            this.notifyIcon.Icon = connectedIcon;
            this.notifyIcon.ShowBalloonTip(3000, "Connected", "Connected to " + Settings.Default.Connection, ToolTipIcon.Info);

            StartPinging();
        }
        
        void Disconnect()
        {
            StopPinging();

            var args = string.Format("{0} /disconnect",
                Settings.Default.Connection);
            Process
                .Start("rasdial", args)
                .WaitForExit();
            connected = false;
            RefreshControl();
            notifyIcon.Icon = disconnectedIcon;
        }


        void RefreshControl()
        {
            notifyIcon.Text = connected ? "Connected to " + Settings.Default.Connection : "Not connected";
            connectMenuItem.Text = connected ? "Dis&connect" : "&Connect";
        }


        void StartPinging()
        {
            timer.Interval = Settings.Default.PingDelay * 1000;
            timer.Start();
        }

        void StopPinging()
        {
            timer.Stop();
        }

        void DoPing()
        {
            // reset the interval if the settings have been changed
            timer.Interval = Settings.Default.PingDelay * 1000;

            var p = new Ping();
            p.PingCompleted += new PingCompletedEventHandler((o, e) =>
            {
                if (e.Reply.Status != IPStatus.Success)
                {
                    var error = string.Format(
                        "Ping to {0} failed: {1}. Click here to reconnect.",
                        Settings.Default.ServerName, e.Reply.Status.ToString());
                    this.notifyIcon.BalloonTipClicked += new EventHandler(notifyIcon_BalloonTipClicked);
                    this.notifyIcon.ShowBalloonTip(5000, "Ping failed", error, ToolTipIcon.Error);
                    this.notifyIcon.Icon = errorIcon;
                    this.pingFailed = true;
                }
                else
                {
                    this.notifyIcon.Icon = connectedIcon;
                    this.pingFailed = false;
                }
            });
            p.SendAsync(Settings.Default.ServerName, null);
        }

        void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            this.notifyIcon.BalloonTipClicked -= new EventHandler(notifyIcon_BalloonTipClicked);
            this.notifyIcon.ShowBalloonTip(3000, "Reconnecting", "Reconnecting " + Settings.Default.Connection, ToolTipIcon.Info);
            Disconnect();
            Connect();
        }

        
    }
}
