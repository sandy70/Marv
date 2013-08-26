using System.Collections.Generic;
using System.Linq;

namespace LibNetwork
{
    public class BnInputStore
    {
        private Dictionary<BnInputType, Dictionary<int, List<BnVertexInput>>> appInputs = new Dictionary<BnInputType, Dictionary<int, List<BnVertexInput>>>();

        public List<BnVertexInput> GetGraphInput(BnInputType inputType, int year)
        {
            var modelInput = this.GetModelInput(inputType);

            var graphInput = modelInput.SingleOrDefault(x => x.Key == year).Value;

            if (graphInput == null)
            {
                graphInput = new List<BnVertexInput>();
                modelInput.Add(year, graphInput);
            }

            return graphInput;
        }

        public Dictionary<int, List<BnVertexInput>> GetModelInput(BnInputType inputType)
        {
            var appInput = this.appInputs.SingleOrDefault(x => x.Key == inputType).Value;

            if (appInput == null)
            {
                appInput = new Dictionary<int, List<BnVertexInput>>();
                this.appInputs.Add(inputType, appInput);
            }

            return appInput;
        }

        public BnVertexInput GetVertexInput(BnInputType inputType, int year, string key)
        {
            var graphInput = this.GetGraphInput(inputType, year);

            var vertexInput = graphInput.SingleOrDefault(x => x.Key.Equals(key));

            if (vertexInput == null)
            {
                vertexInput = new BnVertexInput();
                graphInput.Add(vertexInput);
            }

            return vertexInput;
        }

        public void RemoveVertexInput(BnInputType inputType, int year, string key)
        {
            var graphInput = this.GetGraphInput(inputType, year);
            graphInput.RemoveAll(x => x.Key.Equals(key));
        }
    }
}