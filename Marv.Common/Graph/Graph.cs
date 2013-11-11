using NLog;
using QuickGraph.Algorithms.RankedShortestPath;
using Smile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Marv.Common
{
    public partial class Graph : ViewModel
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string defaultGroup;
        private EdgeCollection edges = new EdgeCollection();
        private Dict<string, string> loops = new Dict<string, string>();
        private Network network = new Network();
        private ViewModelCollection<Vertex> vertices = new ViewModelCollection<Vertex>();

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

        public EdgeCollection Edges
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

        public Dict<string, string> Loops
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

        public Dict<string, string, double> Value
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

        public ViewModelCollection<Vertex> Vertices
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
                vertex.ConnectorPositions = structureVertex.Properties["ConnectorPositions"].Dequote().ParseJson<Dict<string, string, EdgeConnectorPositions>>();
                vertex.Description = structureVertex.ParseStringProperty("HR_HTML_Desc");
                vertex.Groups = structureVertex.ParseGroups();
                vertex.HeaderOfGroup = structureVertex.ParseStringProperty("headerofgroup");
                vertex.InputVertexKey = structureVertex.ParseStringProperty("inputvertexkey");
                vertex.IsExpanded = structureVertex.ParseIsExpanded();
                vertex.IsHeader = !string.IsNullOrWhiteSpace(vertex.HeaderOfGroup);
                vertex.Name = structureVertex.ParseStringProperty("label");
                vertex.Position = structureVertex.ParsePosition();
                vertex.PositionForGroup = structureVertex.Properties["PositionForGroup"].Dequote().ParseJson<Dictionary<string, Point>>();
                vertex.Units = structureVertex.ParseStringProperty("units");
                vertex.States = structureVertex.ParseStates();
                vertex.Type = structureVertex.ParseSubType();

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

            graph.DefaultGroup = structure.ParseUserProperty("defaultgroup", defaultValue: "all");
            graph.Name = structure.ParseUserProperty("key", defaultValue: "");

            graph.UpdateValue();
            return graph;
        }

        public static Task<Graph> ReadAsync(string fileName)
        {
            return Task.Run(() => Graph.Read(fileName));
        }

        public void AddEdge(Vertex source, Vertex target)
        {
            this.Edges.Add(new Edge(source, target));
        }

        public Dict<string, string, double> ClearEvidence()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.IsEvidenceEntered = false;
            }

            this.network.ClearAllEvidence();
            return this.GetNetworkValue();
        }

        public Dict<string, string, double> ClearEvidence(string vertexKey)
        {
            this.network.ClearEvidence(vertexKey);
            return this.GetNetworkValue();
        }

        public double GetMean(string vertexKey, Dictionary<string, double> vertexValue)
        {
            return this.GetVertex(vertexKey).GetMean(vertexValue);
        }

        public Dict<string, string, double> GetNetworkValue()
        {
            this.UpdateBeliefs();

            var graphValue = new Dict<string, string, double>();

            foreach (var vertex in this.Vertices)
            {
                var vertexValue = new Dict<string, double>();

                foreach (var state in vertex.States)
                {
                    try
                    {
                        vertexValue[state.Key] = this.network.GetNodeValue(vertex.Key)[vertex.GetStateIndex(state.Key)];
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

        public double GetStandardDeviation(string vertexKey, Dictionary<string, double> vertexValue)
        {
            return this.GetVertex(vertexKey).GetStandardDeviation(vertexValue);
        }

        public Graph GetSubGraph(string group)
        {
            // Extract the header vertices
            Graph subGraph = new Graph();

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
                        Vertex src = path.First().Source;

                        foreach (var edge in path)
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

        public Dict<string, string, double> ReadValueCsv(string fileName)
        {
            var graphValue = new Dict<string, string, double>();

            foreach (var line in File.ReadLines(fileName))
            {
                var vertexValue = new Dict<string, double>();

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

        public Dict<string, string, double> Run(string vertexKey, IEvidence evidence)
        {
            evidence.Set(this, vertexKey);
            return this.GetNetworkValue();
        }

        public Dict<string, string, double> Run(Dictionary<string, IEvidence> graphEvidence)
        {
            foreach (var vertexKey in graphEvidence.Keys)
            {
                graphEvidence[vertexKey].Set(this, vertexKey);
            }

            return this.GetNetworkValue();
        }

        public Dict<int, string, string, double> Run(Dictionary<string, IEvidence> graphEvidence, int startYear, int endYear)
        {
            var graphValueTimeSeries = new Dict<int, string, string, double>();

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

        public Dict<string, string, double> UpdateValue()
        {
            return this.Value = this.GetNetworkValue();
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

        private IEnumerable<Vertex> GetChildren(Vertex vertex)
        {
            return this.Edges.Where(edge => edge.Source == vertex).Select(edge => edge.Target);
        }
    }
}