﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Marv.Common.Distributions;
using Marv.Common.Types;
using Newtonsoft.Json;
using Smile;

namespace Marv.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Network : Smile.Network, INotifyPropertyChanged
    {
        private const string BeliefFileExtension = "marv-networkbelief";

        public readonly Dictionary<string, string> Properties = new Dictionary<string, string>();

        [JsonProperty] public readonly KeyedCollection<NetworkVertex> Vertices = new KeyedCollection<NetworkVertex>();

        private string fileName;

        [JsonProperty]
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
            get { return this.Vertices.ToDict(node => node.Key, node => node.InitialBelief); }

            set
            {
                foreach (var vertex in this.Vertices)
                {
                    if (value.ContainsKey(vertex.Key))
                    {
                        vertex.InitialBelief = value[vertex.Key];
                    }
                }

                this.RaisePropertyChanged();
            }
        }

        // Dictionary<targetNodeKey, sourceVertexKey>
        // Beliefs from sourceVertexKey should go into targetNodeKey
        public Dictionary<string, string> Loops
        {
            get { return this.Vertices.Where(node => !string.IsNullOrWhiteSpace(node.InputNodeKey)).ToDictionary(node => node.Key, node => node.InputNodeKey); }
        }

        public static Network Read(string filePath)
        {
            if (Path.GetExtension(filePath) == ".enet")
            {
                return ReadEncrypted(filePath);
            }

            return ReadText(filePath);
        }

        private static Network ReadEncrypted(string filePath)
        {
            byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            var cryptoStream = new CryptoStream(new FileStream(filePath, FileMode.Open), (new RijndaelManaged()).CreateDecryptor(key, iv), CryptoStreamMode.Read);

            var unencryptedFilePath = Path.Combine(Path.GetDirectoryName(filePath), "temp_" + Path.GetFileNameWithoutExtension(filePath) + ".net");

            using (var streamReader = new StreamReader(cryptoStream))
            {
                using (var streamWriter = new StreamWriter(unencryptedFilePath))
                {
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        streamWriter.WriteLine(line);
                    }
                }
            }

            var network = Read(unencryptedFilePath);
            network.FileName = filePath;

            File.Delete(unencryptedFilePath);

            return network;
        }

        private static Network ReadText(string path)
        {
            var network = new Network
            {
                FileName = path
            };

            network.ReadFile(path);

            var currentNodeKey = "";
            var networkFileLocation = NetworkFileLocation.Root;

            using (var streamReader = new StreamReader(path))
            {
                string line;
                var cumulativeLine = "";

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

                            network.Vertices.Add(new NetworkVertex
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
                            network.Vertices[currentNodeKey].Properties[parts[0].Trim()] = new string(parts[1].Trim().AllButLast().ToArray());
                        }
                    }

                    else if (networkFileLocation == NetworkFileLocation.Potential)
                    {
                        if (line.Equals("{"))
                        {
                            // do nothing
                        }
                        else if (line.Contains("model_nodes") && line.Contains(";"))
                        {
                            network.Vertices[currentNodeKey].ModelNodes = line.Split("=;".ToArray())[1].Trim().Dequote('(', ')').Trim();
                        }
                        else if (line.Contains("model_data") && line.Contains(";"))
                        {
                            network.Vertices[currentNodeKey].Expression = line.Split("=;".ToArray())[1].Trim().Dequote('(', ')').Trim();
                        }
                        else if (line.Contains("model_data"))
                        {
                            cumulativeLine = line;
                            networkFileLocation = NetworkFileLocation.ModelData;
                        }
                        else if (line.Equals("}"))
                        {
                            networkFileLocation = NetworkFileLocation.Root;
                        }
                    }

                    else if (networkFileLocation == NetworkFileLocation.ModelData)
                    {
                        cumulativeLine += line;

                        if (line.Contains(";"))
                        {
                            network.Vertices[currentNodeKey].Expression = cumulativeLine.Split("=;".ToArray())[1].Trim().Dequote('(', ')').Trim();
                            networkFileLocation = NetworkFileLocation.Potential;
                        }
                    }
                }
            }

            // Parse Children
            foreach (var node in network.Vertices)
            {
                foreach (var childHandle in network.GetChildren(node.Key))
                {
                    var childKey = network.GetNodeId(childHandle);
                    node.Children.Add(network.Vertices[childKey]);
                }
            }

            return network;
        }

        public void AddGroup(string nodeKey, string group)
        {
            this.Vertices[nodeKey].Groups.AddUnique(group);
        }

        public void ClearGroups()
        {
            foreach (var node in this.Vertices)
            {
                node.Groups.Clear();
            }
        }

        public void ClearGroups(string nodeKey)
        {
            this.Vertices[nodeKey].Groups.Clear();
        }

        public double[] GetBeliefs(string vertexKey)
        {
            return this.GetNodeValue(vertexKey);
        }

        public Dict<string, double[]> GetBeliefs()
        {
            this.UpdateBeliefs();

            var nodeBelief = new Dict<string, double[]>();

            foreach (var nodeKey in this.GetAllNodeIds())
            {
                try
                {
                    nodeBelief[nodeKey] = this.GetNodeValue(nodeKey);
                }
                catch (SmileException)
                {
                    nodeBelief[nodeKey] = this.GetEvidence(nodeKey);
                }
            }

            return nodeBelief;
        }

        public double[] GetEvidence(string nodeKey)
        {
            var softEvidence = this.GetSoftEvidence(nodeKey);
            var evidenceIndex = this.IsEvidence(nodeKey) ? this.GetHardEvidence(nodeKey) : -1;

            if (softEvidence != null)
            {
                return softEvidence;
            }

            if (evidenceIndex >= 0)
            {
                var evidence = new double[this.GetOutcomeCount(nodeKey)];
                evidence[evidenceIndex] = 1;
                return evidence;
            }

            throw new SmileException("No evidence is set on node " + nodeKey);
        }

        public Dict<string, VertexEvidence> GetEvidences()
        {
            var graphData = new Dict<string, VertexEvidence>();

            foreach (var vertexKey in this.GetAllNodeIds())
            {
                try
                {
                    var evidence = this.GetEvidence(vertexKey);
                    graphData[vertexKey].Value = evidence;
                }
                catch (SmileException)
                {
                    // do nothing
                }
            }

            return graphData;
        }

        public double GetMean(string vertexKey)
        {
            return this.Vertices[vertexKey].Mean(this.GetNodeValue(vertexKey));
        }

        public NetworkVertex GetNode(string nodeKey)
        {
            return this.Vertices[nodeKey];
        }

        public IEnumerable<string> GetNodeKeys()
        {
            return this.Vertices.Select(node => node.Key);
        }

        public Dict<string, double[]> GetSensitivity(string targetNodeKey, Func<NetworkVertex, double[], double[], double> statisticFunc)
        {
            var targetVertex = this.Vertices[targetNodeKey];

            // Dictionary<sourceVertexKey, sourceStateKey, targetValue>
            var value = new Dict<string, double[]>();

            double[] targetNodeEvidence = null;

            if (this.IsEvidenceSet(targetNodeKey))
            {
                targetNodeEvidence = this.GetEvidence(targetNodeKey);
                this.ClearEvidence(targetNodeKey);
            }

            foreach (var sourceVertex in this.Vertices.Except(targetVertex))
            {
                double[] originalEvidence = null;
                value[sourceVertex.Key] = new double[sourceVertex.States.Count];

                if (this.IsEvidenceSet(sourceVertex.Key))
                {
                    originalEvidence = this.GetEvidence(sourceVertex.Key);
                    this.ClearEvidence(sourceVertex.Key);
                }

                foreach (var sourceState in sourceVertex.States)
                {
                    var stateIndex = sourceVertex.States.IndexOf(sourceState);
                    try
                    {
                        var evidence = new double[sourceVertex.States.Count];
                        evidence[stateIndex] = 1;

                        this.SetEvidence(sourceVertex.Key, evidence);

                        this.UpdateBeliefs();

                        var beliefs = this.GetBeliefs();

                        this.ClearEvidence(sourceVertex.Key);

                        var targetVertexValue = beliefs[targetVertex.Key];

                        value[sourceVertex.Key][stateIndex] = statisticFunc(targetVertex, targetVertexValue, targetVertex.InitialBelief);

                        if (originalEvidence != null)
                        {
                            value[sourceVertex.Key][stateIndex] *= originalEvidence[stateIndex];
                        }
                    }
                    catch (SmileException exp)
                    {
                        Console.WriteLine(exp.Message);
                        Console.WriteLine("Node: {0}, State: {1}", sourceVertex.Key, sourceState.Key);
                        value[sourceVertex.Key][stateIndex] = double.NaN;
                    }
                }

                // Set the stored evidence
                if (originalEvidence != null)
                {
                    this.SetEvidence(sourceVertex.Key, originalEvidence);
                }
            }

            if (targetNodeEvidence != null)
            {
                this.SetEvidence(targetNodeKey, targetNodeEvidence);
            }

            return value;
        }

        public Dict<string, double[]> GetSensitivity(string targetNodeKey, IVertexValueComputer computer)
        {
            return this.GetSensitivity(targetNodeKey, computer.Compute);
        }

        public string[] GetStateKeys(string nodeKey)
        {
            return this.Vertices[nodeKey].States.Select(state => state.Key).ToArray();
        }

        public VertexType GetType(string nodeKey)
        {
            return this.Vertices[nodeKey].Type;
        }

        public bool IsEvidenceSet(string nodeKey)
        {
            return this.IsEvidence(nodeKey) || this.GetSoftEvidence(nodeKey) != null;
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

        public void RemoveGroup(string group)
        {
            foreach (var node in this.Vertices)
            {
                node.Groups.Remove(group);

                if (node.HeaderOfGroup == group)
                {
                    node.HeaderOfGroup = null;
                }
            }
        }

        public void RemoveGroup(string nodeKey, string group)
        {
            this.Vertices[nodeKey].Groups.Remove(group);
        }

        public void RenameGroup(string oldGroup, string newGroup)
        {
            foreach (var node in this.Vertices)
            {
                if (node.Groups.Contains(oldGroup))
                {
                    node.Groups.Replace(oldGroup, newGroup);
                }

                if (node.HeaderOfGroup == oldGroup)
                {
                    node.HeaderOfGroup = newGroup;
                }
            }
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

        public Dict<string, double[]> Run(Dict<string, VertexEvidence> vertexEvidences)
        {
            this.ClearEvidence();

            foreach (var vertex in this.Vertices)
            {
                if (vertexEvidences.ContainsKey(vertex.Key)
                    && vertexEvidences[vertex.Key] != null
                    && vertexEvidences[vertex.Key].Type != VertexEvidenceType.Null
                    && vertexEvidences[vertex.Key].Value != null
                    && vertexEvidences[vertex.Key].Value.Sum() > 0)
                {
                    // Hugin allows a node to have a single state. Smile doesn't. So Smile adds a state
                    // to the node silently. This causes the length of the evidence array to not match
                    // the number of states. In this case, we throw an exception.
                    var outcomeCount = this.GetOutcomeCount(vertex.Key);

                    if (outcomeCount != vertexEvidences[vertex.Key].Value.Length)
                    {
                        throw new InvalidEvidenceException("The length of evidence array does not match the number of states for node: " + vertex.Key) { VertexKey = vertex.Key };
                    }

                    this.SetSoftEvidence(vertex.Key, vertexEvidences[vertex.Key].Value);
                }
            }

            try
            {
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

        public Dict<string, double[]> Run(string nodeKey, string evidence)
        {
            this.ClearEvidence();

            this.SetEvidence(nodeKey, evidence);

            this.UpdateBeliefs();

            return this.GetBeliefs();
        }

        public Dict<string, double[]> Run(Dict<string, string> evidenceStrings)
        {
            this.ClearEvidence();

            foreach (var kvp in evidenceStrings)
            {
                this.SetEvidence(kvp.Key, kvp.Value);
            }

            this.UpdateBeliefs();

            return this.GetBeliefs();
        }

        public Dict<DateTime, string, double[]> Run(Dict<DateTime, string, VertexEvidence> evidences)
        {
            var beliefs = new Dict<DateTime, string, double[]>();
            var lastDateTime = evidences.Keys[0];

            foreach (var dateTime in evidences.Keys)
            {
                if (lastDateTime == dateTime)
                {
                    beliefs[dateTime] = this.Run(evidences[dateTime]);
                }
                else
                {
                    foreach (var dstNodeKey in this.Loops.Keys)
                    {
                        evidences[dateTime][dstNodeKey] = new VertexEvidence
                        {
                            Type = VertexEvidenceType.Distribution,
                            Value = beliefs[lastDateTime][this.Loops[dstNodeKey]]
                        };
                    }

                    beliefs[dateTime] = this.Run(evidences[dateTime]);
                }

                lastDateTime = dateTime;
            }

            return beliefs;
        }

        public void SetEvidence(string nodeKey, string evidenceString)
        {
            if (!this.Vertices.ContainsKey(nodeKey))
            {
                throw new SmileException("The node '" + nodeKey + "' does not exist in the network.");
            }

            var vertexEvidence = this.Vertices[nodeKey].States.ParseEvidenceString(evidenceString);

            if (vertexEvidence.Type == VertexEvidenceType.Null)
            {
                return;
            }

            if (vertexEvidence.Type == VertexEvidenceType.Invalid)
            {
                throw new InvalidEvidenceException("The evidence (" + evidenceString + ") is invalid for node " + nodeKey);
            }

            this.SetSoftEvidence(nodeKey, vertexEvidence.Value);
        }

        public void SetEvidence(string nodeKey, double value)
        {
            this.SetEvidence(nodeKey, value.ToString());
        }

        public void SetEvidence(string nodeKey, double[] evidence)
        {
            this.SetSoftEvidence(nodeKey, evidence);
        }

        public void SetEvidence(Dict<string, VertexEvidence> vertexEvidences)
        {
            foreach (var nodeKey in vertexEvidences.Keys)
            {
                this.SetEvidence(nodeKey, vertexEvidences[nodeKey].Value);
            }
        }

        public void SetEvidence(SectionEvidence sectionEvidence, int year)
        {
            this.SetEvidence(sectionEvidence[year]);
        }

        public void SetHeader(string nodeKey, string headerOfGroup)
        {
            this.Vertices[nodeKey].HeaderOfGroup = headerOfGroup;
        }

        public void SetNormalDistribution(string nodeKey, double mean, double variance)
        {
            var distribution = this.Vertices[nodeKey].States.ParseEvidence(new NormalDistribution(mean, variance));
            this.SetSoftEvidence(nodeKey, distribution);
        }

        public void SetTriangularDistribution(string nodeKey, double min, double mode, double max)
        {
            var distribution = this.Vertices[nodeKey].States.ParseEvidence(new TriangularDistribution(min, mode, max));
            this.SetSoftEvidence(nodeKey, distribution);
        }

        public void SetUniformDistribution(string nodeKey, double min, double max)
        {
            var distribution = this.Vertices[nodeKey].States.ParseEvidence(new UniformDistribution(min, max));
            this.SetSoftEvidence(nodeKey, distribution);
        }

        public void UpdateInitialBeliefs()
        {
            this.ClearEvidence();
            this.UpdateBeliefs();

            foreach (var node in this.Vertices)
            {
                node.InitialBelief = this.GetBeliefs(node.Key);
            }
        }

        public void Write()
        {
            this.Write(this.FileName);
        }

        public void Write(string filePath)
        {
            if (Path.GetExtension(filePath) == "enet")
            {
                this.WriteEncrypted(filePath);
            }
            else
            {
                this.Write(new StreamWriter(Path.ChangeExtension(filePath, "net")));
            }
        }

        public void Write(StreamWriter streamWriter)
        {
            using (streamWriter)
            {
                // Write network header
                streamWriter.WriteLine("net");
                streamWriter.WriteLine("{");

                foreach (var prop in this.Properties)
                {
                    streamWriter.WriteLine("\t{0} = {1};", prop.Key, prop.Value);
                }

                streamWriter.WriteLine("}");

                // Write node definitions
                foreach (var node in this.Vertices)
                {
                    node.Properties["groups"] = node.Groups.String().Enquote();
                    node.Properties["headerofgroup"] = node.HeaderOfGroup.Enquote();

                    streamWriter.WriteLine();
                    streamWriter.WriteLine("node {0}", node.Key);
                    streamWriter.WriteLine("{");

                    foreach (var prop in node.Properties)
                    {
                        streamWriter.WriteLine("\t{0} = {1};", prop.Key, prop.Value);
                    }

                    streamWriter.WriteLine("}");
                }

                streamWriter.WriteLine();

                // Write node CPTs
                foreach (var node in this.Vertices)
                {
                    // Write the potential line
                    streamWriter.Write("potential ({0}", node.Key);

                    var parentNodeKeys = this.GetParentIds(node.Key);

                    if (parentNodeKeys.Any())
                    {
                        streamWriter.Write(" |");

                        foreach (var parentNodeKey in parentNodeKeys)
                        {
                            streamWriter.Write(" {0}", parentNodeKey);
                        }
                    }

                    streamWriter.WriteLine(")");

                    streamWriter.WriteLine("{");

                    if (node.Expression != null)
                    {
                        streamWriter.WriteLine("\tmodel_nodes = ({0});", node.ModelNodes);
                        streamWriter.WriteLine("\tmodel_data = ({0});", node.Expression);
                    }

                    streamWriter.Write("\tdata = ");

                    streamWriter.WriteLine(this.FormPotentialString(node, this.GetTable(node.Key), parentNodeKeys) + ";");

                    streamWriter.WriteLine("}");

                    streamWriter.WriteLine();
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

        public void WriteEncrypted(string filePath)
        {
            byte[] key = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };
            byte[] iv = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16 };

            var cryptoStream = new CryptoStream(new FileStream(filePath, FileMode.Create), (new RijndaelManaged()).CreateEncryptor(key, iv), CryptoStreamMode.Write);

            this.Write(new StreamWriter(cryptoStream));
        }

        public void WriteEvidences(string filePath)
        {
            var vertexEvidences = this.GetEvidences();

            if (Path.GetExtension(filePath) == ".hcs")
            {
                vertexEvidences.WriteHcs(filePath);
                return;
            }

            vertexEvidences.WriteJson(filePath);
        }

        private string FormPotentialString(NetworkVertex networkVertex, double[,] table, string[] parentNodeKeys, int row = 0, int col = 0, int level = 0)
        {
            var potentialString = "";

            parentNodeKeys = parentNodeKeys ?? this.GetParentIds(networkVertex.Key);

            if (!parentNodeKeys.Any())
            {
                potentialString += "(";

                foreach (var state in networkVertex.States)
                {
                    potentialString += table[row++, col];

                    if (state != networkVertex.States.Last())
                    {
                        potentialString += " ";
                    }
                }

                potentialString += ")";
            }

            else
            {
                var currentNodeKey = parentNodeKeys.First();
                var currentNodeStates = this.Vertices[currentNodeKey].States;
                var nCols = parentNodeKeys.Except(currentNodeKey).Aggregate(1, (current, parentNodeKey) => current * this.Vertices[parentNodeKey].States.Count);

                potentialString += "(";

                foreach (var state in currentNodeStates)
                {
                    potentialString += this.FormPotentialString(networkVertex, table, parentNodeKeys.Except(currentNodeKey).ToArray(), row, col, level + 1);

                    col += nCols;

                    if (state != currentNodeStates.Last())
                    {
                        potentialString += "\n\t\t    " + String.Concat(Enumerable.Repeat(" ", level));
                    }
                }

                potentialString += ")";
            }

            return potentialString;
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private enum NetworkFileLocation
        {
            Header,
            ModelData,
            Node,
            Potential,
            Root,
        };
    }
}