namespace Marv.Common
{
    public class ValueChangedArgs<T>
    {
        public T NewValue { get; set; }
        public T OldValue { get; set; }
    }
}