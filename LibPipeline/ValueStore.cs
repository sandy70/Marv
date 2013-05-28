using LibBn;
using System.Collections.Generic;

namespace LibPipeline
{
    public class ModelValue : Dictionary<BnGraph, PipelineValue> { }

    public class PipelineValue : Dictionary<ILocation, IntervalValue> { }

    public class ValueStore
    {
        private ModelValue modelValue = new ModelValue();

        public GraphValue GetGraphValue(int year, ILocation location, BnGraph graph)
        {
            var intervalValue = this.GetIntervalValue(location, graph);

            if (intervalValue.ContainsKey(year))
            {
                return intervalValue[year];
            }
            else
            {
                return intervalValue[year] = new GraphValue();
            }
        }

        public IntervalValue GetIntervalValue(ILocation location, BnGraph graph)
        {
            var pipelineValue = this.GetPipelineValue(graph);

            if (pipelineValue.ContainsKey(location))
            {
                return pipelineValue[location];
            }
            else
            {
                return pipelineValue[location] = new IntervalValue();
            }
        }

        public PipelineValue GetPipelineValue(BnGraph graph)
        {
            if (this.modelValue.ContainsKey(graph))
            {
                return this.modelValue[graph];
            }
            else
            {
                return this.modelValue[graph] = new PipelineValue();
            }
        }

        public bool HasGraphValue(int year, ILocation location, BnGraph graph)
        {
            if (graph == null || location == null)
            {
                return false;
            }

            if (this.HasIntervalValue(location, graph))
            {
                var intervalValue = this.GetIntervalValue(location, graph);

                if (intervalValue.ContainsKey(year))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool HasGraphValues(int year, ILocation location, IEnumerable<BnGraph> graphs)
        {
            var hasGraphValues = true;

            foreach (var graph in graphs)
            {
                if (!this.HasGraphValue(year, location, graph))
                {
                    hasGraphValues = false;
                }
            }

            return hasGraphValues;
        }

        public bool HasIntervalValue(ILocation location, BnGraph graph)
        {
            if (graph == null) return false;
            if (location == null) return false;

            if (this.HasPipelineValue(graph))
            {
                var pipelineValue = this.GetPipelineValue(graph);

                if (pipelineValue.ContainsKey(location))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool HasPipelineValue(BnGraph graph)
        {
            if (graph == null) return false;

            if (this.modelValue.ContainsKey(graph))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetIntervalValue(IntervalValue intervalValue, ILocation location, BnGraph graph)
        {
            this.GetPipelineValue(graph)[location] = intervalValue;
        }
    }
}