using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataAccess.Model;
using NotificationProject.HelperClasses;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using BusinessLayer;
using DataAccess.Model.Base;
using NotificationProjet.Controller;

namespace NotificationProject.ViewModel
{
    class CommunicationViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private Device _selectedDevice;
        private string _communicationStatus;
        private DevicesController _devicesController;
        private ICommand _startServerCommand;

        #endregion

       public CommunicationViewModel()
        {
            _devicesController = DevicesController.getInstance();
        }

        #region Properties
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

        public String Name
        {
            get
            {
                return "Communication";
            }
        }

        public String CommunicationStatus
        {
            get
            {
                if(_communicationStatus == null)
                {
                    _communicationStatus = "Server not started";
                }
                return _communicationStatus;
            }
            set
            {
                _communicationStatus = value;
                // Tell to the view that communicationStatus has changed
                OnPropertyChanged("CommunicationStatus");
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
                // Tell to the view that SelectedDevice has changed
                OnPropertyChanged("Messages");
            }
        }

        public ObservableCollection<Notification> Messages
        {
            get
            {
                if (SelectedDevice != null)
                {
                    return new ObservableCollection<Notification>(SelectedDevice.ListMessages);
                }
                return null;
               
            }
        }

        #endregion
        #region method

        #endregion
    }
} 