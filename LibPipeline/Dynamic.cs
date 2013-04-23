using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace LibPipeline
{
    public class Dynamic : DynamicObject
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        [Browsable(false)]
        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        [Browsable(false)]
        public IEnumerable<string> PropertyNames
        {
            get
            {
                return this.dictionary.Keys.AsEnumerable<string>();
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.dictionary.Keys.AsEnumerable<string>();
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            string name = (string)indexes[0];
            return dictionary.TryGetValue(name, out result);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            string name = (string)indexes[0];
            dictionary[name.ToLower()] = value;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dictionary[binder.Name.ToLower()] = value;
            return true;
        }
    }
}