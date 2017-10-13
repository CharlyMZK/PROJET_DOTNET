using NotificationProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NotificationProject.View
{
    /// <summary>
    /// Logique d'interaction pour NotificationView.xaml
    /// </summary>
    public partial class NotificationView : Window
    {
        private Storyboard myStoryboard;
        private Rect desktopWorkingArea;

        public NotificationView()
        {
            InitializeComponent();
            this.desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = this.desktopWorkingArea.Right + this.Width;
            //this.Left = 0;
            this.Top = this.desktopWorkingArea.Bottom - (this.Height + 5);

            // Animation settings
            this.myStoryboard = new Storyboard();
        }

        private void setSlideInAnimation()
        {
            DoubleAnimation popIn = new DoubleAnimation();
            popIn.From = this.desktopWorkingArea.Right + this.Width;
            popIn.To = this.desktopWorkingArea.Right - (this.Width + 10);
            popIn.Duration = new Duration(TimeSpan.FromSeconds(1));

            Storyboard.SetTarget(popIn, this.myNotificationWindow);
            Storyboard.SetTargetProperty(popIn, new PropertyPath(LeftProperty));

            this.myStoryboard.Children.Add(popIn);
        }

        private void setSlideOutAnimation(int slideOutTime)
        {
            DoubleAnimation popOut = new DoubleAnimation();
            popOut.From = this.desktopWorkingArea.Right - (this.Width + 10);
            popOut.To = this.desktopWorkingArea.Right + this.Width;
            popOut.Duration = new Duration(TimeSpan.FromSeconds(1));
            popOut.BeginTime = TimeSpan.FromSeconds(slideOutTime);
            //popOut.Completed += (sender, eArgs) => this.closeNotif();
            popOut.Completed += new EventHandler(this.closeNotif);
            Storyboard.SetTarget(popOut, this.myNotificationWindow);
            Storyboard.SetTargetProperty(popOut, new PropertyPath(LeftProperty));

            this.myStoryboard.Children.Add(popOut);
        }

        private NotificationViewModel getMyDataContext()
        {
            return this.DataContext as NotificationViewModel;
        }

        public void displayNotif(int slideOutTimer)
        {
            this.setSlideInAnimation();
            this.setSlideOutAnimation(slideOutTimer);
            this.Show();
            this.myStoryboard.Begin(this);
        }

        public void yesButtonClick(object sender, EventArgs e)
        {
            this.getMyDataContext().clickButton(true);
        }

        public void noButtonClick(object sender, EventArgs e)
        {
            this.getMyDataContext().clickButton(false);
        }

        public void closeNotif(object sender, EventArgs e)
        {
            this.Close();
            this.myStoryboard = new Storyboard();
            
        }
    }
}
