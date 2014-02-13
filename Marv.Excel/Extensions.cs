using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Excel
{
    public static class Extensions
    {
        public static void SetCellValue(this Worksheet worksheet, int rowIndex, int colIndex, string value)
        {
            var range = (Range)worksheet.Cells.get_Item(rowIndex, colIndex);
            range.Value2 = value;
        }
    }
}
