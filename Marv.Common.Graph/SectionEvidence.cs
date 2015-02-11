using Marv.Common;

namespace Marv
{
    /// <summary>
    /// This class exists so that we can interface with MATLAB. Do not try to replace with base class.
    /// </summary>
    public class SectionEvidence : Dict<int, string, VertexEvidence>
    {
        public static SectionEvidence Read(string filePath)
        {
            return Utils.ReadJson<SectionEvidence>(filePath);
        }

        public void Write(string filePath)
        {
            this.WriteJson(filePath);
        }
    }
}