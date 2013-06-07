using System.Collections.Generic;

namespace LibBn
{
    public class BnVertexInput
    {
        public Dictionary<string, double> aEvidence = new Dictionary<string, double>();
        public List<double> Evidence = new List<double>();
        public VertexInputType InputType;
        public string InputVertexKey;
        public string Key;
        public int SelectedStateIndex;

        public void FillFrom(Vertex vertex)
        {
            int selectedStateIndex = vertex.GetSelectedStateIndex();

            this.Key = vertex.Key;

            if (selectedStateIndex >= 0)
            {
                this.InputType = VertexInputType.StateSelected;
                this.SelectedStateIndex = selectedStateIndex;
            }
            else
            {
                this.InputType = VertexInputType.SoftEvidence;
                this.Evidence = new List<double>();

                foreach (var state in vertex.States)
                {
                    this.Evidence.Add(state.Value);
                }
            }
        }
    }
}