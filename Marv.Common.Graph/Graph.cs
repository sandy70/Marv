using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using NLog;
using QuickGraph.Algorithms.RankedShortestPath;
using Smile;

namespace Marv
{
    public partial class Graph : NotifyPropertyChanged
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string defaultGroup;
        private Graph displayGraph;
        private ObservableCollection<Edge> edges = new ObservableCollection<Edge>();
        private Guid guid;
        private bool isDefaultGroupVisible;
        private bool isExpanded = true;
        private string key;
        private Vertex selectedVertex;
        private KeyedCollection<Vertex> vertices = new KeyedCollection<Vertex>();

        public Dict<string, VertexData> Data
        {
            get
            {
                var graphData = new Dict<string, VertexData>();

                foreach (var vertex in this.Vertices)
                {
                    graphData[vertex.Key] = vertex.Data;
                }

                return graphData;
            }

            set
            {
                foreach (var vertexKey in value.Keys)
                {
                    this.Vertices[vertexKey].Data = value[vertexKey];
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
                    this.RaisePropertyChanged();
                }
            }
        }

        public Graph DisplayGraph
        {
            get
            {
                return this.displayGraph;
            }

            private set
            {
                if (value != this.displayGraph)
                {
                    this.displayGraph = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Edge> Edges
        {
            get
            {
                return this.edges;
            }

            set
            {
                if (value != this.edges)
                {
                    this.edges = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Guid Guid
        {
            get
            {
                return this.guid;
            }

            set
            {
                if (value != this.guid)
                {
                    this.guid = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsDefaultGroupVisible
        {
            get
            {
                return this.isDefaultGroupVisible;
            }

            set
            {
                if (value != this.isDefaultGroupVisible)
                {
                    this.isDefaultGroupVisible = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }

            set
            {
                this.isExpanded = value;

                foreach (var vertex in this.Vertices)
                {
                    vertex.IsExpanded = value;
                }
            }
        }

        public bool IsMostlyExpanded
        {
            get
            {
                return this.Vertices.Count(vertex => vertex.IsExpanded) > this.Vertices.Count / 2;
            }
        }

        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                if (value.Equals(this.key))
                {
                    return;
                }

                this.key = value;
                this.RaisePropertyChanged();
            }
        }

        public Network Network { get; private set; }

        public Vertex SelectedVertex
        {
            get
            {
                return this.selectedVertex;
            }

            set
            {
                if (value != this.selectedVertex)
                {
                    this.selectedVertex = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public KeyedCollection<Vertex> Vertices
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
                    this.RaisePropertyChanged();
                }
            }
        }

        public static Graph Read(string fileName)
        {
            //Network.Decrypt(fileName);
            var graph = new Graph
            {
                Network = Network.Read(fileName)
            };

            graph.DefaultGroup = graph.Network.ParseUserProperty("defaultgroup", "all");
            graph.Guid = Guid.Parse(graph.Network.ParseUserProperty("guid", Guid.NewGuid().ToString()));
            graph.Key = graph.Network.ParseUserProperty("key", "");

            // Add all the vertices
            foreach (var networkVertex in graph.Network.Vertices)
            {
                var vertex = new Vertex
                {
                    ConnectorPositions = networkVertex.ParseJson<Dict<string, string, EdgeConnectorPositions>>("ConnectorPositions"),
                    Description = networkVertex.ParseStringProperty("HR_HTML_Desc"),
                    Groups = networkVertex.ParseGroups(),
                    HeaderOfGroup = networkVertex.ParseStringProperty("headerofgroup"),
                    InputVertexKey = networkVertex.ParseStringProperty("InputNode"),
                    IsExpanded = networkVertex.ParseIsExpanded(),
                    Key = networkVertex.Key,
                    Name = networkVertex.ParseStringProperty("label"),
                    Position = networkVertex.ParsePosition(),
                    PositionForGroup = networkVertex.ParseJson<Dictionary<string, Point>>("PositionForGroup"),
                    Units = networkVertex.ParseStringProperty("units"),
                    States = networkVertex.ParseStates(),
                    Type = networkVertex.ParseSubType()
                };

                vertex.IsHeader = !string.IsNullOrWhiteSpace(vertex.HeaderOfGroup);

                if (string.IsNullOrWhiteSpace(vertex.Units))
                {
                    vertex.Units = "n/a";
                }

                graph.Vertices.Add(vertex);
            }

            // Add all the edges
            foreach (var srcVertex in graph.Network.Vertices)
            {
                foreach (var dstVertex in srcVertex.Children)
                {
                    graph.Edges.Add(new Edge(graph.Vertices[srcVertex.Key], graph.Vertices[dstVertex.Key]));
                }
            }

            graph.UpdateDisplayGraph(graph.DefaultGroup);

            return graph;
        }

        public void ClearEvidence()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.ClearEvidence();
            }
        }

        public Dict<string, string, double> GetSensitivity(string targetVertexKey, Func<Vertex, double[], double[], double> statisticFunc, Dictionary<string, VertexData> graphEvidence = null)
        {
            var targetVertex = this.Vertices[targetVertexKey];

            // Dictionary<sourceVertexKey, sourceStateKey, targetValue>
            var value = new Dict<string, string, double>();

            // Clear all evidence to begin with
            this.Network.ClearEvidence();

            // Collect vertices to ignore
            var verticesToIgnore = new List<Vertex>
            {
                targetVertex
            };

            if (graphEvidence != null)
            {
                // Set the given evidence
                //this.Evidence = graphEvidence;

                // Collect more vertices to ignore
                verticesToIgnore.Add(this.Vertices[graphEvidence.Keys]);
            }

            foreach (var sourceVertex in this.Vertices.Except(verticesToIgnore))
            {
                foreach (var sourceState in sourceVertex.States)
                {
                    try
                    {
                        var stateIndex = sourceVertex.States.IndexOf(sourceState);
                        this.Network.SetEvidence(sourceVertex.Key, stateIndex);

                        var graphValue = this.Network.GetBeliefs();
                        var targetVertexValue = graphValue[targetVertex.Key];

                        value[sourceVertex.Key][sourceState.Key] = statisticFunc(targetVertex, targetVertexValue, targetVertex.InitialBelief.Select(kvp => kvp.Value).ToArray());

                        sourceVertex.States.ClearEvidence();
                        sourceVertex.EvidenceString = null;
                    }
                    catch (SmileException exception)
                    {
                        Logger.Error(exception.Message);

                        value[sourceVertex.Key][sourceState.Key] = double.NaN;
                    }
                }
            }

            return value;
        }

        public Graph GetSubGraph(string group)
        {
            // Extract the header vertices
            var subGraph = new Graph();

            // Add the vertices
            foreach (var vertex in this.Vertices)
            {
                if (vertex.Groups.Contains(group))
                {
                    subGraph.Vertices.Add(vertex);

                    if (!vertex.PositionForGroup.ContainsKey(group))
                    {
                        vertex.PositionForGroup[group] = vertex.Position;
                    }

                    if (group == this.DefaultGroup)
                    {
                        if (!vertex.Commands.Contains(VertexCommands.SubGraph))
                        {
                            vertex.Commands.Push(VertexCommands.SubGraph);
                        }
                    }
                    else
                    {
                        if (vertex.Commands.Contains(VertexCommands.SubGraph))
                        {
                            vertex.Commands.Remove(VertexCommands.SubGraph);
                        }
                    }

                    vertex.SelectedGroup = group;
                    vertex.DisplayPosition = vertex.PositionForGroup[group];
                }
            }

            // Process for each pair and add edges
            foreach (var srcVertex in subGraph.Vertices)
            {
                foreach (var dstVertex in subGraph.Vertices)
                {
                    var algorithm = new HoffmanPavleyRankedShortestPathAlgorithm<Vertex, Edge>(this, edge => 1);
                    algorithm.Compute(srcVertex, dstVertex);

                    foreach (var path in algorithm.ComputedShortestPaths)
                    {
                        // Convert to list to avoid multiple enumerations.
                        var pathList = path as IList<Edge> ?? path.ToList();
                        var src = pathList.First().Source;

                        foreach (var edge in pathList)
                        {
                            if (subGraph.Vertices.Contains(edge.Target))
                            {
                                var connectorPostions = srcVertex.ConnectorPositions[group][dstVertex.Key];

                                subGraph.Edges.AddUnique(src, edge.Target, connectorPostions);

                                src = edge.Target;
                            }
                        }
                    }
                }
            }

            subGraph.DefaultGroup = group;

            return subGraph;
        }

        public Dict<string, string, double> ReadValueCsv(string fileName)
        {
            var graphValue = new Dict<string, string, double>();

            foreach (var line in File.ReadLines(fileName))
            {
                var vertexValue = new Dict<string, double>();

                var parts = line.Split(new[]
                {
                    ','
                });

                var vertexKey = parts[0];

                var vertex = this.Vertices[vertexKey];

                // Ignore if the vertex key does not exist in the given graph
                if (vertex == null)
                {
                    continue;
                }

                var nParts = parts.Count();

                for (var p = 1; p < nParts; p++)
                {
                    // This assumes that states order is preserved
                    var stateKey = vertex.States[p - 1].Key;

                    vertexValue[stateKey] = Double.Parse(parts[p]);
                }

                graphValue[vertexKey] = vertexValue;
            }

            return graphValue;
        }

        public void Run()
        {
            var network = Network.Read(this.Network.FileName);
            var graphData = this.Data;

            network.Run(graphData);

            this.Data = graphData;
        }

        public void UpdateDisplayGraph(string group)
        {
            this.DisplayGraph = this.GetSubGraph(group);
            this.IsDefaultGroupVisible = group == this.DefaultGroup;
        }

        public void Write()
        {
            this.Network.Write(this);
        }
    }
}