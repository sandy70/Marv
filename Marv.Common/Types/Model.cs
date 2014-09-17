namespace Marv
{
    public interface IModel
    {
        bool IsEnabled { get; set; }
        bool IsSelected { get; set; }
        string Key { get; set; }
        string Name { get; set; }
    }
}