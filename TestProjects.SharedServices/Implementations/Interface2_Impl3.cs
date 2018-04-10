using System;
using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface2_Impl3 : IInterface2
    {
        #region  Constructors

        public Interface2_Impl3(DateTime param1, double param2, IInterface3 param3)
        {
            Property1 = param1;
            Property2 = param2;
            Property3 = param3;
            Param3_InitializedInConstructor = param3;
        }

        #endregion

        #region IInterface2 Interface Implementation

        public DateTime Property1 { get; set; }
        public double Property2 { get; set; }
        public IInterface3 Property3 { get; set; }

        #endregion

        #region Member Functions

        public IInterface3 Param3_InitializedInConstructor { get; }

        #endregion
    }
}