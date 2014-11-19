namespace Marv
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
}