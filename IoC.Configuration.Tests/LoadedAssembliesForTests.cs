using JetBrains.Annotations;
using OROptimizer;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Configuration.Tests;

/// <inheritdoc />
public class LoadedAssembliesForTests : ILoadedAssemblies
{
    [NotNull, ItemNotNull]
    private static readonly List<System.Reflection.Assembly> _assemblies = new();

    // Lets initialize the list of loaded assemblies _assemblies only once, so that the list does not get new assemblies when 
    // some tests load some new assemblies. If we do not store the list of assemblies in a static field and initialize it once,
    // we might end up in situation when the list keeps getting bigger, when some tests are executed, which might affect other tests.
    static LoadedAssembliesForTests()
    {
        var allLoadedAssemblies = new AllLoadedAssemblies();

        _assemblies.AddRange(allLoadedAssemblies.GetAssemblies().Where(x =>
            !string.Equals(x.GetName().Name, "JetBrains.ReSharper.TestRunner.Merged")));
    }

    /// <inheritdoc />
    public IEnumerable<System.Reflection.Assembly> GetAssemblies()
    {
        return _assemblies;
    }
}