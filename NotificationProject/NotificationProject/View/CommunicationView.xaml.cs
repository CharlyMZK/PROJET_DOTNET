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
        CommunicationService.CommunicationService communicationServer;

        public CommunicationView() 
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
            if (!serverStarted)
            {
                serverStarted = true;
                communicationServer = new CommunicationService.CommunicationService();
                buttonStartServer.Content = "Stop server";
            }else
            {
                serverStarted = false;
                communicationServer.disconnect();   
            }
            
=======
            /*
            CommunicationService.CommunicationService cs = new CommunicationService.CommunicationService();
            buttonStartServer.Content = "Stop server";*/
>>>>>>> 25696f2a7f5c3cf0a23417c31cc2ab932addb30d
        }
    }
}
 