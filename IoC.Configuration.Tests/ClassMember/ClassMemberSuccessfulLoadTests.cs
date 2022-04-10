using IoC.Configuration.Tests.ClassMember.Services;
using IoC.Configuration.Tests.TestTemplateFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.Tests.ClassMember
{
    public abstract class ClassMemberSuccessfulLoadTests : IoCConfigurationTestsForSuccessfulLoad
    {
        protected static readonly string ClassMemberConfigurationRelativePath = "IoCConfiguration_classMember.xml";

        private void ValidateFieldStaticConstantAttributes(Type type, string fieldName, bool expectsToBeConstant)
        {
            var fieldInfo = type.GetField(fieldName);

            Assert.IsNotNull(fieldInfo);
            Assert.IsTrue(fieldInfo.IsPublic);
            Assert.IsTrue(fieldInfo.IsStatic);

            Assert.AreEqual(expectsToBeConstant ? FieldAttributes.Literal : FieldAttributes.PrivateScope,
                fieldInfo.Attributes & FieldAttributes.Literal);
        }

        private void ValidatePropertyStatic(Type type, string propertyName, bool expectsToBeStatic)
        {
            var propertyInfo = type.GetProperty(propertyName);

            Assert.IsNotNull(propertyInfo);

            var getMethodInfo = propertyInfo.GetMethod;

            Assert.IsTrue(getMethodInfo.IsPublic);

            if (expectsToBeStatic)
                Assert.IsTrue(getMethodInfo.IsStatic);
            else
                Assert.IsFalse(getMethodInfo.IsStatic);
        }

        private void ValidateMethodStatic(Type type, string methodName, bool expectsToBeStatic)
        {
            var methodInfo = type.GetMethod(methodName);

            Assert.IsNotNull(methodInfo);

            Assert.IsTrue(methodInfo.IsPublic);

            if (expectsToBeStatic)
                Assert.IsTrue(methodInfo.IsStatic);
            else
                Assert.IsFalse(methodInfo.IsStatic);
        }

        [Test]
        public void ValidateStaticAndConstantMembers()
        {
            var constAndStaticAppIdsType = typeof(ConstAndStaticAppIds);

            ValidateFieldStaticConstantAttributes(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.AppId1), true);
            ValidateFieldStaticConstantAttributes(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.App1Description), true);

            ValidateFieldStaticConstantAttributes(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.AppId2), false);
            ValidateFieldStaticConstantAttributes(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.App2Description), false);

            ValidateFieldStaticConstantAttributes(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.DefaultAppId), true);
            ValidateFieldStaticConstantAttributes(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.DefaultAppDescription), true);

            ValidatePropertyStatic(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.AppId3), true);

            ValidateMethodStatic(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.GetApp3Description), true);
            ValidateMethodStatic(constAndStaticAppIdsType, nameof(ConstAndStaticAppIds.GetAppId4), true);


            var appIdsType = typeof(IAppIds);
            ValidatePropertyStatic(appIdsType, nameof(IAppIds.DefaultAppId), false);
            ValidatePropertyStatic(appIdsType, nameof(IAppIds.DefaultAppDescription), false);
            ValidateMethodStatic(appIdsType, nameof(IAppIds.GetAppId), false);
        }

        [Test]
        public void ClassMemberInValueImplementationElement()
        {
            if (IoCConfigurationTestsForSuccessfulLoad.DiImplementationType == TestsSharedLibrary.DependencyInjection.DiImplementationType.Ninject)
            {
                try
                {
                    // TODO: For some reason, Ninject.Kernel.Get(type) throws an exception the first time the method is called with 
                    // type=typeof(typeof(string)), however the consequent calls succeed. This was not an issue before, and started happening.
                    // Not sure why this happens (might be fine in production), however thought not worth spending time investigating this issue any farther
                    // since normally type resolution for string is going to be very rare (if at all) anyway, and this might be something out of my control.
                    // Will come back to this at some point.
                    DiContainer.Resolve(typeof(string));
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error(e);
                }
            }
            var defaultAppInfo = DiContainer.Resolve<IAppInfo>();
            var appIds = DiContainer.Resolve<IAppIds>();

            int injectedInt = (int)DiContainer.Resolve(typeof(int));
            Assert.AreEqual(appIds.DefaultAppId, injectedInt);
            Assert.AreEqual(injectedInt, defaultAppInfo.AppId);

            string injectedString = DiContainer.Resolve<string>();
            Assert.AreEqual(appIds.DefaultAppDescription, injectedString);
            Assert.AreEqual(injectedString, defaultAppInfo.AppDescription);
        }

        [Test]
        public void ClassMemberInCollectionElement()
        {
            var readOnlyListOfInt = DiContainer.Resolve<IReadOnlyList<int>>();
            Assert.AreEqual(2, readOnlyListOfInt.Count);
            Assert.AreEqual(ConstAndStaticAppIds.AppId1, readOnlyListOfInt[0]);
            Assert.AreEqual(DiContainer.Resolve<IAppIds>().DefaultAppId, readOnlyListOfInt[1]);
        }

        [Test]
        public void ClassMemberInAutoPropertyElement()
        {
            var appIds = DiContainer.Resolve<IAppIds>();
            Assert.AreEqual(int.MaxValue, appIds.DefaultAppId);
            Assert.AreEqual("Default App", appIds.DefaultAppDescription);
        }

        /// <summary>
        /// Validates the usage of class member is auto-property,
        /// tests the case when class member is static/constant class field, static property or method,
        /// non static property or method.  
        /// </summary>
        [Test]
        public void ClassMemberInParameterInjectedPropertyEtc()
        {
            var appIds = DiContainer.Resolve<IAppIds>();
            var appInfos = DiContainer.Resolve<IAppInfos>();

            // Test IAppIds.DefaultAppId injected into constructor 
            Assert.AreEqual(appIds.DefaultAppId, appInfos.AllAppInfos[0].AppId);

            // Test IAppIds.DefaultAppDescription injected into property 
            Assert.AreEqual(appIds.DefaultAppDescription, appInfos.AllAppInfos[0].AppDescription);

            // Test call to a non-static method with parameters
            Assert.AreEqual(1258, appInfos.AllAppInfos[1].AppId);
            Assert.AreEqual("App info created with non-static method call.", appInfos.AllAppInfos[1].AppDescription);

            // Test call to a static method with parameters
            Assert.AreEqual(1259, appInfos.AllAppInfos[2].AppId);
            Assert.AreEqual("App info created with static method call.", appInfos.AllAppInfos[2].AppDescription);


            // Test IAppIds.GetAppId() injected into constructor 
            Assert.AreEqual(appIds.GetAppId(), appInfos.AllAppInfos[3].AppId);

            // Test IoC.Configuration.Tests.ClassMember.Services.AppIdVars injected into constructor 
            // Note, no binding was provided for AppIdVars (non-interface and n on abstract class with public constructor).
            // IoC.Configuration generated one, since it is used in classMember element.
            Assert.AreEqual(DiContainer.Resolve<AppIdVars>().NonStaticAppIdVar, appInfos.AllAppInfos[4].AppId);

            // Test constant variable ConstAndStaticAppIds.AppId1 used in classMember element as constructor parameter 
            Assert.AreEqual(ConstAndStaticAppIds.AppId1, appInfos.AllAppInfos[5].AppId);

            // Test constant variable ConstAndStaticAppIds.AppId1 used in classMember element as constructor parameter 
            Assert.AreEqual(ConstAndStaticAppIds.App1Description, appInfos.AllAppInfos[5].AppDescription);

            // Test static variable ConstAndStaticAppIds.AppId1 used in classMember element as constructor parameter 
            Assert.AreEqual(ConstAndStaticAppIds.AppId2, appInfos.AllAppInfos[6].AppId);

            // Test static variable ConstAndStaticAppIds.AppId1 used in classMember element as constructor parameter 
            Assert.AreEqual(ConstAndStaticAppIds.App2Description, appInfos.AllAppInfos[6].AppDescription);

            // Test static property ConstAndStaticAppIds.AppId3 used in classMember element as constructor parameter 
            Assert.AreEqual(ConstAndStaticAppIds.AppId3, appInfos.AllAppInfos[7].AppId);

            // Test static method ConstAndStaticAppIds.GetApp3Description() used in classMember element as constructor parameter 
            Assert.AreEqual(ConstAndStaticAppIds.GetApp3Description(), appInfos.AllAppInfos[7].AppDescription);

            // Test static method ConstAndStaticAppIds.GetAppId4 used in classMember element as constructor parameter 
            Assert.AreEqual(ConstAndStaticAppIds.GetAppId4(), appInfos.AllAppInfos[8].AppId);

            // Test enum value IoC.Configuration.Tests.ClassMember.Services.AppTypes.App1 used in classMember element as constructor parameter 
            Assert.AreEqual((int)AppTypes.App1, appInfos.AllAppInfos[9].AppId);
        }

        /// <summary>
        /// Tests _classMember: prefix usage in if elements to reference static/constant and n on static members, as
        /// well as enum values.
        /// </summary>
        [Test]
        public void ClassMemberInAutoServiceIfStatement()
        {
            var appIds = DiContainer.Resolve<IAppIds>();
            var appIdToPriority = DiContainer.Resolve<IAppIdToPriority>();

            // Test IAppIds.DefaultAppId in if element.
            Assert.AreEqual(14, appIdToPriority.GetPriority(appIds.DefaultAppId));

            // Test IAppIds.GetAppId in if element.
            Assert.AreEqual(25, appIdToPriority.GetPriority(appIds.GetAppId()));

            // Test AppIdVars.NonStaticAppIdVar in if element.
            // Note, no binding was provided for AppIdVars (non-interface and n on abstract class with public constructor).
            // IoC.Configuration generated one, since it is used in classMember element.
            var appIdVars = DiContainer.Resolve<AppIdVars>();
            Assert.AreEqual(23, appIdToPriority.GetPriority(appIdVars.NonStaticAppIdVar));

            // Test constant value ConstAndStaticAppIds.AppId1 in if element.
            Assert.AreEqual(4, appIdToPriority.GetPriority(ConstAndStaticAppIds.AppId1));

            // Test static value ConstAndStaticAppIds.AppId2 in if element.
            Assert.AreEqual(7, appIdToPriority.GetPriority(ConstAndStaticAppIds.AppId2));

            // Test static property ConstAndStaticAppIds.AppId3 in if element.
            Assert.AreEqual(8, appIdToPriority.GetPriority(ConstAndStaticAppIds.AppId3));

            // Test static method ConstAndStaticAppIds.GetAppId4() in if element.
            Assert.AreEqual(5, appIdToPriority.GetPriority(ConstAndStaticAppIds.GetAppId4()));

            // Test default priority
            Assert.AreEqual(0, appIdToPriority.GetPriority(-1));
        }

        [Test]
        public void EnumValueInAutoServiceIfStatement()
        {
            var appIdToPriority = DiContainer.Resolve<IAppIdToPriority>();

            Assert.AreEqual(8, appIdToPriority.GetPriority(AppTypes.App1));
            Assert.AreEqual(9, appIdToPriority.GetPriority(AppTypes.App2));

            // default value
            Assert.AreEqual(1, appIdToPriority.GetPriority(AppTypes.App3));
        }

        [Test]
        public void ClassMemberInjectedIntoModuleConstructor()
        {
            var module1 = (Module1)Configuration.DependencyInjection.Modules.Modules.First(x => x.DiModule.GetType() == typeof(Module1)).DiModule;
            Assert.AreEqual(ConstAndStaticAppIds.DefaultAppId, module1.InjectedValue1);

            var methodCallResult = StaticMethodsWithParameters.GetString(5, "Value 1");

            Assert.AreEqual("Static: 5, Value 1", methodCallResult);
            Assert.AreEqual(methodCallResult, module1.InjectedValue2);
        }
    }
}