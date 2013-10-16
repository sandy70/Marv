using Marv.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace LibNetwork
{
    public class Vertex : ViewModel
    {
        private ObservableCollection<IVertexCommand> commands = new ObservableCollection<IVertexCommand>
        {
            VertexCommand.VertexExpandCommand,
            VertexCommand.VertexLockCommand,
            VertexCommand.VertexClearCommand
        };

        private string description = "";
        private Point displayPosition;
        private ObservableCollection<String> groups = new ObservableCollection<String>();
        private string headerOfGroup;
        private string inputVertexKey;
        private bool isEvidenceEntered = false;
        private bool isExpanded;
        private bool isHeader = false;
        private string key = "";
        private State mostProbableState = null;
        private string name = "";
        private Graph parent;
        private Point position;
        private Dictionary<string, Point> positionsForGroup = new Dictionary<string, Point>();
        private string selectedGroup;
        private State selectedState;
        private ObservableCollection<State> states = new ObservableCollection<State>();
        private VertexType type = VertexType.None;
        private string units = "";

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

                if (this.IsHeader)
                {
                    this.Commands.Push(VertexCommand.VertexSubGraphCommand);
                }
            }
        }

        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                RaisePropertyChanged("Key");
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

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public Graph Parent
        {
            get
            {
                return this.parent;
            }

            set
            {
                if (value != this.parent)
                {
                    this.parent = value;
                    this.RaisePropertyChanged("Parent");
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

        public ObservableCollection<State> States
        {
            get { return states; }
            set
            {
                states = value;
                RaisePropertyChanged("States");
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

        public VertexValue Value
        {
            set
            {
                foreach (var state in this.States)
                {
                    state.Value = value[state.Key];
                }

                this.UpdateMostProbableState();
                this.IsEvidenceEntered = value.IsEvidenceEntered;

                this.RaisePropertyChanged("Value");
            }
        }

        public double GetMean(VertexValue vertexValue)
        {
            double numer = 0;
            double denom = 0;

            if (this.Type == VertexType.Number)
            {
                foreach (var state in this.States)
                {
                    numer += state.Range.Min * vertexValue[state.Key];
                    denom += vertexValue[state.Key];
                }
            }
            else if (this.Type == VertexType.Interval)
            {
                foreach (var state in this.States)
                {
                    double mid = (state.Range.Min + state.Range.Max) / 2;

                    numer += mid * vertexValue[state.Key];
                    denom += vertexValue[state.Key];
                }
            }

            return numer / denom;
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

        public double GetStandardDeviation(VertexValue vertexValue)
        {
            // Formula for standard deviation of a pdf
            // stdev = sqrt(sum((x - mu)^2 * P(x));

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
    }
}