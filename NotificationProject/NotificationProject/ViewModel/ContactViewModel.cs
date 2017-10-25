using System;
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
using System.Windows;
using System.Windows.Interop;

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
            var hbmp = NotificationProject.Properties.Resources.synchronize.GetHbitmap();
            _imageSource = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

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
                OnPropertyChanged("SelectedDevice");
                this.Contacts = value.listContact;
            }
        }

        private ObservableCollection<Contact> _contacts;
        public ObservableCollection<Contact> Contacts
        {
            get
            {
                //return new ObservableCollection(SelectedDevice.listContact);

                if (_contacts == null)
                    _contacts = new ObservableCollection<Contact>();

                return _contacts;
            }
            set
            {
                _contacts = value;
                OnPropertyChanged("Contacts");
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
            if (_selectedDevice != null)
            {
                JSONHandler.creationContactRequest("bob", _selectedDevice.Name);
            }

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
