namespace Marv
{
    public delegate void EventHandler<in T1, in T2>(object sender, T1 arg1, T2 arg2);
}