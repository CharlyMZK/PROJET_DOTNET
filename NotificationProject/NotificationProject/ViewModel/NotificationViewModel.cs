using NotificationProject.HelperClasses;
using NotificationProject.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationProject.ViewModel
{
    public class NotificationViewModel : ObservableObject
    {
        private string _title;
        public string TitleNotif
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        private string _content;
        public string ContentNotif
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }
        private string _type;
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
        private bool _affiche_boutons;
        public bool AfficheBoutons
        {
            get
            {
                return _affiche_boutons;
            }
            set
            {
                _affiche_boutons = value;
            }
        }
        private string _accepter;
        public string Accepter
        {
            get
            {
                return "ok";
            }
        }
        private string _refuser;
        public string Refuser
        {
            get
            {
                return "ok";
            }
        }
        public NotificationViewModel(string t, string c, string type)
        {
            this.TitleNotif = t;
            this.ContentNotif = c;
            this.Type = type;
            this.AfficheBoutons = false;

            if(this.Type == "connexion")
            {
                this.uneConnexion();
            } 
            else if(this.Type == "appel")
            {
                this.unAppel();
            }
            else if(this.Type == "notif")
            {
                this.uneNotif();
            }
        }
        public void unAppel()
        {
            this.AfficheBoutons = true;
            Console.WriteLine("un appel !");
        }
        public void uneConnexion()
        {
            this.AfficheBoutons = true;
            Console.WriteLine("un connexion !");
        }
        public void uneNotif()
        {
            this.AfficheBoutons = false;
            Console.WriteLine("une notif !");
        }
    }
}
