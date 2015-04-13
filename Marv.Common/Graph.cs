﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using QuickGraph.Algorithms.RankedShortestPath;

namespace Marv.Common
{
    public partial class Graph : NotifyPropertyChanged
    {
        private readonly ObservableCollection<Edge> edges = new ObservableCollection<Edge>();

        private string defaultGroup;
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

        public bool IsExpanded
        {
            set
            {
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

            // Add all the vertices
            foreach (var networkVertex in graph.Network.Vertices)
            {
                var vertex = new Vertex
                {
                    ConnectorPositions = networkVertex.ParseJson<Dict<string, string, EdgeConnectorPositions>>("ConnectorPositions"),
                    Description = networkVertex.ParseStringProperty("HR_HTML_Desc"),
                    Groups = networkVertex.Groups,
                    HeaderOfGroup = networkVertex.HeaderOfGroup,
                    InputVertexKey = networkVertex.InputNodeKey,
                    IsExpanded = networkVertex.ParseIsExpanded(),
                    Key = networkVertex.Key,
                    Name = networkVertex.ParseStringProperty("label"),
                    Position = networkVertex.ParsePosition(),
                    PositionForGroup = networkVertex.ParseJson<Dict<string, Point>>("PositionForGroup"),
                    Units = networkVertex.ParseStringProperty("units"),
                    States = networkVertex.States,
                    Type = networkVertex.Type
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
                    graph.edges.Add(new Edge(graph.Vertices[srcVertex.Key], graph.Vertices[dstVertex.Key]));
                }
            }

            return graph;
        }

        public Vertex GetSink()
        {
            return this.Vertices.First(vertex => this.edges.All(edge => edge.Source != vertex));
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

                                subGraph.edges.AddUnique(newEdge);

                                src = edge.Target;
                            }
                        }
                    }
                }
            }

            subGraph.DefaultGroup = group;

            return subGraph;
        }

        public void SetEvidence(Dict<string, VertexEvidence> vertexEvidences)
        {
            foreach (var vertex in this.Vertices)
            {
                if (vertexEvidences.ContainsKey(vertex.Key))
                {
                    vertex.SetEvidence(vertexEvidences[vertex.Key]);
                }
                else
                {
                    vertex.SetEvidence(null);
                }
            }
        }

        public void Write()
        {
            this.Write(this.Network.FileName);
        }

        private void Write(string filePath)
        {
            var userProperties = new List<string>
            {
                "defaultgroup=" + this.DefaultGroup,
            };

            this.Network.Properties["HR_Desc"] = userProperties.String().Enquote();

            foreach (var networkNode in this.Network.Vertices)
            {
                var vertex = this.Vertices[networkNode.Key];

                networkNode.Properties["ConnectorPositions"] = vertex.ConnectorPositions.ToJson().Replace('"', '\'').Enquote();
                networkNode.Properties["HR_Desc"] = vertex.Description.Enquote();
                networkNode.Properties["HR_HTML_Desc"] = vertex.Description.Enquote();
                networkNode.Properties["isexpanded"] = vertex.IsExpanded.ToString().Enquote();
                networkNode.Properties["label"] = "\"" + vertex.Name + "\"";
                networkNode.Properties["PositionForGroup"] = vertex.PositionForGroup.ToJson().Replace('"', '\'').Enquote();
                networkNode.Properties["units"] = "\"" + vertex.Units + "\"";

                // Remove legacy properties
                networkNode.Properties.Remove("grouppositions");
                networkNode.Properties.Remove("isheaderofgroup");
            }

            this.Network.Write(filePath);
        }
    }
}