﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MoreLinq;

namespace Marv.Common.Graph
{
    public class Vertex : Model
    {
        private ObservableCollection<Command<Vertex>> commands = new ObservableCollection<Command<Vertex>>
        {
            VertexCommands.Expand,
            VertexCommands.Lock
        };

        private Dictionary<string, string, EdgeConnectorPositions> connectorPositions = new Dictionary<string, string, EdgeConnectorPositions>();
        private string description = "";
        private Point displayPosition;
        private string evidenceString;
        private ObservableCollection<string> groups = new ObservableCollection<string>();
        private string headerOfGroup;
        private string inputVertexKey;
        private bool isExpanded;
        private bool isHeader;
        private bool isLocked = true;
        private State mostProbableState;
        private Point position;
        private Dictionary<string, Point> positionsForGroup = new Dictionary<string, Point>();
        private string selectedGroup;
        private ModelCollection<State> states;
        private Dictionary<string, double> statistics = new Dictionary<string, double>();
        private VertexType type = VertexType.Labelled;
        private string units = "";

        public Dictionary<string, double> Belief
        {
            get
            {
                return this.States.ToDictionary(state => state.Key, state => state.Belief);
            }

            set
            {
                foreach (var state in this.States)
                {
                    state.Belief = value == null ? 0 : value[state.Key];
                }

                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<Command<Vertex>> Commands
        {
            get
            {
                return this.commands;
            }

            set
            {
                if (value != this.commands)
                {
                    this.commands = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        // Dictionary<group, targetVertexKey, EdgeConnectorPositions>
        public Dictionary<string, string, EdgeConnectorPositions> ConnectorPositions
        {
            get
            {
                return this.connectorPositions;
            }

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
            get
            {
                return this.description;
            }

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
            get
            {
                return this.displayPosition;
            }

            set
            {
                if (value == this.displayPosition) return;

                this.displayPosition = value;
                this.RaisePropertyChanged();

                if (this.SelectedGroup != null)
                {
                    this.PositionForGroup[this.SelectedGroup] = this.DisplayPosition;
                }
            }
        }

        public Dictionary<string, double> Evidence
        {
            get
            {
                return this.States.ToDictionary(state => state.Key, state => state.Evidence);
            }

            set
            {
                foreach (var state in this.States)
                {
                    state.Evidence = (value == null) || !value.ContainsKey(state.Key) ? 0 : value[state.Key];
                }

                this.RaisePropertyChanged();
            }
        }

        public string EvidenceString
        {
            get
            {
                return this.evidenceString;
            }

            set
            {
                this.evidenceString = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> Groups
        {
            get
            {
                return this.groups;
            }

            set
            {
                this.groups = value;
                this.RaisePropertyChanged();
            }
        }

        public string HeaderOfGroup
        {
            get
            {
                return this.headerOfGroup;
            }
            set
            {
                this.headerOfGroup = value;
                this.RaisePropertyChanged();
            }
        }

        public Dictionary<string, double> InitialBelief
        {
            get
            {
                return this.States.ToDictionary(state => state.Key, state => state.InitialBelief);
            }

            set
            {
                foreach (var state in this.States)
                {
                    state.InitialBelief = value == null ? 0 : value[state.Key];
                }

                this.RaisePropertyChanged();
            }
        }

        public string InputVertexKey
        {
            get
            {
                return this.inputVertexKey;
            }

            set
            {
                if (value != this.inputVertexKey)
                {
                    this.inputVertexKey = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsEvidenceEntered
        {
            get
            {
                return this.States.Sum(state => state.Evidence) > 0 ||
                       this.EvidenceString != null;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

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
            get
            {
                return this.isHeader;
            }

            set
            {
                this.isHeader = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsLocked
        {
            get
            {
                return this.isLocked;
            }

            set
            {
                if (this.isLocked != value)
                {
                    this.isLocked = value;
                    this.RaisePropertyChanged();
                }

                if (this.IsLocked == false)
                {
                    this.IsExpanded = true;
                }
            }
        }

        public State MostProbableState
        {
            get
            {
                return this.mostProbableState;
            }

            set
            {
                if (value != this.mostProbableState)
                {
                    this.mostProbableState = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Point Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (value != this.position)
                {
                    this.position = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Dictionary<string, Point> PositionForGroup
        {
            get
            {
                return this.positionsForGroup;
            }

            set
            {
                if (value != this.positionsForGroup)
                {
                    this.positionsForGroup = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public string SelectedGroup
        {
            get
            {
                return this.selectedGroup;
            }

            set
            {
                if (value != this.selectedGroup)
                {
                    this.selectedGroup = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ModelCollection<State> States
        {
            get
            {
                return this.states;
            }

            set
            {
                if (this.States != null)
                {
                    this.States.CollectionChanged -= States_CollectionChanged;

                    foreach (var state in this.States)
                    {
                        state.PropertyChanged -= state_PropertyChanged;
                    }
                }

                this.states = value;
                this.RaisePropertyChanged();

                if (this.States == null) return;

                this.States.CollectionChanged += this.States_CollectionChanged;

                foreach (var state in this.States)
                {
                    state.PropertyChanged += this.state_PropertyChanged;
                }
            }
        }

        public Dictionary<string, double> Statistics
        {
            get
            {
                return this.statistics;
            }

            set
            {
                if (value != this.statistics)
                {
                    this.statistics = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public VertexType Type
        {
            get
            {
                return this.type;
            }

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
            get
            {
                return this.units;
            }

            set
            {
                if (value != this.units)
                {
                    this.units = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double GetMean(Dictionary<string, double> vertexValue)
        {
            double numer = 0;
            double denom = 0;

            foreach (var state in this.States)
            {
                var mid = (state.Min + state.Max)/2;

                numer += mid*vertexValue[state.Key];
                denom += vertexValue[state.Key];
            }

            return numer/denom;
        }

        public double GetMean(double[] evidence)
        {
            var vertexValue = new Dictionary<string, double>();

            foreach (var state in this.States)
            {
                var stateIndex = this.States.IndexOf(state);
                vertexValue[state.Key] = evidence[stateIndex];
            }

            return this.GetMean(vertexValue);
        }

        public double GetStandardDeviation(Dictionary<string, double> vertexValue)
        {
            // Formula for standard deviation of a pdf stdev = sqrt(sum((x - mu)^2 * P(x)); From
            // here: http://www.wyzant.com/resources/lessons/math/statistics_and_probability/expected_value/variance

            var mu = this.GetMean(vertexValue);
            var stdev = 0.0;

            if (this.Type == VertexType.Interval)
            {
                var sum = 0.0;

                foreach (var state in this.States)
                {
                    var x = (state.Min + state.Max)/2;
                    var px = vertexValue[state.Key];

                    sum += Math.Pow(x - mu, 2)*px;
                }

                stdev = Math.Sqrt(sum);
            }

            return stdev;
        }

        public double GetStatistics(string statisticsKey, IVertexValueComputer vertexValueComputer)
        {
            if (!this.Statistics.ContainsKey(statisticsKey))
            {
                this.Statistics[statisticsKey] = vertexValueComputer.Compute(this, this.Belief);
            }

            return this.Statistics[statisticsKey];
        }

        // Do not remove! This is for Marv.Matlab
        public double[] GetValue()
        {
            return this.States.Select(state => state.Belief).ToArray();
        }

        public void SelectState(int index)
        {
            for (var i = 0; i < this.States.Count; i++)
            {
                this.States[i].Belief = i == index ? 1 : 0;
            }
        }

        public void SetEvidence(State aState)
        {
            foreach (var state in this.States)
            {
                state.Evidence = state == aState ? 1 : 0;
            }
        }

        // Returns true if  the evidence was set successfully. The method might fail if VertexEvidenceString is provided and 
        // the string is malformed.
        public bool SetEvidence(IVertexEvidence vertexEvidence)
        {
            return vertexEvidence.Set(this);
        }

        public void SetEvidenceUniform()
        {
            var value = 1.0/this.States.Count;

            foreach (var state in this.States)
            {
                state.Evidence = value;
            }
        }

        public override string ToString()
        {
            return String.Format("[{0}:{1}]", this.Key, this.Name);
        }

        public void UpdateEvidence()
        {
            this.Evidence = EvidenceStringFactory.Create(this.EvidenceString).Parse(this.States, this.EvidenceString);
        }

        public void UpdateMostProbableState()
        {
            this.MostProbableState = this.States.MaxBy(state => state.Belief);
        }

        private void States_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (State state in e.NewItems)
                {
                    state.PropertyChanged += state_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (State state in e.OldItems)
                {
                    state.PropertyChanged -= state_PropertyChanged;
                }
            }
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