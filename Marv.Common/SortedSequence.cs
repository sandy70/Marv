using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Marv.Common
{
    internal class SortedSequence<T> : ObservableCollection<T> where T : IComparable<T>
    {

    }
}