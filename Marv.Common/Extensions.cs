using System.Collections.ObjectModel;

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