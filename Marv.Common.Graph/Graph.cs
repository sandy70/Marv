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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string defaultGroup;
        private ModelCollection<Edge> edges = new ModelCollection<Edge>();
        private Dictionary<string, string> loops = new Dictionary<string, string>();
        private Network network = new Network();
        private ModelCollection<Vertex> vertices = new ModelCollection<Vertex>();

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
                    vertex.Belief = value[vertex.Key];
                }

                this.RaisePropertyChanged("Belief");
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
                    this.RaisePropertyChanged("DefaultGroup");
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
                    this.RaisePropertyChanged("Edges");
                }
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
                    this.RaisePropertyChanged("Loops");
                }
            }
        }

        public Dictionary<string, string, double> Value
        {
            set
            {
                foreach (var vertex in this.Vertices)
                {
                    vertex.Value = value[vertex.Key];
                }

                this.RaisePropertyChanged("Value");
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
                    this.RaisePropertyChanged("Vertices");
                }
            }
        }

        public void AddEdge(Vertex source, Vertex target)
        {
            this.Edges.Add(new Edge(source, target));
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
            return this.Vertices[vertexKey].GetMean(vertexValue);
        }

        public Dictionary<string, string, double> GetNetworkValue()
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
                        logger.Warn(exception.Message);
                        vertexValue[state.Key] = 0;
                    }
                }

                graphValue[vertex.Key] = vertexValue;
            }

            return graphValue;
        }

        public Dictionary<string, string, double> GetSensitivity(string targetVertexKey, IVertexValueComputer vertexValueComputer, Dictionary<string, IEvidence> graphEvidence = null)
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
                this.SetEvidence(graphEvidence);

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

                        var graphValue = this.GetNetworkValue();
                        var targetVertexValue = graphValue[targetVertex.Key];

                        value[sourceVertex.Key, sourceState.Key] = vertexValueComputer.Compute(targetVertex, targetVertexValue);

                        this.ClearEvidence(sourceVertex.Key);
                    }
                    catch (SmileException exception)
                    {
                        logger.Error(exception.Message);

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
                        if (!vertex.Commands.Contains(VertexCommand.VertexSubGraphCommand))
                        {
                            vertex.Commands.Push(VertexCommand.VertexSubGraphCommand);
                        }
                    }
                    else
                    {
                        if (vertex.Commands.Contains(VertexCommand.VertexSubGraphCommand))
                        {
                            vertex.Commands.Remove(VertexCommand.VertexSubGraphCommand);
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
                                var connectorPostions = srcVertex.ConnectorPositions[group, dstVertex.Key];
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
            var graph = new Graph();

            try
            {
                graph.network.ReadFile(fileName);
            }
            catch (SmileException exception)
            {
                logger.Warn("Error reading file {0}: {1}", fileName, exception.Message);
                return graph;
            }

            var structure = NetworkStructure.Read(fileName);

            // Add all the vertices
            foreach (var structureVertex in structure.Vertices)
            {
                var vertex = new Vertex();

                vertex.Key = structureVertex.Key;
                vertex.ConnectorPositions = structureVertex.ParseJson<Dictionary<string, string, EdgeConnectorPositions>>("ConnectorPositions");
                vertex.Description = structureVertex.ParseStringProperty("HR_HTML_Desc");
                vertex.Groups = structureVertex.ParseGroups();
                vertex.HeaderOfGroup = structureVertex.ParseStringProperty("headerofgroup");
                vertex.InputVertexKey = structureVertex.ParseStringProperty("InputVertexKey");
                vertex.IsExpanded = structureVertex.ParseIsExpanded();
                vertex.IsHeader = !string.IsNullOrWhiteSpace(vertex.HeaderOfGroup);
                vertex.Name = structureVertex.ParseStringProperty("label");
                vertex.Position = structureVertex.ParsePosition();
                vertex.PositionForGroup = structureVertex.ParseJson<Dictionary<string, Point>>("PositionForGroup");
                vertex.Units = structureVertex.ParseStringProperty("units");
                vertex.States = structureVertex.ParseStates();
                vertex.Type = structureVertex.ParseSubType();

                // If all states have ranges, then it is an interval vertex. This is a hack. Remove
                // this once the team starts using Hugin node types consistently.
                if (vertex.States.Select(state => state.Range != null).Count() == vertex.States.Count()) vertex.Type = VertexType.Interval;

                if (string.IsNullOrWhiteSpace(vertex.Units))
                {
                    vertex.Units = "n/a";
                }

                graph.Vertices.Add(vertex);

                if (!string.IsNullOrWhiteSpace(vertex.InputVertexKey))
                {
                    graph.Loops[vertex.InputVertexKey] = vertex.Key;
                }
            }

            // Add all the edges
            foreach (var srcNode in structure.Vertices)
            {
                foreach (var dstNode in srcNode.Children)
                {
                    graph.AddEdge(graph.Vertices[srcNode.Key], graph.Vertices[dstNode.Key]);
                }
            }

            graph.DefaultGroup = structure.ParseUserProperty("defaultgroup", "all");
            graph.Name = structure.ParseUserProperty("key", "");

            graph.UpdateValue();
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

        public Dictionary<string, string, double> Run(string vertexKey, IEvidence evidence)
        {
            evidence.Set(this, vertexKey);
            return this.GetNetworkValue();
        }

        public Dictionary<string, string, double> Run(Dictionary<string, IEvidence> graphEvidence)
        {
            this.SetEvidence(graphEvidence);
            return this.GetNetworkValue();
        }

        public Dictionary<int, string, string, double> Run(Dictionary<string, IEvidence> graphEvidence, int startYear, int endYear)
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

        public Dictionary<int, string, string, double> Run(Dictionary<int, string, IEvidence> graphEvidenceTimeSeries, int startYear, int endYear)
        {
            var graphValueTimeSeries = new Dictionary<int, string, string, double>();

            for (var year = startYear; year <= endYear; year++)
            {
                var graphEvidence = graphEvidenceTimeSeries[year];

                // If this is not the start year, then add the looped evidences
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

        public void SetEvidence(string vertexKey, string stateKey)
        {
            var stateIndex = this.Vertices[vertexKey].States.IndexOf(stateKey);
            this.SetEvidence(vertexKey, stateIndex);
        }

        public void SetEvidence(string vertexKey, int stateIndex)
        {
            this.network.SetEvidence(vertexKey, stateIndex);
        }

        public void SetEvidence(string vertexKey, double[] evidence)
        {
            this.network.SetSoftEvidence(vertexKey, evidence);
        }

        public void SetEvidence(Dictionary<string, IEvidence> graphEvidence)
        {
            foreach (var vertexKey in graphEvidence.Keys)
            {
                graphEvidence[vertexKey].Set(this, vertexKey);
            }
        }

        public void SetTable(string vertexKey, double[,] table)
        {
            this.network.SetNodeTable(vertexKey, table);
        }

        public void SetValueToZero()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.SetValueToZero();
            }
        }

        public void UpdateNetworkBeliefs()
        {
            this.network.UpdateBeliefs();
        }

        public void UpdateValue()
        {
            var graphValue = this.GetNetworkValue();
            this.Belief = graphValue;
            this.Value = graphValue;
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
    }
}