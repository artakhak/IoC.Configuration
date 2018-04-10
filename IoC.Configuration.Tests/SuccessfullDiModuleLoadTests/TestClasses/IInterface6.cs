namespace IoC.Configuration.Tests.SuccessfullDiModuleLoadTests.TestClasses
{
    public interface IInterface6
    {
        #region Current Type Interface

        int Property1 { get; }
        IInterface1 Property2 { get; }

        #endregion
    }

    public class Interface6_Impl1 : IInterface6
    {
        #region  Constructors

        public Interface6_Impl1(int param1, IInterface1 param2)
        {
            Property1 = param1;
            Property2 = param2;
        }

        #endregion

        #region IInterface6 Interface Implementation

        public int Property1 { get; }
        public IInterface1 Property2 { get; }

        #endregion
    }
}