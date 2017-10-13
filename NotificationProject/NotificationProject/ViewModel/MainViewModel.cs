using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using NotificationProject.HelperClasses;
using BusinessLayer;
using System;
using System.Net.Sockets;
using NotificationProject.View;
using DataAccess;
using DataAccess.Model;
using Newtonsoft.Json.Linq;
using NotificationProjet.Controller;
using BusinessLayer.Model;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Timers;

namespace NotificationProject.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private ICommand _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private DevicesController _devicesController;

        public event PropertyChangedEventHandler PropertyChanged;


        #endregion Fields

        #region constructor

        public MainViewModel()
        {
            //Add the pages
            PageViewModels.Add(new HomeViewModel());
            PageViewModels.Add(new QRCodeViewModel());
            PageViewModels.Add(new CommunicationViewModel());
            PageViewModels.Add(new SmsViewModel());
            PageViewModels.Add(new ConfigurationViewModel());
            PageViewModels.Add(new ContactViewModel());
            PageViewModels.Add(new EtatViewModel());
            // Set default page
            CurrentPageViewModel = PageViewModels[0];
            _devicesController = DevicesController.getInstance();
            this.StartServer();

            //this.DisplayNotif("Appel", "Vous avez un appel de Tony Stark sur l'appareil 'LGG4'", "appel", null); // Decommenter pour avoir un apercu d'une notif
        }

        #endregion

        #region properties
        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();
                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public DevicesController Devices
        {
            get
            {
                return _devicesController;
            }
            set
            {
                _devicesController = value;
            }
        }
        #endregion

        #region Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }
                return _changePageCommand;
            }
        }
        #endregion

        #region methods

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);
            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        #endregion



        public void CallBackAfterAnalysis(String name, String message)
        {
            //Création des objets vides
            Notification notification = new Notification("", "");
            ConnectionRequest connectionReq = new ConnectionRequest("", "");

            /*using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString() + "- Message reçu : " + message);
            }*/

            //Conversion et traitement et parsing d'un JSON
            JObject jsonMessage = JSONHandler.stringToJson(message);
            string[] parsedJson = JSONHandler.interpretation(jsonMessage);
            Device device = new Device();
            Boolean addMessage = false;
            //Interprétation du JSON parsé
            //Demande de connexion
            if (parsedJson[0].ToLower() == "connection")
            {
                connectionReq.Appareil = parsedJson[1];
                connectionReq.Autor = parsedJson[2];
                var pairaineKey = parsedJson[2].Split(':');
                //--Demande d'acceptation de connexion--
                //TODO: créer une méthode qui gère le choix de l'utilisateur JObject messageToDevice = JSONHandler.messageRetour("connected", connectionReq.Appareil, connectionReq.Autor);
                if (int.Parse(pairaineKey[2]) == CommunicationService.getInstance().randomSecretNumberAccess)
                {
                    device = Devices.Devices.FirstOrDefault(o => o.Name == name);
                    notification.Application = parsedJson[1];
                    notification.Message = "demande de connexion";
                    Console.WriteLine("Successfuly connexion !");
                    this.DisplayNotif("Connexion", "Vous êtes désormais connecté avec l'appareil " + connectionReq.Appareil, "Connection", null);
                }
                else
                {
                    this.DisplayNotif("Connexion", "Echec de connexion avec l'appareil " + connectionReq.Appareil + ". La clé temporaire n'est plus correcte, veuillez réessayer.", "Message", null);
                }
                addMessage = true;
            }
            //Demande de deconnexion
            else if (parsedJson[0].ToLower() == "disconnection")
            {
                //JObject messageToDevice = JSONHandler.messageRetour("disconnected", parsedJson[1], parsedJson[2]);
                device = Devices.Devices.FirstOrDefault(o => o.Name == name);
                CallBackAfterDeconnexion(device);
                //--Envoi du message

                //--Deconnexion de l'appareil--

                // Notifaction des vues
                CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
                communicationViewModel.OnPropertyChanged("ListDevices");
                SmsViewModel smsViewModel = (SmsViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Sms View");
                smsViewModel.OnPropertyChanged("ListDevices");

            }
            //Reception d'un message
            else if (parsedJson[0].ToLower() == "notification")
            {
                notification.Application = parsedJson[1];
                notification.Message = parsedJson[2];
                addMessage = true;
                this.DisplayNotif("Message", notification.Message, "Notification", null);
            }



            // -- TODO : Remove its a test
            /*Console.WriteLine("Affichage des devices : "); 
            foreach (Device d in Devices.Devices)
            {
                Console.WriteLine("Nom du device : " + d.Name);
                foreach (Notification n in d.ListMessages)
                {
                    Console.WriteLine("Message : " + n.Message);

                }
            }*/
            // -- 
            if (addMessage)
            {

                device = Devices.Devices.FirstOrDefault(o => o.Name == name);
                device.ListMessages.Add(notification);
                CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
                communicationViewModel.CommunicationStatus = message;
                communicationViewModel.OnPropertyChanged("Messages");
            }

        }

        public void CallBackAfterDeconnexion(Device clientDevice)
        {
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Device déconnecté";
            clientDevice.sendMessage(JSONHandler.creationDisconnectString("bob", clientDevice.Name));
            Devices.deleteDevice(clientDevice);
            /* CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
             communicationViewModel.CommunicationStatus = "Device connecté";
             Devices.addDevice(newDevice);
             OnPropertyChanged("Devices");*/

            //var dataAccess = new XmlAccess("./data.xml");
            //dataAccess.saveDevice(newDevice);
        }

        public void CallBackAfterConnexion(String name, Socket clientDevice)
        {
            Device newDevice = new Device(name, clientDevice);
            /*using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString() + "- Device connecté : " + newDevice.Name);
            }*/
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Device connecté";
            Devices.addDevice(newDevice);
            OnPropertyChanged("Devices");
            communicationViewModel.OnPropertyChanged("ListDevices");


            //var dataAccess = new XmlAccess("./data.xml");
            //dataAccess.saveDevice(newDevice);
        }

        public void DisplayNotif(string title, string content, string type, string application)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate
            {
                int slideOutTimer = 5;
                if (type == "appel")
                {
                    slideOutTimer = 30;
                }

                NotificationView notif = new NotificationView();
                NotificationViewModel notifContext = new NotificationViewModel(title, content, type, application);
                notif.DataContext = notifContext;
                notif.displayNotif(slideOutTimer);
            });
        }

        private void StartServer()
        {
            CommunicationService cs = CommunicationService.getInstance();
            cs.callBackAfterConnexion = CallBackAfterConnexion;
            cs.callBackAfterAnalysis = CallBackAfterAnalysis;
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Server Started";
        }
    }
}
