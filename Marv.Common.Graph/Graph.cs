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
        private bool isDefaultGroupVisible;
        private bool isExpanded = true;
        private Dictionary<string, string> loops = new Dictionary<string, string>();
        private Network network = new Network();
        private ModelCollection<Vertex> vertices = new ModelCollection<Vertex>();

        public String FileName { get; set; }

        public Dictionary<string, string, double> Belief
        {
            get
            {
                return new Dictionary<string, string, double>(this.Vertices.ToDictionary(vertex => vertex.Key, vertex => vertex.Belief));
            }

            set
            {
                foreach (var vertex in this.Vertices)
                {
                    vertex.Belief = value == null ? null : value[vertex.Key];
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

        public Dictionary<string, string, double> Evidence
        {
            get
            {
                var graphEvidence = new Dictionary<string, string, double>();

                foreach (var vertex in this.Vertices.Where(vertex => vertex.IsEvidenceEntered))
                {
                    graphEvidence[vertex.Key] = vertex.Evidence;
                }

                return graphEvidence;
            }

            set
            {
                foreach (var vertex in this.Vertices)
                {
                    vertex.Evidence = value == null ? null : value[vertex.Key];
                }
            }
        }

        public Dictionary<string, string, double> InitialBelief
        {
            set
            {
                foreach (var vertex in this.Vertices)
                {
                    vertex.InitialBelief = value == null ? null : value[vertex.Key];
                }

                this.RaisePropertyChanged();
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
                return this.Vertices.Count(vertex => vertex.IsExpanded) > this.Vertices.Count/2;
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

        public void AddEdge(Vertex source, Vertex target)
        {
            this.Edges.Add(new Edge(source, target));
        }

        public Dictionary<string, string, double> ClearEvidence()
        {
            this.network.ClearAllEvidence();
            return this.GetNetworkBelief();
        }

        public Dictionary<string, string, double> ClearEvidence(string vertexKey)
        {
            this.network.ClearEvidence(vertexKey);
            return this.GetNetworkBelief();
        }

        public Dictionary<string, IVertexEvidence> GetEvidence()
        {
            var graphEvidence = new Dictionary<string, IVertexEvidence>();

            foreach (var vertex in this.Vertices.Where(vertex => vertex.IsEvidenceEntered))
            {
                if (!string.IsNullOrEmpty(vertex.EvidenceString))
                {
                    graphEvidence[vertex.Key] = new VertexEvidenceString(vertex.EvidenceString);
                }
                else
                {
                    graphEvidence[vertex.Key] = new VertexEvidence(vertex.Evidence);
                }
            }

            return graphEvidence;
        }

        public double GetMean(string vertexKey, Dictionary<string, double> vertexValue)
        {
            return this.Vertices[vertexKey].GetMean(vertexValue);
        }

        public Dictionary<string, string, double> GetNetworkBelief()
        {
            this.UpdateNetworkBeliefs();

            var graphValue = new Dictionary<string, string, double>();

            foreach (var vertex in this.Vertices)
            {
                var vertexValue = new Dictionary<string, double>();

                foreach (var state in vertex.States)
                {
                    try
                    {
                        vertexValue[state.Key] = this.network.GetNodeValue(vertex.Key)[vertex.States.IndexOf(state.Key)];
                    }
                    catch (SmileException exception)
                    {
                        Logger.Warn(exception.Message);
                        vertexValue[state.Key] = 0;
                    }
                }

                graphValue[vertex.Key] = vertexValue;
            }

            return graphValue;
        }

        public Dictionary<string, string, double> GetSensitivity(string targetVertexKey, IVertexValueComputer vertexValueComputer, Dictionary<string, string, double> graphEvidence = null)
        {
            var targetVertex = this.Vertices[targetVertexKey];

            // Dictionary<sourceVertexKey, sourceStateKey, targetValue>
            var value = new Dictionary<string, string, double>();

            // Clear all evidence to begin with
            this.ClearEvidence();

            // Collect vertices to ignore
            var verticesToIgnore = new List<Vertex>();
            verticesToIgnore.Add(targetVertex);

            if (graphEvidence != null)
            {
                // Set the given evidence
                this.Evidence = graphEvidence;

                // Collect more vertices to ignore
                verticesToIgnore.Add(this.Vertices[graphEvidence.Keys]);
            }

            foreach (var sourceVertex in this.Vertices.Except(verticesToIgnore))
            {
                foreach (var sourceState in sourceVertex.States)
                {
                    try
                    {
                        this.SetEvidence(sourceVertex.Key, sourceState.Key);

                        var graphValue = this.GetNetworkBelief();
                        var targetVertexValue = graphValue[targetVertex.Key];

                        value[sourceVertex.Key, sourceState.Key] = vertexValueComputer.Compute(targetVertex, targetVertexValue);

                        this.ClearEvidence(sourceVertex.Key);
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

        public double GetStandardDeviation(string vertexKey, Dictionary<string, double> vertexValue)
        {
            return this.Vertices[vertexKey].GetStandardDeviation(vertexValue);
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

        public double[,] GetTable(string vertexKey)
        {
            return this.network.GetNodeTable(vertexKey);
        }

        public Vertex GetVertex(string vertexKey)
        {
            // Do not remove! This is for Marv.Matlab
            return this.Vertices[vertexKey];
        }

        public bool HasEdge(string srcKey, string dstKey)
        {
            var hasEdge = false;

            foreach (var edge in this.Edges)
            {
                if (edge.Source.Key.Equals(srcKey) && edge.Target.Key.Equals(dstKey))
                {
                    hasEdge = true;
                }
            }

            return hasEdge;
        }

        public static Graph Read(string fileName)
        {
            var structure = NetworkStructure.Read(fileName);

            var graph = new Graph
            {
                DefaultGroup = structure.ParseUserProperty("defaultgroup", "all"),
                Name = structure.ParseUserProperty("key", ""),
                network = structure.Network
            };

            // Add all the vertices
            foreach (var structureVertex in structure.Vertices)
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
            foreach (var srcVertex in structure.Vertices)
            {
                foreach (var dstVertex in srcVertex.Children)
                {
                    graph.AddEdge(graph.Vertices[srcVertex.Key], graph.Vertices[dstVertex.Key]);
                }
            }

            graph.UpdateDisplayGraph(graph.DefaultGroup);
            graph.UpdateBelief();

            return graph;
        }

        public static Task<Graph> ReadAsync(string fileName)
        {
            return Task.Run(() => Read(fileName));
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
            this.ClearNetworkEvidence();
            this.SetNetworkEvidence(this.Evidence);
            this.Belief = this.GetNetworkBelief();
        }

        public Dictionary<string, string, double> Run(string vertexKey, Dictionary<string, double> vertexEvidence)
        {
            this.SetNetworkEvidence(vertexKey, vertexEvidence);
            return this.GetNetworkBelief();
        }

        public Dictionary<string, string, double> Run(Dictionary<string, string, double> graphEvidence)
        {
            this.Evidence = graphEvidence;
            return this.GetNetworkBelief();
        }

        public Dictionary<int, string, string, double> Run(Dictionary<string, string, double> graphEvidence, int startYear, int endYear)
        {
            var graphValueTimeSeries = new Dictionary<int, string, string, double>();

            for (var year = startYear; year <= endYear; year++)
            {
                // If this is not the start year, then add the looped evidences
                if (year > startYear)
                {
                    foreach (var srcVertexKey in this.Loops.Keys)
                    {
                        var dstVertexKey = this.Loops[srcVertexKey];

                        var lastGraphValue = graphValueTimeSeries[year - 1];

                        var lastVertexValue = lastGraphValue[srcVertexKey];

                        graphEvidence[dstVertexKey] = lastVertexValue;
                    }
                }

                graphValueTimeSeries[year] = this.Run(graphEvidence);
            }

            return graphValueTimeSeries;
        }

        public Dictionary<int, string, string, double> Run(Dictionary<int, string, string, double> modelEvidence, int startYear, int endYear)
        {
            var graphValueTimeSeries = new Dictionary<int, string, string, double>();

            for (var year = startYear; year <= endYear; year++)
            {
                if (!modelEvidence.ContainsKey(year))
                {
                    graphValueTimeSeries[year] = this.Belief;
                    continue;
                }

                var graphEvidence = modelEvidence[year];

                // If this is not the start year, then add the looped evidences
                if (year > startYear)
                {
                    foreach (var srcVertexKey in this.Loops.Keys)
                    {
                        var dstVertexKey = this.Loops[srcVertexKey];

                        var lastGraphValue = graphValueTimeSeries[year - 1];

                        var lastVertexValue = lastGraphValue[srcVertexKey];

                        graphEvidence[dstVertexKey] = lastVertexValue;
                    }
                }

                graphValueTimeSeries[year] = this.Run(graphEvidence);

                this.ClearEvidence();
            }

            return graphValueTimeSeries;
        }

        public void SetEvidence(Dictionary<string, IVertexEvidence> graphEvidence)
        {
            foreach (var vertex in this.Vertices)
            {
                if (graphEvidence == null)
                {
                    vertex.Evidence = null;
                    vertex.EvidenceString = null;
                }
                else
                {
                    if (graphEvidence.ContainsKey(vertex.Key))
                    {
                        vertex.SetEvidence(graphEvidence[vertex.Key]);
                    }
                    else
                    {
                        vertex.Evidence = null;
                        vertex.EvidenceString = null;
                    }
                }
            }
        }

        public void SetEvidence(string vertexKey, string stateKey)
        {
            var stateIndex = this.Vertices[vertexKey].States.IndexOf(stateKey);
            this.SetEvidence(vertexKey, stateIndex);
        }

        public void SetEvidence(string vertexKey, int stateIndex)
        {
            this.network.SetEvidence(vertexKey, stateIndex);
        }

        public void SetEvidence(string vertexKey, VertexEvidenceString vertexEvidenceString)
        {
            this.Vertices[vertexKey].SetEvidence(vertexEvidenceString);
        }

        public void SetNetworkEvidence(string vertexKey, Dictionary<string, double> vertexEvidence)
        {
            this.network.SetSoftEvidence(vertexKey, vertexEvidence.ToArray());
        }

        public void SetNetworkTable(string vertexKey, double[,] table)
        {
            this.network.SetNodeTable(vertexKey, table);
        }

        public void UpdateBelief()
        {
            var networkBelief = this.GetNetworkBelief();
            this.Belief = networkBelief;
        }

        public void UpdateDisplayGraph(string group)
        {
            this.DisplayGraph = this.GetSubGraph(group);

            this.IsDefaultGroupVisible = @group == this.DefaultGroup;
        }

        public void UpdateNetworkBeliefs()
        {
            this.network.UpdateBeliefs();
        }

        public void Write(string fileName)
        {
            var structure = NetworkStructure.Read(fileName);

            var userProperties = new List<string>
            {
                "defaultgroup=" + this.DefaultGroup,
                "key=" + this.Name,
            };

            structure.Properties["HR_Desc"] = userProperties.String().Enquote();

            foreach (var networkStructureVertex in structure.Vertices)
            {
                var vertex = this.Vertices[networkStructureVertex.Key];

                networkStructureVertex.Properties["ConnectorPositions"] = vertex.ConnectorPositions.ToJson().Replace('"', '\'').Enquote();
                networkStructureVertex.Properties["groups"] = vertex.Groups.String().Enquote();
                networkStructureVertex.Properties["isexpanded"] = vertex.IsExpanded.ToString().Enquote();
                networkStructureVertex.Properties["label"] = "\"" + vertex.Name + "\"";
                networkStructureVertex.Properties["PositionForGroup"] = vertex.PositionForGroup.ToJson().Replace('"', '\'').Enquote();
                networkStructureVertex.Properties["units"] = "\"" + vertex.Units + "\"";

                // Remove legacy properties
                networkStructureVertex.Properties.Remove("grouppositions");
                networkStructureVertex.Properties.Remove("isheaderofgroup");
            }

            structure.Write(fileName);
        }

        private void ClearNetworkEvidence()
        {
            this.network.ClearAllEvidence();
        }

        private void SetNetworkEvidence(Dictionary<string, string, double> graphEvidence)
        {
            foreach (var vertexKey in graphEvidence.Keys)
            {
                this.SetNetworkEvidence(vertexKey, graphEvidence[vertexKey]);
            }
        }
    }
}