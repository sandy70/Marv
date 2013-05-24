using Smile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace LibBn
{
    public class BnVertex : INotifyPropertyChanged
    {
        private Dictionary<string, double> _value;
        private string description = "";
        private Point displayPosition;
        private ObservableCollection<String> groups = new ObservableCollection<String>();
        private string headerOfGroup;
        private bool isEvidenceEntered = false;
        private bool isHeader = false;
        private string key = "";
        private BnState mostProbableState = null;
        private string name = "";
        private Network network;
        private int nodeHandle;
        private BnGraph parent;
        private Point position;
        private Dictionary<string, Point> positionsByGroup = new Dictionary<string, Point>();
        private ObservableCollection<BnState> states = new ObservableCollection<BnState>();
        private string units = "";

        public BnVertex()
        {
        }

        public BnVertex(BnGraph parent, Network aNetwork)
        {
            this.Network = network;
            this.nodeHandle = this.Network.GetNode(key);
            this.Parent = parent;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
                    this.OnPropertyChanged("Description");
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
                    this.OnPropertyChanged("DisplayPosition");
                }
            }
        }

        public ObservableCollection<String> Groups
        {
            get { return groups; }
            set
            {
                groups = value;
                OnPropertyChanged("Groups");
            }
        }

        public string HeaderOfGroup
        {
            get { return headerOfGroup; }
            set
            {
                headerOfGroup = value;
                OnPropertyChanged("HeaderOfGroup");
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
                    this.OnPropertyChanged("IsEvidenceEntered");
                }
            }
        }

        public bool IsHeader
        {
            get { return isHeader; }
            set
            {
                isHeader = value;
                OnPropertyChanged("IsHeader");
            }
        }

        public string Key
        {
            get { return key; }
            set
            {
                key = value;
                OnPropertyChanged("Key");

                if (this.Network != null)
                {
                    this.nodeHandle = this.Network.GetNode(this.Key);
                }
            }
        }

        public BnState MostProbableState
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
                    this.OnPropertyChanged("MostProbableState");
                }
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public Network Network
        {
            get
            {
                return this.network;
            }

            set
            {
                if (value != this.network)
                {
                    this.network = value;
                    this.OnPropertyChanged("Network");

                    if (this.Network != null)
                    {
                        this.Network.GetNode(this.Key);
                    }
                }
            }
        }

        public BnGraph Parent
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
                    this.OnPropertyChanged("Parent");
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
                    this.OnPropertyChanged("Position");
                }
            }
        }

        public Dictionary<string, Point> Positions
        {
            get
            {
                return this.positionsByGroup;
            }

            set
            {
                if (value != this.positionsByGroup)
                {
                    this.positionsByGroup = value;
                    this.OnPropertyChanged("Positions");
                }
            }
        }

        public ObservableCollection<BnState> States
        {
            get { return states; }
            set
            {
                states = value;
                OnPropertyChanged("States");
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
                    this.OnPropertyChanged("Units");
                }
            }
        }

        public Dictionary<string, double> Values
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Values");

                    foreach (var state in this.States)
                    {
                        state.Value = this.Values[state.Key];
                    }

                    this.UpdateMostProbableState();
                }
            }
        }

        public void CopyFrom(BnVertexValue srcVertexValue)
        {
            this.IsEvidenceEntered = srcVertexValue.IsEvidenceEntered;
            int nStates = srcVertexValue.States.Count;

            for (int s = 0; s < nStates; s++)
            {
                this.States[s].Value = srcVertexValue.States[s];
            }
        }

        public int GetSelectedStateIndex()
        {
            BnState selectedState = null;
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

        public double GetStateValue(string stateKey)
        {
            var stateIndex = this.GetStateIndex(stateKey);
            var nodeValue = this.Network.GetNodeValue(this.Key);
            var stateValue = nodeValue[stateIndex];
            return stateValue;

            // return this.network.GetNodeValue(this.Key)[this.GetStateIndex(stateKey)];
        }

        public void SetEvidence(VertexEvidence vertexEvidence)
        {
            if (vertexEvidence.EvidenceType == EvidenceType.StateSelected)
            {
                this.SetEvidence(vertexEvidence.StateIndex);
            }
            else if (vertexEvidence.EvidenceType == EvidenceType.SoftEvidence)
            {
                this.SetEvidence(vertexEvidence.Evidence);
            }
        }

        public void SetSelectedStateIndex(int index)
        {
            int nStates = this.States.Count;

            for (int i = 0; i < nStates; i++)
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

        public void UpdateMostProbableState()
        {
            BnState mostProbableState = new BnState { Value = double.MinValue, Key = "" };

            foreach (var state in this.States)
            {
                if (state.Value > mostProbableState.Value)
                {
                    mostProbableState = state;
                }
            }

            this.MostProbableState = mostProbableState;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetEvidence(double[] evidence)
        {
            this.network.SetSoftEvidence(this.Key, evidence);
        }

        private void SetEvidence(int stateIndex)
        {
            this.Network.SetEvidence(this.Key, stateIndex);
        }
    }
}