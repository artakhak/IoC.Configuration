using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface10_Impl1 : IInterface10
    {
        public Interface10_Impl1(int param1)
        {
            Property1 = param1;
        }
        public int Property1 { get; }
        public string Property2 { get; set; }
    }
}