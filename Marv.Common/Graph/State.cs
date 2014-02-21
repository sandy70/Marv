﻿using System.Linq;

namespace Marv.Common
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
                if (value != this.belief)
                {
                    this.belief = value;
                    this.RaisePropertyChanged("Belief");
                }
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
                if (this.Range != null && this.Range.Count > 0)
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