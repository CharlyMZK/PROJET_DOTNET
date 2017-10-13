using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Model;
using NotificationProject.HelperClasses;
using System.Windows.Input;

namespace NotificationProject.ViewModel
{
    class ConfigurationViewModel : ObservableObject, IPageViewModel
    {
        #region Properties
        public string Name
        {
            get
            {
                return "Configuration";
            }
        }

        private NotificationConfiguration _config;

        public NotificationConfiguration Config
        {
            get
            {
                if (_config == null)
                    _config = NotificationConfiguration.getInstance();
                return _config;
            }
        }
        #endregion Properties

        #region Method
        public void changeOption()
        {

        }
        #endregion Method

        public bool canChangeOption()
        {
            return true;
        }

        #region Command
        private ICommand _changeOptionCommand;
        public ICommand ChangeOptionCommand
        {
            get
            {
                if (_changeOptionCommand == null)
                    _changeOptionCommand = new RelayCommand(o => changeOption(), n => canChangeOption());
                return _changeOptionCommand;
            }
        }
        #endregion Command

    }
}
