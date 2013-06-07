using System;

namespace LibBn
{
    public class BnState : BnStateValue
    {
        private double max = double.MinValue;
        private double min = double.MinValue;

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

                    this.OnPropertyChanged("Max");
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

                    this.OnPropertyChanged("Min");
                }
            }
        }
    }
}