using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Marv.Controls
{
    public partial class SliderProgressBar
    {
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof (bool), typeof (SliderProgressBar), new PropertyMetadata(false));

        public static readonly DependencyProperty IsTextVisibleProperty =
            DependencyProperty.Register("IsTextVisible", typeof (bool), typeof (SliderProgressBar), new PropertyMetadata(true));

        public static readonly DependencyProperty SliderForegroundProperty =
            DependencyProperty.Register("SliderForeground", typeof (Brush), typeof (SliderProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof (double), typeof (SliderProgressBar), new PropertyMetadata(32.0));

        private bool isDoubleClick;

        public bool IsEditable
        {
            get { return (bool) GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public bool IsTextVisible
        {
            get { return (bool) GetValue(IsTextVisibleProperty); }
            set { SetValue(IsTextVisibleProperty, value); }
        }

        public Brush SliderForeground
        {
            get { return (Brush) GetValue(SliderForegroundProperty); }
            set { SetValue(SliderForegroundProperty, value); }
        }

        public double Value
        {
            get { return (double) GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public SliderProgressBar()
        {
            InitializeComponent();
        }

        private void ProgressBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Value = 100;
            this.RaiseValueEntered();
        }

        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.isDoubleClick = e.ClickCount == 2;

            if (!this.isDoubleClick)
            {
                this.SetValue(e);
            }
        }

        private void ProgressBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !isDoubleClick)
            {
                this.SetValue(e);
                e.Handled = true;
            }
        }

        private void ProgressBar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.RaiseValueEntered();
        }

        private void RaiseValueEntered()
        {
            if (this.ValueEntered != null)
            {
                this.ValueEntered(this, this.Value);
            }
        }

        private void SetValue(MouseEventArgs e)
        {
            if (!this.IsEditable)
            {
                return;
            }

            this.Value = (e.GetPosition(this.ProgressBar).X - 1) / (this.ProgressBar.ActualWidth - 2) * 100;
        }

        public event EventHandler<double> ValueEntered;
    }
}