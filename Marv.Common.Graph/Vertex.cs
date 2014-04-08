using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private bool isEvidenceEntered;
        private bool isExpanded;
        private bool isHeader;
        private bool isLocked = true;
        private State mostProbableState;
        private Point position;
        private Dictionary<string, Point> positionsForGroup = new Dictionary<string, Point>();
        private string selectedGroup;
        private State selectedState;
        private ModelCollection<State> states = new ModelCollection<State>();
        private Dictionary<string, double> statistics = new Dictionary<string, double>();
        private double[,] table;
        private VertexType type = VertexType.Labelled;
        private string units = "";

        public Dictionary<string, double> Belief
        {
            get
            {
                var belief = new Dictionary<string, double>();

                foreach (var state in this.States)
                {
                    belief[state.Key] = state.Belief;
                }

                return belief;
            }

            set
            {
                foreach (var state in this.States)
                {
                    state.Belief = value[state.Key];
                }

                this.RaisePropertyChanged("Belief");
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
                    this.RaisePropertyChanged("Commands");
                }
            }
        }

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
                    this.RaisePropertyChanged("ConnectorPositions");
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
                    this.RaisePropertyChanged("Description");
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
                if (value != this.displayPosition)
                {
                    this.displayPosition = value;
                    this.RaisePropertyChanged("DisplayPosition");

                    if (this.DisplayPosition != null && this.SelectedGroup != null)
                    {
                        this.PositionForGroup[this.SelectedGroup] = this.DisplayPosition;
                    }
                }
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
                this.RaisePropertyChanged("EvidenceString");

                var evidence = EvidenceStringFactory.Create(this.EvidenceString).Parse(this, this.EvidenceString);

                if (evidence == null)
                {
                    this.SetValue(0);
                    this.IsEvidenceEntered = false;
                }
                else
                {
                    this.Value = evidence;
                    this.IsEvidenceEntered = true;
                }
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
                RaisePropertyChanged("Groups");
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
                RaisePropertyChanged("HeaderOfGroup");
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
                    this.RaisePropertyChanged("InputVertexKey");
                }
            }
        }

        public bool IsEvidenceEntered
        {
            get
            {
                return this.isEvidenceEntered;
            }

            set
            {
                this.isEvidenceEntered = value;
                this.RaisePropertyChanged("IsEvidenceEntered");
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
                    this.RaisePropertyChanged("IsExpanded");
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
                RaisePropertyChanged("IsHeader");
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
                    this.RaisePropertyChanged("IsLocked");
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
                    this.RaisePropertyChanged("MostProbableState");
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
                    this.RaisePropertyChanged("Position");
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
                    this.RaisePropertyChanged("PositionForGroup");
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
                    this.RaisePropertyChanged("SelectedGroup");
                }
            }
        }

        public State SelectedState
        {
            get
            {
                return this.selectedState;
            }

            set
            {
                if (value != this.selectedState)
                {
                    this.selectedState = value;
                    this.RaisePropertyChanged("SelectedState");
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
                this.states = value;
                RaisePropertyChanged("States");
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
                    this.RaisePropertyChanged("Statistics");
                }
            }
        }

        public double[,] Table
        {
            get
            {
                return this.table;
            }

            set
            {
                if (value != this.table)
                {
                    this.table = value;
                    this.RaisePropertyChanged("Table");
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
                    this.RaisePropertyChanged("Type");
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
                    this.RaisePropertyChanged("Units");
                }
            }
        }

        public Dictionary<string, double> Value
        {
            get
            {
                return this.States.ToDictionary(state => state.Key, state => state.Value);
            }

            set
            {
                foreach (var state in this.States)
                {
                    state.Value = value == null ? 0 : value[state.Key];
                }

                this.UpdateMostProbableState();

                this.RaisePropertyChanged("Value");
            }
        }

        public Dictionary<string, double> CreateEvidence()
        {
            var evidence = new Dictionary<string, double>();

            foreach (var state in this.States)
            {
                evidence[state.Key] = 0;
            }

            return evidence;
        }

        public Evidence GetEvidence()
        {
            return new Evidence
            {
                String = this.EvidenceString, 
                Value = this.Value
            };
        }

        public double GetMean(Dictionary<string, double> vertexValue)
        {
            double numer = 0;
            double denom = 0;

            foreach (var state in this.States)
            {
                var mid = (state.Range.Min + state.Range.Max)/2;

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

        public int GetSelectedStateIndex()
        {
            State selectedState = null;
            var oneCount = 0;

            foreach (var state in this.States)
            {
                if (state.Value == 1)
                {
                    oneCount++;
                    selectedState = state;
                }

                if (oneCount > 1) break;
            }

            if (selectedState == null)
                return -1;
            return this.States.IndexOf(selectedState);
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
                    var x = (state.Range.Min + state.Range.Max)/2;
                    var Px = vertexValue[state.Key];

                    sum += Math.Pow(x - mu, 2)*Px;
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
            return this.States.Select(state => state.Value).ToArray();
        }

        public void SelectState(int index)
        {
            for (var i = 0; i < this.States.Count; i++)
            {
                this.States[i].Value = i == index ? 1 : 0;
            }
        }

        public void SelectState(State aState)
        {
            foreach (var state in this.States)
            {
                state.Value = state == aState ? 1 : 0;
            }
        }

        public void SetEvidence(Evidence evidence)
        {
            this.EvidenceString = evidence.String;
            this.Value = evidence.Value;
            this.IsEvidenceEntered = true;
        }

        public void SetValue(int i)
        {
            foreach (var state in this.States)
            {
                state.Value = i;
            }

            this.Value = this.Value.Normalized();
        }

        public override string ToString()
        {
            return String.Format("[{0}:{1}]", this.Key, this.Name);
        }

        public void UpdateMostProbableState()
        {
            this.MostProbableState = this.States.MaxBy(state => state.Value);
        }

        internal void SetValue(Evidence evidence)
        {
            this.EvidenceString = evidence.String;
            this.Value = evidence.Value;
            this.IsEvidenceEntered = true;
        }
    }
}