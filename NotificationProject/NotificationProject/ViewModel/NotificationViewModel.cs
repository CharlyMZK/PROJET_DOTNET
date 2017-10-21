
using NotificationProject.HelperClasses;
using NotificationProject.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        private ICommand _displayCommand;
        public ICommand DisplayCommand
        {
            get
            {
                if (_displayCommand == null)
                    _displayCommand = new RelayCommand(o => Display());
                return _displayCommand;
            }
        }

        private ICommand _clickButtonYes;
        public ICommand ClickButtonYes
        {
            get
            {
                if (_clickButtonYes == null)
                    _clickButtonYes = new RelayCommand(o => clickButtonYes());
                return _clickButtonYes;
            }
        }

        private ICommand _clickButtonNo;
        public ICommand ClickButtonNo
        {
            get
            {
                if (_clickButtonNo == null)
                    _clickButtonNo = new RelayCommand(o => clickButtonNo());
                return _clickButtonNo;
            }
        }

        private string application;

        private Action callbackYes;
        private Action callbackNo;
        public NotificationViewModel(string t, string c, string type, string app, Action cbYes, Action cbNo)
        {

            this.AfficheBoutons = false;
            this.application = app;
            this.callbackYes = cbYes;
            this.callbackNo = cbNo;

            this.TitleNotif = t;
            this.ContentNotif = c;
            this.Type = t.ToUpper();
            
            switch(this.Type)
            {
                case "CONNEXION":
                    this.uneConnexion();
                    break;
                case "APPEL":
                    this.unAppel();
                    break;
                case "NOTIF":
                    this.uneNotif();
                    break;
                default:
                    break;
            }
        }

        public void unAppel()
        {
            this.AfficheBoutons = true;
            this.application = "appel";
            Console.WriteLine("un appel !");
        }

        public void uneConnexion()
        {
            this.AfficheBoutons = true;
            this.application = "connexion";
            Console.WriteLine("un connexion !");
        }

        public void uneNotif()
        {
            this.AfficheBoutons = false;
            Console.WriteLine("une notif !");
        }

        public void clickButtonYes()
        {
            if (this.callbackYes != null)
            {
                this.callbackYes();
            }
        }

        public void clickButtonNo()
        {
            if (this.callbackNo != null)
            {
                this.callbackNo();
            }
        }
        private void Display()
        {
            Window.GetWindow(Application.Current.MainWindow).WindowState = WindowState.Maximized;
        }
    }
}
