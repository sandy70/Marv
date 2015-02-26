using System.Collections.ObjectModel;

namespace Marv.Common
{
    public interface IVertex
    {
        ObservableCollection<State> States { get; }
        VertexType Type { get; set; }
    }
}