using IoC.Configuration.Tests.Collection.Services;
using IoC.Configuration.Tests.TestTemplateFiles;
using NUnit.Framework;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Configuration.Tests.Collection
{
    public abstract class CollectionSuccessfulLoadTests : IoCConfigurationTestsForSuccessfulLoad
    {
        protected readonly static string CollectionConfigurationRelativePath = "IoCConfiguration_collection.xml";

        [Test]
        public void CollectionInjectedionInConstructedValueElement_Tests()
        {
            var collectionsSetting = Settings.GetSettingValueOrThrow<DemoCollectionInjection>("Collections");

            Assert.AreEqual(2, collectionsSetting.IntValues.Count);
            Assert.AreEqual(17, collectionsSetting.IntValues[0]);
            Assert.AreEqual(14, collectionsSetting.IntValues[1]);

            Assert.AreEqual(3, collectionsSetting.Texts.Count);
            Assert.AreEqual("Microsoft", collectionsSetting.Texts[0]);
            Assert.AreEqual("Google", collectionsSetting.Texts[1]);
            Assert.AreEqual("Amazon", collectionsSetting.Texts[2]);
        }

        [Test]
        public void CollectionInjectedionIntoModuleConstructor_Tests()
        {
            var module1 = (Module1)Configuration.DependencyInjection.Modules.Modules.FirstOrDefault(x => x.DiModule is Module1).DiModule;
            var module1InjectedValuesList = new List<int>(module1.Values);

            Assert.AreEqual(2, module1InjectedValuesList.Count);
            Assert.AreEqual(5, module1InjectedValuesList[0]);
            Assert.AreEqual(7, module1InjectedValuesList[1]);
        }
        [Test]
        public void CollectionInValueImplementation_Tests1()
        {
            var dbConnectionsList = DiContainer.Resolve<IReadOnlyList<IDbConnection>>();
            Assert.AreEqual(4, dbConnectionsList.Count);

            var sqliteDbConnection = (SqliteDbConnection)dbConnectionsList[0];
            Assert.AreEqual(@"c:\SQLiteFiles\MySqliteDb.sqlite", sqliteDbConnection.FilePath);

            var sqlServerDbConnection = (SqlServerDbConnection)dbConnectionsList[1];

            Assert.AreEqual("SQLSERVER2012", sqlServerDbConnection.ServerName);
            Assert.AreEqual("DB1", sqlServerDbConnection.DatabaseName);
            Assert.AreEqual("user1", sqlServerDbConnection.UserName);
            Assert.AreEqual("password123", sqlServerDbConnection.Password);

            sqlServerDbConnection = (SqlServerDbConnection)dbConnectionsList[2];

            Assert.AreEqual("SQLSERVER2016", sqlServerDbConnection.ServerName);
            Assert.AreEqual("DB2", sqlServerDbConnection.DatabaseName);
            Assert.AreEqual("user2", sqlServerDbConnection.UserName);
            Assert.AreEqual("password456", sqlServerDbConnection.Password);

            var mySqlServerDbConnection = dbConnectionsList[3];
            Assert.AreEqual("TestPluginAssembly1.Implementations.MySqlDbConnection", mySqlServerDbConnection.GetType().FullName);

            Assert.AreEqual("user=User1;password=123", mySqlServerDbConnection.ConnectionString);
        }

        [Test]
        public void CollectionInValueImplementation_Tests2()
        {
            var resolvedArray = DiContainer.Resolve<TestLocalTypesClass.IInterface1[]>();
            Assert.AreEqual(2, resolvedArray.Length);

            Assert.IsInstanceOf<TestLocalTypesClass.Interface1_Impl1>(resolvedArray[0]);
            Assert.AreEqual(13, resolvedArray[0].Value);

            Assert.IsInstanceOf<TestLocalTypesClass.Interface1_Impl1>(resolvedArray[1]);
            Assert.AreEqual(17, resolvedArray[1].Value);
        }

        [Test]
        public void CollectionsInjectedIntoSelfBoundService_Tests()
        {
            var collectionsTestClass1Instance = DiContainer.Resolve<CollectionsTestClass1>();

            // collectionsTestClass1Instance.ReadOnlyListValues
            Assert.AreEqual(3, collectionsTestClass1Instance.ReadOnlyListValues.Count);
            Assert.AreEqual(17, collectionsTestClass1Instance.ReadOnlyListValues[0]);
            Assert.AreEqual(24, collectionsTestClass1Instance.ReadOnlyListValues[1]);
            Assert.AreEqual(27, collectionsTestClass1Instance.ReadOnlyListValues[2]);

            // collectionsTestClass1Instance.ArrayValues
            Assert.AreEqual(2, collectionsTestClass1Instance.ArrayValues.Length);

            Assert.IsInstanceOf<Interface1_Impl>(collectionsTestClass1Instance.ArrayValues[0]);
            Assert.AreEqual(37, collectionsTestClass1Instance.ArrayValues[0].Property1);

            Assert.IsInstanceOf<Interface1_Impl>(collectionsTestClass1Instance.ArrayValues[1]);
            Assert.AreEqual(29, collectionsTestClass1Instance.ArrayValues[1].Property1);

            // collectionsTestClass1Instance.EnumerableValues
            var enumValuesToList = collectionsTestClass1Instance.EnumerableValues.ToList();

            Assert.AreEqual(3, enumValuesToList.Count);

            Assert.IsInstanceOf<Interface1_Impl>(enumValuesToList[0]);
            Assert.AreEqual(18, enumValuesToList[0].Property1);

            Assert.IsInstanceOf<Interface1_Impl>(enumValuesToList[1]);
            Assert.AreEqual(21, enumValuesToList[1].Property1);

            Assert.IsInstanceOf<Interface1_Impl>(enumValuesToList[2]);
            Assert.AreEqual(37, enumValuesToList[2].Property1);

            // collectionsTestClass1Instance.ListValues
            var listValues = collectionsTestClass1Instance.ListValues;

            Assert.AreEqual(3, listValues.Count);

            Assert.IsInstanceOf<Interface1_Impl>(listValues[0]);
            Assert.AreEqual(37, listValues[0].Property1);

            Assert.IsInstanceOf<Interface1_Impl>(listValues[1]);
            Assert.AreEqual(21, listValues[1].Property1);

            Assert.IsInstanceOf<Interface1_Impl>(listValues[2]);
            Assert.AreEqual(139, listValues[2].Property1);
        }

        [Test]
        public void CollectionsInAutoService_Tests()
        {
            var auroService = DiContainer.Resolve<IAutoService1>();

            var getAllActionIdsReturnValue = auroService.GetAllActionIds(3);
            Assert.AreEqual(2, getAllActionIdsReturnValue.Count);
            Assert.AreEqual(27, getAllActionIdsReturnValue[0]);
            Assert.AreEqual(17, getAllActionIdsReturnValue[1]);

            getAllActionIdsReturnValue = auroService.GetAllActionIds(12);
            Assert.AreEqual(3, getAllActionIdsReturnValue.Count);
            Assert.AreEqual(13, getAllActionIdsReturnValue[0]);
            Assert.AreEqual(27, getAllActionIdsReturnValue[1]);
            Assert.AreEqual(17, getAllActionIdsReturnValue[2]);
        }
    }
}