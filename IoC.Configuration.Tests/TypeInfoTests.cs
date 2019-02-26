using System;
using System.Collections.Generic;
using System.Text;
using IoC.Configuration.ConfigurationFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IoC.Configuration.Tests
{
    [TestClass]
    public class TypeInfoTests
    {
        private Mock<IAssembly> _plugin1AssemblyMock;
        private Mock<IAssembly> _plugin2AssemblyMock;
        private Mock<IAssembly> _nonPluginAssemblyMock;

        private Mock<IPluginElement> _pluginElementMock1;
        private Mock<IPluginElement> _pluginElementMock2;

        [TestInitialize]
        public void TestInitialize()
        {
            Mock<IPluginElement> CreatePluginElementMock(string pluginName)
            {
                var pluginMock = new Mock<IPluginElement>();
                pluginMock.SetupGet(x => x.Name).Returns(pluginName);

                return pluginMock;
            }

            Mock<IAssembly> CreateAssemblyMock(string assemblyName, string assemblyAlias, IPluginElement pluginElement = null)
            {
                var assemblyMock = new Mock<IAssembly>();
                assemblyMock.SetupGet(x => x.Name).Returns(assemblyName);
                assemblyMock.SetupGet(x => x.Alias).Returns(assemblyAlias);
                assemblyMock.SetupGet(x => x.Plugin).Returns(pluginElement);

                return assemblyMock;
            }

            _pluginElementMock1 = CreatePluginElementMock("Plugin1");
            _pluginElementMock2 = CreatePluginElementMock("Plugin2");

            _plugin1AssemblyMock = CreateAssemblyMock("Dynamic.Plugin1Assembly", "plugin1Assembly", _pluginElementMock1.Object);
            _plugin2AssemblyMock = CreateAssemblyMock("Dynamic.Plugin2Assembly", "plugin2Assembly", _pluginElementMock2.Object);
            _nonPluginAssemblyMock = CreateAssemblyMock("Dynamic.NonPluginAssembly", "nonPluginAssembly", null);
        }

        [TestMethod]
        public void NonGenericTypeWithoutPluginTest()
        {
            var typeInfo = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface1), _nonPluginAssemblyMock.Object, new ITypeInfo[] { });

            var uniquePluginTypes = typeInfo.GetUniquePluginTypes();

            Assert.IsNotNull(uniquePluginTypes);
            Assert.AreEqual(0, uniquePluginTypes.Count);
        }

        [TestMethod]
        public void NonGenericTypeWithPluginTest()
        {
            var typeInfo = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface1), _plugin2AssemblyMock.Object, new ITypeInfo[] { });

            var uniquePluginTypes = typeInfo.GetUniquePluginTypes();

            Assert.IsNotNull(uniquePluginTypes);
            Assert.AreEqual(1, uniquePluginTypes.Count);
            Assert.AreEqual(typeof(SharedServices.Interfaces.IInterface1), uniquePluginTypes[0].Type);
            Assert.AreEqual(_pluginElementMock2.Object, uniquePluginTypes[0].Assembly.Plugin);
        }

        [TestMethod]
        public void GenericTypeWithOnePluginTest1()
        {
            var typeInfo1 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface1), _plugin1AssemblyMock.Object, new ITypeInfo[] { });
            
            var typeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("System.Collections.Generic.List`1"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo1 });

            var uniquePluginTypes = typeInfo.GetUniquePluginTypes();

            Assert.IsNotNull(uniquePluginTypes);
            Assert.AreEqual(1, uniquePluginTypes.Count);
            Assert.AreEqual(typeof(SharedServices.Interfaces.IInterface1), uniquePluginTypes[0].Type);
            Assert.AreEqual(_pluginElementMock1.Object, uniquePluginTypes[0].Assembly.Plugin);
        }

        [TestMethod]
        public void GenericTypeWithTwoPluginsTest()
        {
            var typeInfo1 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface1), _plugin1AssemblyMock.Object, new ITypeInfo[] { });
            var typeInfo2 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface2), _plugin2AssemblyMock.Object, new ITypeInfo[] { });
           
            var typeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo1, typeInfo2 });

            var uniquePluginTypes = typeInfo.GetUniquePluginTypes();

            Assert.IsNotNull(uniquePluginTypes);
            Assert.AreEqual(2, uniquePluginTypes.Count);

            Assert.AreEqual(typeof(SharedServices.Interfaces.IInterface1), uniquePluginTypes[0].Type);
            Assert.AreEqual(_pluginElementMock1.Object, uniquePluginTypes[0].Assembly.Plugin);

            Assert.AreEqual(typeof(SharedServices.Interfaces.IInterface2), uniquePluginTypes[1].Type);
            Assert.AreEqual(_pluginElementMock2.Object, uniquePluginTypes[1].Assembly.Plugin);
        }

        [TestMethod]
        public void GenericTypeWithOnePluginTest()
        {
            var typeInfo1 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface1), _plugin1AssemblyMock.Object, new ITypeInfo[] { });
            var typeInfo2 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface2), _plugin1AssemblyMock.Object, new ITypeInfo[] { });

            var typeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo1, typeInfo2 });

            var uniquePluginTypes = typeInfo.GetUniquePluginTypes();

            Assert.IsNotNull(uniquePluginTypes);
            Assert.AreEqual(1, uniquePluginTypes.Count);

            Assert.AreEqual(typeof(SharedServices.Interfaces.IInterface1), uniquePluginTypes[0].Type);
            Assert.AreEqual(_pluginElementMock1.Object, uniquePluginTypes[0].Assembly.Plugin);
        }

        [TestMethod]
        public void GenericTypeWithTwoPluginsTest2()
        {
            var typeInfo1 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface1), _plugin1AssemblyMock.Object, new ITypeInfo[] { });
            var typeInfo2 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface2), _plugin2AssemblyMock.Object, new ITypeInfo[] { });

            var typeInfo3 = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo1, typeInfo2 });
            var typeInfo4 = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo1, typeInfo2 });

            var typeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo3, typeInfo4 });

            var uniquePluginTypes = typeInfo.GetUniquePluginTypes();

            Assert.IsNotNull(uniquePluginTypes);
            Assert.AreEqual(2, uniquePluginTypes.Count);

            Assert.AreEqual(typeof(SharedServices.Interfaces.IInterface1), uniquePluginTypes[0].Type);
            Assert.AreEqual(_pluginElementMock1.Object, uniquePluginTypes[0].Assembly.Plugin);

            Assert.AreEqual(typeof(SharedServices.Interfaces.IInterface2), uniquePluginTypes[1].Type);
            Assert.AreEqual(_pluginElementMock2.Object, uniquePluginTypes[1].Assembly.Plugin);
        }

        [TestMethod]
        public void GenericTypeWithOnePluginsTest2()
        {
            var typeInfo1 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface1), _plugin1AssemblyMock.Object, new ITypeInfo[] { });
            var typeInfo2 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface2), _plugin1AssemblyMock.Object, new ITypeInfo[] { });

            var typeInfo3 = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo1, typeInfo2 });
            var typeInfo4 = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo1, typeInfo2 });

            var typeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo3, typeInfo4 });

            var uniquePluginTypes = typeInfo.GetUniquePluginTypes();

            Assert.IsNotNull(uniquePluginTypes);
            Assert.AreEqual(1, uniquePluginTypes.Count);

            Assert.AreEqual(typeof(SharedServices.Interfaces.IInterface1), uniquePluginTypes[0].Type);
            Assert.AreEqual(_pluginElementMock1.Object, uniquePluginTypes[0].Assembly.Plugin);
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void ProcessTypeAndGenericParametersTests(bool testStopProcessing)
        {
            var typeInfo1 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface1), _nonPluginAssemblyMock.Object, new ITypeInfo[] { });
            var typeInfo2 = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric1Param1`1"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo1 });

            var typeInfo3 = TypeInfo.CreateNonArrayTypeInfo(typeof(SharedServices.Interfaces.IInterface2), _nonPluginAssemblyMock.Object, new ITypeInfo[] { });
            var typeInfo4 = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric1Param2`1"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo3 });
           
            var typeInfo = TypeInfo.CreateNonArrayTypeInfo(Type.GetType("IoC.Configuration.Tests.TypeInfoTests+IGneneric2Param`2"), _nonPluginAssemblyMock.Object, new ITypeInfo[] { typeInfo2, typeInfo4 });

            List<ITypeInfo> processedTypeInfos = new List<ITypeInfo>(10);

            void typeInfoProcessor(ITypeInfo typeInfoParam, ref bool stopProcessingParam)
            {
                processedTypeInfos.Add(typeInfoParam);

                if (typeInfoParam == typeInfo2 && testStopProcessing)
                    stopProcessingParam = true;
            }

            bool stopProcessing = false;
            typeInfo.ProcessTypeAndGenericParameters(typeInfoProcessor, ref stopProcessing);

            Assert.AreSame(typeInfo, processedTypeInfos[0]);
            Assert.AreSame(typeInfo2, processedTypeInfos[1]);

            if (testStopProcessing)
            {
                Assert.IsTrue(stopProcessing);
                Assert.AreEqual(2, processedTypeInfos.Count);
            }
            else
            {
                Assert.AreEqual(5, processedTypeInfos.Count);

                Assert.AreSame(typeInfo1, processedTypeInfos[2]);

                Assert.AreSame(typeInfo4, processedTypeInfos[3]);
                Assert.AreSame(typeInfo3, processedTypeInfos[4]);
            }
        }

        public interface IGneneric2Param<T,K>
        {

        }

        public interface IGneneric1Param1<T>
        {

        }

        public interface IGneneric1Param2<T>
        {

        }
    }
}
