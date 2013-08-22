using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LibPipeline
{
    public class MultiLocation : SelectableCollection<Location>
    {
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();
        private string name;

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
            }
        }

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
    }
}