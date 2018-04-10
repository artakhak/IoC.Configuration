using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests.SuccessfullDiModuleLoadTests
{
    [TestClass]
    public class SuccessfullDiModuleLoadTestsFileConfigurationAutofac : SuccessfullDiModuleLoadTests
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
            ClassInitializeCommon(DiImplementationType.Autofac, () => CreateFileConfigurationBasedContainerInfo(DiImplementationType.Autofac));
        }

        #endregion
    }
}