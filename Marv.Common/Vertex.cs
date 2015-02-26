using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Marv.Common.Types;

namespace Marv.Common
{
    public class Vertex : NotifyPropertyChanged, IKeyed
    {
        private Dict<string, string, EdgeConnectorPositions> connectorPositions = new Dict<string, string, EdgeConnectorPositions>();
        private string description = "";
        private Point displayPosition;
        private VertexEvidence evidence;
        private string evidenceString;
        private ObservableCollection<string> groups = new ObservableCollection<string>();
        private string headerOfGroup;
        private string inputVertexKey;
        private bool isDraggingEnabled = true;
        private bool isExpanded;
        private bool isHeader;
        private bool isLocked = true;
        private bool isSelected;
        private string key;
        private State mostProbableState;
        private string name;
        private Point position;
        private Dict<string, Point> positionsForGroup = new Dict<string, Point>();
        private string selectedGroup;
        private ObservableCollection<State> states = new ObservableCollection<State>();
        private VertexType type = VertexType.Labelled;
        private string units = "";

        public double[] Belief
        {
            get { return this.States.Select(state => state.Belief).ToArray(); }

            set
            {
                this.States.ForEach((state, i) => state.Belief = value == null ? 0 : value[i]);
                this.RaisePropertyChanged();
            }
        }

        // Dictionary<group, targetVertexKey, EdgeConnectorPositions>
        public Dict<string, string, EdgeConnectorPositions> ConnectorPositions
        {
            get { return this.connectorPositions; }

            set
            {
                if (value != this.connectorPositions)
                {
                    this.connectorPositions = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Description
        {
            get { return this.description; }

            set
            {
                if (value != this.description)
                {
                    this.description = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Point DisplayPosition
        {
            get { return this.displayPosition; }

            set
            {
                if (value == this.displayPosition)
                {
                    return;
                }

                this.displayPosition = value;
                this.RaisePropertyChanged();

                if (this.SelectedGroup != null)
                {
                    this.PositionForGroup[this.SelectedGroup] = this.DisplayPosition;
                }
            }
        }

        public VertexEvidence Evidence
        {
            get { return this.evidence; }

            set
            {
                this.evidence = value;

                this.States.ForEach((state, i) => state.Evidence = value == null || value.Value == null ? 0 : value.Value[i]);
                this.RaisePropertyChanged();
            }
        }

        public string EvidenceString
        {
            get { return this.evidenceString; }

            set
            {
                this.evidenceString = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> Groups
        {
            get { return this.groups; }

            set
            {
                this.groups = value;
                this.RaisePropertyChanged();
            }
        }

        public string HeaderOfGroup
        {
            get { return this.headerOfGroup; }

            set
            {
                this.headerOfGroup = value;
                this.RaisePropertyChanged();
            }
        }

        public string InputVertexKey
        {
            get { return this.inputVertexKey; }

            set
            {
                if (value != this.inputVertexKey)
                {
                    this.inputVertexKey = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsDraggingEnabled
        {
            get { return this.isDraggingEnabled; }

            set
            {
                if (value.Equals(this.isDraggingEnabled))
                {
                    return;
                }

                this.isDraggingEnabled = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsEvidenceEntered
        {
            get { return this.States.Sum(state => state.Evidence) > 0; }
        }

        public bool IsExpanded
        {
            get { return this.isExpanded; }

            set
            {
                if (value != this.isExpanded)
                {
                    this.isExpanded = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsHeader
        {
            get { return this.isHeader; }

            set
            {
                this.isHeader = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsLocked
        {
            get { return this.isLocked; }

            set
            {
                if (this.isLocked != value)
                {
                    this.isLocked = value;
                    this.RaisePropertyChanged();
                }

                if (this.IsLocked == false)
                {
                    this.IsDraggingEnabled = false;
                    this.IsExpanded = true;
                }
                else
                {
                    this.IsDraggingEnabled = true;
                }
            }
        }

        public bool IsNumeric
        {
            get { return this.Type == VertexType.Interval || this.Type == VertexType.Numbered; }
        }

        public bool IsSelected
        {
            get { return this.isSelected; }

            set
            {
                if (value.Equals(this.isSelected))
                {
                    return;
                }

                this.isSelected = value;
                this.RaisePropertyChanged();
            }
        }

        public string Key
        {
            get { return this.key; }

            set
            {
                if (value.Equals(this.key))
                {
                    return;
                }

                this.key = value;
                this.RaisePropertyChanged();
            }
        }

        public State MostProbableState
        {
            get { return this.mostProbableState; }

            set
            {
                if (value != this.mostProbableState)
                {
                    this.mostProbableState = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return this.name; }

            set
            {
                if (value.Equals(this.name))
                {
                    return;
                }

                this.name = value;
                this.RaisePropertyChanged();
            }
        }

        public Point Position
        {
            get { return this.position; }

            set
            {
                if (value != this.position)
                {
                    this.position = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Dict<string, Point> PositionForGroup
        {
            get { return this.positionsForGroup; }

            set
            {
                if (value != this.positionsForGroup)
                {
                    this.positionsForGroup = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double SafeMax
        {
            get { return this.States.Max(state => state.SafeMax); }
        }

        public double SafeMin
        {
            get { return this.States.Min(state => state.SafeMin); }
        }

        public string SelectedGroup
        {
            get { return this.selectedGroup; }

            set
            {
                if (value != this.selectedGroup)
                {
                    this.selectedGroup = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<State> States
        {
            get { return this.states; }

            set
            {
                if (this.States != null)
                {
                    this.States.CollectionChanged -= this.States_CollectionChanged;

                    foreach (var state in this.States)
                    {
                        state.PropertyChanged -= this.state_PropertyChanged;
                    }
                }

                this.states = value;
                this.RaisePropertyChanged();

                if (this.States == null)
                {
                    return;
                }

                this.States.CollectionChanged += this.States_CollectionChanged;

                foreach (var state in this.States)
                {
                    state.PropertyChanged += this.state_PropertyChanged;
                }
            }
        }

        public VertexType Type
        {
            get { return this.type; }

            set
            {
                if (value != this.type)
                {
                    this.type = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string Units
        {
            get { return this.units; }

            set
            {
                if (value != this.units)
                {
                    this.units = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return String.Format("{0} [{1}]", this.Name, this.Key);
        }

        private void States_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (State state in e.NewItems)
                {
                    state.PropertyChanged += this.state_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (State state in e.OldItems)
                {
                    state.PropertyChanged -= this.state_PropertyChanged;
                }
            }
        }

        private void UpdateMostProbableState()
        {
            this.MostProbableState = this.States.MaxBy(state => state.Belief);
        }

        private void state_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Belief")
            {
                this.UpdateMostProbableState();
            }
        }
    }
}