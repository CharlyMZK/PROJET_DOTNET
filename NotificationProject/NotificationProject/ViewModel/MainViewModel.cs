using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using NotificationProject.HelperClasses;
using BusinessLayer;
using System;
using System.Net.Sockets;
using NotificationProject.View;
using DataAccess.Model;
using Newtonsoft.Json.Linq;

namespace NotificationProject.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private ICommand _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private DevicesController _devicesController;
       
        public event PropertyChangedEventHandler PropertyChanged;

     
        #endregion Fields

        #region constructor

        public MainViewModel() 
        {
            //Add the pages
            PageViewModels.Add(new HomeViewModel());
            PageViewModels.Add(new QRCodeViewModel());
            PageViewModels.Add(new CommunicationViewModel(Devices));
            // Set default page
            CurrentPageViewModel = PageViewModels[0];
            this.StartServer();
        }

        #endregion

        #region properties
        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();
                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if(_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        public DevicesController Devices
        {
            get
            {
                if (_devicesController == null)
                    _devicesController = new DevicesController();
                return _devicesController;
            }
            set
            {
                _devicesController = value;
            }
        }
        #endregion

        #region Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if(_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }
                return _changePageCommand;
            }
        }
        #endregion

        #region methods

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);
            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        #endregion



        public void CallBackAfterAnalysis(String name, String message)
        {

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString() + "- Message reçu : " + message);
            }

            //JObject jsonMessage = JSONHandler.stringToJson(message);
            //Notification notification = JSONHandler.interpretation(jsonMessage);
            //Device device = Devices.Devices.FirstOrDefault(o => o.Name == name);
            //device.ListMessages.Add(notification);

            // -- TODO : Remove its a test
            foreach (Device d in Devices.Devices)
            {
                Console.WriteLine("Nom du device : " + d.Name);
                foreach (Notification n in d.ListMessages)
                {
                    Console.WriteLine("Message : " + n.Message);

                }
            }
            // -- 

            CommunicationViewModel communicationViewModel = (CommunicationViewModel) PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = message;


        }

        public void CallBackAfterConnexion(String name, Socket clientDevice)
        {
            Device newDevice = new Device(name, clientDevice);
           
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString() + "- Device connecté : " + newDevice.Name);
            }
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Device connecté";
            Devices.addDevice(newDevice);
            OnPropertyChanged("Devices");
        }


        private void StartServer()
        {
            CommunicationService cs = CommunicationService.getInstance();
            cs.callBackAfterConnexion = CallBackAfterConnexion;
            cs.callBackAfterAnalysis = CallBackAfterAnalysis;
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Server Started";
        }


    }
}
 