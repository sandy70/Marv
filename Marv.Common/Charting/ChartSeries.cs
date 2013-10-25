using System;
using System.ComponentModel;
using System.Windows.Media;
using Telerik.Charting;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Common
{
    public class ChartSeries<T> : ViewModelCollection<T>, IChartSeries where T : INotifyPropertyChanged
    {
        private CartesianAxis horizontalAxis;
        private Brush stroke = new SolidColorBrush(Colors.LightBlue);
        private Type type;
        private CartesianAxis verticalAxis;

        private string xLabel;

        public ChartSeries()
            : base()
        {
            if (this is ChartSeries<ScatterDataPoint>)
            {
                this.HorizontalAxis = ChartAxes.HorizontalLinearAxis;
                this.VerticalAxis = ChartAxes.VerticalLinearAxis;
            }
            else if (this is ChartSeries<CategoricalDataPoint>)
            {
                this.HorizontalAxis = ChartAxes.HorizontalCategoricalAxis;
                this.VerticalAxis = ChartAxes.VerticalCategoricalAxis;
            }
        }

        public CartesianAxis HorizontalAxis
        {
            get
            {
                return this.horizontalAxis;
            }

            set
            {
                if (value != this.horizontalAxis)
                {
                    this.horizontalAxis = value;
                    this.RaisePropertyChanged("HorizontalAxis");
                }
            }
        }

        public Brush Stroke
        {
            get
            {
                return this.stroke;
            }

            set
            {
                if (value != this.stroke)
                {
                    this.stroke = value;
                    this.RaisePropertyChanged("Stroke");
                }
            }
        }

        public Type Type
        {
            get
            {
                return this.type;
            }

            set
            {
                if (value != this.type)
                {
                    this.type = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }

        public CartesianAxis VerticalAxis
        {
            get
            {
                return this.verticalAxis;
            }

            set
            {
                if (value != this.verticalAxis)
                {
                    this.verticalAxis = value;
                    this.RaisePropertyChanged("VerticalAxis");
                }
            }
        }

        public string XLabel
        {
            get
            {
                return this.xLabel;
            }

            set
            {
                if (value != this.xLabel)
                {
                    this.xLabel = value;
                    this.RaisePropertyChanged("XLabel");

                    this.HorizontalAxis.Title = this.XLabel;
                }
            }
        }
    }
}