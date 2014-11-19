using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Marv.Common.Graph;
using Smile;

namespace Marv
{
    public class Network : Smile.Network, INotifyPropertyChanged
    {
        private const string BeliefFileExtension = "marv-networkbelief";
        private const string EvidenceFileExtension = "marv-networkevidence";

        public readonly KeyedCollection<NetworkNode> Nodes = new KeyedCollection<NetworkNode>();
        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();
        private readonly List<string> footer = new List<string>();

        private string fileName;
        private Dictionary<string, string> loops = new Dictionary<string, string>();

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

        public Dict<string, double[]> InitialBelief
        {
            get { return this.Nodes.ToDict(vertex => vertex.Key, vertex => vertex.InitialBelief); }

            set
            {
                foreach (var vertex in this.Nodes)
                {
                    if (value.ContainsKey(vertex.Key))
                    {
                        vertex.InitialBelief = value[vertex.Key];
                    }
                }

                this.RaisePropertyChanged();
            }
        }

        // Dictionary<targetVertexKey, sourceVertexKey>
        // Beliefs from sourceVertexKey should go into targetVertexKey
        public Dictionary<string, string> Loops
        {
            get { return this.Nodes.Where(node => !string.IsNullOrWhiteSpace(node.InputNodeKey)).ToDictionary(node => node.Key, node => node.InputNodeKey); }

            set { this.loops = value; }
        }

        public static void Decrypt(string path)
        {
            var pathStream = new FileStream(path, FileMode.OpenOrCreate);
            var rMCrypto = new RijndaelManaged();

            byte[] key =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
            };

            byte[] iv =
            {
                0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
            };

