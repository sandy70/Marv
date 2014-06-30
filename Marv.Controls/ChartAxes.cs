using Telerik.Windows.Controls.ChartView;

namespace Marv.Controls
{
    public static class ChartAxes
    {
        public static CartesianAxis HorizontalLinearAxis = new LinearAxis();
        public static CartesianAxis VerticalLinearAxis = new LinearAxis();
        public static CartesianAxis HorizontalCategoricalAxis = new CategoricalAxis();
        public static CartesianAxis VerticalCategoricalAxis = new LinearAxis();
    }
}