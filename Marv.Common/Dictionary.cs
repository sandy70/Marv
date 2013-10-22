using System.Collections.Generic;

namespace Marv.Common
{
    public class Dictionary<T1, T2, TValue> : Dictionary<T1, Dictionary<T2, TValue>> { }
    public class Dictionary<T1, T2, T3, TValue> : Dictionary<T1, Dictionary<T2, T3, TValue>> { }
}