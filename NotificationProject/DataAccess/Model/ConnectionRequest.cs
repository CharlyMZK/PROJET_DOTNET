using DataAccess.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class ConnectionRequest : BaseMessage
    {
        public string Appareil { get; set; }
        public bool Acceptation { get; set; }
        public string Autor { get; set; }

        public ConnectionRequest(string Appareil, string Autor)
        {
            this.Appareil = Appareil;
            this.Autor = Autor;
        }
    }
}

