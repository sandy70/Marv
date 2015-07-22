namespace Marv.Common
{
    public interface ICommand
    {
        void Execute();
        bool Undo();
    }
}