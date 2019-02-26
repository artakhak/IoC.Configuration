using SharedServices.Interfaces.Generic;

namespace SharedServices.Implementations.Generic
{
    public class Generic4_2<T, K> : IGeneric4_2<T, K>
    {
        public Generic4_2(T param1)
        {
            Value1 = param1;
        }
        public T Value1 { get; }
        public K Value2 { get; set; }
    }
}