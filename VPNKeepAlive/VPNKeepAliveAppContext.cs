using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;
using VPNKeepAlive.Properties;

namespace VPNKeepAlive
{
    public class VPNKeepAliveAppContext : ApplicationContext
    {
        bool connected = false;

        IContainer components;
        NotifyIcon notifyIcon;
        ContextMenu contextMenu;
        MenuItem settingsMenuItem;
        MenuItem exitMenuItem;
        MenuItem connectMenuItem;

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
            settingsMenuItem = new MenuItem();
            exitMenuItem = new MenuItem();

            // notifyIcon
            notifyIcon.ContextMenu = contextMenu;
            try
            {
                notifyIcon.Icon = new System.Drawing.Icon("App.ico");
                notifyIcon.Visible = true;
            }
            catch { }
            notifyIcon.Text = "Not connected";
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);

            // contextMenu
            contextMenu.MenuItems.Add(connectMenuItem);
            contextMenu.MenuItems.Add(new MenuItem
            {
                Text = "-",
                Index = 1
            });
            contextMenu.MenuItems.Add(settingsMenuItem);
            contextMenu.MenuItems.Add(exitMenuItem);

            // connectMenuItem
            connectMenuItem.Index = 0;
            connectMenuItem.Text = "&Connect";
            connectMenuItem.Click += new EventHandler(connectMenuItem_Click);

            // settingsMenuItem
            settingsMenuItem.Index = 2;
            settingsMenuItem.Text = "&Settings";
            settingsMenuItem.Click += new EventHandler(settingsMenuItem_Click);

            // exitMenuItem
            exitMenuItem.Index = 3;
            exitMenuItem.Text = "E&xit";
            exitMenuItem.Click += new EventHandler(exitMenuItem_Click);
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
            if (connected) Disconnect(); else Connect();
        }


        void Connect()
        {
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
        }
        
        void Disconnect()
        {
            var args = string.Format("{0} /disconnect",
                Settings.Default.Connection);
            Process
                .Start("rasdial", args)
                .WaitForExit();
            connected = false;
            RefreshControl();
        }


        void RefreshControl()
        {
            notifyIcon.Text = connected ? "Connected to " + Settings.Default.Connection : "Not connected";
            connectMenuItem.Text = connected ? "Dis&connect" : "&Connect";
        }
    }
}
