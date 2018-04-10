using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using IoC.Configuration.DependencyInjection;
using IoC.Configuration.DiContainer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer;
using OROptimizer.DynamicCode;
using OROptimizer.Serializer;
using TestsSharedLibrary;
using TestsSharedLibrary.Diagnostics.Log;
using ParameterInfo = OROptimizer.ParameterInfo;

namespace IoC.Configuration.Tests.DynamicCode
{
    [TestClass]
    public class TypesListFactoryTypeGeneratorTests
    {
        #region Member Variables

        private AssemblyResolver _assemblyResolver;
        private static readonly Assembly _dynamicallyLoadedAssembly2;

        private bool _expectsTypeBuildValidationFailure;

        private static readonly Type _typeActionValidator1;
        private static readonly Type _typeActionValidator2;
        private static readonly Type _typeActionValidator3;
        private static readonly string DynamicDllsFolder = Path.Combine(Helpers.TestsEntryAssemblyFolder, nameof(TypesListFactoryTypeGeneratorTests));

        private static readonly string TestDllsFolder = Helpers.GetTestDllsFolderPath();

        #endregion

        #region  Constructors

        static TypesListFactoryTypeGeneratorTests()
        {
            TestsHelper.SetupLogger();

            var dynamicDllsFolder = new DirectoryInfo(DynamicDllsFolder);

            if (!dynamicDllsFolder.Exists)
                dynamicDllsFolder.Create();
            else
                foreach (var fileInfo in dynamicDllsFolder.EnumerateFiles())
                    fileInfo.Delete();

            using (CreateAssemblyResolver())
            {
                _dynamicallyLoadedAssembly2 = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(TestDllsFolder, "DynamicallyLoadedDlls", "TestProjects.DynamicallyLoadedAssembly2.dll"));

                _typeActionValidator1 = _dynamicallyLoadedAssembly2.GetType("DynamicallyLoadedAssembly2.ActionValidator1");
                _typeActionValidator2 = _dynamicallyLoadedAssembly2.GetType("DynamicallyLoadedAssembly2.ActionValidator2");
                _typeActionValidator3 = _dynamicallyLoadedAssembly2.GetType("DynamicallyLoadedAssembly2.ActionValidator3");
            }
        }

        #endregion

        #region Member Functions

        private static AssemblyResolver CreateAssemblyResolver()
        {
            return new AssemblyResolver(new[]
            {
                Path.Combine(TestDllsFolder, "DynamicallyLoadedDlls"),
                Path.Combine(TestDllsFolder, "ThirdPartyLibs"),
                Path.Combine(TestDllsFolder, "ContainerImplementations", "Autofac")
            });
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _assemblyResolver.Dispose();
        }

