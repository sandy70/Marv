using System;
using System.Windows;
using System.Windows.Controls;
using Marv.Common;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    public partial class VertexControl
    {
        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (VertexControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsInputVisibleProperty =
            DependencyProperty.Register("IsInputVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsToolbarVisibleProperty =
            DependencyProperty.Register("IsToolbarVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsStatesVisibleProperty =
            DependencyProperty.Register("IsStatesVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsMostProbableStateVisibleProperty =
            DependencyProperty.Register("IsMostProbableStateVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public VertexControl()
        {
            InitializeComponent();
            this.Loaded += VertexControl_Loaded;
        }

        public bool IsEditable
        {
            get
            {
                return (bool) GetValue(IsEditableProperty);
            }

            set
            {
                SetValue(IsEditableProperty, value);
            }
        }

        public bool IsInputVisible
        {
            get
            {
                return (bool) GetValue(IsInputVisibleProperty);
            }
            set
            {
                SetValue(IsInputVisibleProperty, value);
            }
        }

        public bool IsMostProbableStateVisible
        {
            get
            {
                return (bool) GetValue(IsMostProbableStateVisibleProperty);
            }

            set
            {
                SetValue(IsMostProbableStateVisibleProperty, value);
            }
        }

        public bool IsStatesVisible
        {
            get
            {
                return (bool) GetValue(IsStatesVisibleProperty);
            }
            set
            {
                SetValue(IsStatesVisibleProperty, value);
            }
        }

        public bool IsToolbarVisible
        {
            get
            {
                return (bool) GetValue(IsToolbarVisibleProperty);
            }
            set
            {
                SetValue(IsToolbarVisibleProperty, value);
            }
        }

        public Vertex Vertex
        {
            get
            {
                return (Vertex) GetValue(VertexProperty);
            }
            set
            {
                SetValue(VertexProperty, value);
            }
        }

        private void All0Button_Click(object sender, RoutedEventArgs e)
        {
            this.Vertex.SetValue(0);
        }

        private void All1Button_Click(object sender, RoutedEventArgs e)
        {
            this.Vertex.SetValue(1);
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var temp = EvidenceStringFactory.Create(this.InputTextBox.Text);

            var evidence = EvidenceStringFactory.Create(this.InputTextBox.Text).Parse(this.Vertex);

            if (evidence == null)
            {
                this.Vertex.SetValue(0);
                this.Vertex.IsEvidenceEntered = false;
            }
            else
            {
                this.Vertex.Value = evidence;
                this.Vertex.IsEvidenceEntered = true;
            }
        }

        public void RaiseCommandExecuted(Command<Vertex> command)
        {
            if (this.CommandExecuted != null)
            {
                this.CommandExecuted(this, command);
            }
        }

        public void RaiseEvidenceEntered()
        {
            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, this.Vertex);
            }
        }

        private void VertexControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.All0Button.Click += All0Button_Click;
            this.All1Button.Click += All1Button_Click;
            this.InputTextBox.TextChanged += InputTextBox_TextChanged;
        }

        public event EventHandler<Command<Vertex>> CommandExecuted;

        public event EventHandler<Vertex> EvidenceEntered;
    }
}