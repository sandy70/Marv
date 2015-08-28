﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Marv.Controls.Converters
{
   [ValueConversion(typeof(string), typeof(string))]
    public class LongDateToShortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
                
            }
            if (value.ToString().Equals("From") || value.ToString().Equals("To"))
            {
                return "";

            }

            var shortString = value.ToString().Substring(5, 2) + "/"+ value.ToString().Substring(7, 2) + "/" + value.ToString().Substring(1, 4);

            return shortString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
