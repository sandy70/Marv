using System;
using System.Collections.Generic;

namespace Marv.Common.Types
{
    public partial class Dict<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public bool IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return this.keys; }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return this.values; }
        }

        public void Add(TKey key, TValue value)
        {
            if (this.ContainsKey(key))
            {
                var oldKvp = this.dictionary[key];
                oldKvp.Value = value;

                this.Values.Replace(oldKvp.Value, value);
            }
            else
            {
                this.Add(new Kvp<TKey, TValue>
                {
                    Key = key,
                    Value = value
                });

                this.Keys.Add(key);
                this.Values.Add(value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.dictionary.ContainsKey(item.Key) && this.dictionary[item.Key].Equals(item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return key != null && this.dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new DictEnumerator<TKey, TValue>(this);
        }

        public bool Remove(TKey key)
        {
            if (this.dictionary.ContainsKey(key))
            {
                var oldKvp = this.dictionary[key];

                this.Keys.Remove(oldKvp.Key);
                this.Values.Remove(oldKvp.Value);

                return this.Remove(oldKvp);
            }

            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (this.dictionary.ContainsKey(item.Key))
            {
                var kvp = this.dictionary[item.Key];
                this.Remove(kvp);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Kvp<TKey, TValue> kvp;

            var isSuccess = this.dictionary.TryGetValue(key, out kvp);

            value = isSuccess ? kvp.Value : default(TValue);

            return isSuccess;
        }
    }
}