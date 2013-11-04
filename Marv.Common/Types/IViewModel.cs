using System.ComponentModel;

namespace Marv.Common
{
    public interface IViewModel : INotifyPropertyChanged
    {
        string Key { get; set; }

        string Name { get; set; }
    }
}