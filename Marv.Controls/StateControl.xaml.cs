using System;
using System.Windows;
using Marv.Common;

namespace Marv.Controls
{
    public partial class StateControl
    {
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof (bool), typeof (StateControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsEvidenceVisibleProperty =
            DependencyProperty.Register("IsEvidenceVisible", typeof (bool), typeof (StateControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsValueVisibleProperty =
            DependencyProperty.Register("IsValueVisible", typeof (bool), typeof (StateControl), new PropertyMetadata(true));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof (State), typeof (StateControl), new PropertyMetadata(null));

        public bool IsEditable
        {
            get { return (bool) GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        public bool IsEvidenceVisible
        {
            get { return (bool) GetValue(IsEvidenceVisibleProperty); }
            set { SetValue(IsEvidenceVisibleProperty, value); }
        }

        public bool IsValueVisible
        {
            get { return (bool) GetValue(IsValueVisibleProperty); }
            set { SetValue(IsValueVisibleProperty, value); }
        }

        public State State
        {
            get { return (State) GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public StateControl()
        {
            InitializeComponent();
        }

        private void RaiseValueEntered(double value)
        {
            if (this.ValueEntered != null)
            {
                this.ValueEntered(this, value);
            }
        }

        private void SliderProgressBar_OnValueEntered(object sender, double value)
        {
            this.RaiseValueEntered(value);
        }

        public event EventHandler<double> ValueEntered;
    }
}