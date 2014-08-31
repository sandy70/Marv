using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Marv.Common.Graph;
using Telerik.Charting;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    [ValueConversion(typeof (GridViewCell), typeof (IEnumerable<ScatterDataPoint>))]
    internal class GridViewCellToScatterPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GridViewCell)
            {
                var gridViewCell = value as GridViewCell;
                var cellModel = gridViewCell.ToModel();

                if (cellModel.IsColumnSectionId)
                {
                    return Binding.DoNothing;
                }

                var vertexData = cellModel.Data as VertexData;

                return vertexData.Evidence.Select((y, i) => new ScatterDataPoint
                {
                    XValue = i,
                    YValue = y
                });
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}