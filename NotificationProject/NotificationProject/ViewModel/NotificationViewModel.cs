using NotificationProject.HelperClasses;
using NotificationProject.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationProject.ViewModel
{
    public class NotificationViewModel : ObservableObject
    {
        private string _title;
        public string TitleNotif
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        private string _content;
        public string ContentNotif
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }

        public NotificationViewModel(string t, string c)
        {
            this.TitleNotif = t;
            this.ContentNotif = c;
        }
    }
}
