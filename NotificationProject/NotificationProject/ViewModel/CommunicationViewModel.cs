using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataAccess.Model;
using NotificationProject.HelperClasses;


namespace NotificationProject.ViewModel
{
    class CommunicationViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private string _communicationStatus;
        private ICommand _startServerCommand;
        private List<Device> _listDevices;

        #endregion

        #region Properties
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

        public List<Device> ListDevices
        {
            get
            {
                if(_listDevices == null)
                {
                    _listDevices = new List<Device>();
                }
                return _listDevices;
            }
            set
            {
                _listDevices = value;
                OnPropertyChanged("ListDevices");
            }
        }

        public ICommand StartServerCommand
        {
            get
            {
                if(_startServerCommand == null)
                {
                    _startServerCommand = new RelayCommand(
                        param => this.StartServer(),
                        param =>this.CanStartServer()
                        );
                }
                return _startServerCommand;
            }
        }

        // Determine if the StartServer command should or should not be used
        private bool CanStartServer()
        {
            //Should use a bool and not the value of CommunicationStatus, but work for the moment
            return (CommunicationStatus != "Server Started");
        }

        public void CallBackAfterAnalysis(String message)
        {
            CommunicationStatus = message; 
        }

        public void CallBackAfterConnexion(String message)
        {
            CommunicationStatus = message;
        }


        private void StartServer()
        {
            CommunicationService.CommunicationService cs = new CommunicationService.CommunicationService();
            cs.callBackAfterConnexion = CallBackAfterConnexion; 
            CommunicationStatus = "Server Started"; 
        }


        #endregion
    }
} 