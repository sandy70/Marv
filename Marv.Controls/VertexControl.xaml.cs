using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Marv.Common;

namespace Marv.Controls
{
    public partial class VertexControl : INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsEvidenceVisibleProperty =
            DependencyProperty.Register("IsEvidenceVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty IsInputVisibleProperty =
            DependencyProperty.Register("IsInputVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsToolbarVisibleProperty =
            DependencyProperty.Register("IsToolbarVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsValueVisibleProperty =
            DependencyProperty.Register("IsValueVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (VertexControl), new PropertyMetadata(null));

        public bool IsEditable
        {
            get { return (bool) this.GetValue(IsEditableProperty); }

            set { this.SetValue(IsEditableProperty, value); }
        }

        public bool IsEvidenceVisible
        {
            get { return (bool) this.GetValue(IsEvidenceVisibleProperty); }

            set { this.SetValue(IsEvidenceVisibleProperty, value); }
        }

        public bool IsExpanded
        {
            get { return (bool) this.GetValue(IsExpandedProperty); }
            set { this.SetValue(IsExpandedProperty, value); }
        }

        public bool IsInputVisible
        {
            get { return (bool) this.GetValue(IsInputVisibleProperty); }
            set { this.SetValue(IsInputVisibleProperty, value); }
        }

        public bool IsToolbarVisible
        {
            get { return (bool) this.GetValue(IsToolbarVisibleProperty); }
            set { this.SetValue(IsToolbarVisibleProperty, value); }
        }

        public bool IsValueVisible
        {
            get { return (bool) this.GetValue(IsValueVisibleProperty); }

            set { this.SetValue(IsValueVisibleProperty, value); }
        }

        public Vertex Vertex
        {
            get { return (Vertex) this.GetValue(VertexProperty); }
            set { this.SetValue(VertexProperty, value); }
        }

        public VertexControl()
        {
            InitializeComponent();
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

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private void RaiseEvidenceEntered(NodeEvidence nodeEvidence = null)
        {
            if (nodeEvidence == null)
            {
                nodeEvidence = new NodeEvidence
                {
                    Value = this.Vertex.Evidence,
                    Type = NodeEvidenceType.Distribution,
                    Params = this.Vertex.Evidence
                };
            }

            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, nodeEvidence);
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RaiseShowGroupButtonClicked()
        {
            if (this.ShowGroupButtonClicked != null)
            {
                this.ShowGroupButtonClicked(this, new EventArgs());
            }
        }

        private void ShowGroupButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.RaiseShowGroupButtonClicked();
        }

        private void SliderProgressBar_ValueEntered(object sender, double e)
        {
            if (Math.Abs(e - 100) < Common.Utils.Epsilon)
            {
                this.Vertex.SetEvidence((sender as SliderProgressBar).DataContext as State);
            }

            this.Vertex.Normalize();
            this.Vertex.UpdateEvidenceString();

            this.RaiseEvidenceEntered();
        }

        private void UniformEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            var evidenceValue = this.Vertex.States.Select(state => 1.0).Normalized().ToArray();

            var nodeEvidence = this.Vertex.IsNumeric
                                   ? new NodeEvidence
                                   {
                                       Params = new[] { this.Vertex.SafeMin, this.Vertex.SafeMax },
                                       Type = NodeEvidenceType.Range,
                                       Value = evidenceValue
                                   }
                                   : new NodeEvidence
                                   {
                                       Params = evidenceValue,
                                       Type = NodeEvidenceType.Distribution,
                                       Value = evidenceValue
                                   };

            this.Vertex.Evidence = nodeEvidence.Value;
            this.Vertex.EvidenceString = nodeEvidence.ToString();

            this.RaiseEvidenceEntered(nodeEvidence);
        }

        public event EventHandler<NodeEvidence> EvidenceEntered;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ShowGroupButtonClicked;
    }
}