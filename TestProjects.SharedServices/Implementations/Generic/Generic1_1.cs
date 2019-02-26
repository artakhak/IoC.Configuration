using SharedServices.Interfaces;
using SharedServices.Interfaces.Generic;

namespace SharedServices.Implementations.Generic
{
    public class Generic1_1<T> : IGeneric1_1<T> where T : IInterface1
    {
        public Generic1_1(T param1)
        {

        }
        public T Value { get; }
    }
}