using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model
{
    class NotificationConfiguration
    {
        private Boolean isEnabled { get; set; }
        private Boolean smsEnabled { get; set; }
        private Boolean callEnabled { get; set; }
        private Boolean otherEnabled { get; set; }

        public NotificationConfiguration(bool isEnabled, bool smsEnabled, bool callEnabled, bool otherEnabled)
        {
            this.isEnabled = isEnabled;
            this.smsEnabled = smsEnabled;
            this.callEnabled = callEnabled;
            this.otherEnabled = otherEnabled;
        }
    }
}
