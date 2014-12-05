using Marv.Common;

namespace Marv
{
    public class SectionBelief : Dict<int, string, double[]>
    {
        public void Write(string filePath)
        {
            this.WriteJson(filePath);
        }
    }
}