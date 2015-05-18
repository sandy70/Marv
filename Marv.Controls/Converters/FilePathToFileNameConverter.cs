using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Marv;
using Marv.Common;

namespace Marv.Controls.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class FilePathToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
      /*       if (this.Network.FileName != null)
           {
               char [] delimiter= {'\\'};
               string [] folders= this.Network.FileName.Split(delimiter);
               string filename = folders[folders.Length - 1];
               this.Network.FileName = filename;
           }*/

            var filename = "";

                if (value.ToString() != "")
                {
                  filename =  Path.GetFileName(value.ToString());
                }

            return filename;
        }
     

            
        

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
