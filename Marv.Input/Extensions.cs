using System;
using System.Collections.Generic;
using System.Linq;
using Marv.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public static class Extensions
    {
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