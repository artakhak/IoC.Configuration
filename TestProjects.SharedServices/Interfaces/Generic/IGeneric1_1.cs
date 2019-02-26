namespace SharedServices.Interfaces.Generic
{
    public interface IGeneric1_1<T> where T : IInterface1 
    {
        T Value { get; }
    }
}
