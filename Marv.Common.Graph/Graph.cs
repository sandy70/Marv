using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using QuickGraph.Algorithms.RankedShortestPath;
using Smile;

namespace Marv.Common.Graph
{
    public partial class Graph : Model
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private string defaultGroup;
        private Graph displayGraph;
        private ModelCollection<Edge> edges = new ModelCollection<Edge>();
        private Guid guid;
        private bool isDefaultGroupVisible;
        private bool isExpanded = true;
        private Dictionary<string, string> loops = new Dictionary<string, string>();
        private NetworkStructure networkStructure;
        private Vertex selectedVertex;
        private ModelCollection<Vertex> vertices = new ModelCollection<Vertex>();

        public Dict<string, VertexData> Data
        {
            set
            {
                foreach (var vertexKey in value.Keys)
                {
                    this.Vertices[vertexKey].Data = value[vertexKey];
                }

                this.RaisePropertyChanged();
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

        public ModelCollection<Edge> Edges
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
                    this.RaisePropertyChanged();
                }
            }
        }

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

        public ModelCollection<Vertex> Vertices
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
            //NetworkStructure.Decrypt(fileName);
            var graph = new Graph
            {
                networkStructure = NetworkStructure.Read(fileName)
            };

            graph.DefaultGroup = graph.networkStructure.ParseUserProperty("defaultgroup", "all");
            graph.Guid = Guid.Parse(graph.networkStructure.ParseUserProperty("guid", Guid.NewGuid().ToString()));
            graph.Name = graph.networkStructure.ParseUserProperty("key", "");

            // Add all the vertices
            foreach (var structureVertex in graph.networkStructure.Vertices)
            {
                var vertex = new Vertex
                {
                    ConnectorPositions = structureVertex.ParseJson<Dictionary<string, string, EdgeConnectorPositions>>("ConnectorPositions"),
                    Description = structureVertex.ParseStringProperty("HR_HTML_Desc"),
                    Groups = structureVertex.ParseGroups(),
                    HeaderOfGroup = structureVertex.ParseStringProperty("headerofgroup"),
                    InputVertexKey = structureVertex.ParseStringProperty("InputNode"),
                    IsExpanded = structureVertex.ParseIsExpanded(),
                    Key = structureVertex.Key,
                    Name = structureVertex.ParseStringProperty("label"),
                    Position = structureVertex.ParsePosition(),
                    PositionForGroup = structureVertex.ParseJson<Dictionary<string, Point>>("PositionForGroup"),
                    Units = structureVertex.ParseStringProperty("units"),
                    States = structureVertex.ParseStates(),
                    Type = structureVertex.ParseSubType()
                };

                vertex.IsHeader = !string.IsNullOrWhiteSpace(vertex.HeaderOfGroup);

                if (string.IsNullOrWhiteSpace(vertex.Units))
                {
                    vertex.Units = "n/a";
                }

                if (!string.IsNullOrWhiteSpace(vertex.InputVertexKey))
                {
                    graph.Loops[vertex.InputVertexKey] = vertex.Key;
                }

                graph.Vertices.Add(vertex);
            }

            // Add all the edges
            foreach (var srcVertex in graph.networkStructure.Vertices)
            {
                foreach (var dstVertex in srcVertex.Children)
                {
                    graph.Edges.Add(new Edge(graph.Vertices[srcVertex.Key], graph.Vertices[dstVertex.Key]));
                }
            }

            graph.UpdateDisplayGraph(graph.DefaultGroup);
            graph.Run();

            return graph;
        }

        public static Task<Graph> ReadAsync(string fileName)
        {
            return Task.Run(() => Graph.Read(fileName));
        }

        public Dictionary<string, string, double> GetSensitivity(string targetVertexKey, Func<Vertex, double[], double[], double> statisticFunc, Dictionary<string, VertexData> graphEvidence = null)
        {
            var targetVertex = this.Vertices[targetVertexKey];

            // Dictionary<sourceVertexKey, sourceStateKey, targetValue>
            var value = new Dictionary<string, string, double>();

            // Clear all evidence to begin with
            this.networkStructure.ClearEvidence();

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
                        this.networkStructure.SetEvidence(sourceVertex.Key, stateIndex);

                        var graphValue = this.networkStructure.GetBelief();
                        var targetVertexValue = graphValue[targetVertex.Key];

                        value[sourceVertex.Key, sourceState.Key] = statisticFunc(targetVertex, targetVertexValue, targetVertex.InitialBelief.Select(kvp => kvp.Value).ToArray());

                        sourceVertex.States.ClearEvidence();
                        sourceVertex.EvidenceString = null;
                    }
                    catch (SmileException exception)
                    {
                        Logger.Error(exception.Message);

                        value[sourceVertex.Key, sourceState.Key] = double.NaN;
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
                                var connectorPostions = srcVertex.ConnectorPositions.GetValueOrNew(group, dstVertex.Key);

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

        public Dictionary<string, string, double> ReadValueCsv(string fileName)
        {
            var graphValue = new Dictionary<string, string, double>();

            foreach (var line in File.ReadLines(fileName))
            {
                var vertexValue = new Dictionary<string, double>();

                var parts = line.Split(new[] {','});

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
            this.networkStructure.ClearEvidence();

            foreach (var vertex in this.Vertices)
            {
                if (vertex.IsEvidenceEntered)
                {
                    this.networkStructure.SetEvidence(vertex.Key, vertex.States.GetEvidence());
                }
            }

            this.UpdateBelief();
        }

        public void SetEvidence(Dict<string, VertexData> vertexEvidences)
        {
            this.Vertices.ClearEvidence();

            foreach (var vertexKey in vertexEvidences.Keys)
            {
                var vertex = this.Vertices[vertexKey];
                var vertexEvidence = vertexEvidences[vertexKey];

                vertex.EvidenceString = vertexEvidence == null ? null : vertexEvidences[vertexKey].String;
                vertex.UpdateStateEvidences();
            }
        }

        public void UpdateBelief()
        {
            this.networkStructure.Run();

            var belief = this.networkStructure.GetBelief();

            foreach (var vertexKey in belief.Keys)
            {
                this.Vertices[vertexKey].States.SetBelief(belief[vertexKey]);
            }
        }

        public void UpdateDisplayGraph(string group)
        {
            this.DisplayGraph = this.GetSubGraph(group);
            this.IsDefaultGroupVisible = group == this.DefaultGroup;
        }

        public void Write()
        {
            this.networkStructure.Write(this);
        }

        public void Run(SectionEvidence sectionEvidence)
        {
            foreach (var yearEvidence in sectionEvidence.YearEvidences)
            {
                this.networkStructure.Run(yearEvidence.VertexEvidences);
            }
        }
    }
}