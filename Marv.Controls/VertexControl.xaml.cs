﻿using System;
using System.Linq;
using System.Windows;
using Marv.Common;

namespace Marv.Controls
{
    public partial class VertexControl
    {
        public static readonly DependencyProperty IsBeliefVisibleProperty =
            DependencyProperty.Register("IsBeliefVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

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

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (VertexControl), new PropertyMetadata(null));

        public bool IsBeliefVisible
        {
            get { return (bool) this.GetValue(IsBeliefVisibleProperty); }
            set { this.SetValue(IsBeliefVisibleProperty, value); }
        }

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
            this.Vertex.EvidenceString = null;

            this.RaiseEvidenceEntered(new VertexEvidence
            {
                Params = null,
                Type = VertexEvidenceType.Null,
                Value = null
            });
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsExpanded = !this.IsExpanded;
            this.RaiseExpandButtonClicked();
        }

        private void LockButton_Checked(object sender, RoutedEventArgs e)
        {
            this.IsBeliefVisible = false;
            this.IsEditable = true;
        }

        private void LockButton_Unchecked(object sender, RoutedEventArgs e)
        {
            this.IsBeliefVisible = true;
            this.IsEditable = false;
        }

        private void RaiseEvidenceEntered(VertexEvidence vertexEvidence)
        {
            if (this.EvidenceEntered != null)
            {
                this.EvidenceEntered(this, vertexEvidence);
            }
        }

        private void RaiseExpandButtonClicked()
        {
            if (this.ExpandButtonClicked != null)
            {
                this.ExpandButtonClicked(this, new EventArgs());
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
            if (!this.IsEditable)
            {
                return;
            }

            var anEvidenceString = Math.Abs(e - 100) < Common.Utils.Epsilon && this.Vertex.Type != VertexType.Interval
                                       ? ((sender as SliderProgressBar).DataContext as State).Key
                                       : this.Vertex.States.Select(state => state.Evidence).String();

            var vertexEvidence = this.Vertex.States.ParseEvidenceString(anEvidenceString);

            this.Vertex.SetEvidence(vertexEvidence);

            this.RaiseEvidenceEntered(vertexEvidence);
        }

        public event EventHandler<VertexEvidence> EvidenceEntered;
        public event EventHandler ExpandButtonClicked;
        public event EventHandler ShowGroupButtonClicked;
    }
}