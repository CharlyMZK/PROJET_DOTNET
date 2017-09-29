using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;

namespace NotificationProject.ViewModel
{
    class ContactViewModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Contact";
            }
        }
    }
}
