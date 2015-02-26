﻿using System;
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
            this.Vertex.Evidence = new VertexEvidence
            {
                Params = null,
                Type = VertexEvidenceType.Null,
                Value = null
            };

            this.RaiseEvidenceEntered(this.Vertex.Evidence);
        }

        private void EvidenceStringTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var vertexEvidence = this.Vertex.States.ParseEvidenceString(this.Vertex.EvidenceString);
            this.Vertex.Evidence = vertexEvidence;
            this.RaiseEvidenceEntered(vertexEvidence);
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = !this.IsExpanded;
        }

        private void RaiseEvidenceEntered(VertexEvidence vertexEvidence)
        {
            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, vertexEvidence);
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
                this.Vertex.Evidence = this.Vertex.States.ParseEvidenceString(((sender as SliderProgressBar).DataContext as State).Key);
            }

            this.RaiseEvidenceEntered(this.Vertex.Evidence);
        }

        private void UniformEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            var evidenceValue = this.Vertex.States.Select(state => 1.0).Normalized().ToArray();

            var nodeEvidence = this.Vertex.IsNumeric
                                   ? new VertexEvidence
                                   {
                                       Params = new[] { this.Vertex.SafeMin, this.Vertex.SafeMax },
                                       Type = VertexEvidenceType.Range,
                                       Value = evidenceValue
                                   }
                                   : new VertexEvidence
                                   {
                                       Params = evidenceValue,
                                       Type = VertexEvidenceType.Distribution,
                                       Value = evidenceValue
                                   };

            this.Vertex.Evidence = nodeEvidence;

            this.RaiseEvidenceEntered(nodeEvidence);
        }

        public event EventHandler<VertexEvidence> EvidenceEntered;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ShowGroupButtonClicked;
    }
}