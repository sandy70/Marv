using System;
using System.Windows.Media;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Controls
{
    public interface IChartSeries : IModel
    {
        Brush Stroke { get; set; }

        Type Type { get; set; }

        string XLabel { get; set; }
    }

    public class ChartSeries<T> : KeyedCollection<T> where T : IKeyed
    {
        private Brush stroke = new SolidColorBrush(Colors.LightBlue);
        private Type type;

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
                    this.RaisePropertyChanged();
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
                    this.RaisePropertyChanged();
                }
            }
        }
    }
}