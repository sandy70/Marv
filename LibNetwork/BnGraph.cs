using QuickGraph;
using QuickGraph.Algorithms.RankedShortestPath;
using Smile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace LibNetwork
{
    public class BnGraph : BidirectionalGraph<BnVertex, BnEdge>, IGraphSource, INotifyPropertyChanged
    {
        private BnGraphValue _value;
        private string associatedGroup;
        private string defaultGroup = "all";
        private string fileName;
        private ObservableCollection<string> groups = new ObservableCollection<string>();
        private Dictionary<string, string> loops = new Dictionary<string, string>();
        private string name;
        private Network network = new Network();

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

        public string FileName
        {
            get
            {
                return this.fileName;
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

        public Dictionary<string, string> Loops
        {
            get
            {
                return this.loops;
            }

            set
            {
                if (value != this.loops)
                {
                    this.loops = value;
                    this.OnPropertyChanged("Loops");
                }
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value != this.name)
                {
                    this.name = value;

                    this.OnPropertyChanged("Name");
                }
            }
        }

        public IEnumerable<BnVertex> NumericVertices
        {
            get
            {
                var numericVertices = new List<BnVertex>();

                foreach (var vertex in this.Vertices)
                {
                    if (vertex.Type == VertexType.Interval || vertex.Type == VertexType.Number)
                    {
                        numericVertices.Add(vertex);
                    }
                }

                return numericVertices.AsEnumerable();
            }
        }

        public BnGraphValue Value
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

                    foreach (var vertex in this.Vertices)
                    {
                        vertex.Value = this.Value[vertex.Key];
                    }
                }
            }
        }

        public static BnGraph Read<TVertex>(string fileName) where TVertex : BnVertex, new()
        {
            var graph = new BnGraph();
            graph.fileName = fileName;

            try
            {
                graph.network.ReadFile(fileName);
            }
            catch (SmileException exp)
            {
                return graph;
            }

            graph.network.UpdateBeliefs();

            var structure = NetworkStructure.Read(fileName);
            graph.DefaultGroup = structure.ParseDefaultGroup();
            graph.Name = structure.ParseName();

            // Add all the vertices
            foreach (var structureVertex in structure.Vertices)
            {
                var vertex = new TVertex();

                vertex.Key = structureVertex.Key;
                vertex.Description = structureVertex.ParseStringProperty("HR_HTML_Desc");
                vertex.Groups = structureVertex.ParseGroups();
                vertex.HeaderOfGroup = structureVertex.ParseStringProperty("headerofgroup");
                vertex.InputVertexKey = structureVertex.ParseStringProperty("inputvertexkey");
                vertex.IsExpanded = structureVertex.ParseIsExpanded();
                vertex.IsHeader = !string.IsNullOrWhiteSpace(vertex.HeaderOfGroup);
                vertex.Name = structureVertex.ParseStringProperty("label");
                vertex.Parent = graph;
                vertex.Position = structureVertex.ParsePosition();
                vertex.PositionForGroup = structureVertex.ParsePositionByGroup();
                vertex.Units = structureVertex.ParseStringProperty("units");
                vertex.States = structureVertex.ParseStates();
                vertex.Type = structureVertex.ParseSubType();

                if (string.IsNullOrWhiteSpace(vertex.Units))
                {
                    vertex.Units = "n/a";
                }

                graph.AddVertex(vertex);

                if (!String.IsNullOrWhiteSpace(vertex.InputVertexKey))
                {
                    graph.Loops[vertex.InputVertexKey] = vertex.Key;
                }
            }

            // Add all the edges
            foreach (var srcNode in structure.Vertices)
            {
                foreach (var dstNode in srcNode.Children)
                {
                    graph.AddEdge(srcNode.Key, dstNode.Key);
                }
            }

            graph.UpdateValue();
            return graph;
        }

        public static Task<BnGraph> ReadAsync<TVertex>(string fileName) where TVertex : BnVertex, new()
        {
            return Task.Run(() => BnGraph.Read<TVertex>(fileName));
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

        public BnGraphValue ClearEvidence()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.IsEvidenceEntered = false;
            }

            this.network.ClearAllEvidence();
            return this.GetNetworkValue();
        }

        public BnGraphValue ClearEvidence(string vertexKey)
        {
            this.network.ClearEvidence(vertexKey);
            return this.GetNetworkValue();
        }

        public BnGraphValue GetNetworkValue()
        {
            this.UpdateBeliefs();

            var graphValue = new BnGraphValue();

            foreach (var vertex in this.Vertices)
            {
                var vertexValue = new BnVertexValue();

                foreach (var state in vertex.States)
                {
                    try
                    {
                        vertexValue[state.Key] = this.network.GetNodeValue(vertex.Key)[vertex.GetStateIndex(state.Key)];
                        vertexValue.IsEvidenceEntered = this.network.IsEvidence(vertex.Key);
                    }
                    catch (SmileException smileException)
                    {
                        Console.WriteLine(smileException.Message);
                        vertexValue[state.Key] = 0;
                    }
                }

                graphValue[vertex.Key] = vertexValue;
            }

            return graphValue;
        }

        public int GetNodeHandle(string vertexKey)
        {
            return this.network.GetNode(vertexKey);
        }

        public BnGraph GetSubGraph(string group)
        {
            // Extract the header vertices
            BnGraph subGraph = new BnGraph();
            subGraph.AssociatedGroup = group;

            foreach (var vertex in this.Vertices)
            {
                if (vertex.Groups.Contains(group))
                {
                    subGraph.AddVertex(vertex);

                    if (!vertex.PositionForGroup.ContainsKey(group))
                    {
                        vertex.PositionForGroup[group] = vertex.Position;
                    }

                    vertex.SelectedGroup = group;
                    vertex.DisplayPosition = vertex.PositionForGroup[group];
                }
            }

            // Process for each pair
            foreach (var srcVertex in subGraph.Vertices)
            {
                foreach (var dstVertex in subGraph.Vertices)
                {
                    var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<BnVertex, BnEdge>(this, (edge) => { return 1; });
                    algorithm.Compute(srcVertex, dstVertex);

                    foreach (var path in algorithm.ComputedShortestPaths)
                    {
                        BnVertex src = path.First().Source;

                        foreach (var edge in path)
                        {
                            if (subGraph.Vertices.Contains(edge.Target))
                            {
                                if (!subGraph.HasEdge(src.Key, edge.Target.Key))
                                {
                                    subGraph.AddEdge(src.Key, edge.Target.Key);
                                }

                                src = edge.Target;
                            }
                        }
                    }
                }
            }

            return subGraph;
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

        public BnGraphValue Run(string vertexKey, IEvidence evidence)
        {
            evidence.Set(this, vertexKey);
            return this.GetNetworkValue();
        }

        public BnGraphValue Run(Dictionary<string, IEvidence> graphEvidence)
        {
            foreach (var vertexKey in graphEvidence.Keys)
            {
                graphEvidence[vertexKey].Set(this, vertexKey);
            }

            return this.GetNetworkValue();
        }

        public ModelValue Run(Dictionary<string, IEvidence> graphEvidence, int startYear, int endYear)
        {
            var modelValue = new ModelValue();

            for (int year = startYear; year <= endYear; year++)
            {
                // If this is not the start year,
                // then add the looped evidences
                if (year > startYear)
                {
                    foreach (var srcVertexKey in this.Loops.Keys)
                    {
                        var dstVertexKey = this.Loops[srcVertexKey];

                        var lastGraphValue = modelValue[year - 1];

                        var lastVertexValue = lastGraphValue[srcVertexKey];

                        graphEvidence[dstVertexKey] = new SoftEvidence
                        {
                            Evidence = lastVertexValue.Values.ToArray()
                        };
                    }
                }

                modelValue[year] = this.Run(graphEvidence);
            }

            return modelValue;
        }

        public void SetEvidence(string vertexKey, int stateIndex)
        {
            this.network.SetEvidence(vertexKey, stateIndex);
        }

        public void SetEvidence(string vertexKey, double[] evidence)
        {
            this.network.SetSoftEvidence(vertexKey, evidence);
        }

        public void UpdateBeliefs()
        {
            try
            {
                this.network.UpdateBeliefs();
            }
            catch (SmileException exception)
            {
                throw new InconsistentEvidenceException();
            }
        }

        public void UpdateDisplayPositions()
        {
            foreach (var vertex in this.Vertices)
            {
                if (vertex.PositionForGroup.ContainsKey(this.AssociatedGroup))
                {
                    vertex.DisplayPosition = vertex.PositionForGroup[this.AssociatedGroup];
                }
                else
                {
                    vertex.DisplayPosition = vertex.Position;
                }
            }
        }

        public BnGraphValue UpdateValue()
        {
            return this.Value = this.GetNetworkValue();
        }

        public void Write(string fileName)
        {
            var structure = NetworkStructure.Read(fileName);

            foreach (var node in structure.Vertices)
            {
                node.Properties["groups"] = "\"" + this.GetVertex(node.Key).Groups.String() + "\"";
                node.Properties["grouppositions"] = "\"" + this.GetVertex(node.Key).PositionForGroup.String() + "\"";
                node.Properties["isexpanded"] = "\"" + this.GetVertex(node.Key).IsExpanded + "\"";
                node.Properties["label"] = "\"" + this.GetVertex(node.Key).Name + "\"";
                node.Properties["units"] = "\"" + this.GetVertex(node.Key).Units + "\"";
            }

            structure.Write(fileName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetValueToZero()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.SetValueToZero();
            }
        }
    }
}