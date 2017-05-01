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

namespace NotificationProject.ViewModel
{
    class CommunicationViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private string _communicationStatus;
        private DevicesController _devicesController;
        private ICommand _startServerCommand;

        #endregion

       public CommunicationViewModel(DevicesController devicesController)
        {
            _devicesController = devicesController;
        }

        #region Properties
        public ObservableCollection<Device> ListDevices
        {
            get
            {
                if (_devicesController == null)
                {
                    _devicesController = new DevicesController();
                }
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

        #endregion
        #region method

        #endregion
    }
} 