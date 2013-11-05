using System;
using System.ComponentModel;
using System.Windows.Media;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Common
{
    public interface IChartSeries : IViewModel
    {
        Brush Stroke { get; set; }

        Type Type { get; set; }

        string XLabel { get; set; }
    }

    public class ChartSeries<T> : ViewModelCollection<T>, IChartSeries where T : class, IViewModel
    {
        private CartesianAxis horizontalAxis;
        private Brush stroke = new SolidColorBrush(Colors.LightBlue);
        private Type type;
        private CartesianAxis verticalAxis;

        private string xLabel;

        public ChartSeries()
            : base()
        {
            if (this is ChartSeries<ScatterPoint>)
            {
                this.HorizontalAxis = ChartAxes.HorizontalLinearAxis;
                this.VerticalAxis = ChartAxes.VerticalLinearAxis;
            }
            else if (this is ChartSeries<CategoricalPoint>)
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