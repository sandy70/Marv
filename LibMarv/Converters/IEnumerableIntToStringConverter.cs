using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LibMarv
{
    [ValueConversion(typeof(IEnumerable<int>), typeof(string))]
    public class IEnumerableIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var values = value as IEnumerable<int>;

            if (values != null)
            {
                string str = "";

                int nValues = values.Count();

                if (nValues > 0)
                {
                    for (int i = 0; i < nValues - 1; i++)
                    {
                        str = str + values.ElementAt(i) + ", ";
                    }

                    str = str + values.ElementAt(nValues - 1);
                }

                return str;
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var str = value as string;
            var list = new ObservableCollection<int>();

            if (str != null)
            {
                string[] parts = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    int group;

                    if (int.TryParse(part, out group))
                    {
                        list.Add(group);
                    }
                    else
                    {
                        Console.WriteLine("Cannot parse: " + part);
                    }
                }

                return list;
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}
