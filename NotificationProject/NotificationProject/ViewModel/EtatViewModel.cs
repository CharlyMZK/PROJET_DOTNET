using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;
using NotificationProjet.Controller;
using DataAccess.Model;
using System.Collections.ObjectModel;

namespace NotificationProject.ViewModel
{
    class EtatViewModel : ObservableObject, IPageViewModel
    {
        #region constructor
        public EtatViewModel()
        {
            _devicesController = DevicesController.getInstance();
        }
        #endregion constructor

        #region fields
        private DevicesController _devicesController;
        #endregion fields

        #region Properties
        public string Name
        {
            get
            {
                return "Etat";
            }
        }



        public ObservableCollection<Device> Devices
        {
            get
            {
                return new ObservableCollection<Device>(_devicesController.Devices);
            }
        }
        #endregion Properties





    }
}
