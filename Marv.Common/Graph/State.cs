using System.Linq;

namespace Marv.Common
{
    public class State : ViewModel
    {
        private double _value;
        private Sequence<double> range = new Sequence<double>();

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
            get { return _value; }
            set
            {
                this._value = value;
                this.RaisePropertyChanged("Value");
            }
        }

        public string ValueString
        {
            get
            {
                if(this.Range != null && this.Range.Count > 0)
                {
                    // This will work because Range is of type <double>
                    return this.Range.Average().ToString();
                }
                else
                {
                    return this.Key;
                }
            }
        }
    }
}