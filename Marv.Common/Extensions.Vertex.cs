﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Marv.Common
{
    public static partial class Extensions
    {
        public static double Entropy(this IVertex networkVertex, double[] newValue, double[] oldValue = null)
        {
            CheckValueArrayLength(networkVertex, newValue);

            return newValue.Entropy();
        }

        public static double EntropyDifference(this IVertex networkVertex, double[] newValue, double[] oldValue = null)
        {
            CheckValueArrayLength(networkVertex, newValue);
            CheckValueArrayLength(networkVertex, oldValue);

            return newValue.Entropy() - oldValue.Entropy();
        }

        public static IEnumerable<double> GetIntervals(this IVertex vertex)
        {
            if (vertex.Type == VertexType.Interval)
            {
                return vertex.States.Select(state => state.Min).Concat(vertex.States.Last().Max.Yield()).ToArray();
            }

            if (vertex.Type == VertexType.Numbered)
            {
                return vertex.States.Select(state => state.Min).ToArray();
            }

            return Enumerable.Range(0, vertex.States.Count).Select(i => (double) i).ToArray();
        }

        public static double Mean(this IVertex networkVertex, double[] newValue, double[] oldValue = null)
        {
            try
            {
                CheckVertexStatisticComputable(networkVertex, newValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return Math.Round(networkVertex.States
                                .Select((state, i) => (newValue[i] * (state.SafeMax + state.SafeMin) / 2))
                                .Sum(),2);
        }

        public static double MeanDifference(this IVertex networkVertex, double[] newValue, double[] oldValue = null)
        {
            try
            {
                CheckVertexStatisticComputable(networkVertex, newValue);
                CheckVertexStatisticComputable(networkVertex, oldValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return networkVertex.Mean(newValue) - networkVertex.Mean(oldValue);
        }

        public static double StandardDeviation(this IVertex networkVertex, double[] newValue, double[] oldValue = null)
        {
            try
            {
                CheckVertexStatisticComputable(networkVertex, newValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var mu = networkVertex.Mean(newValue);

            var stdev = networkVertex.States.Select((state, i) =>
                                                    newValue[i] *
                                                    (
                                                        1.0 / 3 * (Math.Pow(state.SafeMax, 3) - Math.Pow(state.SafeMin, 3)) +
                                                        Math.Pow(mu, 2) * (state.SafeMax - state.SafeMin) -
                                                        mu * (Math.Pow(state.SafeMax, 2) - Math.Pow(state.SafeMin, 2))
                                                    ))
                                     .Sum();

            return Math.Round(Math.Sqrt(stdev / networkVertex.States.Count), 2);
        }


        private static void CheckValueArrayLength(IVertex networkVertex, double[] value)
        {
            if (networkVertex.States.Count != value.Length)
            {
                var message = System.String.Format("The length of value array [{0}] should be = number of states in this node [{1}:{2}].", value.Length, networkVertex, networkVertex.States.Count);
                throw new ArgumentException(message);
            }
        }

        private static void CheckVertexStatisticComputable(IVertex networkVertex, double[] value)
        {
            CheckValueArrayLength(networkVertex, value);

            if (networkVertex.Type == VertexType.Interval)
            {
                return;
            }
            var message = System.String.Format("Mean is undefined for non-interval type node [{0}].", networkVertex);
            throw new ArgumentException(message);
        }
    }
}