using SharedServices.Interfaces.Generic;

namespace SharedServices.Implementations.Generic
{
    public class Generic3_1<T> : IGeneric3_1<T>
    {
        public Generic3_1(T param1)
        {
            Value = param1;
        }

        public T Value { get; }
    }
}