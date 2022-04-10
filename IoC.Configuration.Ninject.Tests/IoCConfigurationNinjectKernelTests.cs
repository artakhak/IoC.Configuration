using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using NUnit.Framework;
using SharedServices.Implementations;
using SharedServices.Interfaces;

namespace IoC.Configuration.Ninject.Tests;

[TestFixture]
internal class IoCConfigurationNinjectKernelTests
{
    private static readonly List<IDbConnection> ExpectedConnections = new()
    {
        new SqliteDbConnection("DB1.sqlite"),
        new SqliteDbConnection("DB2.sqlite")
    };

    [Test]
    public void TestImplicitEnumerableBinding()
    {
        var kernel = new IoCConfigurationNinjectKernel();

        AddImplicitBindings(kernel);

        Validate(kernel.Get<IEnumerable<IDbConnection>>());
        Validate(kernel.Get<ICollection<IDbConnection>>());
        Validate(kernel.Get<IDbConnection[]>());
        Validate(kernel.Get<IList<IDbConnection>>());
        Validate(kernel.Get<List<IDbConnection>>());
    }

    [Test]
    public void TestExplicitEnumerableBinding()
    {
        TestExplicitCollectionBinding<IEnumerable<IDbConnection>>(ExpectedConnections);
    }

    [Test]
    public void TestExplicitEmptyEnumerableBinding()
    {
        TestExplicitEmptyCollectionBinding<IList<IDbConnection>>(new List<IDbConnection>(0));
    }

    [Test]
    public void TestExplicitIListBinding()
    {
        TestExplicitCollectionBinding<IList<IDbConnection>>(ExpectedConnections);
    }

    [Test]
    public void TestExplicitEmptyIListBinding()
    {
        TestExplicitEmptyCollectionBinding<IList<IDbConnection>>(new List<IDbConnection>(0));
    }

    [Test]
    public void TestExplicitListBinding()
    {
        TestExplicitCollectionBinding(ExpectedConnections);
    }

    [Test]
    public void TestExplicitEmptyListBinding()
    {
        TestExplicitEmptyCollectionBinding(new List<IDbConnection>(0));
    }

    [Test]
    public void TestExplicitICollectionBinding()
    {
        TestExplicitCollectionBinding<ICollection<IDbConnection>>(ExpectedConnections);
    }

    [Test]
    public void TestExplicitEmptyICollectionBinding()
    {
        TestExplicitEmptyCollectionBinding<ICollection<IDbConnection>>(new List<IDbConnection>(0));
    }

    [Test]
    public void TestExplicitArrayBinding()
    {
        TestExplicitCollectionBinding(ExpectedConnections.ToArray());
    }

    [Test]
    public void TestExplicitEmptyArrayBinding()
    {
        TestExplicitEmptyCollectionBinding(Array.Empty<IDbConnection>());
    }

    private void TestExplicitCollectionBinding<TCollection>(TCollection boundCollection) where TCollection : IEnumerable<IDbConnection>
    {
        var kernel = new IoCConfigurationNinjectKernel();

        kernel.Bind<TCollection>().ToMethod(context => boundCollection).InSingletonScope();

        // Explicit bindings take precedence
        AddImplicitBindings(kernel);

        Validate(kernel.Get<TCollection>());
    }

    private void TestExplicitEmptyCollectionBinding<TCollection>(TCollection emptyCollection) where TCollection : IEnumerable<IDbConnection>
    {
        var kernel = new IoCConfigurationNinjectKernel();

        kernel.Bind<TCollection>().ToMethod(context => emptyCollection).InSingletonScope();

        // Explicit bindings take precedence
        AddImplicitBindings(kernel);
        Assert.AreEqual(0, kernel.Get<TCollection>().ToList().Count);
    }


    private void AddImplicitBindings(IoCConfigurationNinjectKernel kernel)
    {
        foreach (var dbConnection in ExpectedConnections)
            kernel.Bind<IDbConnection>().ToConstant(dbConnection).InSingletonScope();
    }

    private void Validate(IEnumerable<IDbConnection> resolvedConnections)
    {
        var resolvedConnectionsList = resolvedConnections.ToList();

        Assert.AreEqual(ExpectedConnections.Count, resolvedConnectionsList.Count);

        for (var i = 0; i < resolvedConnectionsList.Count; ++i)
        {
            Assert.AreEqual(typeof(SqliteDbConnection), resolvedConnectionsList[i].GetType());
            Assert.AreEqual(ExpectedConnections[i].ConnectionString, resolvedConnectionsList[i].ConnectionString);
        }
    }
}