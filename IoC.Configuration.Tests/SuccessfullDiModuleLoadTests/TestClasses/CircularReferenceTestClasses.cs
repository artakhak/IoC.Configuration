namespace IoC.Configuration.Tests.SuccessfullDiModuleLoadTests.TestClasses
{
    public interface ICircularReferenceTestInterface1
    {
        #region Current Type Interface

        ICircularReferenceTestInterface2 Property1 { get; }

        #endregion
    }

    public interface ICircularReferenceTestInterface2
    {
        #region Current Type Interface

        ICircularReferenceTestInterface1 Property1 { get; }

        #endregion
    }

    public class CircularReferenceTestInterface1_Impl : ICircularReferenceTestInterface1
    {
        #region ICircularReferenceTestInterface1 Interface Implementation

        public ICircularReferenceTestInterface2 Property1 { get; set; }

        #endregion
    }

    public class CircularReferenceTestInterface2_Impl : ICircularReferenceTestInterface2
    {
        #region  Constructors

        public CircularReferenceTestInterface2_Impl(ICircularReferenceTestInterface1 param1)
        {
            Property1 = param1;
        }

        #endregion

        #region ICircularReferenceTestInterface2 Interface Implementation

        public ICircularReferenceTestInterface1 Property1 { get; }

        #endregion
    }
}