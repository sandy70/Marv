using System.Collections.Generic;

namespace LibPipeline
{
    public class ValueStore
    {
        private ILocation defaultLocation = new Location();

        private Dictionary<ILocation, Dictionary<int, Dictionary<string, Dictionary<string, double>>>> pipelineValue =
            new Dictionary<ILocation, Dictionary<int, Dictionary<string, Dictionary<string, double>>>>();

        public Dictionary<string, Dictionary<string, double>> GetGraphValue(int year, ILocation location = null)
        {
            if (location == null) location = this.defaultLocation;

            var locationValue = this.GetLocationValue(location);

            if (locationValue.ContainsKey(year))
            {
                return locationValue[year];
            }
            else
            {
                return locationValue[year] = new Dictionary<string, Dictionary<string, double>>();
            }
        }

        public Dictionary<int, Dictionary<string, Dictionary<string, double>>> GetLocationValue(ILocation location)
        {
            if (this.pipelineValue.ContainsKey(location))
            {
                return this.pipelineValue[location];
            }
            else
            {
                return this.pipelineValue[location] = new Dictionary<int, Dictionary<string, Dictionary<string, double>>>();
            }
        }

        public double GetStateValue(string stateKey, string vertexKey = "__DEFAULT__", int year = int.MinValue, ILocation location = null)
        {
            if (location == null) location = this.defaultLocation;

            var vertexValue = this.GetVertexValue(vertexKey, year, location);

            if (vertexValue.ContainsKey(stateKey))
            {
                return vertexValue[stateKey];
            }
            else
            {
                return vertexValue[stateKey] = 0;
            }
        }

        public Dictionary<string, double> GetVertexValue(string key, int year = int.MinValue, ILocation location = null)
        {
            if (location == null) location = this.defaultLocation;

            var graphValue = this.GetGraphValue(year, location);

            if (graphValue.ContainsKey(key))
            {
                return graphValue[key];
            }
            else
            {
                return graphValue[key] = new Dictionary<string, double>();
            }
        }

        public bool HasGraphValue(int year, ILocation location = null)
        {
            if (location == null) return false;

            if (this.HasLocationValue(location))
            {
                var locationValue = this.pipelineValue[location];

                if (locationValue.ContainsKey(year))
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

        public bool HasLocationValue(ILocation location)
        {
            if (this.pipelineValue.ContainsKey(location))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetGraphValue(Dictionary<string, Dictionary<string, double>> graphValue, int year, ILocation location = null)
        {
            if (location == null) location = this.defaultLocation;

            var locationValue = this.GetLocationValue(location);

            locationValue[year] = graphValue;
        }

        public void SetLocationValue(Dictionary<int, Dictionary<string, Dictionary<string, double>>> locationValue, ILocation location)
        {
            this.pipelineValue[location] = locationValue;
        }

        public void SetStateValue(double value, string stateKey, string vertexKey = "__DEFAULT__", int year = int.MinValue, ILocation location = null)
        {
            if (location == null) location = this.defaultLocation;

            var vertexValue = this.GetVertexValue(vertexKey, year, location);

            vertexValue[stateKey] = value;
        }
    }
}