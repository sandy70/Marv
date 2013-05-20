using LibBn;
using System.Collections.Generic;

namespace LibPipeline
{
    public class ValueStore
    {
        private Dictionary<Location, Dictionary<int, Dictionary<string, Dictionary<string, double>>>> pipelineValue = 
            new Dictionary<Location, Dictionary<int, Dictionary<string, Dictionary<string, double>>>>();
        
        private Location defaultLocation = new Location();

        public Dictionary<int, Dictionary<string, Dictionary<string, double>>> GetLocationValue(Location location)
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

        public Dictionary<string, Dictionary<string, double>> GetGraphValue(int year, Location location = null)
        {
            if(location == null) location = this.defaultLocation;

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

        public Dictionary<string, double> GetVertexValue(string key, int year = int.MinValue, Location location = null)
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

        public double GetStateValue(string stateKey, string vertexKey = "__DEFAULT__", int year = int.MinValue, Location location = null)
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

        public void SetStateValue(double value, string stateKey, string vertexKey = "__DEFAULT__", int year = int.MinValue, Location location = null)
        {
            if (location == null) location = this.defaultLocation;

            var vertexValue = this.GetVertexValue(vertexKey, year, location);

            vertexValue[stateKey] = value;
        }
    }
}