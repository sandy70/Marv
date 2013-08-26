using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media;

namespace LibPipeline
{
    public class MultiLocationSegment : ViewModel, IEnumerable<Location>
    {
        private IDoubleToBrushMap doubleToBrushMap = null;
        private Location end = null;
        private Location middle = null;
        private Location start = null;
        private Brush stroke;
        
        public IDoubleToBrushMap DoubleToBrushMap
        {
            get
            {
                return this.doubleToBrushMap;
            }

            set
            {
                if (value != this.doubleToBrushMap)
                {
                    this.doubleToBrushMap = value;
                    this.OnPropertyChanged("DoubleToBrushMap");
                }
            }
        }

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
                    this.OnPropertyChanged("ValueStroke");
                }
            }
        }

        public Brush ValueStroke
        {
            get
            {
                if (this.DoubleToBrushMap == null)
                {
                    return this.Stroke;
                }
                else
                {
                    return this.DoubleToBrushMap.Map(this.Value);
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