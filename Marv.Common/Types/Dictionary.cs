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

        public new List<T1> Keys
        {
            get
            {
                return new List<T1>(base.Keys);
            }
        }
    }

    public class Dictionary<T1, T2, T3, TValue> : Dictionary<T1, Dictionary<T2, T3, TValue>> where TValue : new()
    {
        public Dictionary<T3, TValue> this[T1 key1, T2 key2]
        {
            get
            {
                if (this.ContainsKey(key1))
                {
                    if (this[key1].ContainsKey(key2))
                    {
                        return this[key1][key2];
                    }

                    return this[key1][key2] = new Dictionary<T3, TValue>();
                }

                this[key1] = new Dictionary<T2, T3, TValue>();

                return this[key1][key2] = new Dictionary<T3, TValue>();
            }

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
    }
}