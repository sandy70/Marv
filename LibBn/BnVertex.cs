using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace LibBn
{
    public class BnVertex : INotifyPropertyChanged
    {
        private string description = "";
        private Point displayPosition;
        private ObservableCollection<String> groups = new ObservableCollection<String>();
        private string headerOfGroup;
        private bool isEvidenceEntered = false;
        private bool isHeader = false;
        private string key = "";
        private string name = "";
        private Point position;
        private Dictionary<string, Point> positionsByGroup = new Dictionary<string, Point>();
        private ObservableCollection<BnState> states = new ObservableCollection<BnState>();
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

        public Dictionary<string, Point> PositionsByGroup
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
                    this.OnPropertyChanged("PositionsByGroup");
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

        public void CopyFrom(BnVertexValue srcVertexValue)
        {
            this.IsEvidenceEntered = srcVertexValue.IsEvidenceEntered;
            int nStates = srcVertexValue.States.Count;

            for (int s = 0; s < nStates; s++)
            {
                this.States[s].Value = srcVertexValue.States[s];
            }
        }

        public BnState GetMostProbableState()
        {
            BnState mostProbableState = new BnState { Value = double.MinValue, Key = "" };

            foreach (var state in this.States)
            {
                if (state.Value > mostProbableState.Value)
                {
                    mostProbableState = state;
                }
            }

            return mostProbableState;
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}