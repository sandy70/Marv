using Marv.Common;

namespace Marv
{
    public interface IGraphValueReader
    {
        Dict<int, string, string, double> Read(string lineKey, string locationKey);
    }
}