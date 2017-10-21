﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NotificationProject.ViewModel;
using NotificationProject.View;
using System.IO;

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
            DirectoryInfo di = Directory.CreateDirectory("XmlFile");
            MainWindow main = new MainWindow();
            MainViewModel context = new MainViewModel();
            main.DataContext = context;
            main.Show();

        }

        private void onStateChange()
        {

        }
    }
}
