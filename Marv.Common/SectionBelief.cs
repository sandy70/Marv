using Marv.Common.Types;

namespace Marv.Common
{
    public class SectionBelief : Dict<int, string, double[]>
    {
        public void Write(string filePath)
        {
            this.WriteJson(filePath);
        }
    }
}