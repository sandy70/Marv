using System;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public static class Extensions
    {
        public static string String(this DateTime dateTime)
        {
            return dateTime.ToString("DyyyyMMddThhmmss");
        }

        public static CellModel ToModel(this GridViewCell cell)
        {
            return new CellModel(cell);
        }

        public static CellModel ToModel(this GridViewCellInfo cellInfo)
        {
            return new CellModel(cellInfo);
        }

        public static bool TryParse(this string str, out DateTime dateTime)
        {
            int date;
            int time;

            if (str[0] == 'D' && int.TryParse(str.Substring(1, 8), out date) && str[9] == 'T' && int.TryParse(str.Substring(10, 6), out time))
            {
                dateTime = new DateTime(date / 10000, (date % 10000) / 100, date % 100, time / 10000, (time % 10000) / 100, time % 100);
                return true;
            }

            dateTime = new DateTime();
            return false;
        }
    }
}