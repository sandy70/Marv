using System;
using System.Windows;
using Marv.Common.Graph;

namespace Marv.Controls.Graph
{
    public partial class VertexControl
    {
        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (VertexControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public VertexControl()
        {
            InitializeComponent();
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

        public event EventHandler<Command<Vertex>> CommandExecuted;

        public event EventHandler<Vertex> EvidenceEntered;
    }
}