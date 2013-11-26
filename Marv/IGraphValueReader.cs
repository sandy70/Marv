using Marv.Common;

namespace Marv
{
    public interface IGraphValueReader
    {
        Dictionary<int, string, string, double> Read(string lineKey, string locationKey);
    }
}