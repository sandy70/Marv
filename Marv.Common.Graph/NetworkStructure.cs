using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Smile;

namespace Marv
{
    public class NetworkStructure
    {
        private readonly Network network = new Network();
        public readonly List<string> Footer = new List<string>();
        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();
        public readonly List<NetworkStructureVertex> Vertices = new List<NetworkStructureVertex>();

        public string FileName;

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

        public static NetworkStructure Read(string path)
        {
            var structure = new NetworkStructure();
            structure.network.ReadFile(path);
            structure.FileName = path;

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

                    var node = new NetworkStructureVertex
                    {
                        Key = parts[1]
                    };

                    structure.Vertices.Add(node);

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
                foreach (var childHandle in structure.network.GetChildren(vertex.Key))
                {
                    var childKey = structure.network.GetNodeId(childHandle);
                    vertex.Children.Add(structure.GetVertex(childKey));
                }
            }

            return structure;
        }

        private void Run(Dict<string, VertexData> graphData)
        {
            this.ClearEvidence();

            foreach (var vertexKey in graphData.Keys)
            {
                this.SetEvidence(vertexKey, graphData[vertexKey].Evidence);
            }

            this.UpdateBeliefs();

            var beliefs = this.GetBelief();

            foreach (var vertexKey in beliefs.Keys)
            {
                graphData[vertexKey].Beliefs = beliefs[vertexKey];
            }
        }

        public void ClearEvidence()
        {
            this.network.ClearAllEvidence();
        }

        public void ClearEvidence(string vertexKey)
        {
            this.network.ClearEvidence(vertexKey);
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

        public Dictionary<string, double[]> GetBelief()
        {
            var graphValue = new Dictionary<string, double[]>();

            foreach (var vertex in this.Vertices)
            {
                graphValue[vertex.Key] = this.network.GetNodeValue(vertex.Key);
            }

            return graphValue;
        }

        public double[,] GetTable(string vertexKey)
        {
            return this.network.GetNodeTable(vertexKey);
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

        public void UpdateBeliefs()
        {
            this.network.UpdateBeliefs();
        }

        public void Run(Dict<int, string, VertexData> sectionData)
        {
            foreach (var year in sectionData.Keys)
            {
                this.Run(sectionData[year]);
            }
        }

        public void Run(Dictionary<string, VertexData> vertexEvidences)
        {
            this.ClearEvidence();

            foreach (var vertexKey in vertexEvidences.Keys)
            {
                this.SetEvidence(vertexKey, vertexEvidences[vertexKey].Evidence);
            }

            foreach (var kvp in this.GetBelief())
            {
                if (!vertexEvidences.ContainsKey(kvp.Key))
                {
                    vertexEvidences[kvp.Key] = new VertexData();
                }

                vertexEvidences[kvp.Key].Beliefs = kvp.Value;
            }
        }

        public void SetEvidence(string vertexKey, int stateIndex)
        {
            this.network.SetEvidence(vertexKey, stateIndex);
        }

        public void SetEvidence(string vertexKey, IEnumerable<double> evidence)
        {
            if (evidence == null)
            {
                try
                {
                    this.network.ClearEvidence(vertexKey);
                }
                catch
                {
                    // do nothing
                }

                return;
            }

            this.network.SetSoftEvidence(vertexKey, evidence.ToArray());
        }

        public void SetTable(string vertexKey, double[,] table)
        {
            this.network.SetNodeTable(vertexKey, table);
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
    }
}