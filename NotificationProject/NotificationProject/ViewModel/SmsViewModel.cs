using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;
using System.Windows.Input;
using DataAccess.Model;
using System.Collections.ObjectModel;
using NotificationProjet.Controller;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Configuration;

namespace NotificationProject.ViewModel
{
    class SmsViewModel : ObservableObject, IPageViewModel
    {

        #region constructor
        public SmsViewModel()
        {
            _devicesController = DevicesController.getInstance();
        }
        #endregion

        #region Fields
        private string _phoneNumber;
        private string _smsText;
        public Device _selectedDevice;
        public Contact _selectedContact;
        public string configPath = ConfigurationManager.AppSettings["XmlFilePath"];
        private DevicesController _devicesController;
        #endregion


        #region Properties
        public string Name
        {
            get
            {
                return "Conversation";
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
            }
        }

        private string ContactPhoneNumber { get; set; }


        public string SmsText
        {
            get
            {
                return _smsText;
            }
            set
            {
                _smsText = value;
            }
        }

        public ObservableCollection<Device> ListDevices
        {
            get
            {
                return new ObservableCollection<Device>(_devicesController.Devices);
            }
            set
            {
                OnPropertyChanged("ListDevices");
            }
        }

        public Device SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                _selectedDevice.listContact.Add(Contact.GetContact());
                _selectedDevice.listContact.Add(Contact.GetContact2());
                ListContacts = _selectedDevice.listContact;
            }
        }

        public ObservableCollection<Contact> ListContacts
        {
            get
            {
                if (SelectedDevice != null)
                    return new ObservableCollection<Contact>(SelectedDevice.listContact);
                return null;  
            }
            set
            {
                OnPropertyChanged("ListContacts");
                if (_selectedContact != null)
                {
                    ContactPhoneNumber = _selectedContact.Number;
                    _selectedContact.Chatter.Clear();
                    RetrieveConversation(_selectedContact);
                }
            
            }
        }

        public Contact SelectedContact
        {
            get
            {
               return _selectedContact;
            }
            set
            {
                _selectedContact = value;
                 ContactPhoneNumber = _selectedContact.Number;
                 _selectedContact.Chatter.Clear();
                RetrieveConversation(_selectedContact);
                if (_selectedContact.HasSentNewMessage) _selectedContact.HasSentNewMessage = false;
                OnPropertyChanged("SelectedContact");
            }
        }
        #endregion

        #region Method
        private void SendMessage()
        {
            try
            {
                var realNumber = (String.IsNullOrEmpty(PhoneNumber)) || PhoneNumber == null 
                    ? ContactPhoneNumber 
                    : PhoneNumber;
                WriteConversationOnXml(realNumber);
                SelectedDevice.sendMessage(JSONHandler.creationSMSString("bob", SelectedDevice.Name, SmsText, realNumber));

                SmsText = "";
            }
            catch (Exception ex)
            {
                //TODO popup message fail
                Console.WriteLine(ex);

            }
        }

        private bool CanSend()
        {
            return !String.IsNullOrEmpty(SmsText) && (!String.IsNullOrEmpty(PhoneNumber) || !String.IsNullOrEmpty(ContactPhoneNumber)) && SelectedDevice!=null;
        }

        private void Call()
        {
            try
            {
                SelectedDevice.sendMessage(JSONHandler.creationAppelString("bob", SelectedDevice.Name, PhoneNumber));
            }
           catch(Exception ex)
            {
                //TODO faire popup fail message
                Console.WriteLine(ex);
            }
        }

        private bool CanCall()
        {
            return !String.IsNullOrEmpty(PhoneNumber) && SelectedDevice != null;
        }

        private void WriteConversationOnXml(string number) {
            string filename = configPath + number + ".xml";

            if (File.Exists(filename))
            {
                _selectedContact.Chatter.Add(new Sms(DateTime.Now, SmsText, true));

                XmlDocument doc = new XmlDocument();
                //load from file
                doc.Load(filename);

                //create node and add value
                XmlNode node = doc.CreateNode(XmlNodeType.Element, "Envoi",null);
                XmlAttribute attr = doc.CreateAttribute("native");
                attr.Value = "true";

                //Add the attribute to the node     
                node.Attributes.SetNamedItem(attr);
                //create title node
                XmlNode nodeDate = doc.CreateElement("DateTime");
                //add value for it
                nodeDate.InnerText = DateTime.Now.ToString();

                //create Url node
                XmlNode nodeMessage = doc.CreateElement("Content");
                nodeMessage.InnerText = SmsText;

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
                Contact newContact = new Contact(PhoneNumber, PhoneNumber, "");
                newContact.Chatter.Add(new Sms(DateTime.Now, SmsText, true));
                SelectedDevice.listContact.Add(newContact);
                ListContacts = SelectedDevice.listContact;

                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Message");
                    writer.WriteStartElement("Envoi");
                    writer.WriteAttributeString("native", "true");
                    writer.WriteElementString("DateTime", DateTime.Now.ToString());
                    writer.WriteElementString("Content", SmsText);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
           
        }

        private void RetrieveConversation(Contact contact)
        {
            contact.Chatter.Clear();
            var filename = configPath + contact.Number + ".xml";
            if (File.Exists(filename))
            {
                foreach (XElement level1Element in XElement.Load(filename).Elements("Envoi"))
                {
                    Sms result = new Sms();

                    result.IsOriginNative = (level1Element.Attribute("native").Value == "true") ? true : false;
                    foreach (XElement level2Element in level1Element.Elements("DateTime"))
                    {
                        result.SendHour = Convert.ToDateTime((string)level2Element.Value);
                        //DateTime.ParseExact((string)level2Element.Value, "dd-MM-yyyy HH:mm:ss",
                        //                     System.Globalization.CultureInfo.InvariantCulture);
                    }

                    foreach (XElement level2Element in level1Element.Elements("Content"))
                    {
                        result.Content = level2Element.Value;
                    }
                    contact.Chatter.Add(result);
                }
            }
        }

       
        #endregion

        #region Command
        private ICommand _sendSmsCommand;
        public ICommand SendSmsCommand
        {
            get
            {
                if(_sendSmsCommand == null)
                    _sendSmsCommand = new RelayCommand(o => SendMessage(), n => CanSend());
                return _sendSmsCommand;
            }
        }

        private ICommand _callCommand;
        public ICommand CallCommand
        {
            get
            {
                if (_callCommand == null)
                    _callCommand = new RelayCommand(o => Call(), n => CanCall());
                return _callCommand;
            }
        }
        #endregion


    }
}
