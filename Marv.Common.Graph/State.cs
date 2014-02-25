namespace Marv.Common.Graph
{
    public class State : Model
    {
        private double _value;
        private double belief;
        private Sequence<double> range = new Sequence<double>();

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

        public Sequence<double> Range
        {
            get
            {
                return this.range;
            }

            set
            {
                if (value != this.range)
                {
                    this.range = value;
                    this.RaisePropertyChanged("Range");
                }
            }
        }

        public double Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.RaisePropertyChanged("Value");
            }
        }
    }
}