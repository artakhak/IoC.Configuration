using DynamicallyLoadedAssembly1.Interfaces;

namespace DynamicallyLoadedAssembly1.Implementations
{
    public class SelfBoundService1
    {
        #region  Constructors

        public SelfBoundService1(int param1, double param2, IInterface1 param3)
        {
            Property1 = param1;
            Property2 = param2;
            Property3 = param3;
        }

        #endregion

        #region Member Functions

        public int Property1 { get; }
        public double Property2 { get; }
        public IInterface1 Property3 { get; }

        #endregion
    }
}