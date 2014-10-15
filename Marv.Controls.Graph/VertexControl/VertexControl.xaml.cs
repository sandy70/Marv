using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Marv.Controls.Graph
{
    public partial class VertexControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsEvidenceVisibleProperty =
            DependencyProperty.Register("IsEvidenceVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsInputVisibleProperty =
            DependencyProperty.Register("IsInputVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsMostProbableStateVisibleProperty =
            DependencyProperty.Register("IsMostProbableStateVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsStatesVisibleProperty =
            DependencyProperty.Register("IsStatesVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsToolbarVisibleProperty =
            DependencyProperty.Register("IsToolbarVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsValueVisibleProperty =
            DependencyProperty.Register("IsValueVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (VertexControl), new PropertyMetadata(null));

        private bool isExpanded;

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

        public bool IsExpanded
        {
            get { return this.isExpanded; }

            set
            {
                if (value.Equals(this.isExpanded))
                {
                    return;
                }

                this.isExpanded = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsInputVisible
        {
            get { return (bool) GetValue(IsInputVisibleProperty); }
            set { SetValue(IsInputVisibleProperty, value); }
        }

        public bool IsMostProbableStateVisible
        {
            get { return (bool) GetValue(IsMostProbableStateVisibleProperty); }

            set { SetValue(IsMostProbableStateVisibleProperty, value); }
        }

        public bool IsStatesVisible
        {
            get { return (bool) GetValue(IsStatesVisibleProperty); }
            set { SetValue(IsStatesVisibleProperty, value); }
        }

        public bool IsToolbarVisible
        {
            get { return (bool) GetValue(IsToolbarVisibleProperty); }
            set { SetValue(IsToolbarVisibleProperty, value); }
        }

        public bool IsValueVisible
        {
            get { return (bool) GetValue(IsValueVisibleProperty); }

            set { SetValue(IsValueVisibleProperty, value); }
        }

        public Vertex Vertex
        {
            get { return (Vertex) GetValue(VertexProperty); }
            set { SetValue(VertexProperty, value); }
        }

        public VertexControl()
        {
            InitializeComponent();

            this.Loaded -= VertexControl_Loaded;
            this.Loaded += VertexControl_Loaded;
        }

        public void RaiseCommandExecuted(Command<Vertex> command)
        {
            if (this.CommandExecuted != null)
            {
                this.CommandExecuted(this, command);
            }
        }

        public void RaiseEvidenceEntered(VertexEvidence vertexEvidence = null)
        {
            if (vertexEvidence == null)
            {
                vertexEvidence = new VertexEvidence
                {
                    Value = this.Vertex.Evidence,
                    Type = VertexEvidenceType.Distribution,
                    Params = this.Vertex.Evidence
                };
            }

            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, vertexEvidence);
            }
        }

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ShowCollapsed()
        {
            this.IsExpanded = false;
        }

        public void ShowExpanded()
        {
            this.IsExpanded = true;
        }

        private void ClearEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Vertex.Evidence = null;
            this.Vertex.UpdateEvidenceString();
            this.RaiseEvidenceEntered();
        }

        private void EvidenceStringTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var vertexEvidence = this.Vertex.States.ParseEvidenceString(this.Vertex.EvidenceString);
            this.Vertex.Evidence = vertexEvidence.Value;
            this.RaiseEvidenceEntered(vertexEvidence);
        }

        private void UniformEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Vertex.Evidence = null;
            this.RaiseEvidenceEntered();
        }

        private void VertexControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.ClearEvidenceButton.Click -= ClearEvidenceButton_Click;
            this.ClearEvidenceButton.Click += ClearEvidenceButton_Click;

            this.EvidenceStringTextBox.KeyUp -= EvidenceStringTextBox_KeyUp;
            this.EvidenceStringTextBox.KeyUp += EvidenceStringTextBox_KeyUp;

            this.UniformEvidenceButton.Click -= UniformEvidenceButton_Click;
            this.UniformEvidenceButton.Click += UniformEvidenceButton_Click;
        }

        public event EventHandler<Command<Vertex>> CommandExecuted;

        public event EventHandler<VertexEvidence> EvidenceEntered;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}