        private void TestDynamicAssemblyGeneration(TypeInfo factoryTypeInfo, IEnumerable<string[]> selectorParameterValues,
                                                   IEnumerable<Type[]> returnedTypesForEachParameterSelectorsSet,
                                                   Type[] returnedInstanceTypesForDefaultCase,
                                                   bool expectsDynamicLibraryBuildFailure,
                                                   Action<IDiContainer, object, MethodInfo> actionTestImplementation)
        {
            var currentDynamicDllFilePath = Path.Combine(DynamicDllsFolder, $"DynamicCode_{GlobalsCoreAmbientContext.Context.GenerateUniqueId()}.dll");
            var dynamicCodeNameSpace = $"DynamicCode_{GlobalsCoreAmbientContext.Context.GenerateUniqueId()}";

            _expectsTypeBuildValidationFailure = expectsDynamicLibraryBuildFailure;

            string generatedImplementationFullName = null;
            var typeActionValidatorFactory = factoryTypeInfo.Assembly.GetType(factoryTypeInfo.TypeFullName);


            var assemblyBuildCompleteWasCalled = false;

            // Generate Dll
            using (var dynamicAssemblyBuilder = GlobalsCoreAmbientContext.Context.StartDynamicAssemblyBuilder(currentDynamicDllFilePath,
                (assemblyPath, success, emitResult) =>
                {
                    assemblyBuildCompleteWasCalled = true;

                    Assert.AreEqual(_expectsTypeBuildValidationFailure, !success);
                    Assert.AreEqual(_expectsTypeBuildValidationFailure, Log4Tests.GetLogsCountAtLevelOrHigher(LogLevel.Error) > 0);
                    Assert.AreEqual(success, !_expectsTypeBuildValidationFailure);
                    Assert.AreEqual(GlobalsCoreAmbientContext.Context.CurrentInProgressDynamicAssemblyBuilder.BuildStatus,
                        _expectsTypeBuildValidationFailure ? AssemblyBuildStatus.Failed : AssemblyBuildStatus.Succeeded);
                }, true))
            {
                dynamicAssemblyBuilder.AddReferencedAssembly(Path.Combine(TestDllsFolder, "DynamicallyLoadedDlls", "TestProjects.DynamicallyLoadedAssembly1.dll"));
                dynamicAssemblyBuilder.AddReferencedAssembly(Path.Combine(TestDllsFolder, "DynamicallyLoadedDlls", "TestProjects.DynamicallyLoadedAssembly2.dll"));
                dynamicAssemblyBuilder.AddReferencedAssembly(Path.Combine(TestDllsFolder, "ContainerImplementations", "Autofac", "IoC.Configuration.Autofac.dll"));
                dynamicAssemblyBuilder.AddReferencedAssembly(Path.Combine(TestDllsFolder, "ThirdPartyLibs", "Autofac.dll"));
                dynamicAssemblyBuilder.AddReferencedAssembly(Path.Combine(TestDllsFolder, "ThirdPartyLibs", "Autofac.Extensions.DependencyInjection.dll"));

                var typesListFactoryTypeGenerator = IoCServiceFactoryAmbientContext.Context.CreateTypesListFactoryTypeGenerator(TypeBasedSimpleSerializerAggregator.GetDefaultSerializerAggregator());
                try
                {
                    var generatedTypeInfo = typesListFactoryTypeGenerator.GenerateType(GlobalsCoreAmbientContext.Context.CurrentInProgressDynamicAssemblyBuilder,
                        typeActionValidatorFactory,
                        dynamicCodeNameSpace,
                        returnedInstanceTypesForDefaultCase,
                        selectorParameterValues,
                        returnedTypesForEachParameterSelectorsSet);

                    generatedImplementationFullName = generatedTypeInfo?.TypeFullName;

                    GlobalsCoreAmbientContext.Context.CurrentInProgressDynamicAssemblyBuilder.AddCSharpFile(generatedTypeInfo.CSharpFileContents);

                    Assert.IsTrue(!expectsDynamicLibraryBuildFailure);
                }
                catch
                {
                    Assert.IsTrue(expectsDynamicLibraryBuildFailure);
                }

                Assert.AreEqual(GlobalsCoreAmbientContext.Context.CurrentInProgressDynamicAssemblyBuilder.BuildStatus == AssemblyBuildStatus.Aborted,
                    _expectsTypeBuildValidationFailure);
            }

            Assert.IsTrue(assemblyBuildCompleteWasCalled);

            // Load the newly created assembly.
            if (!expectsDynamicLibraryBuildFailure)
            {
                var diContainerBuilder = new DiContainerBuilder.DiContainerBuilder();
                using (var diContainerInfo = diContainerBuilder.StartCodeBasedDi(
                                                                   @"IoC.Configuration.Autofac.AutofacDiManager",
                                                                   Path.Combine(TestDllsFolder, "ContainerImplementations", "Autofac", "IoC.Configuration.Autofac.dll"),
                                                                   new ParameterInfo[0], Helpers.TestsEntryAssemblyFolder)
                                                               .WithoutPresetDiContainer()
                                                               .AddDiModules(new PrimitiveTypeDefaultBindingsModule(DateTime.MinValue, 0, 0, 0),
                                                                   new ReturnedValuesTypeRegistrationsModule(),
                                                                   new DynamicallyGeneratedImplementationsModule(
                                                                       new[]
                                                                       {
                                                                           new DynamicallyGeneratedImplementationsModule.InterfaceImplementationInfo(
                                                                               typeActionValidatorFactory, generatedImplementationFullName)
                                                                       },
                                                                       currentDynamicDllFilePath))
                                                               .RegisterModules().Start())
                {
                    var implementation = diContainerInfo.DiContainer.Resolve(typeActionValidatorFactory);
                    actionTestImplementation(diContainerInfo.DiContainer, implementation,
                        implementation.GetType().GetMethod("GetInstances"));
                }
            }
        }

