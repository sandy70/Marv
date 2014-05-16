namespace Marv.Common.Graph
{
    public class State : Model
    {
        private double belief;
        private double evidence;
        private double max;
        private double min;
        private double originalBelief;

        public double Belief
        {
            get
            {
                return this.belief;
            }

            set
            {
                this.belief = value;
                this.RaisePropertyChanged("Belief");
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
                this.RaisePropertyChanged("Evidence");
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
                    this.RaisePropertyChanged("Max");
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
                    this.RaisePropertyChanged("Min");
                }
            }
        }

        public double OriginalBelief
        {
            get
            {
                return this.originalBelief;
            }

            set
            {
                this.originalBelief = value;
                this.RaisePropertyChanged("OriginalBelief");
            }
        }

        public bool Contains(double value)
        {
            return this.Min < value && value < this.Max;
        }
    }
}