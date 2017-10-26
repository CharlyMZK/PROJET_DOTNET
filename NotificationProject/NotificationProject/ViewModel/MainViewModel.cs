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
        private Dictionary<string, Device> devicesWaitingToBeConnectedList = new Dictionary<string, Device>();

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
            //this.DisplayNotif("Appel", "Vous avez un appel de Tony Stark sur l'appareil 'LGG4'", "appel", null, this.exempleCallbackDisplayNotif, this.exempleCallbackDisplayNotif, null); // Decommenter pour avoir un apercu d'une notif
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
            Device deviceWaitingToBeConnected = null;
            //Conversion et traitement et parsing d'un JSON
            JObject jsonMessage = JSONHandler.stringToJson(message);
            string[] parsedJson = JSONHandler.interpretation(jsonMessage);
            string messageReceivedType = parsedJson[0].ToLower();
            Device device = new Device();
            Boolean addMessage = false;
            //Interprétation du JSON parsé

            //Demande de connexion
            if (messageReceivedType == "connection")
            {
                connectionReq.Appareil = parsedJson[1];
                connectionReq.Autor = parsedJson[2];
                var pairaineKey = parsedJson[2].Split(':');
                var connexionId = pairaineKey[0] + ":" + pairaineKey[1] + "@" + pairaineKey[2];


                deviceWaitingToBeConnected = getDeviceWaitingToBeConnected(name);
                deviceWaitingToBeConnected.ConnexionId = connexionId;
                if (isTestClient(connectionReq) || (int.Parse(pairaineKey[2]) == CommunicationService.getInstance().randomSecretNumberAccess))
                {
                    //device = Devices.Devices.FirstOrDefault(o => o.Name == name);
                    notification.Application = parsedJson[1];
                    notification.Message = "demande de connexion";
                    Console.WriteLine("Successfuly connexion !");
                    deviceWaitingToBeConnected.DeviceType = connectionReq.Appareil;
                    this.DisplayNotif("connexion", "L'appareil " + connectionReq.Appareil + " tente de se connecter", "connexion", "appel", this.callBackYesOnConnexion, this.callBackNoOnConnexion, deviceWaitingToBeConnected);
                }
                else
                {
                    deviceWaitingToBeConnected.sendMessage(JSONHandler.creationRefuseConnexionRequest(deviceWaitingToBeConnected.ConnexionId, "Server"));
                    this.DisplayNotif("connexion", "Echec de connexion avec l'appareil " + connectionReq.Appareil + ". La clé temporaire n'est plus correcte, veuillez réessayer.", "Message", "appel", null, null, null);
                }
                addMessage = true;
            }
            //Demande de deconnexion
            else if (messageReceivedType == "disconnection")
            {
                
                device = Devices.Devices.FirstOrDefault(o => o.Name == name);
                CallBackAfterDeconnexion(device);
                //--Envoi du message

                //--Deconnexion de l'appareil--

                // Notifaction des vues
                CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
                communicationViewModel.OnPropertyChanged("ListDevices");
                SmsViewModel smsViewModel = (SmsViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Sms View");
                if(smsViewModel != null)
                    smsViewModel.OnPropertyChanged("ListDevices");

            }
            //Reception d'un message
            else if (messageReceivedType == "notification")
            {
                notification.Application = parsedJson[1];

                addMessage = true;
                var config = NotificationConfiguration.getInstance();
                if (NotificationConfiguration.getInstance().IsEnabled)
                {
                    addMessage = isMessageDisplayAuthorised(notification);
                }

                notification.Message = parsedJson[3];

                if (addMessage)
                {
                    RetrieveSms(parsedJson[3], parsedJson[4], name);
                    this.DisplayNotif("Message", notification.Message, "Notification", null, null, null, null);
                }
            }
            else if (messageReceivedType == "batterystate")
            {
                handleBatteryState(name, parsedJson);
            }


            if(messageReceivedType != "connection")
            {
                if (addMessage)
                {
                    handleMessageAdded(name, notification, message);
                }
            }
        }

        public Device getDeviceWaitingToBeConnected(String name)
        {
            Device deviceWaitingToBeConnected = null;
            if (devicesWaitingToBeConnectedList.ContainsKey(name))
            {
                deviceWaitingToBeConnected = devicesWaitingToBeConnectedList[name];
            }
            return deviceWaitingToBeConnected;
        }

        public Boolean isMessageDisplayAuthorised(Notification notification)
        {
            Boolean addMessage = true;
            Boolean notificationHandled = false;

            if ((notification.Application.Equals("com.android.sms") || notification.Application.Equals("com.android.mms")))
            {
                if (NotificationConfiguration.getInstance().SmsEnabled)
                {
                    Console.WriteLine("SMS reçu et traité");
                }
                else
                {
                    addMessage = false;
                    Console.WriteLine("Sms reçu et non traité");
                }
                notificationHandled = true;
            }
            if (notificationHandled == false) 
            {
                if (notification.Application.Equals("com.android.server.telecom") || notification.Application.Equals("com.android.incallui"))
                {
                    if (NotificationConfiguration.getInstance().CallEnabled)
                    {
                        Console.WriteLine("Appel reçu et traité");

                    }
                    else
                    {
                        addMessage = false;
                        Console.WriteLine("Appel reçu et non traité");
                    }
                    notificationHandled = true;
                }
            }
            if (notificationHandled == false)
            {
                if (NotificationConfiguration.getInstance().OtherEnabled)
                {
                    Console.WriteLine("Autre reçu et traité");

                }
                else
                {
                    addMessage = false;
                    Console.WriteLine("Autre reçu et non traité");
                }
                notificationHandled = true;
            }
            return addMessage;
        }

        public void handleMessageAdded(String name, Notification notification, String message)
        {
            Device device = Devices.Devices.FirstOrDefault(o => o.Name == name);
            device.ListMessages.Add(notification);
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = message;
            communicationViewModel.OnPropertyChanged("Messages");
        }

        public void handleBatteryState(String name, String[] clientMessage)
        {

            Device device = Devices.Devices.FirstOrDefault(o => o.Name == name);
            if (device == null)
                return;
            if (clientMessage[1] != "")
                device.Pourcentage = clientMessage[1];
            else
                device.Pourcentage = "Non renseigné.";

            if (clientMessage[2] != "")
                device.Etat = clientMessage[2];
            else
                device.Etat = "Non renseigné.";
            EtatViewModel etatViewModel = (EtatViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Etat");
            etatViewModel.OnPropertyChanged("Devices");
        }

        public Boolean isTestClient(ConnectionRequest connectionReq)
        {
            return connectionReq.Appareil.Equals("CommunicationClientTester");
        }

        public void CallBackAfterDeconnexion(Device clientDevice)
        {
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Device déconnecté";
            clientDevice.sendMessage(JSONHandler.creationDisconnectString("bob", clientDevice.Name));
            Devices.deleteDevice(clientDevice);
        }

        public void CallBackAfterConnexion(String name, Socket clientDevice)
        {
            Device newDevice = new Device(name, clientDevice);
            devicesWaitingToBeConnectedList.Add(newDevice.Name, newDevice);
        }

        public void DisplayNotif(string title, string content, string type, string application, Action<Device> callbackYes, Action<Device> callbackNo, Device d)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, (ThreadStart)delegate
            {
                int slideOutTimer = 5;
                if (type == "appel")
                {
                    slideOutTimer = 30;
                }
                if (type == "connexion")
                {
                    slideOutTimer = 120;
                }

                NotificationView notif = new NotificationView();
                NotificationViewModel notifContext = new NotificationViewModel(title, content, type, application, callbackYes, callbackNo, d);
                notif.DataContext = notifContext;
                notif.displayNotif(slideOutTimer);
            });
        }

        public void callBackYesOnConnexion(Device deviceWaitingToBeConnected)
        {
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Device connecté";
            Devices.addDevice(deviceWaitingToBeConnected);
            OnPropertyChanged("Devices");
            communicationViewModel.OnPropertyChanged("ListDevices");
            deviceWaitingToBeConnected.sendMessage(JSONHandler.creationAcceptConnexionRequest(deviceWaitingToBeConnected.ConnexionId, "Server"));
            deviceWaitingToBeConnected.sendMessage(JSONHandler.creationContactRequest(deviceWaitingToBeConnected.ConnexionId, "Server"));
            this.DisplayNotif("Message", "Connexion établie", "Notification", null, null, null, null);
        }

        public void callBackNoOnConnexion(Device deviceWaitingToBeConnected)
        {
            devicesWaitingToBeConnectedList.Remove(deviceWaitingToBeConnected.Name);
            deviceWaitingToBeConnected.sendMessage(JSONHandler.creationRefuseConnexionRequest(deviceWaitingToBeConnected.ConnexionId, "Server"));
            this.DisplayNotif("Message", "La connexion a été refusée", "Notification", null, null, null, null);
        }

        public void exempleCallbackDisplayNotif(Device d)
        {
            Console.WriteLine("Action car il a accepté/refusé");
        }

        public void RetrieveSms(string content, string datetime, string deviceName)
        {
            if (content != "" && content != null)
            {
                var sender = content.Split(':')[0].TrimEnd();
                Sms result = new Sms();

                result.IsOriginNative = false;
                result.SendHour = DateTime.Now;
                if(content != null)
                {
                    result.Content = content.Split(':')[1].TrimStart();
                }

                var device = Devices.Devices.FirstOrDefault(o => o.Name == deviceName);
                Contact contact = device.listContact.Where(x => x.Name == sender).FirstOrDefault();

                if (contact == null)
                {
                    contact = new Contact(sender, sender, "");
                    System.Windows.Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Normal,
                       (Action)delegate()
                       {
                           device.listContact.Add(contact);
                       }
                   );
                }

                System.Windows.Application.Current.Dispatcher.Invoke(
                       DispatcherPriority.Normal,
                       (Action)delegate()
                       {
                           contact.Chatter.Add(result);
                           contact.HasSentNewMessage = true;
                       }
                   );

                

                string filename =  configPath + contact.Number + ".xml";

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
                    nodeMessage.InnerText = result.Content;

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
                        writer.WriteAttributeString("native", "false");
                        writer.WriteElementString("DateTime", DateTime.Now.ToString());
                        writer.WriteElementString("Content", result.Content);
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
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
