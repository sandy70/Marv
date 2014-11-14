using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Marv
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null && propertyName.Length > 0)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}