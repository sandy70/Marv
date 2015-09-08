using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Marv.Common.Interpolators;
using Telerik.Charting;

namespace Marv.Input
{
    internal class TriangularInterpolator : IInterpolatorDataPoints
    {
        private bool isLineCross;

        private ObservableCollection<ScatterDataPoint> maxNumberPoints;
        private ObservableCollection<ScatterDataPoint> minNumberPoints;
        private ObservableCollection<ScatterDataPoint> modeNumberPoints;

        public bool IsLineCross
        {
            get { return isLineCross; }
            set
            {
                isLineCross = value;
                this.RaisePropertyChanged();
            }
        }

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

        public TriangularInterpolator()
        {
            this.MaxNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };
            this.ModeNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };
            this.MinNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };
        }

        public string GetInterpolatedEvidenceString(List<double> interpolatedValues)
        {
            return "tri(" + interpolatedValues[0] + "," + interpolatedValues[1] + "," + interpolatedValues[2] + ")";
        }

        public List<LinearInterpolator> GetLinearInterpolators()
        {
            var linearInterpolators = new List<LinearInterpolator>();
            var xCoordsMaximum = this.GetNumberPoints(Utils.MaxInterpolatorLine).GetXCoords();
            var yCoordsMaximum = this.GetNumberPoints(Utils.MaxInterpolatorLine).GetYCoords();

            linearInterpolators.Add(new LinearInterpolator(xCoordsMaximum, yCoordsMaximum));

            var xCoordsMode = this.GetNumberPoints(Utils.ModeInterpolatorLine).GetXCoords();
            var yCoordsMode = this.GetNumberPoints(Utils.ModeInterpolatorLine).GetYCoords();

            linearInterpolators.Add(new LinearInterpolator(xCoordsMode, yCoordsMode));

            var xCoordsMinimum = this.GetNumberPoints(Utils.MinInterpolatorLine).GetXCoords();
            var yCoordsMinimum = this.GetNumberPoints(Utils.MinInterpolatorLine).GetYCoords();

            linearInterpolators.Add(new LinearInterpolator(xCoordsMinimum, yCoordsMinimum));

            return linearInterpolators;
        }

        public ObservableCollection<ScatterDataPoint> GetNumberPoints(string selectedLine)
        {
            if (selectedLine == null)
            {
                return null;
            }

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