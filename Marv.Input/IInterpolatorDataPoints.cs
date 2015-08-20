using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Charting;

namespace Marv.Input
{
    public interface IInterpolatorDataPoints : INotifyPropertyChanged
    {
        bool IsLineCross { get; set; }
        ObservableCollection<ScatterDataPoint> MaxNumberPoints { get; set; }
        ObservableCollection<ScatterDataPoint> MinNumberPoints { get; set; }
        ObservableCollection<ScatterDataPoint> ModeNumberPoints { get; set; }
        string GetInterpolatedEvidenceString(List<double> interpolatedValues);
        ObservableCollection<ScatterDataPoint> GetNumberPoints(string selectedLine);
    }
}