﻿using Marv.Common;

namespace Marv.Controls
{
    public class ChartSeriesDescriptorSelector : Telerik.Windows.Controls.ChartView.ChartSeriesDescriptorSelector
    {
        public override Telerik.Windows.Controls.ChartView.ChartSeriesDescriptor SelectDescriptor(Telerik.Windows.Controls.ChartView.ChartSeriesProvider provider, object context)
        {
            if (context is ChartSeries<ScatterPoint>)
            {
                return new Telerik.Windows.Controls.ChartView.ScatterSeriesDescriptor
                {
                    XValuePath = "XValue",
                    YValuePath = "YValue",
                    TypePath = "Type",
                };
            }

            if (context is ChartSeries<CategoricalPoint>)
            {
                return new Telerik.Windows.Controls.ChartView.CategoricalSeriesDescriptor
                {
                    CategoryPath = "Category",
                    ValuePath = "Value",
                    TypePath = "Type"
                };
            }

            return base.SelectDescriptor(provider, context);
        }
    }
}