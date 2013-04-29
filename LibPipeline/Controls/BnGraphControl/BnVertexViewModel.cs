﻿﻿using LibBn;
using System.ComponentModel;

namespace LibPipeline
{
    public class BnVertexViewModel : BnVertex
    {
        private bool isExpanded = false;
        private bool isLocked = true;
        private bool isSelected = false;
        private bool isSensorChecked = false;
        private BnState mostProbableState = null;
        private double opacity = 1;

        public BnVertexViewModel()
            : base()
        {
            this.PropertyChanged += BnVertexViewModel_PropertyChanged;
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.isExpanded = value;
                this.OnPropertyChanged("IsExpanded");
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
                    this.OnPropertyChanged("IsLocked");
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged("IsCursorVisible");
                }
            }
        }

        public bool IsSensorChecked
        {
            get
            {
                return this.isSensorChecked;
            }

            set
            {
                if (value != this.isSensorChecked)
                {
                    this.isSensorChecked = value;
                    this.OnPropertyChanged("IsSensorChecked");
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
                    this.OnPropertyChanged("Opacity");
                }
            }
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

        public void SelectState(BnState selectedState)
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

        private void BnVertexViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsCursorVisible") && this.IsSelected == false)
            {
                this.IsLocked = true;
            }

            else if (e.PropertyName.Equals("IsLocked"))
            {
                if (this.IsLocked == false)
                {
                    this.IsExpanded = true;
                }
            }
        }
    }
}