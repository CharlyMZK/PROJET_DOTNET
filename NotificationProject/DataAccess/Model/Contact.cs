namespace DataAccess.Model
{
    public class Contact
    {

        public string Name { get; set; }
        public string Number { get; set; }

        public string Email { get; set; }

        public Contact(string Name, string Number, string Email)
        {
            this.Name = Name;
            this.Number = Number;
            this.Email = Email;
        }
    }
}