using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Marv.Common.Interpolators;
using Telerik.Charting;

namespace Marv.Input
{
    internal class UniformInterpolator : IInterpolatorDataPoints
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

        public UniformInterpolator()
        {
            this.MaxNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };

            this.MinNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };

            this.ModeNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };
        }

        public string GetInterpolatedEvidenceString(List<double> interpolatedValues)
        {
            return "" + interpolatedValues[0] + ":" + interpolatedValues[1];
        }

        public List<LinearInterpolator> GetLinearInterpolators()
        {
            var linearInterpolators = new List<LinearInterpolator>();
            var xCoordsMaximum = this.GetNumberPoints(Utils.MaxInterpolatorLine).GetXCoords();
            var yCoordsMaximum = this.GetNumberPoints(Utils.MaxInterpolatorLine).GetYCoords();

            linearInterpolators.Add(new LinearInterpolator(xCoordsMaximum, yCoordsMaximum));

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

        public bool IsWithInRange()
        {
            var currentLine = this;

            var currentMax = currentLine.GetNumberPoints(Utils.MaxInterpolatorLine);
            var currentMin = currentLine.GetNumberPoints(Utils.MinInterpolatorLine);

            var linearInterpolators = this.GetLinearInterpolators();

            var maxLinInterpolator = linearInterpolators[0];
            var minLinInterpolator = linearInterpolators[1];

            if (currentMax.Any(scatterPoint => !(scatterPoint.YValue > minLinInterpolator.Eval(scatterPoint.XValue))))
            {
                return false;
            }

            if (currentMin.Any(scatterPoint => !(maxLinInterpolator.Eval(scatterPoint.XValue) > scatterPoint.YValue)))
            {
                return false;
            }

            return true;
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