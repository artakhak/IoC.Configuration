using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests.SuccessfullDiModuleLoadTests
{
    [TestClass]
    public class SuccessfullDiModuleLoadTestsFileConfigurationNinject : SuccessfullDiModuleLoadTests
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
            ClassInitializeCommon(DiImplementationType.Ninject, () => CreateFileConfigurationBasedContainerInfo(DiImplementationType.Ninject));
        }

        #endregion
    }
}