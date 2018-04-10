using System;
using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface2_Impl4 : IInterface2
    {
        #region  Constructors

        public Interface2_Impl4(DateTime param1, double param2, IInterface3 param3)
        {
            Property1 = param1;
            Property2 = param2;
            Property3 = param3;
        }

        #endregion

        #region IInterface2 Interface Implementation

        public DateTime Property1 { get; }
        public double Property2 { get; }
        public IInterface3 Property3 { get; }

        #endregion
    }
}