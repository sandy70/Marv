using QuickGraph;
using System;
using System.ComponentModel;
using Telerik.Windows.Diagrams.Core;

namespace LibBn
{
    [Serializable]
    public class BnEdge : Edge<BnVertex>, ILink<BnVertex>, INotifyPropertyChanged
    {
        private double _value = 1;

        public BnEdge(BnVertex source, BnVertex target)
            : base(source, target)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        object ILink.Source
        {
            get
            {
                return this.Source;
            }
            set
            {
                object temp = value;
            }
        }

        object ILink.Target
        {
            get
            {
                return this.Target;
            }
            set
            {
                object temp = value;
            }
        }

        public new BnVertex Source
        {
            get { return base.Source; }
            set { }
        }

        public new BnVertex Target
        {
            get { return base.Target; }
            set { }
        }

        public double Value
        {
            get
            {
                return this._value;
            }

            set
            {
                if (value != this._value)
                {
                    this._value = value;
                    this.OnPropertyChanged("Value");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}