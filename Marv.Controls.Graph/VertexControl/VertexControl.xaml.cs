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

        public static readonly DependencyProperty IsEvidenceVisibleProperty =
            DependencyProperty.Register("IsEvidenceVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsValueVisibleProperty =
            DependencyProperty.Register("IsValueVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

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

        public bool IsEvidenceVisible
        {
            get
            {
                return (bool) GetValue(IsEvidenceVisibleProperty);
            }

            set
            {
                SetValue(IsEvidenceVisibleProperty, value);
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

        public bool IsValueVisible
        {
            get
            {
                return (bool) GetValue(IsValueVisibleProperty);
            }

            set
            {
                SetValue(IsValueVisibleProperty, value);
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

        private void ClearEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Vertex.EvidenceString = null;
            this.RaiseEvidenceEntered();
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

        private void UniformEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Vertex.SetEvidenceUniform();
            this.RaiseEvidenceEntered();
        }

        private void VertexControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ClearEvidenceButton.Click += this.ClearEvidenceButton_Click;
            this.EvidenceStringTextBox.TextChanged += EvidenceStringTextBox_TextChanged;
            this.UniformEvidenceButton.Click += this.UniformEvidenceButton_Click;
        }

        void EvidenceStringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var result = this.Vertex.SetEvidence(new VertexEvidenceString(this.Vertex.EvidenceString));

            if (result)
            {
                this.RaiseEvidenceEntered();
            }
        }

        public event EventHandler<Command<Vertex>> CommandExecuted;

        public event EventHandler<Vertex> EvidenceEntered;
    }
}