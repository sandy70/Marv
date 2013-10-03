using System;
using System.Collections;
using System.Collections.Generic;

namespace Marv.Common
{
    internal class SortedSequence<T> : IEnumerable<T> where T : IComparable<T>
    {
        private List<T> list = new List<T>();

        public void Add(T item)
        {
            this.list.Add(item);
            this.list.Sort();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }
    }
}