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
        public String name;
        public List<BaseMessage> listMessages;
        public Socket handler;

        public Device()
        {
            listMessages = new List<BaseMessage>();
        }

        public Device(String name, Socket handler)
        {
            this.name = name;
            this.listMessages = new List<BaseMessage>();
            this.handler = handler;
        }
    }
}
