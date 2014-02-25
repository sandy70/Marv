using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    public partial class StatesControl : UserControl
    {
        public static readonly DependencyProperty IsEditableProperty =
        DependencyProperty.Register("IsEditable", typeof(bool), typeof(StatesControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register("IsExpanded", typeof(bool), typeof(StatesControl), new PropertyMetadata(true));

        public static readonly DependencyProperty MostProbableStateProperty =
        DependencyProperty.Register("MostProbableState", typeof(State), typeof(StatesControl), new PropertyMetadata(null));

        public static readonly DependencyProperty SliderForegroundProperty =
        DependencyProperty.Register("SliderForeground", typeof(Brush), typeof(StatesControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        public static readonly RoutedEvent StateDoubleClickedEvent =
        EventManager.RegisterRoutedEvent("StateDoubleClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<State>>), typeof(StatesControl));

        public static readonly DependencyProperty StatesFontSizeProperty =
        DependencyProperty.Register("StatesFontSize", typeof(double), typeof(StatesControl), new PropertyMetadata(10.0));

        public static readonly DependencyProperty StatesProperty =
        DependencyProperty.Register("States", typeof(IEnumerable<State>), typeof(StatesControl), new PropertyMetadata(null));

        public static readonly RoutedEvent ValueEnteredEvent =
        EventManager.RegisterRoutedEvent("ValueEntered", RoutingStrategy.Bubble, typeof(RoutedEventHandler<ValueEventArgs<State>>), typeof(StatesControl));

        public StatesControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler<ValueEventArgs<State>> StateDoubleClicked
        {
            add { this.AddHandler(StateDoubleClickedEvent, value); }
            remove { this.RemoveHandler(StateDoubleClickedEvent, value); }
        }

        public event RoutedEventHandler<ValueEventArgs<State>> ValueEntered
        {
            add { this.AddHandler(ValueEnteredEvent, value); }
            remove { this.RemoveHandler(ValueEnteredEvent, value); }
        }

        public bool IsEditable
        {
            get { return (bool)this.GetValue(IsEditableProperty); }
            set { this.SetValue(IsEditableProperty, value); }
        }

        public bool IsExpanded
        {
            get { return (bool)this.GetValue(IsExpandedProperty); }
            set { this.SetValue(IsExpandedProperty, value); }
        }

        public State MostProbableState
        {
            get { return (State)this.GetValue(MostProbableStateProperty); }
            set { this.SetValue(MostProbableStateProperty, value); }
        }

        public Brush SliderForeground
        {
            get { return (Brush)this.GetValue(SliderForegroundProperty); }
            set { this.SetValue(SliderForegroundProperty, value); }
        }

        public IEnumerable<State> States
        {
            get { return (IEnumerable<State>)this.GetValue(StatesProperty); }
            set { this.SetValue(StatesProperty, value); }
        }

        public double StatesFontSize
        {
            get { return (double)this.GetValue(StatesFontSizeProperty); }
            set { this.SetValue(StatesFontSizeProperty, value); }
        }

        private static void SelectedStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var statesControl = d as StatesControl;
            var selectedState = e.NewValue as State;

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