        [TestMethod]
        public void TestFailure_InvalidReturnType()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory_InvalidReturnType)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_NoReturnTypesForDefaultCase()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                null,
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_NoSerializerForParameter()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory_NoSerializerForParameterType)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_NumberOfSelectorsDoesNotMatchNumberOfReturnedTypesCollection()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { },
                    new Type[] { }
                },
                new Type[] { },
                true, null);
        }

        [TestMethod]
        public void TestFailure_OutParameter()
        {
            TestFailure_RefParameter(false);
        }

        [TestMethod]
        public void TestFailure_RefParameter()
        {
            TestFailure_RefParameter(true);
        }

        private void TestFailure_RefParameter(bool isRefParameter)
        {
            TestDynamicAssemblyGeneration(new TypeInfo(isRefParameter ? typeof(IValidatorFactory_RefParameter) : typeof(IValidatorFactory_OutParameter)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_ReturnedItemTypeConversionError()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new[] {typeof(ClassThatDoesNotImplementIValidator)},
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_ReturnedItemTypeConversionError_InDefaultCase()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new[] {typeof(ClassThatDoesNotImplementIValidator)},
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_ReturnedItemTypeIsAbstractClass()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new[] {typeof(ValidatorAbstr)},
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_ReturnedItemTypeIsAbstractClass_InDefaultCase()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new[] {typeof(ValidatorAbstr)},
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_ReturnedItemTypeIsInterface()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new[] {typeof(IValidator)},
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_ReturnedItemTypeIsInterface_InDefaultCase()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new[] {typeof(IValidator)},
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_SelectorValueIsInvalid()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"not_a_number", "p1"},
                    new[] {"not_a_number_2", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_ShouldBeOnlyOneMethod()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory_ShouldBeOnlyOneMethod)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_ToManySelectorValues()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(IValidatorFactory)),
                new List<string[]>
                {
                    new[] {"1", "p1", "5"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestMethod]
        public void TestFailure_TypeIsNotInterface()
        {
            TestDynamicAssemblyGeneration(new TypeInfo(typeof(ValidatorFactory_NotInterface)),
                new List<string[]>
                {
                    new[] {"1", "p1"},
                    new[] {"3", "p2"}
                },
                new List<Type[]>
                {
                    new Type[] { },
                    new Type[] { }
                },
                new Type[] { },
                true, (assemblyPath, success, emitResult) => { });
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Log4Tests.ResetLogStatistics();
            _expectsTypeBuildValidationFailure = false;
            _assemblyResolver = CreateAssemblyResolver();
        }

        [TestMethod]
        public void TestSuccess_SelectorsIncludeValuesForAllParameters()
        {
            var returnedInstanceTypesForDefaultCase = new[] {_typeActionValidator1};

            var selectorParameterValues = new List<string[]>
            {
                new[] {"1", "p1"},
                new[] {"3", "p2"}
            };

            var returnedInstanceTypesForSelectorParameterValues = new List<Type[]>
            {
                new[] {_typeActionValidator2},
                new[] {_typeActionValidator2, _typeActionValidator1}
            };

            TestDynamicAssemblyGeneration(new TypeInfo(_dynamicallyLoadedAssembly2, "DynamicallyLoadedAssembly2.IActionValidatorFactory1"),
                selectorParameterValues,
                returnedInstanceTypesForSelectorParameterValues,
                returnedInstanceTypesForDefaultCase,
                false,
                (lifetimeScope, implementationObject, getInstancesMethodInfo) =>
                {
                    // Validate the first parameter selector
                    {
                        var validatorsList = ToList((IEnumerable) getInstancesMethodInfo.Invoke(implementationObject, new object[]
                        {
                            1, "p1"
                        }));

                        Assert.AreEqual(validatorsList.Count, 1);
                        Assert.AreEqual(validatorsList[0].GetType().FullName, _typeActionValidator2.FullName);
                    }

                    // Validate the second parameter selector
                    {
                        var validatorsList = ToList((IEnumerable) getInstancesMethodInfo.Invoke(implementationObject, new object[]
                        {
                            3, "p2"
                        }));

                        Assert.AreEqual(validatorsList.Count, 2);
                        Assert.AreEqual(validatorsList[0].GetType().FullName, _typeActionValidator2.FullName);
                        Assert.AreEqual(validatorsList[1].GetType().FullName, _typeActionValidator1.FullName);
                    }

                    // Validate the default case
                    {
                        var validatorsList = ToList((IEnumerable) getInstancesMethodInfo.Invoke(implementationObject, new object[]
                        {
                            3, "no-match"
                        }));

                        Assert.AreEqual(validatorsList.Count, 1);
                        Assert.AreEqual(validatorsList[0].GetType().FullName, _typeActionValidator1.FullName);
                    }
                });
        }

        /// <summary>
        ///     This test will fail if it is run after or before TestSuccess_SelectorsIncludeValuesForAllParameters, since
        ///     both these tests create/load the same assembly.
        ///     Run these two tests separately.
        /// </summary>
        [TestMethod]
        public void TestSuccess_SelectorsIncludeValuesForFirstNParameters()
        {
            var returnedInstanceTypesForDefaultCase = new[] {_typeActionValidator1};

            var selectorParameterValues = new List<string[]>
            {
                new[] {"1", "p1"},
                new[] {"1"},
                new[] {"3", "p2"},
                new[] {"3"}
            };

            var returnedInstanceTypesForSelectorParameterValues = new List<Type[]>
            {
                new[] {_typeActionValidator2},
                new[] {_typeActionValidator3, _typeActionValidator1},
                new[] {_typeActionValidator2, _typeActionValidator1},
                new[] {_typeActionValidator3}
            };

            TestDynamicAssemblyGeneration(new TypeInfo(_dynamicallyLoadedAssembly2, "DynamicallyLoadedAssembly2.IActionValidatorFactory1"),
                selectorParameterValues,
                returnedInstanceTypesForSelectorParameterValues,
                returnedInstanceTypesForDefaultCase,
                false,
                (lifetimeScope, implementationObject, getInstancesMethodInfo) =>
                {
                    // Validate the first parameter selector
                    {
                        var validatorsList = ToList((IEnumerable) getInstancesMethodInfo.Invoke(implementationObject, new object[]
                        {
                            1, "p1"
                        }));

                        Assert.AreEqual(validatorsList.Count, 1);
                        Assert.AreEqual(validatorsList[0].GetType().FullName, _typeActionValidator2.FullName);
                    }

                    // Validate the second parameter selector
                    {
                        var validatorsList = ToList((IEnumerable) getInstancesMethodInfo.Invoke(implementationObject, new object[]
                        {
                            1, "anything_with_param1=1"
                        }));

                        Assert.AreEqual(validatorsList.Count, 2);
                        Assert.AreEqual(validatorsList[0].GetType().FullName, _typeActionValidator3.FullName);
                        Assert.AreEqual(validatorsList[1].GetType().FullName, _typeActionValidator1.FullName);
                    }

                    // Validate the third parameter selector
                    {
                        var validatorsList = ToList((IEnumerable) getInstancesMethodInfo.Invoke(implementationObject, new object[]
                        {
                            3, "p2"
                        }));

                        Assert.AreEqual(validatorsList.Count, 2);
                        Assert.AreEqual(validatorsList[0].GetType().FullName, _typeActionValidator2.FullName);
                        Assert.AreEqual(validatorsList[1].GetType().FullName, _typeActionValidator1.FullName);
                    }

                    // Validate the fourth parameter selector
                    {
                        var validatorsList = ToList((IEnumerable) getInstancesMethodInfo.Invoke(implementationObject, new object[]
                        {
                            3, "anything_with_param1=3"
                        }));

                        // _typeActionValidator2, _typeActionValidator1
                        Assert.AreEqual(validatorsList.Count, 1);
                        Assert.AreEqual(validatorsList[0].GetType().FullName, _typeActionValidator3.FullName);
                    }

                    // Validate the default case
                    {
                        var validatorsList = ToList((IEnumerable) getInstancesMethodInfo.Invoke(implementationObject, new object[]
                        {
                            4, "no-match"
                        }));

                        Assert.AreEqual(validatorsList.Count, 1);
                        Assert.AreEqual(validatorsList[0].GetType().FullName, _typeActionValidator1.FullName);
                    }
                });
        }


        private static IList<object> ToList(IEnumerable enumerable)
        {
            var list = new List<object>();

            foreach (var item in enumerable)
                list.Add(item);

            return list;
        }

        #endregion

        #region Nested Types

        private class ClassThatDoesNotImplementIValidator
        {
            #region Member Functions

            public int GetSomething()
            {
                return 2;
            }

            #endregion
        }

        private interface IValidator
        {
            #region Current Type Interface

            int GetSomething();

            #endregion
        }

        private interface IValidatorFactory
        {
            #region Current Type Interface

            IEnumerable<IValidator> GetInstances(int parameter1, string parameter2);

            #endregion
        }

        private interface IValidatorFactory_InvalidReturnType
        {
            #region Current Type Interface

            IReadOnlyList<IValidator> GetInstances(int parameter1, string parameter2);

            #endregion
        }

        private interface IValidatorFactory_NoSerializerForParameterType
        {
            #region Current Type Interface

            IEnumerable<IValidator> GetInstances(TestParameterType parameter1, string parameter2);

            #endregion
        }

        private interface IValidatorFactory_OutParameter
        {
            #region Current Type Interface

            IEnumerable<IValidator> GetInstances(out int parameter1, string parameter2);

            #endregion
        }

        private interface IValidatorFactory_RefParameter
        {
            #region Current Type Interface

            IEnumerable<IValidator> GetInstances(ref int parameter1, string parameter2);

            #endregion
        }

        private interface IValidatorFactory_ShouldBeOnlyOneMethod
        {
            #region Current Type Interface

            IEnumerable<IValidator> GetInstances(int parameter1, string parameter2);
            IEnumerable<IValidator> GetInstances2(int parameter1);
            int GetSomething();

            #endregion
        }

        private class ReturnedValuesTypeRegistrationsModule : ModuleAbstr
        {
            #region Member Functions

            protected override void AddServiceRegistrations()
            {
                Bind(_typeActionValidator1).ToSelf();
                Bind(_typeActionValidator2).ToSelf();
                Bind(_typeActionValidator3).ToSelf();
            }

            #endregion
        }

        private class TestParameterType
        {
            #region Member Functions

            private int Something { get; set; }

            #endregion
        }

        private class TypeInfo
        {
            #region  Constructors

            public TypeInfo(Assembly assembly, string typeFullName)
            {
                Assembly = assembly;
                TypeFullName = typeFullName;
            }

            public TypeInfo(Type type)
            {
                Assembly = type.Assembly;
                TypeFullName = type.FullName;
            }

            #endregion

            #region Member Functions

            public Assembly Assembly { get; }
            public string TypeFullName { get; }

            #endregion
        }

        private class Validator1 : IValidator
        {
            #region IValidator Interface Implementation

            public int GetSomething()
            {
                return 1;
            }

            #endregion
        }

        private class Validator2 : IValidator
        {
            #region IValidator Interface Implementation

            public int GetSomething()
            {
                return 2;
            }

            #endregion
        }

        private abstract class ValidatorAbstr : IValidator
        {
            #region Current Type Interface

            public abstract int GetSomething();

            #endregion
        }

        private class ValidatorFactory_NotInterface
        {
            #region Member Functions

            private IReadOnlyList<IValidator> GetInstances(int parameter1, string parameter2)
            {
                return null;
            }

            #endregion
        }

        #endregion
    }
}