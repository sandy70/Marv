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
                this.UpdateLayout();
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

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var evidence = EvidenceStringFactory.Create(this.InputTextBox.Text).Parse(this.Vertex);
            this.Vertex.Value = evidence ?? this.Vertex.Belief;
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
            this.InputTextBox.TextChanged += InputTextBox_TextChanged;
        }

        public event EventHandler<Command<Vertex>> CommandExecuted;

        public event EventHandler<Vertex> EvidenceEntered;
    }
}