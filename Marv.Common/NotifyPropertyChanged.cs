using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Marv.Common
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null && propertyName.Length > 0)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}