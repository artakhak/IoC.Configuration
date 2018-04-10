using DynamicallyLoadedAssembly1.Interfaces;

namespace DynamicallyLoadedAssembly1.Implementations
{
    public class SelfBoundService2
    {
        #region Member Functions

        public int Property1 { get; set; }
        public double Property2 { get; set; }
        public IInterface1 Property3 { get; set; }

        #endregion
    }
}