using System.Collections.Generic;

namespace Marv.Common
{
    public class Dictionary<T1, T2, TValue> : Dictionary<T1, Dictionary<T2, TValue>> where TValue : new()
    {
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
                if (this.ContainsKey(key1))
                {
                    if (this[key1].ContainsKey(key2))
                    {
                        return this[key1][key2];
                    }
                    else
                    {
                        return this[key1][key2] = new TValue();
                    }
                }
                else
                {
                    this[key1] = new Dictionary<T2, TValue>();
                    return this[key1][key2] = new TValue();
                }
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
    }

    public class Dictionary<T1, T2, T3, TValue> : Dictionary<T1, Dictionary<T2, T3, TValue>> where TValue : new() 
    {
        public Dictionary<T3, TValue> this[T1 key1, T2 key2]
        {
            get
            {
                if(this.ContainsKey(key1))
                {
                    if(this[key1].ContainsKey(key2))
                    {
                        return this[key1][key2];
                    }
                    else
                    {
                        return this[key1][key2] = new Dictionary<T3, TValue>();
                    }
                }
                else
                {
                    this[key1] = new Dictionary<T2, T3, TValue>();
                    return this[key1][key2] = new Dictionary<T3, TValue>();
                }
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

    public class Dictionary<T1, T2, T3, T4, TValue> : Dictionary<T1, Dictionary<T2, T3, T4, TValue>> where TValue : new() { }
}