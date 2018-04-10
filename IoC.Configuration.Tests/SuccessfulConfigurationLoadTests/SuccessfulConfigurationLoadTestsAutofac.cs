using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests.SuccessfulConfigurationLoadTests
{
    [TestClass]
    public class SuccessfulConfigurationLoadTestsAutofac : SuccessfulConfigurationLoadTests
    {
        #region Member Functions

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ClassCleanupCommon();
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            ClassInitializeCommon(DiImplementationType.Autofac, null);
        }

        #endregion
    }
}