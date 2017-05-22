using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationProject.HelperClasses;
using ZXing.QrCode;
using ZXing;
using ZXing.Common;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using BusinessLayer;
using System.Threading;
using System.Windows.Input;

namespace NotificationProject.ViewModel
{
    public class QRCodeViewModel : ObservableObject, IPageViewModel
    {

        #region fields
        public QRCodeViewModel()
        {
            _communicationService = CommunicationService.getInstance();
            ButtonCommand = new RelayCommand(o => QRCodeButtonClick("QRCodeButton"), n => CanClick());
            this.IsEnabled = false;
            loadQRCode();
        }
        private BitmapSource _imageSource;
        private int randomNumberLimitedInTime { get; set; }
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();
        public ICommand ButtonCommand { get; set; }
        private bool IsEnabled { get; set; }
        public CommunicationService _communicationService {get; set;}
        #endregion

        #region methods
        public String Name
        {
            get
            {
                return "QRCode";
            }
        }

        private void QRCodeButtonClick(object sender)
        {
            Console.WriteLine("Regénération du QRCode...");
            this.IsEnabled = false;
            loadQRCode();
        }
        public BitmapSource ImageSource
        {
            get
            {
                if (_imageSource == null)
                {
                    _imageSource = null;
                }
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public bool CanClick()
        {
            return this.IsEnabled;
        }

        public BarcodeWriter createQRCode()
        {
            var qrcode = new QRCodeWriter();

            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 300,
                    Width = 300,
                    Margin = 1
                }
            };
            return barcodeWriter;
        }

        public void loadQRCode()
        {
            
            var getRandomNumber = GetRandomNumber(100000, 999999);
            _communicationService.randomSecretNumberAccess = getRandomNumber;
            deleteNumberAfterXTime(30000);
            var qrValue = _communicationService.getIpAddress().ToString() + ":" + _communicationService.getPort().ToString();
            var barcodeWriter = createQRCode();
            using (var bitmap = barcodeWriter.Write(qrValue))
            {
                var hbmp = bitmap.GetHbitmap();
                var source = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                ImageSource = source;
            } 
        }

        public int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }

        public void deleteNumberAfterXTime(int XTime)
        {
            Console.WriteLine("QR Code Random access number : " + _communicationService.randomSecretNumberAccess.ToString());
            Console.WriteLine("Application thread ID: {0}", Thread.CurrentThread.ManagedThreadId);
            var t = Task.Run(async () =>
            {
                await Task.Delay(XTime);
                Console.WriteLine("Task thread ID: {0}", Thread.CurrentThread.ManagedThreadId);
                _communicationService.randomSecretNumberAccess = GetRandomNumber(100000, 999999);
                this.IsEnabled = true;
                Console.WriteLine("QR Code Random access number : " + _communicationService.randomSecretNumberAccess.ToString());
            });
        }
        #endregion

    }
}
