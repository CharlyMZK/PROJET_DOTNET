using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Model;
using DataAccess.Model.Base;
using System.Net.Sockets;
using System.Collections.ObjectModel;

namespace DataAccess.Model
{
    public class Device
    {
        public String Name { get; set; }
        public String Etat { get; set; }
        public String Pourcentage { get; set; }
        public List<Notification> ListMessages { get; set; }
        public Socket Handler { get; set; }
        public ObservableCollection<Contact> listContact { get; set; }

        public Device()
        {
            ListMessages = new List<Notification>();
            this.listContact = new ObservableCollection<Contact>();
        }

        public Device(String name, Socket handler)
        {
            this.Name = name;
            this.ListMessages = new List<Notification>();
            this.Handler = handler;
            this.listContact = new ObservableCollection<Contact>();
        }

        public void sendMessage(String message)
        {
            if (Handler != null)
            {
                Handler.Send(Encoding.UTF8.GetBytes(message));
            }

        }
    }
}
