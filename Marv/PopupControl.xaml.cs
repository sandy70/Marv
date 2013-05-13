using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Marv
{
    public partial class PopupControl : UserControl
    {
        private DispatcherTimer timer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 5)
        };

        public PopupControl()
        {
            InitializeComponent();

            this.CloseButton.Click += (o, e) =>
                {
                    this.Hide();
                };

            this.timer.Tick += (o2, e2) =>
                {
                    this.Hide();
                    timer.Stop();
                };
        }

        public void ShowText(string text)
        {
            this.TextBlock.Text = text;

            if (this.Visibility == Visibility.Collapsed)
            {
                this.Opacity = 0;
                this.Visibility = Visibility.Visible;

                var animation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                };

                animation.Completed += (o, e) =>
                    {
                        this.timer.Start();
                    };

                this.BeginAnimation(UserControl.OpacityProperty, animation);
            }
        }

        private void Hide()
        {
            this.timer.Stop();

            var animation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            };

            animation.Completed += (o, args) =>
            {
                this.Visibility = Visibility.Collapsed;
            };

            this.BeginAnimation(UserControl.OpacityProperty, animation);
        }
    }
}