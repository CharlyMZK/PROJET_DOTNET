using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    public class NotificationConfiguration
    {
        public Boolean IsEnabled { get; set; }
        public Boolean SmsEnabled { get; set; }
        public Boolean CallEnabled { get; set; }
        public Boolean OtherEnabled { get; set; }
        private static NotificationConfiguration instance;

        public static NotificationConfiguration getInstance()
        {
            if (instance == null)
                //TODO lire le fichier de config
                instance = new NotificationConfiguration(false,false,false,false);
            return instance;
        }

        public NotificationConfiguration(bool isEnabled, bool smsEnabled, bool callEnabled, bool otherEnabled)
        {
            this.IsEnabled = isEnabled;
            this.SmsEnabled = smsEnabled;
            this.CallEnabled = callEnabled;
            this.OtherEnabled = otherEnabled;
        }
    }
}
