using System;
using System.Linq;

namespace Marv.Common.Graph
{
    public class RangeEvidenceString : EvidenceString
    {
        public RangeEvidenceString(string aString)
            : base(aString)
        { }

        public override IEvidence Parse(Vertex vertex)
        {
            IEvidence evidence = null;

            var parts = this._string
                            .Trim()
                            .Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            double minValue;
            double maxValue;

            if (Double.TryParse(parts[0], out minValue) && Double.TryParse(parts[1], out maxValue))
            {
                var evidenceArray = new double[vertex.States.Count];

                foreach (var state in vertex.States)
                {
                    if (maxValue < state.Range.Min)
                    {
                        // do nothing
                    }
                    else if (minValue > state.Range.Max)
                    {
                        // do nothing
                    }
                    else
                    {
                        if (minValue >= state.Range.Min && minValue <= state.Range.Max)
                        {
                            evidenceArray[vertex.States.IndexOf(state)] = (state.Range.Max - minValue) / (state.Range.Max - state.Range.Min);
                        }

                        if (maxValue >= state.Range.Min && maxValue <= state.Range.Max)
                        {
                            evidenceArray[vertex.States.IndexOf(state)] = (maxValue - state.Range.Min) / (state.Range.Max - state.Range.Min);
                        }

                        if (minValue <= state.Range.Min && maxValue >= state.Range.Max)
                        {
                            evidenceArray[vertex.States.IndexOf(state)] = 1;
                        }
                    }
                }

                var maxStateIndex = evidenceArray.MaxIndex();

                evidence = new SoftEvidence
                {
                    Evidence = evidenceArray,
                    SynergiString = vertex.GetMean(evidenceArray) + this._string.Enquote('{', '}')
                };
            }
            else
            {
                throw new Smile.SmileException("");
            }
            return evidence;
        }
    }
}
