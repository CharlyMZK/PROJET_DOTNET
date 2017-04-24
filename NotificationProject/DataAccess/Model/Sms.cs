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

        public Contact Contact { get; set; }

        public string Content { get; set; }

        public Sms(Contact Contact, string Content)
        {
            this.Contact = Contact;
            this.Content = Content;
        }

        public string getTitle()
        {
            return Title;
        }

    }
}
