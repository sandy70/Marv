using LibBn;
using LibPipeline;
using LibPipline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marv
{
    public static class Model
    {
        private static object _lock = new object();

        public static Dictionary<int, Dictionary<string, Dictionary<string, double>>>
            Run(ILocation selectedLocation, BnGraph graph, int startYear, int endYear)
        {
            var inputStore = new InputStore();
            var locationValue = new Dictionary<int, Dictionary<string, Dictionary<string, double>>>();

            for (int year = startYear; year <= endYear; year++)
            {
                var graphInput = inputStore.GetGraphInput(year);
                var graphEvidence = new Dictionary<string, VertexEvidence>();

                var fixedVariables = new Dictionary<string, int>
                {
                    { "dia", 6 },
                    { "t", 5 },
                    { "coattype", 2 },
                    { "surfaceprep", 4 },
                    { "C8", 2 },
                    { "Kd", 0 },
                    { "Cs", 5 },
                    { "Rs", 4 },
                    { "pratio", 3 },
                    { "freq", 3 },
                    { "Kd_w", 10 },
                    { "Kd_b", 10 },
                    { "CP", 5 },
                    { "rho", 4 },
                    { "Co2", 3 },
                    { "millscale", 1 },
                    { "wd", 2 },
                    { "T", 5 },
                    { "P", 5 }
                };

                foreach (var variable in fixedVariables)
                {
                    graphEvidence[variable.Key] = new VertexEvidence
                    {
                        EvidenceType = EvidenceType.StateSelected,
                        StateIndex = variable.Value
                    };
                }

                var stateValues = new List<int> { 1000, 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5, 0 };

                var location = selectedLocation as dynamic;
                var mean = location.Pressure / 14.5;
                var variance = Math.Pow(location.Pressure / 14.5 - location.Pressure_Min / 14.5, 2);
                var normalDistribution = new NormalDistribution(mean, variance);

                graphEvidence["P"] = new VertexEvidence
                {
                    Evidence = new double[stateValues.Count - 1],
                    EvidenceType = EvidenceType.SoftEvidence
                };

                graphEvidence["age"] = new VertexEvidence { EvidenceType = EvidenceType.StateSelected, StateIndex = Math.Min(year - startYear, 99) };

                if (year == startYear)
                {
                    graphEvidence["ocl"] = new VertexEvidence
                    {
                        Evidence = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }
                else
                {
                    graphEvidence["ocl"] = new VertexEvidence
                    {
                        Evidence = locationValue[year - 1]["cl"].Select(x => x.Value).ToArray(),
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }

                if (year == startYear)
                {
                    graphEvidence["ocd"] = new VertexEvidence
                    {
                        Evidence = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }
                else
                {
                    graphEvidence["ocd"] = new VertexEvidence
                    {
                        Evidence = locationValue[year - 1]["cd"].Select(x => x.Value).ToArray(),
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }

                if (year == startYear)
                {
                    graphEvidence["ocdc"] = new VertexEvidence
                    {
                        Evidence = new double[] { 0, 0, 0, 1 },
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }
                else
                {
                    graphEvidence["ocdc"] = new VertexEvidence
                    {
                        Evidence = locationValue[year - 1]["cdc"].Select(x => x.Value).ToArray(),
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }

                for (int i = 0; i < stateValues.Count - 1; i++)
                {
                    graphEvidence["P"].Evidence[i] = normalDistribution.CDF(stateValues[i]) - normalDistribution.CDF(stateValues[i + 1]);
                }

                graph.SetEvidence(graphEvidence);
                graph.UpdateBeliefs();

                locationValue[year] = graph.GetNetworkValue();
            }

            return locationValue;
        }

        public static Task<Dictionary<int, Dictionary<string, Dictionary<string, double>>>>
            RunAsync(ILocation selectedLocation, BnGraph graph, int startYear, int endYear)
        {
            return Task.Run(() =>
            {
                lock (Model._lock)
                {
                    return Model.Run(selectedLocation, graph, startYear, endYear);
                }
            });
        }
    }
}
