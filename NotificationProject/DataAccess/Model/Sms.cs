using DataAccess.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class Sms : BaseMessage
    {
        public string Title = "SMS";

        public string Content { get; set; }
        public bool IsOriginNative { get; set; }

        public Sms(DateTime time, string Content, bool IsOriginNative)
        {
            this.Content = Content;
            this.IsOriginNative = IsOriginNative;
            this.SendHour = time;
        }

        public Sms() { }

        public string getTitle()
        {
            return Title;
        }

    }
}
