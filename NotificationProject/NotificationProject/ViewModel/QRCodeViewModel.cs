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

namespace NotificationProject.ViewModel
{
    public class QRCodeViewModel : ObservableObject, IPageViewModel
    {

        #region fields
        public QRCodeViewModel()
        {
            var _communicationService = new CommunicationService();
            var qrValue = _communicationService.getIpAddress().ToString() + " : " + _communicationService.getPort().ToString();
            var barcodeWriter = loadQRCode();
            using (var bitmap = barcodeWriter.Write(qrValue))
            {
                var hbmp = bitmap.GetHbitmap();
                var source = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                ImageSource = source;
            }
        }
        private BitmapSource _imageSource;


        #endregion
        #region methods
        public String Name
        {
            get
            {
                return "QRCode";
            }
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


        public BarcodeWriter loadQRCode()
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

    #endregion
    }
}
