using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;

namespace LibPipeline
{
    public class MultiLocationSegment : ViewModel, IEnumerable<Location>
    {
        private Location end = null;
        private Location middle = null;
        private Location start = null;
        private Brush stroke;

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

        public Brush Stroke
        {
            get
            {
                return this.stroke;
            }

            set
            {
                if (value != this.stroke)
                {
                    this.stroke = value;
                    this.OnPropertyChanged("Stroke");
                }
            }
        }

        public double Value
        {
            get
            {
                return this.Middle.Value;
            }

            set
            {
                if (value != this.Middle.Value)
                {
                    this.Middle.Value = value;
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