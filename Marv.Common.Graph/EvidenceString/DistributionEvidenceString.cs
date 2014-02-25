using System;
using System.Linq;

namespace Marv.Common.Graph
{
    public class DistributionEvidenceString : EvidenceString
    {
        public DistributionEvidenceString(string aString)
            : base(aString)
        {
        }

        public override IEvidence Parse(Vertex vertex)
        {
            var parts = this._string
                            .Trim()
                            .Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            var evidenceArray = new double[vertex.States.Count];

            foreach (var part in parts)
            {
                var partsOfPart = part.Trim()
                                      .Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                double probability;

                if (Double.TryParse(partsOfPart[1], out probability))
                {
                    double value;

                    if (Double.TryParse(partsOfPart[0], out value))
                    {
                        foreach (var state in vertex.States)
                        {
                            if (state.Range.Bounds(value))
                            {
                                evidenceArray[vertex.States.IndexOf(state)] += probability;
                            }
                        }
                    }
                    else
                    {
                        foreach (var state in vertex.States)
                        {
                            if (state.Key == partsOfPart[0])
                            {
                                evidenceArray[vertex.States.IndexOf(state)] += probability;
                            }
                        }
                    }
                }
                else
                {
                    throw new Smile.SmileException("");
                }
            }

            return new SoftEvidence
            {
                Evidence = evidenceArray,
            };
        }
    }
}