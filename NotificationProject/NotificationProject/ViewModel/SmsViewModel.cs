﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;
using System.Windows.Input;
using DataAccess.Model;
using System.Collections.ObjectModel;
using NotificationProjet.Controller;

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
                SelectedDevice.sendMessage(JSONHandler.creationSMSString("bob", SelectedDevice.Name, SmsText, PhoneNumber));
            }
            catch (Exception ex)
            {
                //TODO popup message fail
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
            }
        }

        private bool CanCall()
        {
            return !String.IsNullOrEmpty(PhoneNumber) && SelectedDevice != null;
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
