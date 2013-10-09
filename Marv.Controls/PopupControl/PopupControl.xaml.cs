using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Marv.Controls
{
    public partial class PopupControl : UserControl
    {
        public static readonly DependencyProperty IsCloseableProperty =
        DependencyProperty.Register("IsCloseable", typeof(bool), typeof(PopupControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsProgressBarVisibleProperty =
        DependencyProperty.Register("IsProgressBarVisible", typeof(bool), typeof(PopupControl), new PropertyMetadata(false));

        public static readonly DependencyProperty RowHeightProperty =
        DependencyProperty.Register("RowHeight", typeof(double), typeof(PopupControl), new PropertyMetadata(50.0));

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

        public bool IsCloseable
        {
            get { return (bool)GetValue(IsCloseableProperty); }
            set { SetValue(IsCloseableProperty, value); }
        }

        public bool IsProgressBarVisible
        {
            get { return (bool)GetValue(IsProgressBarVisibleProperty); }
            set { SetValue(IsProgressBarVisibleProperty, value); }
        }

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
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

        public void ShowTextIndeterminate(string text)
        {
            this.TextBlock.Text = text;
            this.IsCloseable = false;
            this.IsProgressBarVisible = true;

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

        public void Hide()
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
                this.IsCloseable = true;
                this.IsProgressBarVisible = false;
                this.Visibility = Visibility.Collapsed;
            };

            this.BeginAnimation(UserControl.OpacityProperty, animation);
        }
    }
}