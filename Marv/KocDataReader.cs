using LibNetwork;
using LibPipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Marv
{
    internal class KocDataReader
    {
        public void ReadVertexInputsForAllYears(BnInputStore inputManager)
        {
            var vertexInputReader = new BnVertexInputReader();

            for (int year = Config.StartYear; year <= Config.EndYear; year++)
            {
                var graphInput = inputManager.GetGraphInput((int)BnInputType.Default, year);
                vertexInputReader.Read(graphInput, "Resources/Inputs/Input" + year + ".csv");
            }
        }

        public List<BnVertexValue> ReadVertexValues(string kocDir, int year)
        {
            string fileName = Path.Combine("Resources/Outputs", "Output" + year + ".vertices");

            List<BnVertexValue> vertexValues = new List<BnVertexValue>();

            foreach (var line in File.ReadLines(fileName))
            {
                string[] parts = line.Split(new char[] { ',' });

                string key = parts[0];

                int nParts = parts.Count();

                BnVertexValue vertexValue = new BnVertexValue();
                // vertexValue.Key = key;

                for (int p = 1; p < nParts; p++)
                {
                    // vertexValue.States.Add(Double.Parse(parts[p]));
                }

                vertexValues.Add(vertexValue);
            }

            return vertexValues;
        }

        public Dictionary<int, List<BnVertexValue>> ReadVertexValuesForAllYears()
        {
            Dictionary<int, List<BnVertexValue>> vertexValuesByYear = new Dictionary<int, List<BnVertexValue>>();

            for (int year = Config.StartYear; year <= Config.EndYear; year++)
            {
                vertexValuesByYear.Add(year, this.ReadVertexValues(Config.KocDir, year));
            }

            return vertexValuesByYear;
        }
    }
}