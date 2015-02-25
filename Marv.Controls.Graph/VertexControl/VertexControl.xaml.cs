﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Marv.Common;
using Telerik.Windows.Controls;

namespace Marv.Controls.Graph
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

        public static readonly DependencyProperty IsSubGraphCommandVisibleProperty =
            DependencyProperty.Register("IsSubGraphCommandVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false, ChangedIsSubGraphCommandVisible));

        public static readonly DependencyProperty IsToolbarVisibleProperty =
            DependencyProperty.Register("IsToolbarVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(false));

        public static readonly DependencyProperty IsValueVisibleProperty =
            DependencyProperty.Register("IsValueVisible", typeof (bool), typeof (VertexControl), new PropertyMetadata(true));

        public static readonly DependencyProperty VertexProperty =
            DependencyProperty.Register("Vertex", typeof (Vertex), typeof (VertexControl), new PropertyMetadata(null));

        private ObservableCollection<Command<VertexControl>> commands = new ObservableCollection<Command<VertexControl>>
        {
            VertexControlCommands.Expand
        };

        public ObservableCollection<Command<VertexControl>> Commands
        {
            get { return this.commands; }

            set
            {
                if (value.Equals(this.commands))
                {
                    return;
                }

                this.commands = value;
                this.RaisePropertyChanged();
            }
        }

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
            get { return (bool) GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public bool IsInputVisible
        {
            get { return (bool) GetValue(IsInputVisibleProperty); }
            set { SetValue(IsInputVisibleProperty, value); }
        }

        public bool IsSubGraphCommandVisible
        {
            get { return (bool) GetValue(IsSubGraphCommandVisibleProperty); }
            set { SetValue(IsSubGraphCommandVisibleProperty, value); }
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
        }

        private static void ChangedIsSubGraphCommandVisible(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as VertexControl;

            if (control.IsSubGraphCommandVisible)
            {
                control.Commands.PushUnique(VertexControlCommands.SubGraph);
            }
            else
            {
                control.Commands.Remove(VertexControlCommands.SubGraph);
            }
        }

        public void RaiseCommandExecuted(Command<VertexControl> command)
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

        private void ToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            var command = (sender as RadButton).DataContext as Command<VertexControl>;

            command.Excecute(this);

            this.RaiseCommandExecuted(command);
        }

        private void UniformEvidenceButton_Click(object sender, RoutedEventArgs e)
        {
            this.Vertex.Evidence = null;
            this.RaiseEvidenceEntered();
        }

        public event EventHandler<Command<VertexControl>> CommandExecuted;

        public event EventHandler<VertexEvidence> EvidenceEntered;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}