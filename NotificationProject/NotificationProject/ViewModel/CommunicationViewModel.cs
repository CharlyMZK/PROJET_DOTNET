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
        private ObservableCollection<Device> _listDevices;
        private ICommand _startServerCommand;
        public ObservableCollection<Device> ListDevices
        {
            get
            {
                if (_listDevices == null)
                {
                    _listDevices = new ObservableCollection<Device>();
                }
                return _listDevices;
            }
            set
            {
                _listDevices = value;
                OnPropertyChanged("ListDevices");
            }
        }

        #endregion

        #region Properties
        public String Name
        {
            get
            {
                return "Communication";
            }
        }

        public void addDevice(Device device)
        {
            ListDevices = new ObservableCollection<Device>(ListDevices);  // -- TODO : Its supposed to work without this line, but it throw an exception.
            ListDevices.Add(device);                                      // -- Add device on list
            OnPropertyChanged("ListDevices");                             // -- Notify the changes
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
    }
} 