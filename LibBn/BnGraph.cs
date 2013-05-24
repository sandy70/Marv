using QuickGraph;
using QuickGraph.Algorithms.RankedShortestPath;
using Smile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace LibBn
{
    [Serializable]
    public class BnGraph : BidirectionalGraph<BnVertex, BnEdge>, IGraphSource, INotifyPropertyChanged
    {
        public Network Network = new Network();

        private Dictionary<string, Dictionary<string, double>> _value;
        private string associatedGroup;
        private string defaultGroup = "all";
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

        public string AssociatedGroup
        {
            get
            {
                return this.associatedGroup;
            }

            set
            {
                if (value != this.associatedGroup)
                {
                    this.associatedGroup = value;

                    this.OnPropertyChanged("AssociatedGroup");
                }
            }
        }

        public string DefaultGroup
        {
            get
            {
                return this.defaultGroup;
            }

            set
            {
                if (value != this.defaultGroup)
                {
                    this.defaultGroup = value;
                    this.OnPropertyChanged("DefaultGroup");
                }
            }
        }

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

        public Dictionary<string, Dictionary<string, double>> Value
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

                    foreach (var vertexKey in this.Value.Keys)
                    {
                        this.GetVertex(vertexKey).Value = this.Value[vertexKey];
                    }
                }
            }
        }

        public static BnGraph Read<TVertex>(string fileName) where TVertex : BnVertex, new()
        {
            var graph = new BnGraph();
            var structure = NetworkStructure.Read(fileName);

            graph.Network.ReadFile(fileName);
            graph.Network.UpdateBeliefs();

            graph.DefaultGroup = structure.ParseDefaultGroup();

            // Add all the vertices
            foreach (var structureVertex in structure.Vertices)
            {
                var vertex = new TVertex();

                vertex.Key = structureVertex.Key;
                vertex.Description = structureVertex.ParseStringProperty("HR_HTML_Desc");
                vertex.Groups = structureVertex.ParseGroups();
                vertex.HeaderOfGroup = structureVertex.ParseStringProperty("headerofgroup");
                vertex.Name = structureVertex.ParseStringProperty("label");
                vertex.Network = graph.Network;
                vertex.Parent = graph;
                vertex.Position = structureVertex.ParsePosition();
                vertex.PositionsByGroup = structureVertex.ParsePositionByGroup();
                vertex.Units = structureVertex.ParseStringProperty("units");
                vertex.States = graph.Network.ParseStates(structureVertex.Key);

                graph.AddVertex(vertex);
            }

            // Add all the edges
            foreach (var srcNode in structure.Vertices)
            {
                foreach (var dstNode in srcNode.Children)
                {
                    graph.AddEdge(srcNode.Key, dstNode.Key);
                }
            }

            return graph;
        }

        public void Add(BnGraph graph)
        {
            foreach (var vertex in graph.Vertices)
            {
                this.AddVertex(vertex);
            }

            foreach (var edge in graph.Edges)
            {
                this.AddEdge(edge);
            }
        }

        public void AddEdge(string key1, string key2)
        {
            if (key1.Equals(key2)) return;

            BnVertex v1 = this.GetVertex(key1);
            BnVertex v2 = this.GetVertex(key2);

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
                BnVertex dstVertex = this.GetVertex(srcVertexValue.Key);

                if (dstVertex != null)
                {
                    dstVertex.CopyFrom(srcVertexValue);
                }
            }
        }

        public BnGraph GetGroup(string group)
        {
            // Extract the header vertices
            BnGraph partGraph = new BnGraph();
            partGraph.AssociatedGroup = group;

            foreach (var vertex in this.Vertices)
            {
                if (vertex.Groups.Contains(group))
                {
                    partGraph.AddVertex(vertex);

                    Point positionByGroup;

                    if (vertex.PositionsByGroup.TryGetValue(group, out positionByGroup))
                    {
                        vertex.DisplayPosition = positionByGroup;
                    }
                    else
                    {
                        vertex.PositionsByGroup.Add(group, vertex.Position);
                        vertex.DisplayPosition = vertex.Position;
                    }
                }
            }

            // Process for each pair
            foreach (var srcVertex in partGraph.Vertices)
            {
                foreach (var dstVertex in partGraph.Vertices)
                {
                    var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<BnVertex, BnEdge>(this, (edge) => { return 1; });
                    algorithm.Compute(srcVertex, dstVertex);

                    foreach (var path in algorithm.ComputedShortestPaths)
                    {
                        BnVertex src = path.First().Source;

                        foreach (var edge in path)
                        {
                            if (partGraph.Vertices.Contains(edge.Target))
                            {
                                if (!partGraph.HasEdge(src.Key, edge.Target.Key))
                                {
                                    partGraph.AddEdge(src.Key, edge.Target.Key);
                                }

                                src = edge.Target;
                            }
                        }
                    }
                }
            }

            return partGraph;
        }

        public Dictionary<string, Dictionary<string, double>> GetNetworkValue()
        {
            var graphValue = new Dictionary<string, Dictionary<string, double>>();

            foreach (var vertex in this.Vertices)
            {
                var vertexValue = new Dictionary<string, double>();

                foreach (var state in vertex.States)
                {
                    vertexValue[state.Key] = vertex.GetStateValue(state.Key);
                }

                graphValue[vertex.Key] = vertexValue;
            }

            return graphValue;
        }

        public BnVertex GetVertex(string key)
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

        public void SetEvidence(Dictionary<string, VertexEvidence> graphEvidence)
        {
            foreach (var vertexKey in graphEvidence.Keys)
            {
                var vertex = this.GetVertex(vertexKey);

                if (vertex != null)
                {
                    vertex.SetEvidence(graphEvidence[vertexKey]);
                }
            }
        }

        public void UpdateBeliefs()
        {
            this.Network.UpdateBeliefs();
        }

        public void UpdateDisplayPositions()
        {
            foreach (var vertex in this.Vertices)
            {
                if (vertex.PositionsByGroup.ContainsKey(this.AssociatedGroup))
                {
                    vertex.DisplayPosition = vertex.PositionsByGroup[this.AssociatedGroup];
                }
                else
                {
                    vertex.DisplayPosition = vertex.Position;
                }
            }
        }

        public void UpdateMostProbableStates()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.UpdateMostProbableState();
            }
        }

        public Dictionary<string, Dictionary<string, double>> UpdateValue()
        {
            return this.Value = this.GetNetworkValue();
        }

        public void Write(string fileName)
        {
            var structure = NetworkStructure.Read(fileName);

            foreach (var node in structure.Vertices)
            {
                node.Properties["group"] = "\"" + this.GetVertex(node.Key).Groups.String() + "\"";
                node.Properties["grouppositions"] = "\"" + this.GetVertex(node.Key).PositionsByGroup.String() + "\"";
            }

            structure.Write(fileName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}