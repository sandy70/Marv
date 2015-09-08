using System.Collections.Generic;
using System.ComponentModel;
using Marv.Common.Interpolators;

namespace Marv.Input
{
    public interface IInterpolatorDataPoints : INotifyPropertyChanged
    {
        bool IsLineCross { get; }
        string GetInterpolatedEvidenceString(List<double> interpolatedValues);
        List<LinearInterpolator> GetLinearInterpolators();
    }
}