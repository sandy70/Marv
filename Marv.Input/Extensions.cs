using System.Collections.Generic;
using System.Linq;
using Marv;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public static class Extensions
    {
        public static IEnumerable<CellModel> ToCellModels(this IEnumerable<dynamic> rows, string header)
        {
            return rows.Select(row => new CellModel(row, header));
        }

        public static IEnumerable<CellModel> ToCellModels(this GridViewColumnCollection columns, Dynamic row)
        {
            return columns.Select<GridViewColumn, CellModel>(col => new CellModel(row, col.Header as string));
        }

        public static CellModel ToModel(this GridViewCell cell)
        {
            return new CellModel(cell);
        }

        public static CellModel ToModel(this GridViewCellInfo cellInfo)
        {
            return new CellModel(cellInfo);
        }
    }
}