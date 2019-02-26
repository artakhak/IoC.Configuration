using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ClassMember
{
    [TestClass]
    public class ClassMemberSuccessfulLoadTests_Autofac : ClassMemberSuccessfulLoadTests
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            OnClassInitialize(DiImplementationType.Autofac, ClassMemberConfigurationRelativePath);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}