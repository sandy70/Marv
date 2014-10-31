namespace Marv
{
    public interface IVertexValueComputer
    {
        double Compute(NetworkVertex vertex, double[] newValue, double[] oldValue);
    }

    public class VertexEntropyComputer : IVertexValueComputer
    {
        public double Compute(NetworkVertex vertex, double[] newValue, double[] oldValue)
        {
            return vertex.Entropy(newValue, oldValue);
        }
    }

    public class VertexEntropyDifferenceComputer : IVertexValueComputer
    {
        public double Compute(NetworkVertex vertex, double[] newValue, double[] oldValue)
        {
            return vertex.EntropyDifference(newValue, oldValue);
        }
    }

    public class VertexMeanComputer : IVertexValueComputer
    {
        public double Compute(NetworkVertex vertex, double[] newValue, double[] oldValue)
        {
            return vertex.Mean(newValue, oldValue);
        }
    }

    public class VertexMeanDifferenceComputer : IVertexValueComputer
    {
        public double Compute(NetworkVertex vertex, double[] newValue, double[] oldValue)
        {
            return vertex.MeanDifference(newValue, oldValue);
        }
    }

    public class VertexStandardDeviationComputer : IVertexValueComputer
    {
        public double Compute(NetworkVertex vertex, double[] newValue, double[] oldValue)
        {
            return vertex.StandardDeviation(newValue, oldValue);
        }
    }
}