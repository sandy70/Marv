using System.Collections.Generic;

namespace Marv.Common
{
    public class Dictionary<TRowKey, TColKey, TValue>
    {
        private Dictionary<TRowKey, Dictionary<TColKey, TValue>> dictionary = new Dictionary<TRowKey, Dictionary<TColKey, TValue>>();

        public TValue this[TRowKey rowKey, TColKey colKey]
        {
            get
            {
                return this.dictionary[rowKey][colKey];
            }

            set
            {
                if (!this.dictionary.ContainsKey(rowKey))
                {
                    this.dictionary[rowKey] = new Dictionary<TColKey, TValue>();
                }

                this.dictionary[rowKey][colKey] = value;
            }
        }
    }
}