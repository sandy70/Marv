using Smile;
using System.Collections.Generic;
using System.Linq;

namespace LibBn
{
    public class BnUpdater
    {
        public List<BnVertexValue> GetVertexValues(string fileName, List<BnVertexInput> defaultInputs, List<BnVertexInput> userInputs, List<BnVertexValue> lastYearVertexValues)
        {
            Network network = new Network();
            network.ReadFile(fileName);

            foreach (var input in userInputs)
            {
                this.EnterEvidence(network, input, lastYearVertexValues);
            }

            foreach (var input in defaultInputs)
            {
                if (!userInputs.Exists(x => x.Key.Equals(input.Key)))
                {
                    this.EnterEvidence(network, input, lastYearVertexValues);
                }
            }

            network.UpdateBeliefs();

            var vertexValues = new List<BnVertexValue>();

            foreach (var nodeId in network.GetAllNodeIds())
            {
                var vertexValue = new BnVertexValue();
                vertexValue.IsEvidenceEntered = userInputs.Exists(x => x.Key.Equals(nodeId));
                vertexValue.Key = nodeId;

                double[] values = network.GetNodeValue(nodeId);
                var nStates = values.Count();

                for (int s = 0; s < nStates; s++)
                {
                    vertexValue.States.Add(values[s]);
                }

                vertexValues.Add(vertexValue);
            }

            return vertexValues;
        }

        private void EnterEvidence(Network network, BnVertexInput input, List<BnVertexValue> lastYearVertexValues)
        {
            int nodeHandle = network.GetNode(input.Key);

            if (input.InputType == VertexInputType.StateSelected)
            {
                network.SetEvidence(nodeHandle, input.SelectedStateIndex);
            }
            else if (input.InputType == VertexInputType.SoftEvidence)
            {
                int nStates = network.GetOutcomeCount(nodeHandle);
                double[] evidence = new double[nStates];

                for (int s = 0; s < nStates; s++)
                {
                    evidence[s] = input.Evidence[s];
                }

                network.SetSoftEvidence(nodeHandle, evidence);
            }
            else
            {
                if (lastYearVertexValues == null)
                {
                    network.SetEvidence(nodeHandle, input.SelectedStateIndex);
                }
                else
                {
                    var lastVertex = lastYearVertexValues.Single(x => x.Key.Equals(input.InputVertexKey));

                    int nStates = network.GetOutcomeCount(nodeHandle);
                    double[] evidence = new double[nStates];

                    for (int s = 0; s < nStates; s++)
                    {
                        evidence[s] = lastVertex.States[s];
                    }

                    network.SetSoftEvidence(nodeHandle, evidence);
                }
            }
        }
    }
}