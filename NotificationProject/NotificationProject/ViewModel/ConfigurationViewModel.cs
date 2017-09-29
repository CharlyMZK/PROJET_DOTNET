using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Model;
using NotificationProject.HelperClasses;


namespace NotificationProject.ViewModel
{
    class ConfigurationViewModel : ObservableObject, IPageViewModel
    {

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



    }
}
