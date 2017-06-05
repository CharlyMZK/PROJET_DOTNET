using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Model;
using DataAccess.Model.Base;
using System.Net.Sockets;

namespace DataAccess.Model
{
    public class Device
    {
        public String Name { get; set; }
        public List<Notification> ListMessages { get; set; }
        public Socket Handler { get; set; }

        public Device()
        {
            ListMessages = new List<Notification>();
        }

        public Device(String name, Socket handler)
        {
            this.Name = name;
            this.ListMessages = new List<Notification>();
            this.Handler = handler;
        }

        public void sendMessage(String message)
        {
            if(Handler != null)
            {
                Handler.Send(Encoding.UTF8.GetBytes(message));
            }
            
        }
    }
}
