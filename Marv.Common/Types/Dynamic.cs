using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Marv
{
    public class Dynamic : DynamicObject, INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public object this[string name]
        {
            get
            {
                if (!this.GetType().GetProperties().Any(info => info.Name.Equals(name)))
                {
                    return this.dictionary.ContainsKey(name) ? this.dictionary[name] : null;
                }

                return this.GetType().GetProperty(name).GetValue(this);
            }

            set
            {
                if (this.GetType().GetProperties().Any(info => info.Name.Equals(name)))
                {
                    this.GetType().GetProperty(name).SetValue(this, value);
                }
                else
                {
                    this.dictionary[name] = value;
                }

                this.RaisePropertyChanged(name);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.dictionary.Keys.AsEnumerable();
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var name = (string) indexes[0];

            if (this.GetType().GetProperties().Count(info => info.Name.Equals(name)) == 0)
            {
                if (dictionary.ContainsKey(name))
                {
                    result = dictionary[name];
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                result = this.GetType().GetProperty(name).GetValue(this);
            }

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name;
            return dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var name = (string) indexes[0];

            if (this.GetType().GetProperties().Any(info => info.Name.Equals(name)))
            {
                this.GetType().GetProperty(name).SetValue(this, value);
            }
            else
            {
                this.dictionary[name] = value;
            }

            this.RaisePropertyChanged(name);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dictionary[binder.Name] = value;
            this.RaisePropertyChanged(binder.Name);
            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}