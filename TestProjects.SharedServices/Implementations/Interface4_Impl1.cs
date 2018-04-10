using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    public class Interface4_Impl1 : IInterface4
    {
        #region  Constructors

        public Interface4_Impl1(IInterface3 param1)
        {
            Property1 = param1;
        }

        #endregion

        #region IInterface4 Interface Implementation

        public IInterface3 Property1 { get; }

        #endregion
    }
}