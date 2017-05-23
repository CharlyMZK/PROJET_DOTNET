using NotificationProject.HelperClasses;
using NotificationProject.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationProject.ViewModel
{
    class NotificationViewModel : ObservableObject
    {

        private NotificationView myView;
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

        public NotificationViewModel()
        {
            this.myView = new NotificationView();
            this.createNotification("Message", "Salut c'est Thierry Lafronde !");
        }

        public void createNotification(String t, String c)
        {
            this.TitleNotif = t;
            this.ContentNotif = c;
            this.myView.displayNotif();
        }
    }
}
