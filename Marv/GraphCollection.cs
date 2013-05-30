using LibBn;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Marv
{
    public class GraphCollection : ObservableCollection<BnGraph>
    {
        private ObservableCollection<BnVertex> vertices = new ObservableCollection<BnVertex>();

        public GraphCollection()
            : base()
        {
            this.CollectionChanged += GraphCollection_CollectionChanged;
        }

        public ObservableCollection<BnVertex> Vertices
        {
            get
            {
                return this.vertices;
            }

            set
            {
                if (value != this.vertices)
                {
                    this.vertices = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Vertices"));
                }
            }
        }

        public BnGraph this[string name]
        {
            get
            {
                return this.SingleOrDefault(x => x.Name.Equals(name));
            }
        }

        private void GraphCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    var newGraph = newItem as BnGraph;
                    newGraph.VertexAdded += newGraph_VertexAdded;
                    newGraph.VertexRemoved += newGraph_VertexRemoved;

                    foreach (var vertex in newGraph.Vertices)
                    {
                        Console.WriteLine("adding vertex: " + vertex.Name);
                        this.Vertices.Add(vertex);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var oldGraph = oldItem as BnGraph;

                    oldGraph.VertexAdded -= newGraph_VertexAdded;
                    oldGraph.VertexRemoved -= newGraph_VertexRemoved;

                    foreach (var vertex in oldGraph.Vertices)
                    {
                        this.Vertices.Remove(vertex);
                    }
                }
            }
        }

        private void newGraph_VertexAdded(BnVertex vertex)
        {
            this.Vertices.Add(vertex);
        }

        private void newGraph_VertexRemoved(BnVertex vertex)
        {
            this.Vertices.Remove(vertex);
        }
    }
}