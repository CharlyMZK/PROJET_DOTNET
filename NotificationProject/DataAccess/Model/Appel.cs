using DataAccess.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class Appel : BaseMessage
    {
        public string Title = "Appel";
        public Contact Contact { get; set; }

        public Appel(Contact Contact)
        {
            this.Contact = Contact;
        }

        public string getTitle()
        {
            return Title;
        }
    }
}
