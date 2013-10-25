using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Marv.Common
{
    public interface IChartSeries : INotifyPropertyChanged
    {
        string Name { get; set; }

        Brush Stroke { get; set; }

        Type Type { get; set; }

        string XLabel { get; set; }
    }
}