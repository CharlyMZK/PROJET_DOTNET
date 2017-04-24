using DataAccess.Model.Base;

namespace NotificationProject.Model
{
    public class Message
    {
        public string TypeMessage {get; set;}
        public string Connection { get; set; }

        public string Author { get; set; }  

        public BaseMessage Object { get; set; }

        public Message (string TypeMessage, string Connection, string Author, BaseMessage Object = null)
        {
            this.TypeMessage = TypeMessage;
            this.Connection = Connection;
            this.Author = Author;
            this.Object = Object;
        }
    }
}
