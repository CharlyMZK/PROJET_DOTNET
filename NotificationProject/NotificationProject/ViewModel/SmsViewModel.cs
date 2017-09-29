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
        private DevicesController _devicesController;
        #endregion


        #region Properties
        public string Name
        {
            get
            {
                return "Sms View";
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
            }
        }
        #endregion

        #region Method
        private void SendMessage()
        {
            try
            {
                WriteConversationOnXml();
                SelectedDevice.sendMessage(JSONHandler.creationSMSString("bob", SelectedDevice.Name, SmsText, PhoneNumber));
            }
            catch (Exception ex)
            {
                //TODO popup message fail
                Console.WriteLine(ex);

            }
        }

        private bool CanSend()
        {
            return !String.IsNullOrEmpty(SmsText) && !String.IsNullOrEmpty(PhoneNumber) && SelectedDevice!=null;
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

        private void WriteConversationOnXml() {

            if (File.Exists(PhoneNumber + ".xml"))
            {
                string filename = PhoneNumber + ".xml";
                XmlDocument doc = new XmlDocument();
                //load from file
                doc.Load(filename);

                //create node and add value
                XmlNode node = doc.CreateNode(XmlNodeType.Element, "Envoi", null);
                //create title node
                XmlNode nodeDate = doc.CreateElement("DateTime");
                //add value for it
                nodeDate.InnerText = DateTime.Today.ToString();

                //create Url node
                XmlNode nodeMessage = doc.CreateElement("Message");
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
                using (XmlWriter writer = XmlWriter.Create(PhoneNumber + ".xml"))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Envoi");
                    writer.WriteElementString("DateTime", DateTime.Today.ToString());
                    writer.WriteElementString("Message", SmsText);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
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
