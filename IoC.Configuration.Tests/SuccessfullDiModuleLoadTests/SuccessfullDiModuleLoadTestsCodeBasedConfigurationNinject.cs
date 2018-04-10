using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests.SuccessfullDiModuleLoadTests
{
    [TestClass]
    public class SuccessfullDiModuleLoadTestsCodeBasedConfigurationNinject : SuccessfullDiModuleLoadTests
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
            ClassInitializeCommon(DiImplementationType.Ninject, () => CreateCodeBasedContainerInfo(DiImplementationType.Ninject));
        }

        #endregion
    }
}