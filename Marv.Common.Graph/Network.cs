using System.Collections.Generic;

namespace Marv
{
    public class Network : Smile.Network
    {
        public static Network Read(string fileName)
        {
            var network = new Network();
            network.ReadFile(fileName);
            return network;
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

        public void Run(Dict<string, VertexData> graphData)
        {
            this.ClearAllEvidence();

            foreach (var vertexKey in graphData.Keys)
            {
                if (graphData[vertexKey].Evidence != null)
                {
                    this.SetSoftEvidence(vertexKey, graphData[vertexKey].Evidence);
                }
            }

            this.UpdateBeliefs();

            var beliefs = this.GetBeliefs();

            foreach (var vertexKey in beliefs.Keys)
            {
                graphData[vertexKey].Beliefs = beliefs[vertexKey];
            }
        }

        public void Run(Dict<int, string, VertexData> sectionData)
        {
            foreach (var year in sectionData.Keys)
            {
                this.Run(sectionData[year]);
            }
        }
    }
}