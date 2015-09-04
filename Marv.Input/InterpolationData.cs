using System.Collections.ObjectModel;
using Marv.Common;
using Telerik.Charting;

namespace Marv.Input
{
    public class InterpolationData : NotifyPropertyChanged
    {
        private ObservableCollection<ObservableCollection<ScatterDataPoint>> points;
        private InterpolationType? type;

        public ObservableCollection<ObservableCollection<ScatterDataPoint>> Points
        {
            get { return this.points; }

            set
            {
                this.points = value;
                this.RaisePropertyChanged();
            }
        }

        public InterpolationType? Type
        {
            get { return this.type; }

            set
            {
                this.type = value;
                this.RaisePropertyChanged();
            }
        }

        public void CreatePoints(double xMax, double xMin, double yMax, double yMin)
        {
            var mid = (yMax + yMin) / 2;
            var bot = (mid + yMin) / 2;
            var top = (mid + yMax) / 2;

            if (this.Type == InterpolationType.SingleValue)
            {
                this.Points = new ObservableCollection<ObservableCollection<ScatterDataPoint>>
                {
                    new ObservableCollection<ScatterDataPoint>
                    {
                        new ScatterDataPoint { XValue = xMin, YValue = mid },
                        new ScatterDataPoint { XValue = xMax, YValue = mid },
                    }
                };
            }

            else if (this.Type == InterpolationType.Uniform)
            {
                this.Points = new ObservableCollection<ObservableCollection<ScatterDataPoint>>
                {
                    new ObservableCollection<ScatterDataPoint>
                    {
                        new ScatterDataPoint { XValue = xMin, YValue = bot },
                        new ScatterDataPoint { XValue = xMax, YValue = bot },
                    },
                    new ObservableCollection<ScatterDataPoint>
                    {
                        new ScatterDataPoint { XValue = xMin, YValue = top },
                        new ScatterDataPoint { XValue = xMax, YValue = top },
                    }
                };
            }

            else if (this.Type == InterpolationType.Triangular)
            {
                this.Points = new ObservableCollection<ObservableCollection<ScatterDataPoint>>
                {
                    new ObservableCollection<ScatterDataPoint>
                    {
                        new ScatterDataPoint { XValue = xMin, YValue = bot },
                        new ScatterDataPoint { XValue = xMax, YValue = bot },
                    },
                    new ObservableCollection<ScatterDataPoint>
                    {
                        new ScatterDataPoint { XValue = xMin, YValue = mid },
                        new ScatterDataPoint { XValue = xMax, YValue = mid },
                    },
                    new ObservableCollection<ScatterDataPoint>
                    {
                        new ScatterDataPoint { XValue = xMin, YValue = top },
                        new ScatterDataPoint { XValue = xMax, YValue = top },
                    }
                };
            }
        }
    }
}