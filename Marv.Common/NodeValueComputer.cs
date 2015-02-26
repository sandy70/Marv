using System.Linq;

namespace Marv.Common
{
    public interface INodeValueComputer
    {
        double Compute(Node node, double[] newValue, double[] oldValue);
    }

    public class NodeEntropyComputer : INodeValueComputer
    {
        public double Compute(Node node, double[] newValue, double[] oldValue)
        {
            return node.Entropy(newValue, oldValue);
        }
    }

    public class NodeEntropyDifferenceComputer : INodeValueComputer
    {
        public double Compute(Node node, double[] newValue, double[] oldValue)
        {
            return node.EntropyDifference(newValue, oldValue);
        }
    }

    public class NodeMeanComputer : INodeValueComputer
    {
        public double Compute(Node node, double[] newValue, double[] oldValue)
        {
            return node.Mean(newValue, oldValue);
        }
    }

    public class NodeMeanDifferenceComputer : INodeValueComputer
    {
        public double Compute(Node node, double[] newValue, double[] oldValue)
        {
            return node.MeanDifference(newValue, oldValue);
        }
    }

    public class NodeStandardDeviationComputer : INodeValueComputer
    {
        public double Compute(Node node, double[] newValue, double[] oldValue)
        {
            return node.StandardDeviation(newValue, oldValue);
        }
    }

    public class NodeStateComputer : INodeValueComputer
    {
        private readonly int stateIndex;

        public NodeStateComputer(int i)
        {
            this.stateIndex = i;
        }

        public double Compute(Node node, double[] newValue, double[] oldValue)
        {
            return newValue[this.stateIndex];
        }
    }

    public class NodeStateDifferenceComputer : INodeValueComputer
    {
        private readonly int stateIndex;

        public NodeStateDifferenceComputer(int i)
        {
            this.stateIndex = i;
        }

        public double Compute(Node node, double[] newValue, double[] oldValue)
        {
            return newValue[this.stateIndex] - oldValue[this.stateIndex];
        }
    }

    public class NodePercentileComputer : INodeValueComputer
    {
        private readonly double percentile;

        public NodePercentileComputer(double thePercentile)
        {
            this.percentile = thePercentile;
        }

        public double Compute(Node node, double[] newValue, double[] oldValue)
        {
            var totalArea = 0.0;

            node.States.ForEach((state, i) => { totalArea += (state.SafeMax - state.SafeMin) * newValue[i]; });

            var neededArea = totalArea * this.percentile;

            var currentArea = 0.0;

            var k = 0;
            foreach (var state in node.States)
            {
                var thisArea = (state.SafeMax - state.SafeMin) * newValue[k];

                if (currentArea + thisArea > neededArea)
                {
                    return state.SafeMin + (neededArea - currentArea) / newValue[k];
                }

                currentArea += thisArea;
                k++;
            }

            return node.States.Last().SafeMax;
        }
    }
}