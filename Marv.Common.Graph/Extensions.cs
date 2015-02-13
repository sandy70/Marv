using System.IO;
using System.Linq;
using Marv.Common;

namespace Marv
{
    public static partial class Extensions
    {
        public static void RemoveEvidence(this ILineData lineData, string sectionId, int year, string vertexKey)
        {
            var sectionEvidence = lineData.GetEvidence(sectionId);
            sectionEvidence[year][vertexKey] = null;
            lineData.SetEvidence(sectionId, sectionEvidence);
        }

        public static void SetEvidence(this ILineData lineData, string sectionId, int year, string vertexKey, VertexEvidence vertexEvidence)
        {
            var sectionEvidence = lineData.GetEvidence(sectionId);

            if (vertexEvidence.Type == VertexEvidenceType.Null)
            {
                sectionEvidence[year][vertexKey] = null;
            }
            else
            {
                sectionEvidence[year][vertexKey] = vertexEvidence;
            }

            lineData.SetEvidence(sectionId, sectionEvidence);
        }

        public static void WriteHcs(this Dict<string, VertexEvidence> evidences, string filePath)
        {
            using (var hcsFile = new StreamWriter(filePath))
            {
                foreach (var nodeKey in evidences.Keys)
                {
                    if (evidences[nodeKey].Value.Sum() > 0)
                    {
                        hcsFile.WriteLine("{0}: ({1})", nodeKey, evidences[nodeKey].Value.String(delim: " "));
                    }
                }
            }
        }

        public static void WriteHcs(this Dict<string, double[]> evidences, string filePath)
        {
            using (var hcsFile = new StreamWriter(filePath))
            {
                foreach (var nodeKey in evidences.Keys)
                {
                    if (evidences[nodeKey].Sum() > 0)
                    {
                        hcsFile.WriteLine("{0}: ({1})", nodeKey, evidences[nodeKey].String(delim: " "));
                    }
                }
            }
        }
    }
}