            var cryptStream = new CryptoStream(pathStream,
                rMCrypto.CreateDecryptor(key, iv),
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
            catch (CryptographicException)
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
            var network = new Network
            {
                FileName = path
            };

            network.ReadFile(path);

            var currentNodeKey = "";
            var networkFileLocation = NetworkFileLocation.Root;
            var line = "";
            var streamReader = new StreamReader(path);

            while ((line = streamReader.ReadLine()) != null)
            {
                if (networkFileLocation == NetworkFileLocation.Root)
                {
                    if (line.Equals("net"))
                    {
                        // Enter the header 'net' section
                        networkFileLocation = NetworkFileLocation.Header;
                    }

                    else if (line.StartsWith("node"))
                    {
                        // Enter the node section
                        networkFileLocation = NetworkFileLocation.Node;

                        currentNodeKey = line.Split(" ".ToArray(), 2).ToList()[1];

                        network.Nodes.Add(new NetworkNode
                        {
                            Key = currentNodeKey
                        });
                    }

                    else if (line.StartsWith("potential"))
                    {
                        // Enter the potential section
                        networkFileLocation = NetworkFileLocation.Potential;
                        currentNodeKey = line.Trim().Split("()| ".ToArray(), StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                    }
                }

                else if (networkFileLocation == NetworkFileLocation.Header)
                {
                    if (line.Equals("{"))
                    {
                        // do nothing
                    }
                    else if (line.Equals("}"))
                    {
                        // Exit to root
                        networkFileLocation = NetworkFileLocation.Root;
                    }
                    else
                    {
                        var parts = line.Split("=;".ToArray(), 2, StringSplitOptions.RemoveEmptyEntries).ToList();
                        network.Properties[parts[0].Trim()] = new string(parts[1].Trim().AllButLast().ToArray());
                    }
                }

                else if (networkFileLocation == NetworkFileLocation.Node)
                {
                    if (line.Equals("{"))
                    {
                        // do nothing
                    }
                    else if (line.Equals("}"))
                    {
                        // Exit to root
                        networkFileLocation = NetworkFileLocation.Root;
                    }
                    else
                    {
                        var parts = line.Split("=;".ToArray(), 2, StringSplitOptions.RemoveEmptyEntries).ToList();
                        network.Nodes[currentNodeKey].Properties[parts[0].Trim()] = new string(parts[1].Trim().AllButLast().ToArray());
                    }
                }

                else if (networkFileLocation == NetworkFileLocation.Potential)
                {
                    if (line.Equals("{"))
                    {
                        // do nothing
                    }
                    else if (line.Contains("model_data"))
                    {
                        network.Nodes[currentNodeKey].Expression = line.Split("=;".ToArray())[1].Trim().Dequote('(', ')').Trim();
                    }
                    else if (line.Equals("}"))
                    {
                        networkFileLocation = NetworkFileLocation.Root;
                    }
                }
            }

            // Parse Children
            foreach (var node in network.Nodes)
            {
                foreach (var childHandle in network.GetChildren(node.Key))
                {
                    var childKey = network.GetNodeId(childHandle);
                    node.Children.Add(network.Nodes[childKey]);
                }
            }

            network.UpdateBeliefs();
            network.InitialBelief = network.GetBeliefs();
            return network;
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

                foreach (var line in this.footer)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }

        public double[] GetBelief(string vertexKey)
        {
            return this.GetNodeValue(vertexKey);
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

        public string GetBeliefsJson()
        {
            return this.GetBeliefs().ToJson();
        }

        public double[] GetEvidence(string vertexKey)
        {
            var softEvidence = this.GetSoftEvidence(vertexKey);
            var evidenceIndex = this.IsEvidence(vertexKey) ? this.GetHardEvidence(vertexKey) : -1;

            if (softEvidence != null)
            {
                return softEvidence;
            }

            if (evidenceIndex >= 0)
            {
                var evidence = new double[this.GetOutcomeCount(vertexKey)];
                evidence[evidenceIndex] = 1;
                return evidence;
            }

            throw new SmileException("No evidence is set on node " + vertexKey);
        }

        public Dict<string, VertexEvidence> GetEvidences()
        {
            var graphData = new Dict<string, VertexEvidence>();

            foreach (var vertexKey in this.GetAllNodeIds())
            {
                graphData[vertexKey].Value = this.GetEvidence(vertexKey);
            }

            return graphData;
        }

        public string GetEvidencesJson()
        {
            return this.GetEvidences().ToJson();
        }

        public double[] GetIntervals(string vertexKey)
        {
            var states = this.Nodes[vertexKey].States;
            return states.Select(state => state.Min).Concat(states.Last().Max.Yield()).ToArray();
        }

        public double GetMean(string vertexKey)
        {
            return this.Nodes[vertexKey].Mean(this.GetNodeValue(vertexKey));
        }

        public Dict<string, string, double> GetSensitivity(string targetVertexKey, Func<NetworkNode, double[], double[], double> statisticFunc)
        {
            var targetVertex = this.Nodes[targetVertexKey];

            // Dictionary<sourceVertexKey, sourceStateKey, targetValue>
            var value = new Dict<string, string, double>();

            // Collect vertices to ignore
            var verticesToIgnore = new List<NetworkNode>
            {
                targetVertex
            };

            verticesToIgnore.AddRange(this.Nodes.Where(vertex => this.IsEvidence(vertex.Key)));

            foreach (var sourceVertex in this.Nodes.Except(verticesToIgnore))
            {
                foreach (var sourceState in sourceVertex.States)
                {
                    try
                    {
                        var stateIndex = sourceVertex.States.IndexOf(sourceState);
                        this.SetHardEvidence(sourceVertex.Key, stateIndex);

                        this.UpdateBeliefs();

                        var beliefs = this.GetBeliefs();

                        this.ClearEvidence(sourceVertex.Key);

                        var targetVertexValue = beliefs[targetVertex.Key];

                        value[sourceVertex.Key][sourceState.Key] = statisticFunc(targetVertex, targetVertexValue, targetVertex.InitialBelief);
                    }
                    catch (SmileException exp)
                    {
                        Console.WriteLine(exp.Message);
                        value[sourceVertex.Key][sourceState.Key] = double.NaN;
                    }
                }
            }

            return value;
        }

        public Dict<string, string, double> GetSensitivity(string targetVertexKey, IVertexValueComputer computer)
        {
            return this.GetSensitivity(targetVertexKey, computer.Compute);
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
            this.ClearEvidence();

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
            this.ClearEvidence();

            foreach (var vertex in this.Nodes)
            {
                if (graphData.ContainsKey(vertex.Key) && graphData[vertex.Key].Value != null && graphData[vertex.Key].Value.Sum() > 0)
                {
                    // Hugin allows a node to have a single state. Smile doesn't. So Smile adds a state
                    // to the node silently. This causes the length of the evidence array to not match
                    // the number of states. In this case, we throw an exception.
                    var outcomeCount = this.GetOutcomeCount(vertex.Key);

                    if (outcomeCount != graphData[vertex.Key].Value.Length)
                    {
                        throw new InvalidEvidenceException("The length of evidence array does not match the number of states for node: " + vertex.Key) { VertexKey = vertex.Key };
                    }

                    this.SetSoftEvidence(vertex.Key, graphData[vertex.Key].Value);
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

        public void SetEvidence(string vertexKey, string evidenceString)
        {
            var vertexEvidence = this.Nodes[vertexKey].States.ParseEvidenceString(evidenceString);

            if (vertexEvidence.Type == VertexEvidenceType.Null)
            {
                return;
            }

            if (vertexEvidence.Type == VertexEvidenceType.Invalid)
            {
                throw new InvalidEvidenceException("The evidence (" + evidenceString + ") is invalid for node " + vertexKey);
            }

            this.SetSoftEvidence(vertexKey, vertexEvidence.Value);
        }

        public void SetEvidence(string vertexKey, double value)
        {
            this.SetEvidence(vertexKey, value.ToString());
        }

        public void Write(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                // Write network header
                writer.WriteLine("net");
                writer.WriteLine("{");

                foreach (var prop in this.Properties)
                {
                    writer.WriteLine("\t{0} = {1};", prop.Key, prop.Value);
                }

                writer.WriteLine("}");

                // Write node definitions
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

                // Write node CPTs
                foreach (var node in this.Nodes)
                {
                    // Write the potential line
                    writer.Write("potential ({0}", node.Key);

                    var parentNodeKeys = this.GetParentIds(node.Key);

                    if (parentNodeKeys.Any())
                    {
                        writer.Write(" |");

                        foreach (var parentNodeKey in parentNodeKeys)
                        {
                            writer.Write(" {0}", parentNodeKey);
                        }
                    }

                    writer.WriteLine(")");

                    writer.WriteLine("{");

                    if (node.Expression != null)
                    {
                        writer.WriteLine("\tmodel_nodes = ();");
                        writer.WriteLine("\tmodel_data = ({0});", node.Expression);
                    }

                    writer.Write("\tdata = ");

                    writer.WriteLine(this.FormPotentialString(node, this.GetTable(node.Key), parentNodeKeys) + ";");

                    writer.WriteLine("}");

                    writer.WriteLine();
                }
            }
        }

        public void WriteBeliefs(string filePath)
        {
            if (Path.GetExtension(filePath) != BeliefFileExtension)
            {
                filePath = filePath + "." + BeliefFileExtension;
            }

            this.GetBeliefs().WriteJson(filePath);
        }

        public void WriteEvidences(string filePath)
        {
            if (Path.GetExtension(filePath) != EvidenceFileExtension)
            {
                filePath = filePath + "." + EvidenceFileExtension;
            }

            this.GetEvidences().WriteJson(filePath);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string FormPotentialString(NetworkNode node, double[,] table, string[] parentNodeKeys = null, int row = 0, int col = 0, int level = 0)
        {
            var potentialString = "";

            parentNodeKeys = parentNodeKeys ?? this.GetParentIds(node.Key);

            if (!parentNodeKeys.Any())
            {
                potentialString += "(";

                foreach (var state in node.States)
                {
                    potentialString += table[row++, col];

                    if (state != node.States.Last())
                    {
                        potentialString += " ";
                    }
                }

                potentialString += ")";
            }

            else
            {
                var currentNodeKey = parentNodeKeys.First();
                var currentNodeStates = this.Nodes[currentNodeKey].States;

                potentialString += "(";

                foreach (var state in currentNodeStates)
                {
                    potentialString += this.FormPotentialString(node, table, parentNodeKeys.Except(currentNodeKey).ToArray(), row, col++, level + 1);

                    if (state != currentNodeStates.Last())
                    {
                        potentialString += "\n\t\t    " + String.Concat(Enumerable.Repeat(" ", level));
                    }
                }

                potentialString += ")";
            }

            return potentialString;
        }

        private enum NetworkFileLocation
        {
            Header,
            Node,
            Potential,
            Root,
        };

        public event PropertyChangedEventHandler PropertyChanged;
    }
}