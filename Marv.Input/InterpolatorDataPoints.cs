using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telerik.Charting;

namespace Marv.Input
{
    public class InterpolatorDataPoints : INotifyPropertyChanged
    {
        private ObservableCollection<ScatterDataPoint> maxNumberPoints;
        private ObservableCollection<ScatterDataPoint> minNumberPoints;
        private ObservableCollection<ScatterDataPoint> modeNumberPoints;

        public ObservableCollection<ScatterDataPoint> MaxNumberPoints
        {
            get { return this.maxNumberPoints; }
            set
            {
                this.maxNumberPoints = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<ScatterDataPoint> MinNumberPoints
        {
            get { return this.minNumberPoints; }
            set
            {
                this.minNumberPoints = value;
                this.RaisePropertyChanged();
            }
        }

        public ObservableCollection<ScatterDataPoint> ModeNumberPoints
        {
            get { return this.modeNumberPoints; }
            set
            {
                this.modeNumberPoints = value;
                this.RaisePropertyChanged();
            }
        }

        public InterpolatorDataPoints()
        {
            this.MaxNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };
            this.ModeNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };
            this.MinNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };
        }

        public ObservableCollection<ScatterDataPoint> GetNumberPoints(string selectedLine)
        {
            if (selectedLine.Equals(Utils.MaxInterpolatorLine))
            {
                return this.MaxNumberPoints;
            }

            if (selectedLine.Equals(Utils.ModeInterpolatorLine))
            {
                return this.ModeNumberPoints;
            }

            return this.MinNumberPoints;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null && propertyName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}