using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MoreLinq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;
using DataPoint = OxyPlot.DataPoint;
using LinearAxis = Telerik.Windows.Controls.ChartView.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using LogarithmicAxis = Telerik.Windows.Controls.ChartView.LogarithmicAxis;

namespace Marv.Input
{
    public partial class MainWindow
    {
        public enum LineType
        {
            Mode,
            Min,
            Max
        };

        public static readonly DependencyProperty IsChartEditEnabledProperty =
            DependencyProperty.Register("IsChartEditEnabled", typeof (bool), typeof (MainWindow), new PropertyMetadata(false));

        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register("VerticalAxis", typeof (CartesianAxis), typeof (MainWindow), new PropertyMetadata(null));

        private readonly LinearAxis linearAxis = new LinearAxis();
        private readonly LogarithmicAxis logarightmicAxis = new LogarithmicAxis();

        private ScatterSeries inputScatter;
        private LineSeries maxLine;
        private ScatterSeries maxScatter;
        private LineSeries minLine;
        private ScatterSeries minScatter;
        private LineSeries modeLine;
        private ScatterSeries modeScatter;

        public bool IsChartEditEnabled
        {
            get
            {
                return (bool) GetValue(IsChartEditEnabledProperty);
            }
            set
            {
                SetValue(IsChartEditEnabledProperty, value);
            }
        }

        public CartesianAxis VerticalAxis
        {
            get
            {
                return (CartesianAxis) GetValue(VerticalAxisProperty);
            }

            set
            {
                SetValue(VerticalAxisProperty, value);
            }
        }
    }
}