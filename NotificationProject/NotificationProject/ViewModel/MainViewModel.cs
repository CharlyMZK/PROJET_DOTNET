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
using System.Collections.ObjectModel;

namespace NotificationProject.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        #region Fields

        private ICommand _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
       
        public event PropertyChangedEventHandler PropertyChanged;

     
        #endregion Fields

        #region constructor

        public MainViewModel() 
        {
            //Add the pages
            PageViewModels.Add(new HomeViewModel());
            PageViewModels.Add(new QRCodeViewModel());
            PageViewModels.Add(new CommunicationViewModel());
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
            // Device device = ListDevices.First(d => d.Name == name);
            /*Device device = ListDevices.First();
            device.ListMessages.Add(new Notification("", message));*/

            /*using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString() + "- Message reçu : " + message);
            }*/


            CommunicationViewModel communicationViewModel = (CommunicationViewModel) PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = message;

        }

        public void CallBackAfterConnexion(String name, Socket clientDevice)
        {
            Device newDevice = new Device(name, clientDevice);
            
            /*using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"log.txt", true))
            {
                file.WriteLine(DateTime.Now.ToString() + "- Device connecté : " + newDevice.Name);
            }*/
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Device connecté";
            communicationViewModel.addDevice(newDevice);

        }


        private void StartServer()
        {
            CommunicationService cs = new CommunicationService();
            cs.callBackAfterConnexion = CallBackAfterConnexion;
            cs.callBackAfterAnalysis = CallBackAfterAnalysis;
            CommunicationViewModel communicationViewModel = (CommunicationViewModel)PageViewModels.FirstOrDefault(o => o.Name == "Communication");
            communicationViewModel.CommunicationStatus = "Server Started";
        }

    }
}
 