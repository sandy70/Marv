using QuickGraph;
using Smile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Windows.Diagrams.Core;

namespace LibBn
{
    [Serializable]
    public class BnGraph : BidirectionalGraph<BnVertex, BnEdge>, IGraphSource, INotifyPropertyChanged
    {
        public Network Network = new Network();
        private ObservableCollection<string> groups = new ObservableCollection<string>();

        public BnGraph()
        {
        }

        public BnGraph(bool allowParallelEdges)
            : base(allowParallelEdges)
        {
        }

        public BnGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Groups
        {
            get
            {
                return this.groups;
            }

            set
            {
                if (value != this.groups)
                {
                    this.groups = value;
                    this.OnPropertyChanged("Groups");
                }
            }
        }

        IEnumerable IGraphSource.Items
        {
            get { return this.Vertices; }
        }

        IEnumerable<ILink> IGraphSource.Links
        {
            get { return this.Edges; }
        }

        public void AddEdge(string key1, string key2)
        {
            if (key1.Equals(key2)) return;

            BnVertex v1 = this.Vertices.GetVertex(key1);
            BnVertex v2 = this.Vertices.GetVertex(key2);

            if (v1 != null && v2 != null)
            {
                this.AddEdge(new BnEdge(v1, v2));
            }
        }

        public void AddVertices<TVertex>(ObservableCollection<TVertex> vertices)
            where TVertex : BnVertex
        {
            foreach (var vertex in vertices)
            {
                this.AddVertex(vertex);

                foreach (var group in vertex.Groups)
                {
                    if (!this.Groups.Contains(group))
                    {
                        this.Groups.Add(group);
                    }
                }
            }
        }

        public void CopyFrom(IEnumerable<BnVertexValue> vertexValues)
        {
            foreach (var srcVertexValue in vertexValues)
            {
                BnVertex dstVertex = this.GetVertexByKey(srcVertexValue.Key);

                if (dstVertex != null)
                {
                    dstVertex.CopyFrom(srcVertexValue);
                }
            }
        }

        public BnVertex GetVertexByKey(string key)
        {
            foreach (var vertex in this.Vertices)
            {
                if (vertex.Key.Equals(key))
                {
                    return vertex;
                }
            }
            return null;
        }

        public bool HasEdge(string srcKey, string dstKey)
        {
            bool hasEdge = false;

            foreach (var edge in this.Edges)
            {
                if (edge.Source.Key.Equals(srcKey) && edge.Target.Key.Equals(dstKey))
                {
                    hasEdge = true;
                }
            }

            return hasEdge;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}