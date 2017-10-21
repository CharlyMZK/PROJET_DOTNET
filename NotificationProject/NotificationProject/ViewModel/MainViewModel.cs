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
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Timers;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace NotificationProject.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private ICommand _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private DevicesController _devicesController; 
        public string configPath = ConfigurationManager.AppSettings["XmlFilePath"];
        private List<string> listConversationName { get; set; }

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
            PageViewModels.Add(new ContactViewModel());
            PageViewModels.Add(new ConfigurationViewModel());
            PageViewModels.Add(new EtatViewModel());
            // Set default page
            CurrentPageViewModel = PageViewModels[0];
            _devicesController = DevicesController.getInstance();
            this.StartServer();
            //this.DisplayNotif("Connexion", "Vous avez un appel de Tony Stark sur l'appareil 'LGG4'", "appel", null, this.exempleCallbackDisplayNotif, this.exempleCallbackDisplayNotif); // Decommenter pour avoir un apercu d'une notif
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
                    this.DisplayNotif("Connexion", "Vous êtes désormais connecté avec l'appareil " + connectionReq.Appareil, "Connection", null, null, null);
                }
                else
                {
                    this.DisplayNotif("Connexion", "Echec de connexion avec l'appareil " + connectionReq.Appareil + ". La clé temporaire n'est plus correcte, veuillez réessayer.", "Message", null, null, null);
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
                if(parsedJson[1].Contains("messaging")) {
                    if (!String.IsNullOrEmpty(parsedJson[3]))
                    {
                        var contact = parsedJson[3].Split(':')[0];
                        var content = parsedJson[3].Split(':')[1];
                        RetrieveSms(contact, content, parsedJson[4]);
                    }
                }
               
                addMessage = true;
                this.DisplayNotif("Message", notification.Message, "Notification", null, null, null);
            }

            else if (parsedJson[0].ToLower() == "batterystate")
            {
                device = Devices.Devices.FirstOrDefault(o => o.Name == name);
                if (parsedJson[1] != "")
                    device.Pourcentage = parsedJson[1];
                else
                    device.Pourcentage = "Non renseigné.";

                if (parsedJson[2] != "")
                    device.Etat = parsedJson[2];
                else
                    device.Etat = "Non renseigné.";
                EtatViewModel etatViewModel = (EtatViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Etat");
                etatViewModel.OnPropertyChanged("Devices");


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
            }
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

            //newDevice.sendMessage(JSONHandler.creationContactRequest("bob", newDevice.Name));
            //var dataAccess = new XmlAccess("./data.xml");
            //dataAccess.saveDevice(newDevice);
        }

        public void DisplayNotif(string title, string content, string type, string application, Action callbackYes, Action callbackNo)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate
            {
                int slideOutTimer = 5;
                if (type == "appel")
                {
                    slideOutTimer = 30;
                }

                NotificationView notif = new NotificationView();
                NotificationViewModel notifContext = new NotificationViewModel(title, content, type, application, callbackYes, callbackNo);
                notif.DataContext = notifContext;
                notif.displayNotif(slideOutTimer);
            });
        }

        public void exempleCallbackDisplayNotif()
        {
            Console.WriteLine("Action car il a accepté/refusé");
        }


        public void RetrieveSms(string contact, string content, string datetime)
        {

            var regex = new Regex("[0-9]");
            Contact realContact = Contact.GetContact();
            string number = "";
            if (regex.IsMatch(contact))
            {
                number = contact.Replace(" ", "").Substring(3).ToString();
                DevicesController.getInstance().Devices.ForEach(x => realContact = x.GetContactByNumber(number));
            }

            Sms result = new Sms();

            result.IsOriginNative = false;
            result.SendHour = DateTime.Now;
            //result.SendHour = Convert.ToDateTime(datetime, culture);
                //DateTime.ParseExact((string)level2Element.Value, "dd-MM-yyyy HH:mm:ss",
                //                     System.Globalization.CultureInfo.InvariantCulture);
            result.Content = content;
            System.Windows.Application.Current.Dispatcher.Invoke(
                   DispatcherPriority.Normal,
                   (Action)delegate()
                   {
                       realContact.Chatter.Add(result);
                   }
               );
            
            string filename = configPath + realContact.Number + ".xml";

            if (File.Exists(filename))
            {
                XmlDocument doc = new XmlDocument();
                //load from file
                doc.Load(filename);

                //create node and add value
                XmlNode node = doc.CreateNode(XmlNodeType.Element, "Envoi", null);
                XmlAttribute attr = doc.CreateAttribute("native");
                attr.Value = "false";

                //Add the attribute to the node     
                node.Attributes.SetNamedItem(attr);
                //create title node
                XmlNode nodeDate = doc.CreateElement("DateTime");
                //add value for it
                nodeDate.InnerText = DateTime.Now.ToString();

                //create Url node
                XmlNode nodeMessage = doc.CreateElement("Content");
                nodeMessage.InnerText = content;

                //add to parent node
                node.AppendChild(nodeDate);
                node.AppendChild(nodeMessage);

                //add to elements collection
                doc.DocumentElement.AppendChild(node);

                //save back
                doc.Save(filename);

            }
            else
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Message");
                    writer.WriteStartElement("Envoi");
                    writer.WriteElementString("DateTime", DateTime.Now.ToString());
                    writer.WriteElementString("Content", content);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
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
