using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Marv.Common;
using Marv.Common.Types;
using Marv.Controls;
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

           return this.MinNumberPoints;
        }

        public string GetInterpolatedEvidenceString(List<double> interpolatedValues)
        {
            return "" + interpolatedValues[0] + ":" + interpolatedValues[2] ;
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