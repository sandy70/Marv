using Smile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marv.Common
{
    public class NetworkStructure
    {
        public List<string> Footer = new List<string>();
        public Dictionary<string, string> Properties = new Dictionary<string, string>();
        public List<NetworkStructureVertex> Vertices = new List<NetworkStructureVertex>();

        public static NetworkStructure Read(string path)
        {
            var network = new Network();
            network.ReadFile(path);

            var structure = new NetworkStructure();

            var fileLines = File.ReadAllLines(path).Trimmed().ToList();

            var nLines = fileLines.Count;

            for (var i = 0; i < nLines; i++)
            {
                // Parse the node section
                if (fileLines[i].StartsWith("node"))
                {
                    var parts = fileLines[i].Split(new char[] { ' ' }, 2).ToList();

                    var node = new NetworkStructureVertex { Key = parts[1] };

                    structure.Vertices.Add(node);

                    while (!fileLines[i].Equals("{"))
                    {
                        i++;
                    }

                    i++;

                    while (!fileLines[i].Equals("}"))
                    {
                        parts = fileLines[i].Split(new char[] { '=', ';' }, 2, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                        var parts = fileLines[i].Split(new char[] { '=', ';' }, 2, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                foreach (var childHandle in network.GetChildren(vertex.Key))
                {
                    var childKey = network.GetNodeId(childHandle);
                    vertex.Children.Add(structure.GetVertex(childKey));
                }
            }

            return structure;
        }

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
    }
}