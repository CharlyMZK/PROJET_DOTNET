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
            // Set default page
            CurrentPageViewModel = PageViewModels[0];
            _devicesController = DevicesController.getInstance();
            this.StartServer();
            //this.DisplayNotif("Messenger", "Hey ! Send nudes plz", "Message"); // Decommenter pour avoir un apercu d'une notif
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
                if(_currentPageViewModel != value)
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
                if(_changePageCommand == null)
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
            if(handler != null)
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
            //Interprétation du JSON parsé
            //Demande de connexion
            if (parsedJson[0].ToLower() == "connection")
            {
                connectionReq.Appareil = parsedJson[1];
                connectionReq.Autor = parsedJson[2];
                var pairaineKey = parsedJson[2].Split(':');
                //--Demande d'acceptation de connexion--
                //TODO: créer une méthode qui gère le choix de l'utilisateur JObject messageToDevice = JSONHandler.messageRetour("connected", connectionReq.Appareil, connectionReq.Autor);
                if(int.Parse(pairaineKey[2]) == CommunicationService.getInstance().randomSecretNumberAccess)
                {
                    device = Devices.Devices.FirstOrDefault(o => o.Name == name);
                    notification.Application = parsedJson[1];
                    notification.Message ="demande de connexion"; 
                    Console.WriteLine("Successfuly connexion !");  
                } else
                {
                    throw new Exception("Wrong PairaineKey");
                }

            }
                //Demande de deconnexion
            else if(parsedJson[0].ToLower() == "disconnection")
            {
                JObject messageToDevice = JSONHandler.messageRetour("disconnected", parsedJson[1], parsedJson[2]);
                //--Envoi du message
                
                //--Deconnexion de l'appareil--

            }
                //Reception d'un message
            else if (parsedJson[0].ToLower() == "notification")
            {
                notification.Application = parsedJson[1];
                notification.Message = parsedJson[2];
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
            device = Devices.Devices.FirstOrDefault(o => o.Name == name);
            device.ListMessages.Add(notification);
            CommunicationViewModel communicationViewModel = (CommunicationViewModel) PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = message;


        }

        public void CallBackAfterConnexion(String name, Socket clientDevice)
        {
            Device newDevice = new Device(name, clientDevice);
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString() + "- Device connecté : " + newDevice.Name);
            }
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Device connecté";
            Devices.addDevice(newDevice);
            OnPropertyChanged("Devices");

            var dataAccess = new XmlAccess("./data.xml");
            dataAccess.saveDevice(newDevice);
        }

        public void DisplayNotif(string title, string content, string type)
        {
            NotificationView notif = new NotificationView();
            NotificationViewModel notifContext = new NotificationViewModel(title, content);
            notif.DataContext = notifContext;

            int slideOutTimer = 5;

            if (type == "appel?")
            {
                slideOutTimer = 30;
                // Ajouts boutons décrocher/racrocher
            } else if (type == "demande_connexion")
            {
                // Ajouts boutons accepter/refuser
            }
            notif.displayNotif(slideOutTimer);
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
 