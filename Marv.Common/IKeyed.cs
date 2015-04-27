namespace Marv.Common
{
    public interface IKeyed : IKeyed<string> {}

    public interface IKeyed<out TKey>
    {
        TKey Key { get; }
    }
}