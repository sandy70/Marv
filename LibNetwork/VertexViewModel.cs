﻿using System.Collections.ObjectModel;
using System.ComponentModel;

namespace LibNetwork
{
    public class VertexViewModel : Vertex
    {
        private ObservableCollection<IVertexCommand> commands = new ObservableCollection<IVertexCommand>
        {
            VertexCommand.ExpandVertexCommand
        };

        private bool isLocked = true;
        private bool isSelected = false;
        private bool isSensorChecked = false;
        private double opacity = 1;

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