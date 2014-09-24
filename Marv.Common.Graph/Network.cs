using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Smile;

namespace Marv
{
    public class Network : Smile.Network
    {
        public const string DataFileDescription = "MARV Network Data";
        public const string DataFileExtension = "marv-networkdata";

        public readonly List<string> Footer = new List<string>();
        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();
        public readonly KeyedCollection<NetworkVertex> Vertices = new KeyedCollection<NetworkVertex>();

        public string FileName { get; set; }

        // Dictionary<targetVertexKey, sourceVertexKey>
        // Beliefs from sourceVertexKey should go into targetVertexKey
        public Dictionary<string, string> Loops { get; set; }

        public static void Decrypt(string path)
        {
            var pathStream = new FileStream(path, FileMode.OpenOrCreate);
            var rMCrypto = new RijndaelManaged();

            byte[] Key =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
            };

            byte[] IV =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
            };

            var cryptStream = new CryptoStream(pathStream,
                rMCrypto.CreateDecryptor(Key, IV),
                CryptoStreamMode.Read);

            var lineList = new List<String>();
            try
            {
                var sReader = new StreamReader(cryptStream);

                while (!sReader.EndOfStream)
                {
                    lineList.Add(sReader.ReadLine());
                }
                sReader.Close();
            }
            catch (CryptographicException e)
            {
                return;
            }

            var sWriter = new StreamWriter(path);
            foreach (var line in lineList)
            {
                sWriter.WriteLine(line);
            }
            sWriter.Close();
        }

        public static Network Read(string path)
        {
            var network = new Network();
            network.Loops = new Dictionary<string, string>();
            network.FileName = path;
            network.ReadFile(path);

            var fileLines = File.ReadAllLines(path).Trimmed().ToList();

            var nLines = fileLines.Count;

            for (var i = 0; i < nLines; i++)
            {
                // Parse the node section
                if (fileLines[i].StartsWith("node"))
                {
                    var parts = fileLines[i].Split(new[]
                    {
                        ' '
                    },
                        2).ToList();

                    var node = new NetworkVertex
                    {
                        Key = parts[1]
                    };

                    network.Vertices.Add(node);

                    while (!fileLines[i].Equals("{"))
                    {
                        i++;
                    }

                    i++;

                    while (!fileLines[i].Equals("}"))
                    {
                        parts = fileLines[i].Split(new[]
                        {
                            '=', ';'
                        },
                            2,
                            StringSplitOptions.RemoveEmptyEntries).ToList();
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
                        var parts = fileLines[i].Split(new[]
                        {
                            '=', ';'
                        },
                            2,
                            StringSplitOptions.RemoveEmptyEntries).ToList();
                        network.Properties[parts[0].Trim()] = new string(parts[1].Trim().AllButLast().ToArray());
                        i++;
                    }
                }

                    // Else just add to the footer. Later these might be 'potential' objects
                else
                {
                    network.Footer.Add(fileLines[i]);
                }
            }

            // Parse Children
            foreach (var vertex in network.Vertices)
            {
                foreach (var childHandle in network.GetChildren(vertex.Key))
                {
                    var childKey = network.GetNodeId(childHandle);
                    vertex.Children.Add(network.Vertices[childKey]);
                }

                var inputVertexKey = vertex.ParseStringProperty("InputNode");

                if (!string.IsNullOrWhiteSpace(inputVertexKey))
                {
                    network.Loops[vertex.Key] = inputVertexKey;
                }
            }

            return network;
        }

        public void ClearEvidence()
        {
            this.ClearAllEvidence();
        }

        public void EncryptWrite(string path)
        {
            var pathStream = new FileStream(path, FileMode.OpenOrCreate);
            var rMCrypto = new RijndaelManaged();

            byte[] Key =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
            };
            byte[] IV =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
            };

            var cryptStream = new CryptoStream(pathStream,
                rMCrypto.CreateEncryptor(Key, IV),
                CryptoStreamMode.Write);
            using (var writer = new StreamWriter(cryptStream))
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

        public Dictionary<string, double[]> GetBeliefs()
        {
            var vertexBeliefs = new Dictionary<string, double[]>();

            foreach (var nodeKey in this.GetAllNodeIds())
            {
                vertexBeliefs[nodeKey] = this.GetNodeValue(nodeKey);
            }

            return vertexBeliefs;
        }

        public Dict<string, VertexData> GetData()
        {
            var graphData = new Dict<string, VertexData>();

            foreach (var vertexKey in this.GetAllNodeIds())
            {
                graphData[vertexKey].Belief = this.GetNodeValue(vertexKey);
                graphData[vertexKey].Evidence = this.GetSoftEvidence(vertexKey);
            }

            return graphData;
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

        public void Run(Dict<string, VertexData> graphData)
        {
            this.ClearAllEvidence();

            foreach (var vertexKey in graphData.Keys)
            {
                if (graphData[vertexKey].Evidence != null && graphData[vertexKey].Evidence.Sum() > 0)
                {
                    this.SetSoftEvidence(vertexKey, graphData[vertexKey].Evidence);
                }
            }

            try
            {
                this.UpdateBeliefs();
                var beliefs = this.GetBeliefs();

                foreach (var vertexKey in beliefs.Keys)
                {
                    graphData[vertexKey].Belief = beliefs[vertexKey];
                }
            }
            catch (SmileException)
            {
                // do nothing
            }
        }

        public void Run(Dict<int, string, VertexData> sectionData)
        {
            var firstYear = sectionData.Keys.First();
            var lastYear = firstYear;

            foreach (var year in sectionData.Keys)
            {
                if (this.Loops != null)
                {
                    if (year != firstYear)
                    {
                        foreach (var targetVertexKey in this.Loops.Keys)
                        {
                            var sourceVertexKey = this.Loops[targetVertexKey];
                            sectionData[year][targetVertexKey].Evidence = sectionData[lastYear][sourceVertexKey].Belief;
                        }
                    }
                }

                this.Run(sectionData[year]);

                lastYear = year;
            }
        }

        public void Run(Dict<string, int, string, VertexData> lineData, IProgress<double> progress = null)
        {
            var total = lineData.Keys.Count;
            var count = 0;

            foreach (var sectionId in lineData.Keys)
            {
                this.Run(lineData[sectionId]);

                count++;

                if (progress != null)
                {
                    progress.Report((double) count / total);
                }
            }
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
                "guid=" + graph.Guid,
                "key=" + graph.Key,
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

        public void WriteData(string fileName)
        {
            if (Path.GetExtension(fileName) != DataFileExtension)
            {
                fileName = fileName + "." + DataFileExtension;
            }

            this.GetData().WriteJson(fileName);
        }
    }
}