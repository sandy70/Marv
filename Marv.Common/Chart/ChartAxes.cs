using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls.ChartView;

namespace Marv.Common
{
    public static class ChartAxes
    {
        public static CartesianAxis HorizontalLinearAxis = new LinearAxis();
        public static CartesianAxis VerticalLinearAxis = new LinearAxis();
        public static CartesianAxis HorizontalCategoricalAxis = new CategoricalAxis();
        public static CartesianAxis VerticalCategoricalAxis = new LinearAxis();
    }
}
