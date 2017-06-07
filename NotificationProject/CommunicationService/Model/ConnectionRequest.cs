using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Model
{
    public class ConnectionRequest
    {
        public string Appareil { get; set; }
        public bool Acceptation { get; set; }
        public string Autor { get; set; }
        public DateTime SendHour { get; set; }


        public ConnectionRequest(string Appareil, string Autor)
        {
            this.Appareil = Appareil;
            this.Autor = Autor;
        }
    }
}

