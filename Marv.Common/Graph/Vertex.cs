using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Marv.Common
{
    public class Vertex : ViewModel
    {
        private ObservableCollection<IVertexCommand> commands = new ObservableCollection<IVertexCommand>
        {
            VertexCommand.VertexExpandCommand,
            // VertexCommand.VertexLockCommand, VertexCommand.VertexClearCommand
        };

        private Dictionary<string, string, EdgeConnectorPositions> connectorPositions = new Dictionary<string, string, EdgeConnectorPositions>();
        private string description = "";
        private Point displayPosition;
        private ObservableCollection<String> groups = new ObservableCollection<String>();
        private string headerOfGroup;
        private string inputVertexKey;
        private bool isEvidenceEntered = false;
        private bool isExpanded;
        private bool isHeader = false;
        private bool isLocked = true;
        private State mostProbableState = null;
        private double opacity = 1;
        private Point position;
        private Dictionary<string, Point> positionsForGroup = new Dictionary<string, Point>();
        private string selectedGroup;
        private State selectedState;
        private ViewModelCollection<State> states = new ViewModelCollection<State>();
        private Dictionary<string, double> statistics = new Dictionary<string, double>();
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

        public ObservableCollection<IVertexCommand> Commands
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

        public ObservableCollection<String> Groups
        {
            get { return groups; }
            set
            {
                groups = value;
                RaisePropertyChanged("Groups");
            }
        }

        public string HeaderOfGroup
        {
            get { return headerOfGroup; }
            set
            {
                headerOfGroup = value;
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
                if (value != this.isEvidenceEntered)
                {
                    this.isEvidenceEntered = value;
                    this.RaisePropertyChanged("IsEvidenceEntered");
                }
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
                return isHeader;
            }

            set
            {
                isHeader = value;
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

        public double Opacity
        {
            get
            {
                return this.opacity;
            }

            set
            {
                if (value != this.opacity)
                {
                    this.opacity = value;
                    this.RaisePropertyChanged("Opacity");
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

        public ViewModelCollection<State> States
        {
            get { return states; }
            set
            {
                states = value;
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
                var value = new Dictionary<string, double>();

                foreach (var state in this.States)
                {
                    value[state.Key] = state.Value;
                }

                return value;
            }

            set
            {
                foreach (var state in this.States)
                {
                    state.Value = value[state.Key];
                }

                this.UpdateMostProbableState();

                this.RaisePropertyChanged("Value");
            }
        }

        public double GetMean(Dictionary<string, double> vertexValue)
        {
            double numer = 0;
            double denom = 0;

            foreach (var state in this.States)
            {
                double mid = (state.Range.Min + state.Range.Max) / 2;

                numer += mid * vertexValue[state.Key];
                denom += vertexValue[state.Key];
            }

            return numer / denom;
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
            int oneCount = 0;

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
            else
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
                    var x = (state.Range.Min + state.Range.Max) / 2;
                    var Px = vertexValue[state.Key];

                    sum += Math.Pow(x - mu, 2) * Px;
                }

                stdev = Math.Sqrt(sum);
            }

            return stdev;
        }

        public int GetStateIndex(string stateKey)
        {
            int stateIndex = -1;

            foreach (var state in this.States)
            {
                if (state.Key.Equals(stateKey))
                {
                    return states.IndexOf(state);
                }
            }

            return stateIndex;
        }

        public void SelectState(int index)
        {
            for (int i = 0; i < this.States.Count; i++)
            {
                if (i == index)
                {
                    this.States[i].Value = 1;
                }
                else
                {
                    this.States[i].Value = 0;
                }
            }
        }

        public void SelectState(State selectedState)
        {
            foreach (var state in this.States)
            {
                if (state == selectedState)
                {
                    state.Value = 1;
                }
                else
                {
                    state.Value = 0;
                }
            }
        }

        public void SetValueToZero()
        {
            foreach (var state in this.States)
            {
                state.Value = 0;
            }
        }

        public IEvidence ToEvidence()
        {
            int selectedStateIndex = this.GetSelectedStateIndex();
            IEvidence evidence = null;

            if (selectedStateIndex >= 0)
            {
                evidence = new HardEvidence
                {
                    StateIndex = selectedStateIndex
                };
            }
            else
            {
                evidence = new SoftEvidence
                {
                    Evidence = this.States.Select(x => x.Value).ToArray()
                };
            }

            return evidence;
        }

        public override string ToString()
        {
            return String.Format("[{0}:{1}]", this.Key, this.Name);
        }

        public void UpdateMostProbableState()
        {
            State mostProbableState = new State { Value = double.MinValue, Key = "" };

            foreach (var state in this.States)
            {
                if (state.Value > mostProbableState.Value)
                {
                    mostProbableState = state;
                }
            }

            this.MostProbableState = mostProbableState;
        }

        public double GetStatistics(string statisticsKey, IVertexValueComputer vertexValueComputer)
        {
            if (!this.Statistics.ContainsKey(statisticsKey))
            {
                this.Statistics[statisticsKey] = vertexValueComputer.Compute(this, this.Belief);
            }

            return this.Statistics[statisticsKey];
        }
    }
}