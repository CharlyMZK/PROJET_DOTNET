﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;
using DataAccess.Model;
using System.Collections.ObjectModel;
using NotificationProjet.Controller;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace NotificationProject.ViewModel
{
    class ContactViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private DevicesController _devicesController;
        #endregion Fields

        #region Constructor
        public ContactViewModel()
        {
            _devicesController = DevicesController.getInstance();
        }
        #endregion Constructor

        #region Properties
        public string Name
        {
            get
            {
                return "Contact";
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
                // _devicesController.Devices = value;
                OnPropertyChanged("ListDevices");
            }
        }

        private Device _selectedDevice;
        public Device SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                // Tell to the view that SelectedDevice has changed
                OnPropertyChanged("Messages");
            }
        }

        private List<Contact> _contacts;
        public List<Contact> Contacts
        {
            get
            {
                if (_contacts == null)
                    _contacts = new List<Contact>();

                //DEBUG
                _contacts.Add(new Contact("TEST DEBUG", "+33646690454", "test@test.fr"));
                _contacts.Add(new Contact("TEST DEBUG 2", "0646690454", "test@test.fr"));

                return _contacts;
            }
        }

        private BitmapSource _imageSource;

        public BitmapSource ImageSource
        {
            get
            {
                if (_imageSource == null)
                {
                    _imageSource = null;
                }
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }
        #endregion Properties

        #region Method
        private void getContact()
        {
            throw new NotImplementedException();
        }

        private bool canGetContact()
        {
            return true;
        }
        #endregion Method

        #region Command
        #region Command
        private ICommand _getContactCommand;
        public ICommand GetContactCommand
        {
            get
            {
                if (_getContactCommand == null)
                    _getContactCommand = new RelayCommand(o => getContact(), n => canGetContact());
                return _getContactCommand;
            }
        }
        #endregion Command
        #endregion


    }
}
