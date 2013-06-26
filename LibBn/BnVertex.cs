using Smile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace LibBn
{
    public class BnVertex : INotifyPropertyChanged
    {
        private BnVertexValue _value;
        private string description = "";
        private Point displayPosition;
        private ObservableCollection<String> groups = new ObservableCollection<String>();
        private string headerOfGroup;
        private bool isEvidenceEntered = false;
        private bool isExpanded;
        private bool isHeader = false;
        private string key = "";
        private BnState mostProbableState = null;
        private string name = "";
        private Network network;
        private int nodeHandle;
        private BnGraph parent;
        private Point position;
        private Dictionary<string, Point> positionsByGroup = new Dictionary<string, Point>();
        private BnState selectedState;
        private ObservableCollection<BnState> states = new ObservableCollection<BnState>();
        private VertexType type = VertexType.None;
        private string units = "";

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

                    this.OnPropertyChanged("IsExpanded");
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

        public BnState SelectedState
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
                    this.OnPropertyChanged("SelectedState");
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
                    this.OnPropertyChanged("Type");
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
                    this.OnPropertyChanged("Units");
                }
            }
        }

        public BnVertexValue Value
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
                    this.OnPropertyChanged("Value");

                    foreach (var state in this.States)
                    {
                        state.Value = this.Value[state.Key];
                    }

                    this.UpdateMostProbableState();
                }
            }
        }

        public void ClearEvidence()
        {
            try
            {
                this.Network.ClearEvidence(this.Key);
            }
            catch (SmileException exception)
            {
                // do nothing
            }

            this.IsEvidenceEntered = false;
        }

        public BnGraphValue ClearEvidenceAndUpdateParentValue()
        {
            this.ClearEvidence();
            this.Parent.UpdateBeliefs();
            return this.Parent.UpdateValue();
        }

        public double GetMean(BnVertexValue vertexValue)
        {
            double numer = 0;
            double denom = 0;

            if (this.Type == VertexType.Number)
            {
                foreach (var state in this.States)
                {
                    numer += state.Min * vertexValue[state.Key];
                    denom += vertexValue[state.Key];
                }
            }
            else if (this.Type == VertexType.Interval)
            {
                foreach (var state in this.States)
                {
                    double mid = (state.Min + state.Max) / 2;

                    numer += mid * vertexValue[state.Key];
                    denom += vertexValue[state.Key];
                }
            }

            return numer / denom;
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

        public BnVertexValue GetValueFromNetwork()
        {
            var vertexValue = new BnVertexValue();

            foreach (var state in this.States)
            {
                try
                {
                    vertexValue[state.Key] = this.Network.GetNodeValue(this.Key)[this.GetStateIndex(state.Key)];
                }
                catch (SmileException smileException)
                {
                    Console.WriteLine(smileException.Message);
                    vertexValue[state.Key] = 0;
                }
            }

            return vertexValue;
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

            this.IsEvidenceEntered = true;
        }

        public BnGraphValue SetEvidenceAndUpdateParentValue()
        {
            return this.SetEvidenceAndUpdateParentValue(this.ToEvidence());
        }

        public BnGraphValue SetEvidenceAndUpdateParentValue(VertexEvidence vertexEvidence)
        {
            this.SetEvidence(vertexEvidence);
            this.Parent.UpdateBeliefs();
            return this.Parent.UpdateValue();
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

        public VertexEvidence ToEvidence()
        {
            int selectedStateIndex = this.GetSelectedStateIndex();
            var vertexEvidence = new VertexEvidence();

            if (selectedStateIndex >= 0)
            {
                vertexEvidence.EvidenceType = EvidenceType.StateSelected;
                vertexEvidence.StateIndex = selectedStateIndex;
            }
            else
            {
                vertexEvidence.EvidenceType = EvidenceType.SoftEvidence;
                vertexEvidence.Evidence = this.States.Select(x => x.Value).ToArray();
            }

            return vertexEvidence;
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
            try
            {
                this.Network.SetEvidence(this.Key, stateIndex);
                this.SelectedState = this.States[stateIndex];
            }
            catch (SmileException exception)
            {
                throw new InconsistentEvidenceException();
            }
        }
    }
}