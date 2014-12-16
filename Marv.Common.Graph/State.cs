using Marv.Common;

namespace Marv
{
    public class State : NotifyPropertyChanged, IKeyed
    {
        private double belief;
        private double evidence;
        private double initialBelief;
        private string key;
        private double max;
        private double min;

        public double Belief
        {
            get
            {
                return this.belief;
            }

            set
            {
                this.belief = value;
                this.RaisePropertyChanged();
            }
        }

        public double Evidence
        {
            get
            {
                return this.evidence;
            }

            set
            {
                this.evidence = value;
                this.RaisePropertyChanged();
            }
        }

        public double InitialBelief
        {
            get
            {
                return this.initialBelief;
            }

            set
            {
                this.initialBelief = value;
                this.RaisePropertyChanged();
            }
        }

        public double Max
        {
            get
            {
                return this.max;
            }

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
            get
            {
                return this.min;
            }

            set
            {
                if (value != this.min)
                {
                    this.min = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public double SafeMax
        {
            get
            {
                return double.IsPositiveInfinity(this.Max) ? this.Min * 2 : this.Max;
            }
        }

        public double SafeMin
        {
            get
            {
                return double.IsNegativeInfinity(this.Min) ? this.Max * 2 : this.Min;
            }
        }

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                this.key = value;
                this.RaisePropertyChanged();
            }
        }

        public bool Contains(double value)
        {
            return this.Min <= value && value <= this.Max;
        }
    }
}