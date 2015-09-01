using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Marv.Common.Interpolators;
using Telerik.Charting;

namespace Marv.Input
{
    internal class SingleValueInterpolator : IInterpolatorDataPoints
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

        public SingleValueInterpolator()
        {
            this.MaxNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };

            this.MinNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };

            this.ModeNumberPoints = new ObservableCollection<ScatterDataPoint> { new ScatterDataPoint() };
        }

        public string GetInterpolatedEvidenceString(List<double> interpolatedValues)
        {
            return Math.Round(interpolatedValues[0], 2).ToString();
        }

        public List<LinearInterpolator> GetLinearInterpolators()
        {
            var linearInterpolators = new List<LinearInterpolator>();

            var xCoordsMode = this.GetNumberPoints(Utils.ModeInterpolatorLine).GetXCoords();
            var yCoordsMode = this.GetNumberPoints(Utils.ModeInterpolatorLine).GetYCoords();

            linearInterpolators.Add(new LinearInterpolator(xCoordsMode, yCoordsMode));

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