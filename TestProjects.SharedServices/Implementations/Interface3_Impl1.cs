using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface3_Impl1 : IInterface3
    {
        #region IInterface3 Interface Implementation

        public int Property1 => 19;
        public IInterface4 Property2 { get; set; }

        #endregion
    }
}