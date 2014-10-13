using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Smile;

namespace Marv
{
    public class Network : Smile.Network, INotifyPropertyChanged
    {
        public const string BeliefFileExtension = "marv-networkbelief";
        public const string EvidenceFileExtension = "marv-networkevidence";

        public readonly List<string> Footer = new List<string>();
        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();
        public readonly KeyedCollection<NetworkVertex> Vertices = new KeyedCollection<NetworkVertex>();

        private string fileName;

        public string FileName
        {
            get { return this.fileName; }

            set
            {
                if (value.Equals(this.fileName))
                {
                    return;
                }

                this.fileName = value;
                this.RaisePropertyChanged();
            }
        }

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

            byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            var cryptStream = new CryptoStream(pathStream, rMCrypto.CreateEncryptor(key, iv), CryptoStreamMode.Write);

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

        public Dict<string, double[]> GetBeliefs()
        {
            var vertexBeliefs = new Dict<string, double[]>();

            foreach (var nodeKey in this.GetAllNodeIds())
            {
                vertexBeliefs[nodeKey] = this.GetNodeValue(nodeKey);
            }

            return vertexBeliefs;
        }

        public Dict<string, VertexEvidence> GetEvidences()
        {
            var graphData = new Dict<string, VertexEvidence>();

            foreach (var vertexKey in this.GetAllNodeIds())
            {
                var softEvidence = this.GetSoftEvidence(vertexKey);
                var evidenceIndex = this.IsEvidence(vertexKey) ? this.GetEvidence(vertexKey) : -1;

                if (softEvidence != null)
                {
                    graphData[vertexKey].Value = softEvidence;
                }
                else if (evidenceIndex >= 0)
                {
                    graphData[vertexKey].Value = new double[this.GetOutcomeCount(vertexKey)];
                    graphData[vertexKey].Value[evidenceIndex] = 1;
                }
            }

            return graphData;
        }

        public double[] GetIntervals(string vertexKey)
        {
            var states = this.Vertices[vertexKey].States;
            return states.Select(state => state.Min).Concat(states.Last().Max.Yield()).ToArray();
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

        public Dict<string, double[]> Run(Dict<string, double[]> graphData)
        {
            this.ClearAllEvidence();

            foreach (var vertexKey in graphData.Keys)
            {
                if (graphData[vertexKey] != null && graphData[vertexKey].Sum() > 0)
                {
                    this.SetSoftEvidence(vertexKey, graphData[vertexKey]);
                }
            }

            try
            {
                this.UpdateBeliefs();
                return this.GetBeliefs();
            }
            catch (SmileException)
            {
                return null;
            }
        }

        public Dict<string, double[]> Run(Dict<string, VertexEvidence> graphData)
        {
            this.ClearAllEvidence();

            foreach (var vertexKey in graphData.Keys)
            {
                if (graphData[vertexKey].Value != null && graphData[vertexKey].Value.Sum() > 0)
                {
                    this.SetSoftEvidence(vertexKey, graphData[vertexKey].Value);
                }
            }

            try
            {
                this.UpdateBeliefs();
                return this.GetBeliefs();
            }
            catch (SmileException)
            {
                return null;
            }
        }

        public Dict<int, string, double[]> Run(Dict<int, string, VertexEvidence> yearEvidences)
        {
            var yearBeliefs = new Dict<int, string, double[]>();

            var firstYear = yearEvidences.Keys.First();
            var lastYear = firstYear;

            foreach (var year in yearEvidences.Keys)
            {
                if (this.Loops != null)
                {
                    if (year != firstYear)
                    {
                        foreach (var targetVertexKey in this.Loops.Keys)
                        {
                            var sourceVertexKey = this.Loops[targetVertexKey];
                            yearEvidences[year][targetVertexKey].Value = yearBeliefs[lastYear][sourceVertexKey];
                        }
                    }
                }

                yearBeliefs[year] = this.Run(yearEvidences[year]);

                lastYear = year;
            }

            return yearBeliefs;
        }

        public Dict<string, int, string, double[]> Run(Dict<string, int, string, VertexEvidence> lineData, IProgress<double> progress = null)
        {
            var sectionBeliefs = new Dict<string, int, string, double[]>();

            var total = lineData.Keys.Count;
            var count = 0;

            foreach (var sectionId in lineData.Keys)
            {
                sectionBeliefs[sectionId] = this.Run(lineData[sectionId]);

                count++;

                if (progress != null)
                {
                    progress.Report((double) count / total);
                }
            }

            return sectionBeliefs;
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

        public void WriteBeliefs(string fileName)
        {
            if (Path.GetExtension(fileName) != BeliefFileExtension)
            {
                fileName = fileName + "." + BeliefFileExtension;
            }

            this.GetBeliefs().WriteJson(fileName);
        }

        public void WriteEvidences(string fileName)
        {
            if (Path.GetExtension(fileName) != EvidenceFileExtension)
            {
                fileName = fileName + "." + EvidenceFileExtension;
            }

            this.GetEvidences().WriteJson(fileName);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}