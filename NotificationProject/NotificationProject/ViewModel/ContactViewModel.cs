using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;
using DataAccess.Model;
using System.Collections.ObjectModel;
using NotificationProjet.Controller;

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
        #endregion Properties


    }
}
