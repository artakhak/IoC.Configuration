using System.Collections.Generic;
using IoC.Configuration.Tests.TestTemplateFiles;
using IoC.Configuration.Tests.ValueImplementation.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using SharedServices.Implementations;
using SharedServices.Interfaces;

namespace IoC.Configuration.Tests.ValueImplementation
{
    public class ValueImplementationSuccessfulLoadTests : IoCConfigurationTestsForSuccessfullLoad
    {
        protected readonly static string ValueImplementationConfigurationRelativePath = "IoCConfiguration_valueImplementation.xml";

        [TestMethod]
        public void TestRegisterIfNotRegistered()
        {
            List<List<IAppInfo>> listOfAppInfosLists; // = DiContainer.Resolve<IEnumerable<List<IAppInfo>>>().ToList();

            switch (DiImplementationType)
            {
                case TestsSharedLibrary.DependencyInjection.DiImplementationType.Ninject:
                    // Ninject will not have a binding for IEnumerable<List<IAppInfo>>, if only one implementation is registered.
                    // Also, if multiple implementations are registered, trying to retrieve a single implementation will throw an exception.
                    // So the fact that we can resolve List<IAppInfo>, means that only one implementation is registered.

                    listOfAppInfosLists = new List<List<IAppInfo>>
                    {
                        DiContainer.Resolve<List<IAppInfo>>()
                    };

                    break;
                default:
                    listOfAppInfosLists = DiContainer.Resolve<IEnumerable<List<IAppInfo>>>().ToList();
                    break;
            }

            Assert.AreEqual(1, listOfAppInfosLists.Count);

            var appInfoList = listOfAppInfosLists[0];

            Assert.AreEqual(2, appInfoList.Count);
            Assert.AreEqual(5, appInfoList[0].AppId);
            Assert.AreEqual(7, appInfoList[1].AppId);
        }

        [TestMethod]
        public void TestSingletoneScope()
        {
            Assert.AreSame(DiContainer.Resolve<IReadOnlyList<IAppInfo>>(), DiContainer.Resolve<IReadOnlyList<IAppInfo>>());
        }

        [TestMethod]
        public void TestTransientScope()
        {
            Assert.AreNotSame(DiContainer.Resolve<IAppInfo>(), DiContainer.Resolve<IAppInfo>());
        }

        [TestMethod]
        public void TestCollectionAsValue()
        {
            var appInfosList = DiContainer.Resolve<IReadOnlyList<IAppInfo>>();

            Assert.AreEqual(2, appInfosList.Count);
            Assert.AreEqual(1, appInfosList[0].AppId);
            Assert.AreEqual(2, appInfosList[1].AppId);
        }

        [TestMethod]
        public void TestSettingAsValue()
        {
            var defaultIntValue = DiContainer.Resolve(typeof(int));

            Assert.AreEqual(38, Settings.GetSettingValueOrThrow<int>("defaultAppId"));
            Assert.AreEqual(38, defaultIntValue);
        }


        [TestMethod]
        public void TestConstructedValueAsValue()
        {
            var appInfo = DiContainer.Resolve<IAppInfo>();

            Assert.AreEqual(Settings.GetSettingValueOrThrow<int>("defaultAppId"), appInfo.AppId);
            Assert.AreEqual(38, appInfo.AppId);
        }

        [TestMethod]
        public void TestObjectAsValue()
        {
            var doubleValue = (double)DiContainer.Resolve(typeof(double));

            Assert.AreEqual(3.5, doubleValue);
        }

        [TestMethod]
        public void TestNonStaticClassMemberAsValue()
        {
            var dbConnectionProvider = DiContainer.Resolve<IDbConnectionProvider>();
            var sqlServerDbConnectionFromDbConnectionProvider = (SqlServerDbConnection)dbConnectionProvider.GetDbConnection();
            var sqlServerDbConnectionFromDi = (SqlServerDbConnection)DiContainer.Resolve<IDbConnection>();

            // Lets validate that a call GetDbConnection() is done on every resolution.
            Assert.AreNotSame(sqlServerDbConnectionFromDi, DiContainer.Resolve<IDbConnection>());

            Assert.AreEqual(sqlServerDbConnectionFromDbConnectionProvider.ServerName, sqlServerDbConnectionFromDi.ServerName);
            Assert.AreEqual(sqlServerDbConnectionFromDbConnectionProvider.DatabaseName, sqlServerDbConnectionFromDi.DatabaseName);
            Assert.AreEqual(sqlServerDbConnectionFromDbConnectionProvider.UserName, sqlServerDbConnectionFromDi.UserName);
            Assert.AreEqual(sqlServerDbConnectionFromDbConnectionProvider.Password, sqlServerDbConnectionFromDi.Password);
        }

        [TestMethod]
        public void TestStaticClassMemberAsValue()
        {
            var actionValidatorFromDi = (ActionValidator3)DiContainer.Resolve<IActionValidator>();
            var actionValidatorFromStaticMethod = (ActionValidator3)StaticMethods.GetActionValidator();

            Assert.AreEqual(StaticMethods.ActionValidator3ConstructorParameter, actionValidatorFromStaticMethod.IntParam);
            Assert.AreEqual(actionValidatorFromStaticMethod.IntParam, actionValidatorFromDi.IntParam);
        }

        [TestMethod]
        public void TestValueImplementationInPlugin()
        {
            var readOnlyListOfDoorType = Helpers.GetType("System.Collections.Generic.IReadOnlyList`1[[TestPluginAssembly1.Interfaces.IDoor, TestProjects.TestPluginAssembly1]]");

            var listOfDoor = (System.Collections.IList)DiContainer.Resolve(readOnlyListOfDoorType);
          
            Assert.AreEqual(2, listOfDoor.Count);

            var doorObjectType = listOfDoor[0].GetType();
            var doorColorProperty = doorObjectType.GetProperty("Color");
            var doorHeightProperty = doorObjectType.GetProperty("Height");

            Assert.AreEqual("TestPluginAssembly1.Implementations.Door", doorObjectType.FullName);
            
            Assert.AreEqual(4359924, doorColorProperty.GetValue(listOfDoor[0]));
            Assert.AreEqual(80.3, doorHeightProperty.GetValue(listOfDoor[0]));

            Assert.AreEqual(4359934, doorColorProperty.GetValue(listOfDoor[1]));
            Assert.AreEqual(85.2, doorHeightProperty.GetValue(listOfDoor[1]));
        }

        // TODO: Improve ClassMember slightly to store resolved owner objects of class members
        // instead of always getting these objects from the DI.

    }
}