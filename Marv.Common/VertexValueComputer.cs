using System.Linq;

namespace Marv.Common
{
    public interface IVertexValueComputer
    {
        double Compute(NetworkNode node, double[] newValue, double[] oldValue);
    }

    public class VertexEntropyComputer : IVertexValueComputer
    {
        public double Compute(NetworkNode node, double[] newValue, double[] oldValue)
        {
            return node.Entropy(newValue, oldValue);
        }
    }

    public class VertexEntropyDifferenceComputer : IVertexValueComputer
    {
        public double Compute(NetworkNode node, double[] newValue, double[] oldValue)
        {
            return node.EntropyDifference(newValue, oldValue);
        }
    }

    public class VertexMeanComputer : IVertexValueComputer
    {
        public double Compute(NetworkNode node, double[] newValue, double[] oldValue)
        {
            return node.Mean(newValue, oldValue);
        }
    }

    public class VertexMeanDifferenceComputer : IVertexValueComputer
    {
        public double Compute(NetworkNode node, double[] newValue, double[] oldValue)
        {
            return node.MeanDifference(newValue, oldValue);
        }
    }

    public class VertexStandardDeviationComputer : IVertexValueComputer
    {
        public double Compute(NetworkNode node, double[] newValue, double[] oldValue)
        {
            return node.StandardDeviation(newValue, oldValue);
        }
    }

    public class VertexStateComputer : IVertexValueComputer
    {
        private readonly int stateIndex;

        public VertexStateComputer(int i)
        {
            this.stateIndex = i;
        }

        public double Compute(NetworkNode node, double[] newValue, double[] oldValue)
        {
            return newValue[this.stateIndex];
        }
    }

    public class VertexStateDifferenceComputer : IVertexValueComputer
    {
        private readonly int stateIndex;

        public VertexStateDifferenceComputer(int i)
        {
            this.stateIndex = i;
        }

        public double Compute(NetworkNode node, double[] newValue, double[] oldValue)
        {
            return newValue[this.stateIndex] - oldValue[this.stateIndex];
        }
    }

    public class VertexPercentileComputer : IVertexValueComputer
    {
        private readonly double percentile;

        public VertexPercentileComputer(double thePercentile)
        {
            this.percentile = thePercentile;
        }

        public double Compute(NetworkNode node, double[] newValue, double[] oldValue)
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