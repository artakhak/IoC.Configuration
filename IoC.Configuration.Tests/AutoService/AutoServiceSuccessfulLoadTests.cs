using IoC.Configuration.Tests.AutoService.Services;
using IoC.Configuration.Tests.TestTemplateFiles;
using SharedServices.DataContracts;
using SharedServices.Implementations;
using SharedServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IoC.Configuration.Tests.AutoService
{
    public abstract class AutoServiceCustomSuccessfulLoadTests : IoCConfigurationTestsForSuccessfulLoad
    {
        // Set UseOverviewConfigurationFile to true, to test that auto-generated services are properly
        // setup in overview configuration file.
        private static bool UseOverviewConfigurationFile = false;

        protected static readonly string AutoServiceConfigurationRelativePath =
            UseOverviewConfigurationFile ? "IoCConfiguration_Overview.xml" : "IoCConfiguration_autoService.xml";

        [Test]
        public void AutoService_IProjectGuids_Tests()
        {
            var projectGuids = DiContainer.Resolve<IProjectGuids>();

            Assert.AreEqual(new Guid("966FE6A6-76AC-4895-84B2-47E27E58FD02"), projectGuids.Project1);
            Assert.AreEqual(new Guid("AC4EE351-CE69-4F89-A362-F833489FD9A1"), projectGuids.Project2);
            Assert.AreEqual(new Guid("1E08071B-D02C-4830-AE3C-C9E78A29EA37"), projectGuids.GetDefaultProject());

            var strParam = "";
            projectGuids.NotImplementedReturnedValueVoid(1, out var intParam1, ref strParam);
            Assert.AreEqual(0, projectGuids.NotImplementedReturnedValueInt(1, out intParam1, ref strParam));
            Assert.AreEqual(0, projectGuids.NotImplementedProperty);
            projectGuids.NotImplementedProperty = 3;

            Assert.AreEqual(3, projectGuids.NotImplementedProperty);
        }

        [Test]
        public void AutoService_IActionValidatorFactory_DefaultActionValidator_Tests()
        {
            var actionValidatorFactory = DiContainer.Resolve<IActionValidatorFactory>();

            Assert.AreSame(DiContainer.Resolve<ActionValidatorDefault>(), actionValidatorFactory.DefaultActionValidator);
        }

        [Test]
        public void AutoService_IActionValidatorFactory_PublicProjectId_Tests()
        {
            var actionValidatorFactory = DiContainer.Resolve<IActionValidatorFactory>();

            Assert.AreEqual(Guid.Parse("95E352DD-5C79-49D0-BD51-D62153570B61"), actionValidatorFactory.PublicProjectId);
        }

        [Test]
        public void AutoService_IActionValidatorFactory_GetValidators_1_Tests1()
        {
            var actionValidatorFactory = DiContainer.Resolve<IActionValidatorFactory>();
            var settings = DiContainer.Resolve<ISettings>();

            var actionValidatorValuesProvider = DiContainer.Resolve<IActionValidatorValuesProvider>();
            var projectGuids = DiContainer.Resolve<IProjectGuids>();

            // Method call 1
            IReadOnlyList<IActionValidator> actionValidators = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList, new Guid("8663708F-C707-47E1-AEDC-2CD9291AD4CB"));

            Assert.AreEqual(6, actionValidators.Count);

            // IActionValidator 1

            Assert.AreSame(typeof(ActionValidator3), actionValidators[0].GetType());
            Assert.AreEqual(7, ((ActionValidator3)actionValidators[0]).IntParam);

            // IActionValidator 2
            Assert.AreSame(DiContainer.Resolve<ActionValidatorWithDependencyOnActionValidatorFactory>(), actionValidators[1]);
            Assert.AreSame(actionValidatorFactory, ((ActionValidatorWithDependencyOnActionValidatorFactory)actionValidators[1]).ActionValidatorFactory);

            // IActionValidator 3

            ActionValidator1 actionValidator1_3 = actionValidators[2] as ActionValidator1;

            Assert.IsNotNull(actionValidator1_3);
            Assert.AreSame(DiContainer.Resolve<Services.IInterface1>(), actionValidator1_3.Property1);
            Assert.AreSame(DiContainer.Resolve< Services.IInterface2 >(), actionValidator1_3.Property2);

            // IActionValidator 4
            Assert.AreSame(DiContainer.Resolve(Helpers.GetType("TestPluginAssembly1.Implementations.Plugin1ActionValidator")),
                actionValidators[3]);

            // IActionValidator 5
            Assert.AreSame(StaticAndConstMembers.ActionValidator1, actionValidators[4]);
            Assert.IsTrue(StaticAndConstMembers.ActionValidator1.GetType().IsNestedPrivate);

            // IActionValidator 6
           
            Assert.AreSame(actionValidatorValuesProvider.DefaultActionValidator, actionValidators[5]);
            Assert.IsTrue(actionValidatorValuesProvider.DefaultActionValidator.GetType().IsNestedPrivate);

            // Lets explicitly check that plugin 3 value is not included. This is already tested by the checks above, but lets make this explicit
            // for clarity.

            Assert.IsFalse(actionValidators.Any(x => x.GetType().FullName.Contains("TestPluginAssembly3.Implementations.Plugin3ActionValidator")));

            // Method call 2
            actionValidators = actionValidatorFactory.GetValidators(ActionTypes.ViewFileContents, 
                settings.GetSettingValueOrThrow<Guid>("Project1Guid"));

            Assert.AreEqual(2, actionValidators.Count);

            // IActionValidator 1
            Assert.AreSame(DiContainer.Resolve<ActionValidator1>(), actionValidators[0]);

            // IActionValidator 2
            Assert.AreSame(actionValidatorValuesProvider.GetViewOnlyActionvalidator(), actionValidators[1]);

            Assert.IsTrue(actionValidatorValuesProvider.GetViewOnlyActionvalidator().GetType().IsNestedPrivate);

            // Method call 3
            actionValidators = actionValidatorFactory.GetValidators(StaticAndConstMembers.DefaultActionType,
                projectGuids.Project1);

            Assert.AreEqual(0, actionValidators.Count);

            // Method call 4

            actionValidators = actionValidatorFactory.GetValidators(ActionTypes.ViewFileContents,
                StaticAndConstMembers.GetDefaultProjectGuid());

            Assert.AreEqual(2, actionValidators.Count);
            Assert.AreSame(DiContainer.Resolve<ActionValidator2>(), actionValidators[0]);
            Assert.AreSame(DiContainer.Resolve<ActionValidator1>(), actionValidators[1]);

            // Method call 5
            actionValidators = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList,
                actionValidatorFactory.PublicProjectId);

            Assert.AreEqual(1, actionValidators.Count);
            Assert.AreSame(actionValidatorFactory.DefaultActionValidator, actionValidators[0]);

            // Method call 6
            // This is the default case (else case).
            actionValidators = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList, Guid.Empty);
            Assert.AreEqual(3, actionValidators.Count);

            Assert.AreSame(actionValidatorFactory.DefaultActionValidator, actionValidators[0]);

            Assert.AreSame(DiContainer.Resolve<ActionValidator3>(), actionValidators[1]);
            // The value of property ActionValidator3.IntParam is set from int constructor parameter. 
            // The value of int parameter injected into constructor is int.MinValue,
            // since we added a module IoC.Configuration.Tests.PrimitiveTypeDefaultBindingsModule in modules section,
            // with defaultInt32 parameter=int.MinValue.
            Assert.AreEqual(int.MinValue, ((ActionValidator3)actionValidators[1]).IntParam);

            Assert.AreEqual("DynamicallyLoadedAssembly2.ActionValidator4", actionValidators[2].GetType().FullName);
        }

        [Test]
        public void AutoService_IActionValidatorFactory_GetValidators_2_Tests()
        {
            var actionValidatorFactory = DiContainer.Resolve<IActionValidatorFactory>();
            var settings = DiContainer.Resolve<ISettings>();

            var actionValidatorValuesProvider = DiContainer.Resolve<IActionValidatorValuesProvider>();
            var projectGuids = DiContainer.Resolve<IProjectGuids>();

            // Method call 1
            IReadOnlyList<IActionValidator> actionValidators = actionValidatorFactory.GetValidators(0, "8663708F-C707-47E1-AEDC-2CD9291AD4CB");

            Assert.AreEqual(2, actionValidators.Count);

            Assert.AreSame(DiContainer.Resolve<ActionValidator3>(), actionValidators[0]);
            Assert.AreEqual(int.MinValue, ((ActionValidator3)actionValidators[0]).IntParam);

            Assert.AreEqual(DiContainer.Resolve<ActionValidator4>(), actionValidators[1]);
            // IoC.Configuration automatically creates self-bindings for concrete classes with public constructors,
            // if one was not specified, using a DiResolutionScope.Singleton.
            // Since we provided a transient self-binding for ActionValidator4 in module
            // IoC.Configuration.Tests.AutoService.AutoServiceTestsModule, lets validate that 
            // binding in AutoServiceTestsModule is the effective binding.
            Assert.AreNotSame(DiContainer.Resolve<ActionValidator4>(), DiContainer.Resolve<ActionValidator4>());
            Assert.AreEqual(19, DiContainer.Resolve<ActionValidator4>().Property1);

            // Method call 2 (the default case
            actionValidators = actionValidatorFactory.GetValidators(0, "000-invalid-guid-000");

            Assert.AreEqual(4, actionValidators.Count);
            Assert.AreSame(actionValidatorFactory.DefaultActionValidator, actionValidators[0]);

            Assert.AreSame(DiContainer.Resolve<ActionValidator3>(), actionValidators[1]);
            Assert.AreEqual(int.MinValue, ((ActionValidator3)actionValidators[1]).IntParam);

            Assert.AreSame(StaticAndConstMembers.GetDefaultActionValidator(), actionValidators[2]);
            Assert.IsTrue(StaticAndConstMembers.GetDefaultActionValidator().GetType().IsNestedPrivate);

            Assert.AreSame(actionValidatorValuesProvider.AdminLevelActionValidator, actionValidators[3]);
            Assert.IsTrue(actionValidatorValuesProvider.DefaultActionValidator.GetType().IsNestedPrivate);
        }

        [Test]
        public void AutoService_IMemberAmbiguityDemo_Tests()
        {
            var memberAmbiguityDemo = DiContainer.Resolve<IMemberAmbiguityDemo>();

            // IMemberAmbiguityDemo.GetIntValues(...)
            var intValuesReadOnlyList = memberAmbiguityDemo.GetIntValues(1, "str1");
            Assert.AreEqual(1, intValuesReadOnlyList.Count);
            Assert.AreEqual(17, intValuesReadOnlyList[0]);

            intValuesReadOnlyList = memberAmbiguityDemo.GetIntValues(-1, "default");
            Assert.AreEqual(2, intValuesReadOnlyList.Count);
            Assert.AreEqual(18, intValuesReadOnlyList[0]);
            Assert.AreEqual(19, intValuesReadOnlyList[1]);

            // IMemberAmbiguityDemo_Parent2.GetIntValues(...)
            IMemberAmbiguityDemo_Parent2 memberAmbiguityDemo_Parent2 = memberAmbiguityDemo;

            // IMemberAmbiguityDemo_Parent2.GetIntValues() should generate the same values as 
            // IMemberAmbiguityDemo.GetIntValues(), since it is not auto-implemented,
            // however has the same signature and compatible return type.
            intValuesReadOnlyList = memberAmbiguityDemo_Parent2.GetIntValues(1, "str1");
            Assert.AreEqual(1, intValuesReadOnlyList.Count);
            Assert.AreEqual(17, intValuesReadOnlyList[0]);

            intValuesReadOnlyList = memberAmbiguityDemo_Parent2.GetIntValues(-1, "default");
            Assert.AreEqual(2, intValuesReadOnlyList.Count);
            Assert.AreEqual(18, intValuesReadOnlyList[0]);
            Assert.AreEqual(19, intValuesReadOnlyList[1]);

            // IMemberAmbiguityDemo_Parent1_Parent.GetIntValues(...)
            IMemberAmbiguityDemo_Parent1_Parent ambiguityDemo_Parent1_Parent = memberAmbiguityDemo;

            // IMemberAmbiguityDemo_Parent1_Parent.GetIntValues(int, string) has similar signature as
            // IMemberAmbiguityDemo.GetIntValues(int, string), and its return type 
            // is assignable from IMemberAmbiguityDemo.GetIntValues(int, string).
            // Therefore, even though no auto-implementation is provided for IMemberAmbiguityDemo_Parent1_Parent.GetIntValues(int, string) 
            // in configuration file, IoC.Configuration generates an auto-implementation, that returns the same values as
            // IMemberAmbiguityDemo.GetIntValues(int, string)
            var intValuesEnumerableToList = ambiguityDemo_Parent1_Parent.GetIntValues(1, "str1").ToList();
            Assert.AreEqual(1, intValuesEnumerableToList.Count);
            Assert.AreEqual(17, intValuesEnumerableToList[0]);

            intValuesEnumerableToList = ambiguityDemo_Parent1_Parent.GetIntValues(-1, "default").ToList();
            Assert.AreEqual(2, intValuesReadOnlyList.Count);
            Assert.AreEqual(18, intValuesReadOnlyList[0]);
            Assert.AreEqual(19, intValuesReadOnlyList[1]);

            // IMemberAmbiguityDemo_Parent3.GetIntValues(...)
            IMemberAmbiguityDemo_Parent3 memberAmbiguityDemo_Parent3 = memberAmbiguityDemo;
            intValuesReadOnlyList = memberAmbiguityDemo_Parent3.GetIntValues(-1, "anything");
            Assert.AreEqual(1, intValuesReadOnlyList.Count);
            Assert.AreEqual(3, intValuesReadOnlyList[0]);

            IMemberAmbiguityDemo_Parent1 memberAmbiguityDemo_Parent1 = memberAmbiguityDemo;
            // No implementation is provided for IMemberAmbiguityDemo_Parent1.GetIntValues(int, string) 
            // and no implementation is provided in configuration file that has similar signature, and compatible return type,
            // therefore a default implementation is generated, that returns default(int).
            Assert.AreEqual(0, memberAmbiguityDemo_Parent1.GetIntValues(1, "str1"));

            // IMemberAmbiguityDemo_Parent2.GetDbConnection()
            var sqliteDbConnection = (SqliteDbConnection)memberAmbiguityDemo_Parent2.GetDbConnection(Guid.Empty);
            Assert.AreEqual(@"c:\mySqliteDatabase.sqlite", sqliteDbConnection.FilePath);

            // IMemberAmbiguityDemo_Parent1.GetDbConnection()
            // Even though no implementation is provided for IMemberAmbiguityDemo_Parent1.GetDbConnection()
            // in configuration file, IMemberAmbiguityDemo_Parent1.GetDbConnection() will return the same
            // value as IMemberAmbiguityDemo_Parent2.GetDbConnection(), for which there is an implementation in configuration file
            // since both methods have similar signature, and the return type of IMemberAmbiguityDemo_Parent1.GetDbConnection()
            // is assignable from the return type of IMemberAmbiguityDemo_Parent2.GetDbConnection()
            sqliteDbConnection = (SqliteDbConnection)memberAmbiguityDemo_Parent1.GetDbConnection(Guid.Empty);
            Assert.AreEqual(@"c:\mySqliteDatabase.sqlite", sqliteDbConnection.FilePath);

            // IMemberAmbiguityDemo_Parent1.DefaultDbConnection
            sqliteDbConnection = (SqliteDbConnection)memberAmbiguityDemo_Parent1.DefaultDbConnection;
            Assert.AreEqual(@"c:\IMemberAmbiguityDemo_Parent1_Db.sqlite", sqliteDbConnection.FilePath);

            // IMemberAmbiguityDemo_Parent2.DefaultDbConnection
            sqliteDbConnection = (SqliteDbConnection)memberAmbiguityDemo_Parent2.DefaultDbConnection;
            Assert.AreEqual(@"c:\IMemberAmbiguityDemo_Parent2_Db.sqlite", sqliteDbConnection.FilePath);

            // IMemberAmbiguityDemo_Parent3.DefaultDbConnection
            Assert.AreEqual(((SqliteDbConnection)memberAmbiguityDemo_Parent1.DefaultDbConnection).FilePath, 
                ((SqliteDbConnection)memberAmbiguityDemo_Parent3.DefaultDbConnection).FilePath);

            // IMemberAmbiguityDemo_Parent2.GetNumericValue()
            Assert.AreEqual(17.3, memberAmbiguityDemo_Parent2.GetNumericValue());
            Assert.IsInstanceOf<double>(memberAmbiguityDemo_Parent2.GetNumericValue());

            // IMemberAmbiguityDemo_Parent1_Parent.GetNumericValue()
            Assert.AreEqual(19, ambiguityDemo_Parent1_Parent.GetNumericValue());
            Assert.IsInstanceOf<int>(ambiguityDemo_Parent1_Parent.GetNumericValue());

            // IMemberAmbiguityDemo_Parent1.NumericValue
            Assert.AreEqual(18.2, memberAmbiguityDemo_Parent1.NumericValue);
            Assert.IsInstanceOf<double>(memberAmbiguityDemo_Parent1.NumericValue);

            // IMemberAmbiguityDemo_Parent2.NumericValue
            Assert.AreEqual(14, memberAmbiguityDemo_Parent2.NumericValue);
            Assert.IsInstanceOf<int>(memberAmbiguityDemo_Parent2.NumericValue);

            // int IMemberAmbiguityDemo.MethodWithOptionalParameters(int param1, double param2 = 3.5, int param3=7);
            Assert.AreEqual(17, memberAmbiguityDemo.MethodWithOptionalParameters(3, 3.5, 7));
            Assert.AreEqual(17, memberAmbiguityDemo.MethodWithOptionalParameters(3, 3.5));
            Assert.AreEqual(17, memberAmbiguityDemo.MethodWithOptionalParameters(3));

          
            Assert.AreEqual(18, memberAmbiguityDemo.MethodWithOptionalParameters(3, 3.5, 8));
            Assert.AreEqual(18, memberAmbiguityDemo.MethodWithOptionalParameters(3, 3.51, 7));

            // Test some default implementations for methods with ref and out parameters
            // IEnumerable<int> IMemberAmbiguityDemo_Parent1_Parent.GetIntValues(int param1, ref string param2);
            string strParam = "str value";
            var enumarableOfInt = ambiguityDemo_Parent1_Parent.GetIntValues(1, ref strParam);

            Assert.IsNull(enumarableOfInt);
            Assert.IsTrue(string.Equals("str value", strParam, StringComparison.Ordinal ));

            // IEnumerable<int> IMemberAmbiguityDemo_Parent1_Parent.GetIntValues(out int param1, string param2);
            enumarableOfInt = ambiguityDemo_Parent1_Parent.GetIntValues(out var outParam, "some value");
            Assert.IsNull(enumarableOfInt);
            Assert.AreEqual(0, outParam);
        }

        [Test]
        public void AutoService_ResourceAccessValidatorFactory_Tests()
        {
            var serviceType = Helpers.GetType("TestPluginAssembly1.Interfaces.IResourceAccessValidatorFactory");
            var resourceAccessValidatorFactory = DiContainer.Resolve(serviceType);

            var getValidatorsMethodInfo = serviceType.GetMethod("GetValidators", new Type[] { typeof(string) });

            List<object> getListOfValidators(object resolvedObject)
            {
                List<object> validatorsList = new List<object>();
                var validatorsEnum = (System.Collections.IEnumerable)resolvedObject;

                foreach (var validator in validatorsEnum)
                    validatorsList.Add(validator);

                return validatorsList;
            }

            // IResourceAccessValidatorFactory.GetValidators("public_pages");
            var validators = getListOfValidators(getValidatorsMethodInfo.Invoke(resourceAccessValidatorFactory, new string[] { "public_pages" }));

            Assert.AreEqual(1, validators.Count);
            Assert.AreEqual(Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator1"), validators[0].GetType());

            // IResourceAccessValidatorFactory.GetValidators("admin_pages");
            validators = getListOfValidators(getValidatorsMethodInfo.Invoke(resourceAccessValidatorFactory, new string[] { "admin_pages" }));

            Assert.AreEqual(2, validators.Count);
            Assert.AreEqual(Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator1"), validators[0].GetType());
            Assert.AreEqual(Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator2"), validators[1].GetType());

            // IResourceAccessValidatorFactory.GetValidators("anything_else");
            validators = getListOfValidators(getValidatorsMethodInfo.Invoke(resourceAccessValidatorFactory, new string[] { "anything_else" }));

            Assert.AreEqual(2, validators.Count);
            Assert.AreEqual(Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator2"), validators[0].GetType());
            Assert.AreEqual(Helpers.GetType("TestPluginAssembly1.Interfaces.ResourceAccessValidator1"), validators[1].GetType());
        }

        [Test]
        public void ReUseValueAttribute_Tests()
        {
            var actionValidatorFactory = DiContainer.Resolve<IActionValidatorFactory>();

            var settings = DiContainer.Resolve<ISettings>();

            var actionValidatorValuesProvider = DiContainer.Resolve<IActionValidatorValuesProvider>();
            var projectGuids = DiContainer.Resolve<IProjectGuids>();

            void validateReuseResult(IReadOnlyList<IActionValidator> actionValidators1, 
                                     IReadOnlyList<IActionValidator> actionValidators2,
                                     bool isReuseAttributeUsed, int numOfExpectedItems)
            {
                Assert.AreEqual(numOfExpectedItems, actionValidators1.Count);
                Assert.AreEqual(numOfExpectedItems, actionValidators1.Count);
               
                if (isReuseAttributeUsed)
                    Assert.AreSame(actionValidators1, actionValidators2);
                else
                    Assert.AreNotSame(actionValidators1, actionValidators2);
            }

            // IActionValidatorFactory.GetValidators(ActionTypes, Guid) uses reuseValue=true
            var validators1 = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList, Guid.Parse("8663708F-C707-47E1-AEDC-2CD9291AD4CB"));
            var validators2 = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList, Guid.Parse("8663708F-C707-47E1-AEDC-2CD9291AD4CB"));
            validateReuseResult(validators1, validators2, true, 6);

            validators1 = actionValidatorFactory.GetValidators(ActionTypes.ViewFileContents, settings.GetSettingValueOrThrow<Guid>("Project1Guid"));
            validators2 = actionValidatorFactory.GetValidators(ActionTypes.ViewFileContents, settings.GetSettingValueOrThrow<Guid>("Project1Guid"));
            validateReuseResult(validators1, validators2, true, 2);

            validators1 = actionValidatorFactory.GetValidators(StaticAndConstMembers.DefaultActionType, projectGuids.Project1);
            validators2 = actionValidatorFactory.GetValidators(StaticAndConstMembers.DefaultActionType, projectGuids.Project1);
            validateReuseResult(validators1, validators2, true, 0);

            validators1 = actionValidatorFactory.GetValidators(ActionTypes.ViewFileContents, StaticAndConstMembers.GetDefaultProjectGuid());
            validators2 = actionValidatorFactory.GetValidators(ActionTypes.ViewFileContents, StaticAndConstMembers.GetDefaultProjectGuid());
            validateReuseResult(validators1, validators2, true, 2);

            validators1 = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList, actionValidatorFactory.PublicProjectId);
            validators2 = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList, actionValidatorFactory.PublicProjectId);
            validateReuseResult(validators1, validators2, true, 1);

            // The default case
            validators1 = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList, Guid.Empty);
            validators2 = actionValidatorFactory.GetValidators(ActionTypes.ViewFilesList, Guid.Empty);
            validateReuseResult(validators1, validators2, true, 3);

            // IActionValidatorFactory.GetValidators(int, string) uses reuseValue=false
            validators1 = actionValidatorFactory.GetValidators((int)ActionTypes.ViewFilesList, "8663708F-C707-47E1-AEDC-2CD9291AD4CB");
            validators2 = actionValidatorFactory.GetValidators((int)ActionTypes.ViewFilesList, "8663708F-C707-47E1-AEDC-2CD9291AD4CB");
            validateReuseResult(validators1, validators2, false, 2);

            validators1 = actionValidatorFactory.GetValidators((int)ActionTypes.ViewFilesList, Guid.Empty.ToString());
            validators2 = actionValidatorFactory.GetValidators((int)ActionTypes.ViewFilesList, Guid.Empty.ToString());
            validateReuseResult(validators1, validators2, false, 4);
        }

        [Test]
        public void ParameterValue_Tests()
        {
            var appInfoFactory = DiContainer.Resolve<IAppInfoFactory>();

            var appInfo = appInfoFactory.CreateAppInfo(10, "App 10");

            Assert.AreEqual(10, appInfo.AppId);
            Assert.AreEqual("App 10", appInfo.AppDescription);
        }

        [Test]
        public void NullableParametersAndReturnTypes_Tests()
        {
            var nullableTypesTestInterfaceInstance = DiContainer.Resolve<INullableTypesTestInterface>();
           
            Assert.AreEqual(17, nullableTypesTestInterfaceInstance.GetNullableInt());

            var nullablesList = nullableTypesTestInterfaceInstance.GetNullablesList();

            Assert.AreEqual(12, nullablesList[0]);
            Assert.AreEqual(18, nullablesList[1]);

            Assert.AreEqual(23, nullableTypesTestInterfaceInstance.MethodWithNullableParameter(null));

            List<double?> nullableValuesList = new List<double?>();
            nullableValuesList.Add(18);
            nullableValuesList.Add(null);

            Assert.AreEqual(19, nullableTypesTestInterfaceInstance.MethodWithParameterAsListOfNullableValues(nullableValuesList));
        }
    }
}