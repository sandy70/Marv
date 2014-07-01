﻿using System.Collections.Generic;

namespace Marv.Common
{
    public class Dictionary<T1, T2, TValue> : Dictionary<T1, Dictionary<T2, TValue>>
    {
        public Dictionary()
        {
        }

        public Dictionary(Dictionary<T1, Dictionary<T2, TValue>> dict)
        {
            foreach (var key in dict.Keys)
            {
                this[key] = dict[key];
            }
        }

        public new List<T1> Keys
        {
            get
            {
                return new List<T1>(base.Keys);
            }
        }

        public TValue this[T1 key1, T2 key2]
        {
            get
            {
                return this[key1][key2];
            }

            set
            {
                if (this.ContainsKey(key1))
                {
                    this[key1][key2] = value;
                }
                else
                {
                    this[key1] = new Dictionary<T2, TValue>();
                    this[key1][key2] = value;
                }
            }
        }

        public new Dictionary<T2, TValue> this[T1 key]
        {
            get
            {
                if (this.ContainsKey(key))
                {
                    return base[key];
                }

                return this[key] = new Dictionary<T2, TValue>();
            }

            set
            {
                base[key] = value;
            }
        }

        public bool ContainsKey(T1 key1, T2 key2)
        {
            return this.ContainsKey(key1) && this[key1].ContainsKey(key2);
        }

        public TValue GetValueOrNew(T1 key1, T2 key2)
        {
            if (!this.ContainsKey(key1))
            {
                this[key1] = new Dictionary<T2, TValue>();
            }

            if (!this[key1].ContainsKey(key2))
            {
                this[key1][key2] = Utils.Create<TValue>();
            }

            return this[key1][key2];
        }
    }

    public class Dictionary<T1, T2, T3, TValue> : Dictionary<T1, Dictionary<T2, T3, TValue>>
    {
        public Dictionary<T3, TValue> this[T1 key1, T2 key2]
        {
            set
            {
                if (this.ContainsKey(key1))
                {
                    this[key1][key2] = value;
                }
                else
                {
                    this[key1] = new Dictionary<T2, T3, TValue>();
                    this[key1][key2] = value;
                }
            }
        }

        public Dictionary<T3, TValue> GetValueOrNew(T1 key1, T2 key2)
        {
            if (!this.ContainsKey(key1))
            {
                this[key1] = new Dictionary<T2, T3, TValue>();
            }

            if (!this[key1].ContainsKey(key2))
            {
                this[key1][key2] = Utils.Create<Dictionary<T3, TValue>>();
            }

            return this[key1][key2];
        }
    }

    public class Dictionary<T1, T2, T3, T4, TValue> : Dictionary<T1, Dictionary<T2, T3, T4, TValue>>
    {
        public Dictionary<T4, TValue> this[T1 key1, T2 key2, T3 key3]
        {
            set
            {
                if (!this.ContainsKey(key1))
                {
                    this[key1] = new Dictionary<T2, T3, T4, TValue>();
                }

                this[key1][key2, key3] = value;
            }
        }
    }
}