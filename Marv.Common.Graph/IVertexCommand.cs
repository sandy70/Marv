using System;
using System.ComponentModel;

namespace Marv.Common.Graph
{
    public interface IVertexCommand : INotifyPropertyChanged
    {
        event EventHandler<Vertex> Executed;

        string ImageSource { get; set; }

        void RaiseExecuted(Vertex vertexViewModel);
    }
}