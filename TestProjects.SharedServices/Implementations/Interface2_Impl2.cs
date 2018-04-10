using System;
using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface2_Impl2 : IInterface2
    {
        #region IInterface2 Interface Implementation

        public DateTime Property1 { get; set; }
        public double Property2 { get; set; }
        public IInterface3 Property3 { get; set; }

        #endregion
    }
}