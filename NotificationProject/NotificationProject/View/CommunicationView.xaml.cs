﻿using BusinessLayer;
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
using System.Windows.Shapes;

namespace NotificationProject.View
{
    /// <summary>
    /// Logique d'interaction pour CommunicationView.xaml
    /// </summary>
    public partial class CommunicationView : UserControl
    {
        private bool serverStarted = false;
        BusinessLayer.CommunicationService communicationServer;

        public CommunicationView() 
        {
            InitializeComponent();
        }

    }
}
 