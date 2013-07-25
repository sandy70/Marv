using System.Collections;
using System.Collections.Generic;

namespace LibPipeline
{
    public class MultiLocationSegment : ViewModel, IEnumerable<Location>
    {
        private double _value;
        private Location end = null;
        private Location middle = null;
        private Location start = null;

        public Location End
        {
            get
            {
                return this.end;
            }

            set
            {
                if (value != this.end)
                {
                    this.end = value;
                    this.OnPropertyChanged("End");
                }
            }
        }

        public Location Middle
        {
            get
            {
                return this.middle;
            }

            set
            {
                if (value != this.middle)
                {
                    this.middle = value;

                    this.OnPropertyChanged("Middle");
                }
            }
        }

        public Location Start
        {
            get
            {
                return this.start;
            }

            set
            {
                if (value != this.start)
                {
                    this.start = value;
                    this.OnPropertyChanged("Start");
                }
            }
        }

        public double Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }

        public IEnumerator<Location> GetEnumerator()
        {
            return new MultiLocationSegmentEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MultiLocationSegmentEnumerator(this);
        }
    }
}