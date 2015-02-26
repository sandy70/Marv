using Marv.Common.Types;

namespace Marv.Common
{
    /// <summary>
    ///     This class exists so that we can interface with MATLAB. Do not try to replace with base class.
    /// </summary>
    public class SectionEvidence : Dict<int, string, NodeEvidence>
    {
        public static SectionEvidence Read(string filePath)
        {
            return Utils.ReadJson<SectionEvidence>(filePath);
        }

        public Dict<string, NodeEvidence> GetVertexEvidences(int year)
        {
            return this[year];
        }

        public void Write(string filePath)
        {
            this.WriteJson(filePath);
        }
    }
}