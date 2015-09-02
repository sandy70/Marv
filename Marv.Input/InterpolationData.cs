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
    }
}