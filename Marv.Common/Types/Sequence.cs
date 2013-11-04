using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common
{
    public class Sequence<T> : IEnumerable<T> where T : IComparable<T>
    {
        private List<T> list = new List<T>();

        public T Max
        {
            get
            {
                return this.list.Last();
            }
        }

        public T Min
        {
            get
            {
                return this.list.First();
            }
        }

        public void Add(T item)
        {
            this.list.Add(item);
            this.list.Sort();
        }

        public bool Bounds(T item)
        {
            return item.CompareTo(this.Min) >= 0 && item.CompareTo(this.Max) <= 0;
        }

        public bool Contains(T item)
        {
            return this.list.Contains(item);
        }

        public int GetBinIndex(T item)
        {
            var index = -1;

            if (this.Bounds(item))
            {
                foreach (var listItem in this.list)
                {
                    if (item.CompareTo(listItem) >= 0)
                    {
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return index;
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