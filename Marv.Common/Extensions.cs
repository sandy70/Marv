using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Common
{
    public static partial class Extensions
    {
        public static void Push<T>(this ObservableCollection<T> collection, T item)
        {
            collection.Insert(0, item);
        }
    }
}
