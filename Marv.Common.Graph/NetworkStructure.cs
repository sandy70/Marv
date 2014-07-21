using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Smile;

namespace Marv.Common.Graph
{
    public class NetworkStructure
    {
        public readonly List<string> Footer = new List<string>();
        public readonly Network Network = new Network();
        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();
        public readonly List<NetworkStructureVertex> Vertices = new List<NetworkStructureVertex>();
        public string FileName;

        public NetworkStructureVertex GetVertex(string key)
        {
            return this.Vertices.Where(x => x.Key.Equals(key)).FirstOrDefault();
        }

        public string ParseUserProperty(string userPropertyName, string defaultValue)
        {
            if (this.Properties.ContainsKey("HR_Desc"))
            {
                var descValueString = this.Properties["HR_Desc"];

                var parts = descValueString.Split("\",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    var subParts = part.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (subParts.Count() == 2)
                    {
                        if (subParts[0].Equals(userPropertyName))
                        {
                            return subParts[1];
                        }
                    }
                }
            }

            return defaultValue;
        }

        public static NetworkStructure Read(string path)
        {
            var structure = new NetworkStructure();
            structure.Network.ReadFile(path);
            structure.FileName = path;

            var fileLines = File.ReadAllLines(path).Trimmed().ToList();

            var nLines = fileLines.Count;

            for (var i = 0; i < nLines; i++)
            {
                // Parse the node section
                if (fileLines[i].StartsWith("node"))
                {
                    var parts = fileLines[i].Split(new[] {' '}, 2).ToList();

                    var node = new NetworkStructureVertex {Key = parts[1]};

                    structure.Vertices.Add(node);

                    while (!fileLines[i].Equals("{"))
                    {
                        i++;
                    }

                    i++;

                    while (!fileLines[i].Equals("}"))
                    {
                        parts = fileLines[i].Split(new[] {'=', ';'}, 2, StringSplitOptions.RemoveEmptyEntries).ToList();
                        node.Properties[parts[0].Trim()] = new string(parts[1].Trim().AllButLast().ToArray());
                        i++;
                    }
                }

                    // Parse the header 'net' section
                else if (fileLines[i].Equals("net"))
                {
                    while (!fileLines[i].Equals("{"))
                    {
                        i++;
                    }

                    i++;

                    while (!fileLines[i].Equals("}"))
                    {
                        var parts = fileLines[i].Split(new[] {'=', ';'}, 2, StringSplitOptions.RemoveEmptyEntries).ToList();
                        structure.Properties[parts[0].Trim()] = new string(parts[1].Trim().AllButLast().ToArray());
                        i++;
                    }
                }

                    // Else just add to the footer. Later these might be 'potential' objects
                else
                {
                    structure.Footer.Add(fileLines[i]);
                }
            }

            // Parse Children
            foreach (var vertex in structure.Vertices)
            {
                foreach (var childHandle in structure.Network.GetChildren(vertex.Key))
                {
                    var childKey = structure.Network.GetNodeId(childHandle);
                    vertex.Children.Add(structure.GetVertex(childKey));
                }
            }

            return structure;
        }

        public void Write()
        {
            this.Write(this.FileName);
        }

        public void Write(Graph graph)
        {
            this.Write(this.FileName, graph);
        }

        public void Write(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine("net");
                writer.WriteLine("{");

                foreach (var prop in this.Properties)
                {
                    writer.WriteLine("\t{0} = {1};", prop.Key, prop.Value);
                }

                writer.WriteLine("}");

                foreach (var node in this.Vertices)
                {
                    writer.WriteLine();
                    writer.WriteLine("node {0}", node.Key);
                    writer.WriteLine("{");

                    foreach (var prop in node.Properties)
                    {
                        writer.WriteLine("\t{0} = {1};", prop.Key, prop.Value);
                    }

                    writer.WriteLine("}");
                }

                writer.WriteLine();

                foreach (var line in this.Footer)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }

        public void Write(string path, Graph graph)
        {
            var userProperties = new List<string>
            {
                "defaultgroup=" + graph.DefaultGroup,
                "key=" + graph.Name,
            };

            this.Properties["HR_Desc"] = userProperties.String().Enquote();

            foreach (var networkStructureVertex in this.Vertices)
            {
                var vertex = graph.Vertices[networkStructureVertex.Key];

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

            this.Write(path);
        }
    }
}