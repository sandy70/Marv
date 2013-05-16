using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LibBn
{
    public class NetworkStructure
    {
        public List<string> Footer = new List<string>();

        public List<NetworkStructureNode> Nodes = new List<NetworkStructureNode>();

        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public static NetworkStructure Read(string path)
        {
            var structure = new NetworkStructure();

            var fileLines = File.ReadAllLines(path).Trimmed().ToList();

            var nLines = fileLines.Count;

            for (int i = 0; i < nLines; i++)
            {
                if (fileLines[i].StartsWith("node"))
                {
                    var parts = fileLines[i].Split(new char[] { ' ' }, 2).ToList();

                    var node = new NetworkStructureNode { Key = parts[1] };

                    structure.Nodes.Add(node);

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

                else
                {
                    structure.Footer.Add(fileLines[i]);
                }
            }

            foreach (var node in structure.Nodes)
            {
                var linkValueString = node.Properties["HR_LinkMode"];
                var parts = linkValueString.Split(new char[] { '[', ']', '"' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    var subParts = part.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    node.Children.Add(structure.GetNode(subParts[0]));
                }
            }

            return structure;
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

                foreach (var node in this.Nodes)
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
                    writer.WriteLine(line);
                }
            }
        }

        public void FixStates()
        {
            foreach (var node in this.Nodes)
            {
                var states = node.ParseStates();

                foreach (var prop in node.Properties.Where(x => x.Key.StartsWith("HR_State_")).ToList())
                {
                    node.Properties.Remove(prop.Key);
                }

                foreach (var state in states)
                {
                    int n = states.IndexOf(state);
                    node.Properties[String.Format("HR_State_{0}", n)] = "\"" + state + "\"";
                }
            }
        }
    }
}