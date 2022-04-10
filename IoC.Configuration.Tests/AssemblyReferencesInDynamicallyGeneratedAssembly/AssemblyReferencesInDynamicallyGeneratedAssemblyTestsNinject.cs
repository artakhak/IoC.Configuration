using NUnit.Framework;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests.AssemblyReferencesInDynamicallyGeneratedAssembly
{
    [TestFixture]
    public class AssemblyReferencesInDynamicallyGeneratedAssemblyTestsNinject : AssemblyReferencesInDynamicallyGeneratedAssemblyTests
    {
        [SetUp]
        public static void OnTestInitialize()
        {
            OnTestInitialize(DiImplementationType.Ninject);
        }
    }
}
