using LibNetwork;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LibPipeline
{
    public partial class BnStatesControl : UserControl
    {
        public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register("IsEditable", typeof(bool), typeof(BnStatesControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register("IsExpanded", typeof(bool), typeof(BnStatesControl), new PropertyMetadata(true));

        public static readonly DependencyProperty MostProbableStateProperty =
        DependencyProperty.Register("MostProbableState", typeof(BnState), typeof(BnStatesControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SliderForegroundProperty =
        DependencyProperty.Register("SliderForeground", typeof(Brush), typeof(BnStatesControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly RoutedEvent StateDoubleClickedEvent =
        EventManager.RegisterRoutedEvent("StateDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnState>>), typeof(BnStatesControl));

        public static readonly DependencyProperty StatesFontSizeProperty =
        DependencyProperty.Register("StatesFontSize", typeof(double), typeof(BnStatesControl), new PropertyMetadata(10.0));

        public static readonly DependencyProperty StatesProperty =
        DependencyProperty.Register("States", typeof(IEnumerable<BnState>), typeof(BnStatesControl), new PropertyMetadata(null));

        public static readonly RoutedEvent ValueEnteredEvent =
        EventManager.RegisterRoutedEvent("ValueEntered", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<BnState>>), typeof(BnStatesControl));

        public BnStatesControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler<ValueEventArgs<BnState>> StateDoubleClicked
        {
            add { AddHandler(StateDoubleClickedEvent, value); }
            remove { RemoveHandler(StateDoubleClickedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<BnState>> ValueEntered
        {
            add { AddHandler(ValueEnteredEvent, value); }
            remove { RemoveHandler(ValueEnteredEvent, value); }
        }

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public BnState MostProbableState
        {
            get { return (BnState)GetValue(MostProbableStateProperty); }
            set { SetValue(MostProbableStateProperty, value); }
        }

        public Brush SliderForeground
        {
            get { return (Brush)GetValue(SliderForegroundProperty); }
            set { SetValue(SliderForegroundProperty, value); }
        }

        public IEnumerable<BnState> States
        {
            get { return (IEnumerable<BnState>)GetValue(StatesProperty); }
            set { SetValue(StatesProperty, value); }
        }

        public double StatesFontSize
        {
            get { return (double)GetValue(StatesFontSizeProperty); }
            set { SetValue(StatesFontSizeProperty, value); }
        }

        private static void SelectedStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var statesControl = d as BnStatesControl;
            var selectedState = e.NewValue as BnState;

            foreach (var state in statesControl.States)
            {
                if (state == selectedState)
                {
                    state.Value = 1;
                }
                else
                {
                    state.Value = 0;
                }
            }
        }
    }
}