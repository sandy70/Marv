﻿namespace Marv.Common.Graph
{
    public class State : Model
    {
        private double belief;
        private double evidence;
        private double initialBelief;
        private double max;
        private double min;

        public double Belief
        {
            get { return this.belief; }

            set
            {
                this.belief = value;
                this.RaisePropertyChanged();
            }
        }

        public double Evidence
        {
            get { return this.evidence; }

            set
            {
                this.evidence = value;
                this.RaisePropertyChanged();
            }
        }

        public double InitialBelief
        {
            get { return this.initialBelief; }

            set
            {
                this.initialBelief = value;
                this.RaisePropertyChanged();
            }
        }

        public double Max
        {
            get { return this.max; }

            set
            {
                if (value != this.max)
                {
                    this.max = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double Min
        {
            get { return this.min; }

            set
            {
                if (value != this.min)
                {
                    this.min = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool Contains(double value)
        {
            return this.Min <= value && value <= this.Max;
        }
    }
}