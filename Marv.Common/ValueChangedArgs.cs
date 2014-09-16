namespace Marv
{
    public class ValueChangedArgs<T>
    {
        public T NewValue { get; set; }
        public T OldValue { get; set; }
    }
}