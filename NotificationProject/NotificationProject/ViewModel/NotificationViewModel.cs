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

        public NotificationViewModel()
        {
            this.myView = new NotificationView();
            this.createNotification("Message", "Salut c'est Thierry Lafronde !");
        }

        public void createNotification(String title, String content)
        {
            this.myView.displayMessage(title, content);
        }
    }
}
