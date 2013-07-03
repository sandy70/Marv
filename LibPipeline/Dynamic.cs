﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace LibPipeline
{
    public class Dynamic : DynamicObject, INotifyPropertyChanged
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        public object this[string name]
        {
            get
            {
                if (this.GetType().GetProperties().Where(info => info.Name.Equals(name)).Count() == 0)
                {
                    if (dictionary.ContainsKey(name))
                    {
                        return dictionary[name];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return this.GetType().GetProperty(name).GetValue(this);
                }
            }
            set
            {
                if (this.GetType().GetProperties().Where(info => info.Name.Equals(name)).Count() == 0)
                {
                    dictionary[name] = value;
                }
                else
                {
                    this.GetType().GetProperty(name).SetValue(this, value);
                }
            }
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.dictionary.Keys.AsEnumerable<string>();
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            string name = (string)indexes[0];

            if (this.GetType().GetProperties().Where(info => info.Name.Equals(name)).Count() == 0)
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
            string name = binder.Name;
            return dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            string name = (string)indexes[0];

            if (this.GetType().GetProperties().Where(info => info.Name.Equals(name)).Count() == 0)
            {
                dictionary[name] = value;
            }
            else
            {
                this.GetType().GetProperty(name).SetValue(this, value);
            }

            this.OnPropertyChanged(name);

            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dictionary[binder.Name] = value;
            this.OnPropertyChanged(binder.Name);
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}