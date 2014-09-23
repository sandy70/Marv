using System.Collections.Generic;
using System.Linq;
using Smile;

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

        public void Run(Dict<int, string, VertexData> sectionData, Dictionary<string, string> loops = null)
        {
            var firstYear = sectionData.Keys.First();
            var lastYear = firstYear;

            foreach (var year in sectionData.Keys)
            {
                if (loops != null)
                {
                    if (year != firstYear)
                    {
                        foreach (var targetVertexKey in loops.Keys)
                        {
                            var sourceVertexKey = loops[targetVertexKey];
                            sectionData[year][targetVertexKey].Evidence = sectionData[lastYear][sourceVertexKey].Belief;
                        }
                    }
                }

                this.Run(sectionData[year]);

                lastYear = year;
            }
        }

        public void WriteData(string fileName)
        {
            this.GetData().WriteJson(fileName);
        }
    }
}