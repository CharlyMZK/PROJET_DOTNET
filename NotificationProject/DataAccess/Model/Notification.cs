using DataAccess.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class Notification : BaseMessage
    {
        public string Application { get; set; }

        public string Message { get; set; }

        public Notification(string Application, string Message)
        {
            this.Application = Application;
            this.Message = Message;
        }
    }
}
