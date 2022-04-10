using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.ClassMember
{
    [TestFixture]
    public class ClassMemberSuccessfulLoadTests_Ninject : ClassMemberSuccessfulLoadTests
    {
        [OneTimeSetUp]
        public static void ClassInitialize()
        {
            OnClassInitialize(DiImplementationType.Ninject, ClassMemberConfigurationRelativePath);
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
            OnClassCleanup();
        }
    }
}