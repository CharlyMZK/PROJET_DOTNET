﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NotificationProject.HelperClasses;

namespace NotificationProject.ViewModel
{
    class CommunicationViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private string _communicationStatus;
        private ICommand _startServerCommand;

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

        private void StartServer()
        {
            CommunicationService.CommunicationService cs = new CommunicationService.CommunicationService();
            CommunicationStatus = "Server Started";
        }


        #endregion
    }
} 