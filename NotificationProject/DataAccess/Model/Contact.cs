using System.Collections.ObjectModel;
namespace DataAccess.Model
{
    public class Contact
    {

        public string Name { get; set; }
        public string Number { get; set; }
        public string Email { get; set; }
        public bool HasSentNewMessage { get; set; }
        public ObservableCollection<Sms> Chatter { get; set; }

        private static Contact _contact;
        private static Contact _contact2;

        public Contact(string Name, string Number, string Email)
        {
            this.Name = Name;
            this.Number = Number;
            this.Email = Email;
            this.Chatter = new ObservableCollection<Sms>();
            HasSentNewMessage = false;
        }

        public static Contact GetContact() {
            if(_contact == null)
                _contact = new Contact("Laurent", "0688269472", "test@example.fr");

            return _contact;
        }

        public static Contact GetContact2()
        {
            if (_contact2 == null)
                _contact2 = new Contact("Tata", "0606", "test@example.fr");

            return _contact2;
        }
    }
}