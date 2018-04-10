using SharedServices.Interfaces;

namespace SharedServices.Implementations
{
    /// <summary>
    ///     Do not bind this to IInterface3, since it will break tests which rely on IInterface3 having single binding.
    /// </summary>
    public class Interface3_Impl3 : IInterface3
    {
        #region IInterface3 Interface Implementation

        public int Property1 => 19;
        public IInterface4 Property2 { get; set; }

        #endregion
    }
}