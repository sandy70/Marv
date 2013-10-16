﻿using System;
using System.Collections.ObjectModel;

namespace LibNetwork
{
    public class VertexViewModel : Vertex
    {
        private bool isLocked = true;
        private bool isSelected = false;
        private bool isSensorChecked = false;
        private double opacity = 1;

        public event EventHandler Locked;

        public event EventHandler RequestedClear;

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
                    this.RaisePropertyChanged("IsSelected");
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
                    this.RaisePropertyChanged("IsSensorChecked");
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

        public void RaiseLocked()
        {
            if (this.Locked != null)
            {
                this.Locked(this, new EventArgs());
            }
        }

        public void RaiseRequestedClear()
        {
            if (this.RequestedClear != null)
            {
                this.RequestedClear(this, new EventArgs());
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
    }
}