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
        public List<BaseMessage> ListMessages { get; set; }
        public Socket Handler { get; set; }

        public Device()
        {
            ListMessages = new List<BaseMessage>();
        }

        public Device(String name, Socket handler)
        {
            this.Name = name;
            this.ListMessages = new List<BaseMessage>();
            this.Handler = handler;
        }
    }
}
