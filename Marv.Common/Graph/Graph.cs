﻿using QuickGraph;
using QuickGraph.Algorithms.RankedShortestPath;
using Smile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Windows.Diagrams.Core;

namespace Marv.Common
{
    public class Graph : BidirectionalGraph<Vertex, Edge>, IGraphSource, INotifyPropertyChanged
    {
        private Dictionary<string, string, double> _value = new Dictionary<string, string, double>();
        private string associatedGroup;
        private string defaultGroup = "all";
        private ObservableCollection<string> groups = new ObservableCollection<string>();
        private Dictionary<string, string> loops = new Dictionary<string, string>();
        private string name;
        private Network network = new Network();

        public Graph()
            : base()
        {
        }

        public Graph(bool allowParallelEdges)
            : base(allowParallelEdges)
        {
        }

        public Graph(bool allowParallelEdges, int vertexCapacity)
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

        public Dictionary<string, string, double> Value
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

        public static Graph Read<TVertex>(string fileName) where TVertex : Vertex, new()
        {
            var graph = new Graph();

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

        public static Task<Graph> ReadAsync<TVertex>(string fileName) where TVertex : Vertex, new()
        {
            return Task.Run(() => Graph.Read<TVertex>(fileName));
        }

        public void AddEdge(string srcVertexKey, string dstVertexKey)
        {
            if (srcVertexKey.Equals(dstVertexKey)) return;

            Vertex srcVertex = this.GetVertex(srcVertexKey);
            Vertex dstVertex = this.GetVertex(dstVertexKey);

            if (srcVertex != null && dstVertex != null)
            {
                this.AddEdge(new Edge(srcVertex, dstVertex));
            }
        }

        public Dictionary<string, string, double> ClearEvidence()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.IsEvidenceEntered = false;
            }

            this.network.ClearAllEvidence();
            return this.GetNetworkValue();
        }

        public Dictionary<string, string, double> ClearEvidence(string vertexKey)
        {
            this.network.ClearEvidence(vertexKey);
            return this.GetNetworkValue();
        }

        public double GetMean(string vertexKey, Dictionary<string, double> vertexValue)
        {
            return this.GetVertex(vertexKey).GetMean(vertexValue);
        }

        public Dictionary<string, string, double> GetNetworkValue()
        {
            this.UpdateBeliefs();

            var graphValue = new Dictionary<string, string, double>();

            foreach (var vertex in this.Vertices)
            {
                var vertexValue = new Dictionary<string, double>();

                foreach (var state in vertex.States)
                {
                    try
                    {
                        vertexValue[state.Key] = this.network.GetNodeValue(vertex.Key)[vertex.GetStateIndex(state.Key)];
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

        public double GetStandardDeviation(string vertexKey, Dictionary<string, double> vertexValue)
        {
            return this.GetVertex(vertexKey).GetStandardDeviation(vertexValue);
        }

        public Graph GetSubGraph(string group)
        {
            // Extract the header vertices
            Graph subGraph = new Graph
            {
                DefaultGroup = group
            };

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
                    var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<Vertex, Edge>(this, (edge) => { return 1; });
                    algorithm.Compute(srcVertex, dstVertex);

                    foreach (var path in algorithm.ComputedShortestPaths)
                    {
                        Vertex src = path.First().Source;

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

        public Vertex GetVertex(string key)
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

        public Dictionary<string, string, double> ReadValueCsv(string fileName)
        {
            var graphValue = new Dictionary<string, string, double>();

            foreach (var line in File.ReadLines(fileName))
            {
                var vertexValue = new Dictionary<string, double>();

                string[] parts = line.Split(new char[] { ',' });

                var vertexKey = parts[0];

                var vertex = this.GetVertex(vertexKey);

                // Ignore if the vertex key does not exist in the given graph
                if (vertex == null)
                {
                    continue;
                }

                int nParts = parts.Count();

                for (int p = 1; p < nParts; p++)
                {
                    // This assumes that states order is preserved
                    var stateKey = vertex.States[p - 1].Key;

                    vertexValue[stateKey] = Double.Parse(parts[p]);
                }

                graphValue[vertexKey] = vertexValue;
            }

            return graphValue;
        }

        public Dictionary<string, string, double> Run(string vertexKey, IEvidence evidence)
        {
            evidence.Set(this, vertexKey);
            return this.GetNetworkValue();
        }

        public Dictionary<string, string, double> Run(Dictionary<string, IEvidence> graphEvidence)
        {
            foreach (var vertexKey in graphEvidence.Keys)
            {
                graphEvidence[vertexKey].Set(this, vertexKey);
            }

            return this.GetNetworkValue();
        }

        public Dictionary<int, string, string, double> Run(Dictionary<string, IEvidence> graphEvidence, int startYear, int endYear)
        {
            var graphValueTimeSeries = new Dictionary<int, string, string, double>();

            for (int year = startYear; year <= endYear; year++)
            {
                // If this is not the start year,
                // then add the looped evidences
                if (year > startYear)
                {
                    foreach (var srcVertexKey in this.Loops.Keys)
                    {
                        var dstVertexKey = this.Loops[srcVertexKey];

                        var lastGraphValue = graphValueTimeSeries[year - 1];

                        var lastVertexValue = lastGraphValue[srcVertexKey];

                        graphEvidence[dstVertexKey] = new SoftEvidence
                        {
                            Evidence = lastVertexValue.Values.ToArray()
                        };
                    }
                }

                graphValueTimeSeries[year] = this.Run(graphEvidence);
            }

            return graphValueTimeSeries;
        }

        public void SetEvidence(string vertexKey, int stateIndex)
        {
            this.network.SetEvidence(vertexKey, stateIndex);
        }

        public void SetEvidence(string vertexKey, double[] evidence)
        {
            this.network.SetSoftEvidence(vertexKey, evidence);
        }

        public void SetValueToZero()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.SetValueToZero();
            }
        }

        public void UpdateBeliefs()
        {
            this.network.UpdateBeliefs();
        }

        public Dictionary<string, string, double> UpdateValue()
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
    }
}