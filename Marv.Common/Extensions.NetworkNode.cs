using System;
using System.Linq;

namespace Marv.Common
{
    public static partial class Extensions
    {
        public static double Entropy(this Node node, double[] newValue, double[] oldValue = null)
        {
            CheckValueArrayLength(node, newValue);

            return newValue.Entropy();
        }

        public static double EntropyDifference(this Node node, double[] newValue, double[] oldValue = null)
        {
            CheckValueArrayLength(node, newValue);
            CheckValueArrayLength(node, oldValue);

            return newValue.Entropy() - oldValue.Entropy();
        }

        public static double Mean(this Node node, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(node, newValue);

            return node.States
                         .Select((state, i) => (newValue[i] * (state.SafeMax + state.SafeMin) / 2))
                         .Sum();
        }

        public static double MeanDifference(this Node node, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(node, newValue);
            CheckVertexStatisticComputable(node, oldValue);

            return node.Mean(newValue) - node.Mean(oldValue);
        }

        public static double StandardDeviation(this Node node, double[] newValue, double[] oldValue = null)
        {
            CheckVertexStatisticComputable(node, newValue);

            var mu = node.Mean(newValue);

            var stdev = node.States.Select((state, i) =>
                                             newValue[i] *
                                             (
                                                 1.0 / 3 * (Math.Pow(state.SafeMax, 3) - Math.Pow(state.SafeMin, 3)) +
                                                 Math.Pow(mu, 2) * (state.SafeMax - state.SafeMin) -
                                                 mu * (Math.Pow(state.SafeMax, 2) - Math.Pow(state.SafeMin, 2))
                                             ))
                              .Sum();

            return Math.Sqrt(stdev / node.States.Count);
        }

        private static void CheckVertexStatisticComputable(Node node, double[] value)
        {
            CheckValueArrayLength(node, value);

            if (node.Type != NodeType.Interval)
            {
                var message = System.String.Format("Mean is undefined for non-interval type node [{0}].", node);
                throw new InvalidValueException(message);
            }
        }

        private static void CheckValueArrayLength(Node node, double[] value)
        {
            if (node.States.Count != value.Length)
            {
                var message = System.String.Format("The length of value array [{0}] should be = number of states in this node [{1}:{2}].", value.Length, node, node.States.Count);
                throw new InvalidValueException(message);
            }
        }
    }
}