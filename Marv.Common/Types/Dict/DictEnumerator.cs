using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Marv.Common
{
    public class DictEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly IEnumerator<Kvp<TKey, TValue>> enumerator;

        public DictEnumerator(Dict<TKey, TValue> aDict)
        {
            this.enumerator = (aDict as ObservableCollection<Kvp<TKey, TValue>>).GetEnumerator();
        }

        public KeyValuePair<TKey, TValue> Current
        {
            get
            {
                return new KeyValuePair<TKey, TValue>(this.enumerator.Current.Key, this.enumerator.Current.Value);
            }
        }

        public void Dispose()
        {
            this.enumerator.Dispose();
        }

        object IEnumerator.Current
        {
            get
            {
                return this.enumerator.Current;
            }
        }

        public bool MoveNext()
        {
            return this.enumerator.MoveNext();
        }

        public void Reset()
        {
            this.enumerator.Reset();
        }
    }
}