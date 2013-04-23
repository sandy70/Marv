using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GroupViewer
{
    public partial class VertexControl : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(VertexControl), new PropertyMetadata(""));

        private double lazyOpacity;

        public VertexControl()
        {
            InitializeComponent();
        }

        public double LazyOpacity
        {
            get
            {
                return this.lazyOpacity;
            }

            set
            {
                if (value != this.lazyOpacity)
                {
                    // Create a DoubleAnimation to fade the not selected option control
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        From = this.LazyOpacity,
                        To = value,
                        Duration = new Duration(TimeSpan.FromSeconds(10)),
                    };

                    this.BeginAnimation(VertexControl.OpacityProperty, animation);

                    this.lazyOpacity = value;
                    this.OnPropertyChanged("LazyOpacity");
                }
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}