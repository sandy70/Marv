namespace Marv.Common
{
    public class State : ViewModel
    {
        private double _value;
        private SortedSequence<double> range = new SortedSequence<double>();

        public SortedSequence<double> Range
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
            get { return _value; }
            set
            {
                this._value = value;
                this.RaisePropertyChanged("Value");
            }
        }
    }
}