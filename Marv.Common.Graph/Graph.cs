using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Marv.Common;
using QuickGraph.Algorithms.RankedShortestPath;

namespace Marv
{
    public partial class Graph : NotifyPropertyChanged
    {
        private string defaultGroup;
        private ObservableCollection<Edge> edges = new ObservableCollection<Edge>();
        private Guid guid;
        private bool isExpanded = true;
        private string key;
        private Vertex selectedVertex;
        private KeyedCollection<Vertex> vertices = new KeyedCollection<Vertex>();

        public Dict<string, double[]> Belief
        {
            get { return this.Vertices.ToDict(vertex => vertex.Key, vertex => vertex.Belief); }

            set
            {
                foreach (var vertex in this.Vertices)
                {
                    vertex.Belief = value != null && value.ContainsKey(vertex.Key) ? value[vertex.Key] : null;
                }
            }
        }

        public string DefaultGroup
        {
            get { return this.defaultGroup; }

            set
            {
                if (value != this.defaultGroup)
                {
                    this.defaultGroup = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<Edge> Edges
        {
            get { return this.edges; }

            set
            {
                if (value != this.edges)
                {
                    this.edges = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public Dict<string, double[]> Evidence
        {
            get { return this.Vertices.ToDict(vertex => vertex.Key, vertex => vertex.Evidence); }

            set
            {
                foreach (var vertex in this.Vertices)
                {
                    vertex.Evidence = value != null && value.ContainsKey(vertex.Key) ? value[vertex.Key] : null;
                }
            }
        }

        public IEnumerable<string> Groups
        {
            get
            {
                var groups = new List<string>();

                foreach (var vertex in this.Vertices)
                {
                    groups.AddUnique(vertex.Groups);
                }

                return groups;
            }
        }

        public Guid Guid
        {
            get { return this.guid; }

            set
            {
                if (value != this.guid)
                {
                    this.guid = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public bool IsExpanded
        {
            get { return this.isExpanded; }

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
            get { return this.Vertices.Count(vertex => vertex.IsExpanded) > this.Vertices.Count / 2; }
        }

        public string Key
        {
            get { return this.key; }

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
            get { return this.selectedVertex; }

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
            get { return this.vertices; }

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
            foreach (var networkNode in graph.Network.Nodes)
            {
                var vertex = new Vertex
                {
                    ConnectorPositions = networkNode.ParseJson<Dict<string, string, EdgeConnectorPositions>>("ConnectorPositions"),
                    Description = networkNode.ParseStringProperty("HR_HTML_Desc"),
                    Groups = networkNode.Groups,
                    HeaderOfGroup = networkNode.ParseStringProperty("headerofgroup"),
                    InputVertexKey = networkNode.InputNodeKey,
                    IsExpanded = networkNode.ParseIsExpanded(),
                    Key = networkNode.Key,
                    Name = networkNode.ParseStringProperty("label"),
                    Position = networkNode.ParsePosition(),
                    PositionForGroup = networkNode.ParseJson<Dict<string, Point>>("PositionForGroup"),
                    Units = networkNode.ParseStringProperty("units"),
                    States = networkNode.States,
                    Type = networkNode.Type
                };

                vertex.IsHeader = !string.IsNullOrWhiteSpace(vertex.HeaderOfGroup);

                if (string.IsNullOrWhiteSpace(vertex.Units))
                {
                    vertex.Units = "n/a";
                }

                graph.Vertices.Add(vertex);
            }

            // Add all the edges
            foreach (var srcVertex in graph.Network.Nodes)
            {
                foreach (var dstVertex in srcVertex.Children)
                {
                    graph.Edges.Add(new Edge(graph.Vertices[srcVertex.Key], graph.Vertices[dstVertex.Key]));
                }
            }

            return graph;
        }

        public void ClearEvidence()
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.ClearEvidence();
            }
        }

        /// <summary>
        ///     Returns the key of the vertex which is the header of the give group.
        /// </summary>
        /// <param name="group">The group of which the header is required.</param>
        /// <returns>The key of the vertex which is the header of group.</returns>
        public string GetHeaderVertexKey(string group)
        {
            var headerVertex = this.Vertices.FirstOrDefault(vertex => vertex.HeaderOfGroup == @group);
            return headerVertex == null ? null : headerVertex.Key;
        }

        public Vertex GetSinkVertex()
        {
            return this.Vertices.First(vertex => this.Edges.All(edge => edge.Source != vertex));
        }

        public Graph GetSubGraph(string group, string vertexKey = null)
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

                    vertex.SelectedGroup = group;
                    vertex.DisplayPosition = vertex.PositionForGroup[group];
                }
            }

            if (vertexKey != null)
            {
                var defaultGroupPosition = this.Vertices[vertexKey].PositionForGroup[this.DefaultGroup];
                var positionOffset = this.Vertices[vertexKey].PositionForGroup[group] - defaultGroupPosition;

                foreach (var vertex in subGraph.Vertices)
                {
                    vertex.DisplayPosition = vertex.PositionForGroup[group] - positionOffset;
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

                                var newEdge = new Edge(src, edge.Target)
                                {
                                    ConnectorPositions = connectorPostions ?? new EdgeConnectorPositions()
                                };

                                subGraph.Edges.AddUnique(newEdge);

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
            this.Belief = Network.Read(this.Network.FileName).Run(this.Evidence);
        }

        public void SetEvidence(Dict<string, VertexEvidence> vertexEvidences)
        {
            foreach (var vertex in this.Vertices)
            {
                vertex.Evidence = vertexEvidences[vertex.Key].Value;
                vertex.EvidenceString = vertexEvidences[vertex.Key].ToString();
            }
        }

        public void Write()
        {
            this.Write(this.Network.FileName);
        }

        public void Write(string filePath)
        {
            var userProperties = new List<string>
            {
                "defaultgroup=" + this.DefaultGroup,
                "guid=" + this.Guid,
                "key=" + this.Key,
            };

            this.Network.Properties["HR_Desc"] = userProperties.String().Enquote();

            foreach (var networkStructureVertex in this.Network.Nodes)
            {
                var vertex = this.Vertices[networkStructureVertex.Key];

                networkStructureVertex.Properties["ConnectorPositions"] = vertex.ConnectorPositions.ToJson().Replace('"', '\'').Enquote();
                networkStructureVertex.Properties["groups"] = vertex.Groups.String().Enquote();
                networkStructureVertex.Properties["HR_Desc"] = vertex.Description.Enquote();
                networkStructureVertex.Properties["HR_HTML_Desc"] = vertex.Description.Enquote();
                networkStructureVertex.Properties["isexpanded"] = vertex.IsExpanded.ToString().Enquote();
                networkStructureVertex.Properties["label"] = "\"" + vertex.Name + "\"";
                networkStructureVertex.Properties["PositionForGroup"] = vertex.PositionForGroup.ToJson().Replace('"', '\'').Enquote();
                networkStructureVertex.Properties["units"] = "\"" + vertex.Units + "\"";

                // Remove legacy properties
                networkStructureVertex.Properties.Remove("grouppositions");
                networkStructureVertex.Properties.Remove("isheaderofgroup");
            }

            this.Network.Write(filePath);
        }
    }
}