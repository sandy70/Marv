using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibBn
{
    public class KeyValueStore<TKey, TValue> : IEnumerable<TValue>
    {
        private Dictionary<TKey, TValue> valueByKey = new Dictionary<TKey, TValue>();

        public TValue GetValue(TKey key)
        {
            return this.valueByKey[key];
        }

        public void SetValue(TKey key, TValue value)
        {
            this.valueByKey[key] = value;
        }

        public bool HasValue(TKey key)
        {
            return this.valueByKey.ContainsKey(key);
        }

        public IEnumerator GetEnumerator()
        {
            return this.valueByKey.Values.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return this.valueByKey.Values.GetEnumerator();
        }
    }
}
