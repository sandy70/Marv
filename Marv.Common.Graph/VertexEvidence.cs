using System;
using System.Collections.Generic;

namespace Marv
{
    public class VertexEvidence
    {
        public double[] Belief { get; set; }
        public double[] Evidence { get; set; }
        public VertexEvidenceType EvidenceType { get; set; }
        public double[] Params { get; set; }
        public string StateKey { get; set; }

        public static List<double> ParseEvidenceParams(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            var values = new List<double>();

            var parts = str.Trim().Split("(),: ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                double value;
                if (double.TryParse(part, out value))
                {
                    values.Add(value);
                }
            }

            return values;
        }

        public override string ToString()
        {
            if (this.Params == null)
            {
                return null;
            }

            if (this.EvidenceType == VertexEvidenceType.Distribution)
            {
                return this.Params.String();
            }

            if (this.EvidenceType == VertexEvidenceType.Normal)
            {
                return "NORM" + this.Params.String().Enquote('(', ')');
            }

            if (this.EvidenceType == VertexEvidenceType.Number)
            {
                return this.Params[0].ToString();
            }

            if (this.EvidenceType == VertexEvidenceType.Range)
            {
                return this.Params[0] + ":" + this.Params[1];
            }

            if (this.EvidenceType == VertexEvidenceType.State)
            {
                return this.StateKey;
            }

            if (this.EvidenceType == VertexEvidenceType.Triangular)
            {
                return "TRI" + this.Params.String().Enquote('(', ')');
            }

            return null;
        }
    }
}