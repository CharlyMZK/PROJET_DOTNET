using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;
using System.Windows.Input;
using DataAccess.Model;
using System.Collections.ObjectModel;

namespace NotificationProject.ViewModel
{
    class SmsViewModel : ObservableObject, IPageViewModel
    {

        #region constructor
        public SmsViewModel()
        {
            ButtonCommand = new RelayCommand(o => SendMessage(), n => CanSend());
            _devicesController = DevicesController.getInstance();
        }
        #endregion

        #region Fields
        private string _phoneNumber;
        private string _smsText;
        public Device _selectedDevice;
        private DevicesController _devicesController;
        public ICommand ButtonCommand { get; set; }
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
                return _devicesController.Devices;
            }
            set
            {
                _devicesController.Devices = value;
                OnPropertyChanged("ListDevices");
            }
        }
        #endregion
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

        #region Command
        private void SendMessage()
        {
            SelectedDevice.sendMessage(JSONHandler.creationSMSString("bob",PhoneNumber,SelectedDevice.Name,SmsText));
        }

        private bool CanSend()
        {
            return !String.IsNullOrEmpty(SmsText) && !String.IsNullOrEmpty(PhoneNumber) && SelectedDevice!=null;
        }
        #endregion


    }
}
