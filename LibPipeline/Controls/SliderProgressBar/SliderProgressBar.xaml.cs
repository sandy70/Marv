using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace LibPipeline
{
    public partial class SliderProgressBar : RadSlider
    {
        public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register("IsEditable", typeof(bool), typeof(SliderProgressBar), new PropertyMetadata(false));

        public static readonly DependencyProperty SliderForegroundProperty =
        DependencyProperty.Register("SliderForeground", typeof(Brush), typeof(SliderProgressBar), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly RoutedEvent ValueEnteredEvent =
        EventManager.RegisterRoutedEvent("ValueEntered", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<double>>), typeof(SliderProgressBar));

        public SliderProgressBar()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler<ValueEventArgs<double>> ValueEntered
        {
            add { AddHandler(ValueEnteredEvent, value); }
            remove { RemoveHandler(ValueEnteredEvent, value); }
        }

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public Brush SliderForeground
        {
            get { return (Brush)GetValue(SliderForegroundProperty); }
            set { SetValue(SliderForegroundProperty, value); }
        }
    }
}