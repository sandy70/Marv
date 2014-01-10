using NLog;
using System;
using System.Collections.Generic;

namespace Marv.Common
{
    public class VertexMeanComputer : IVertexValueComputer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public double Compute(Vertex vertex, Dictionary<string, double> vertexValue)
        {
            var mean = 0.0;

            if (vertex.Type == VertexType.Interval)
            {
                foreach (var state in vertex.States)
                {
                    mean += (vertexValue[state.Key] / 2) * (Math.Pow(state.Range.Max, 2) - Math.Pow(state.Range.Min, 2));
                }
            }
            else
            {
                var message = String.Format("Mean is undefined for non-interval type sourceVertex: {0}.", vertex);
                
                logger.Error(message);
                
                throw new VertexValueUndefindedException(message);
            }

            return mean / vertex.States.Count;
        }
    }
}