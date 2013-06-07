using LibBn;
using LibPipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marv
{
    public class NearNeutralPhSccModel
    {
        private object _lock = new object();
        private int endYear;
        private IEnumerable<BnGraph> graphs;
        private ILocation selectedLocation;
        private int startYear;

        public int EndYear
        {
            get { return endYear; }
            set { endYear = value; }
        }

        public IEnumerable<BnGraph> Graphs
        {
            get { return graphs; }
            set { graphs = value; }
        }

        public ILocation SelectedLocation
        {
            get { return selectedLocation; }
            set { selectedLocation = value; }
        }

        public int StartYear
        {
            get { return startYear; }
            set { startYear = value; }
        }

        public LocationValue Run()
        {
            var intervalValue = new LocationValue();

            var sccGraph = this.Graphs.GetGraph("nnphscc");
            var failureGraph = this.Graphs.GetGraph("nnphsccfailure");

            var sccIntervalValue = new LocationValue();
            var failureIntervalValue = new LocationValue();

            for (int year = startYear; year <= endYear; year++)
            {
                var sccGraphEvidence = new Dictionary<string, VertexEvidence>();
                var failureGraphEvidence = new Dictionary<string, VertexEvidence>();

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
                    sccGraphEvidence[variable.Key] = new VertexEvidence
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

                sccGraphEvidence["P"] = new VertexEvidence
                {
                    Evidence = new double[stateValues.Count - 1],
                    EvidenceType = EvidenceType.SoftEvidence
                };

                sccGraphEvidence["age"] = new VertexEvidence { EvidenceType = EvidenceType.StateSelected, StateIndex = Math.Min(year - startYear, 99) };

                if (year == startYear)
                {
                    sccGraphEvidence["ocl"] = new VertexEvidence
                    {
                        Evidence = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }
                else
                {
                    sccGraphEvidence["ocl"] = new VertexEvidence
                    {
                        Evidence = intervalValue.GetModelValue(year - 1)
                                                .GetGraphValue("nnphscc")
                                                .GetVertexValue("cl")
                                                .ToArray(),

                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }

                if (year == startYear)
                {
                    sccGraphEvidence["ocd"] = new VertexEvidence
                    {
                        Evidence = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }
                else
                {
                    sccGraphEvidence["ocd"] = new VertexEvidence
                    {
                        Evidence = intervalValue.GetModelValue(year - 1)
                                                .GetGraphValue("nnphscc")
                                                .GetVertexValue("cd")
                                                .ToArray(),

                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }

                if (year == startYear)
                {
                    sccGraphEvidence["ocdc"] = new VertexEvidence
                    {
                        Evidence = new double[] { 0, 0, 0, 1 },
                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }
                else
                {
                    sccGraphEvidence["ocdc"] = new VertexEvidence
                    {
                        Evidence = intervalValue.GetModelValue(year - 1)
                                                .GetGraphValue("nnphscc")
                                                .GetVertexValue("cdc")
                                                .ToArray(),

                        EvidenceType = EvidenceType.SoftEvidence
                    };
                }

                for (int i = 0; i < stateValues.Count - 1; i++)
                {
                    sccGraphEvidence["P"].Evidence[i] = normalDistribution.CDF(stateValues[i]) - normalDistribution.CDF(stateValues[i + 1]);
                }

                sccGraph.SetEvidence(sccGraphEvidence);
                sccGraph.UpdateBeliefs();

                intervalValue.GetModelValue(year).SetGraphValue("nnphscc", sccGraph.GetNetworkValue());

                failureGraphEvidence["cd"] = new VertexEvidence
                {
                    Evidence = intervalValue.GetModelValue(year)
                                            .GetGraphValue("nnphscc")
                                            .GetVertexValue("cd")
                                            .ToArray(),

                    EvidenceType = EvidenceType.SoftEvidence
                };

                failureGraphEvidence["cl"] = new VertexEvidence
                {
                    Evidence = intervalValue.GetModelValue(year)
                                            .GetGraphValue("nnphscc")
                                            .GetVertexValue("cl")
                                            .ToArray(),

                    EvidenceType = EvidenceType.SoftEvidence
                };

                failureGraphEvidence["ft"] = new VertexEvidence
                {
                    EvidenceType = EvidenceType.StateSelected,
                    StateIndex = 5  // 300 - 350
                };

                failureGraph.SetEvidence(failureGraphEvidence);
                failureGraph.UpdateBeliefs();

                intervalValue.GetModelValue(year).SetGraphValue("nnphsccfailure", failureGraph.GetNetworkValue());
            }

            return intervalValue;
        }

        public Task<LocationValue>  RunAsync()
        {
            return Task.Run(() =>
            {
                lock (this._lock)
                {
                    return this.Run();
                }
            });
        }
    }
}