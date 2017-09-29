using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

namespace NotificationProject
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;
        public MainWindow()
        {
            InitializeComponent();
        }

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, string tooltipText, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null)
            {
                item.Click += eventHandler;
            }

            item.ToolTipText = tooltipText;
            return item;
        }

        private void displayApp(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Activate();
            this.ShowInTaskbar = true;
            this.notifyIcon.Visible = false;
        }

        private void closeApp(object sender, EventArgs e)
        {
            this.Close();
        }

        private void onClickNotifyIcon(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Console.WriteLine("Hidari clicko !");
            if(e.Button == MouseButtons.Left)
            {
                this.displayApp(sender, e);
            }
        }

        public void onMinimizeWindow(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;

                var components = new System.ComponentModel.Container();
                this.notifyIcon = new System.Windows.Forms.NotifyIcon(components)
                {
                    ContextMenuStrip = new ContextMenuStrip(),
                    Icon = NotificationProject.Properties.Resources.appIcon1,
                    Text = "The app is still running in background",
                    Visible = true,
                };

                notifyIcon.MouseClick += this.onClickNotifyIcon;

                var openApplicationMenuItem = ToolStripMenuItemWithHandler(
                    "Open application",
                    "Open the application",
                    displayApp);
                this.notifyIcon.ContextMenuStrip.Items.Add(openApplicationMenuItem);

                var closeApplicationMenuItem = ToolStripMenuItemWithHandler(
                    "Close application",
                    "Close the application",
                    closeApp);
                this.notifyIcon.ContextMenuStrip.Items.Add(closeApplicationMenuItem);
            }
        }
    }
}
