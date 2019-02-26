using SharedServices.Interfaces;
using SharedServices.Interfaces.Generic;

namespace SharedServices.Implementations.Generic
{
    public class Generic2_1<T> : IGeneric2_1<T>
    {
        public Generic2_1(T param1)
        {
            Value = param1;
        }
        public T Value { get; }
    }
    
}