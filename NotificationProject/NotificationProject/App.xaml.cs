using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NotificationProject.ViewModel;
using NotificationProject.View;

namespace NotificationProject
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow main = new MainWindow();
            MainViewModel context = new MainViewModel();
            main.DataContext = context;
            //main.Show();

            NotificationView notif = new NotificationView();
            NotificationViewModel notif_context = new NotificationViewModel();
            notif.DataContext = notif_context;
        }
    }
}
