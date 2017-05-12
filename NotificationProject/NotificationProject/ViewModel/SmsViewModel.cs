using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;
using System.Windows.Input;

namespace NotificationProject.ViewModel
{
    class SmsViewModel : ObservableObject, IPageViewModel
    {

        #region constructor
        public SmsViewModel()
        {
            ButtonCommand = new RelayCommand(o => SendMessage(), n => CanSend());
        }
        #endregion

        #region Fields
        private string _phoneNumber;
        private string _smsText;
        public ICommand ButtonCommand { get; set; }
        #endregion


        #region Properties
        public string Name
        {
            get
            {
                return "Sms View";
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
            }
        }

        public string SmsText
        {
            get
            {
                return _smsText;
            }
            set
            {
                _smsText = value;
            }
        }
        #endregion

        #region Command
        private void SendMessage()
        {

        }

        private bool CanSend()
        {
            return !String.IsNullOrEmpty(SmsText) && !String.IsNullOrEmpty(PhoneNumber);
        }
        #endregion


    }
}
