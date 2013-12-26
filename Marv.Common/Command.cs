using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv.Common
{
    public interface ICommand
    {
        void Execute();
    }

    public abstract class Command<T> : ViewModel, ICommand
    {
        private T associatedObject;
        private string imageSource;

        public Command(T _object)
        {
            this.AssociatedObject = _object;
        }

        public T AssociatedObject
        {
            get
            {
                return this.associatedObject;
            }

            set
            {
                if (!this.associatedObject.Equals(value))
                {
                    this.associatedObject = value;
                    this.RaisePropertyChanged("AssociatedObject");
                }
            }
        }

        public string ImageSource
        {
            get
            {
                return this.imageSource;
            }

            set
            {
                if (value != this.imageSource)
                {
                    this.imageSource = value;
                    this.RaisePropertyChanged("ImageSource");
                }
            }
        }

        public virtual void Execute() { }
    }
}
