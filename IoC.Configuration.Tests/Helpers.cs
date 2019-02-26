// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.IO;
using System.Linq;
using System.Xml;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer.Diagnostics.Log;
using TestsSharedLibrary;
using TestsSharedLibrary.DependencyInjection;
using TestsSharedLibrary.Diagnostics.Log;

namespace IoC.Configuration.Tests
{
    public static class Helpers
    {
        #region Member Variables

        public static readonly string TestsEntryAssemblyFolder = Path.GetDirectoryName(typeof(Helpers).Assembly.Location);

        #endregion

        #region Member Functions

        public static T GetPropertyValue<T>(object propertyOwnerObject, string propertyName)
        {
            var propertyInfo = propertyOwnerObject.GetType().GetProperty(propertyName);
            var propertyValue = propertyInfo.GetValue(propertyOwnerObject);
            return propertyValue == null ? default(T) : (T) propertyValue;
        }

        public static string GetPropertyValueToString(object propertyOwnerObject, string propertyName)
        {
            var propertyValue = GetPropertyValue<object>(propertyOwnerObject, propertyName);
            return propertyValue?.ToString();
        }

        public static string GetTestDllsFolderPath()
        {
            var entryAssemblyFolder = TestsEntryAssemblyFolder;

            var projectFolderName = "IoC.Configuration.Tests";

            var indexOfProject = entryAssemblyFolder.LastIndexOf(projectFolderName, StringComparison.OrdinalIgnoreCase);

            if (indexOfProject >= 0)
                return Path.Combine(entryAssemblyFolder.Substring(0, indexOfProject + projectFolderName.Length), "TestDlls");

            return null;
        }

        public static Type GetType(string typeFullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeFullName);
                if (type != null)
                    return type;
            }

            throw new Exception($"Type '{typeFullName}' was not found.");
        }

        public static void ReplaceActiveDiManagerInConfigurationFile(XmlDocument xmlDocument, DiImplementationType diImplementationType)
        {
            string activeDiManagerName = null;

            switch (diImplementationType)
            {
                case DiImplementationType.Autofac:
                case DiImplementationType.Ninject:
                    activeDiManagerName = diImplementationType.ToString();
                    break;
                default:
                    throw new Exception($"The value is not handled: {diImplementationType}.");
            }

            xmlDocument.SelectElement("/iocConfiguration/diManagers", x => true)
                       .SetAttributeValue(ConfigurationFileAttributeNames.ActiveDiManagerName, activeDiManagerName);
        }

        public static void TestExpectedConfigurationParseException(
                                            [NotNull] Action actionResultingInException,
                                            [CanBeNull] Type expectedConfigurationFileElementTypeAtError,
                                            [CanBeNull] Type expectedParentConfigurationFileElementTypeAtError = null,
                                            bool validateLoggedException = false,
                                            [CanBeNull] Action<Exception> doAdditionalTests = null)
        {
            TestExpectedConfigurationParseException<ConfigurationParseException>(actionResultingInException, expectedConfigurationFileElementTypeAtError, 
                expectedParentConfigurationFileElementTypeAtError, validateLoggedException, doAdditionalTests);
        }

        public static void TestExpectedConfigurationParseException<TExpectedException>(
                                              [NotNull] Action actionResultingInException,
                                              [CanBeNull] Type expectedConfigurationFileElementTypeAtError,
                                              [CanBeNull] Type expectedParentConfigurationFileElementTypeAtError = null,
                                              bool validateLoggedException = false,
                                              [CanBeNull] Action<TExpectedException> doAdditionalTests = null) where TExpectedException : Exception
        {
            Type expectedExceptionType = typeof(TExpectedException);

            try
            {
                actionResultingInException();
                Assert.Fail("Load should have failed");
            }
            catch(Exception e)
            {
                if (!(e is TExpectedException expectedException))
                {
                    // Lets try to find our exception in logged exceptions
                    expectedException = Log4Tests.LoggedExceptions.FirstOrDefault(x => x is TExpectedException) as TExpectedException;
                }

                Assert.IsNotNull(expectedException,
                    $"Expected an exception of type '{typeof(TExpectedException).FullName}'. Actual exception is of type {e.GetType().FullName}.");
             
                if (validateLoggedException)
                    Assert.IsTrue(Log4Tests.LoggedExceptions.Count > 0);
               
                if (typeof(ConfigurationParseException).IsAssignableFrom(expectedExceptionType))
                {
                    void validateException(ConfigurationParseException configurationParseException)
                    {
                        Assert.IsNotNull(configurationParseException);

                        //IValueInitializerElementDecorator
                        if (!(configurationParseException.ConfigurationFileElement.GetType() == expectedConfigurationFileElementTypeAtError ||
                              configurationParseException.ConfigurationFileElement is IValueInitializerElementDecorator valueInitializerElementDecorator &&
                              valueInitializerElementDecorator.DecoratedValueInitializerElement.GetType() == expectedConfigurationFileElementTypeAtError))
                            Assert.IsInstanceOfType(configurationParseException.ConfigurationFileElement, expectedConfigurationFileElementTypeAtError);

                        if (expectedParentConfigurationFileElementTypeAtError != null)
                        {
                            if (!(configurationParseException.ParentConfigurationFileElement.GetType() == expectedParentConfigurationFileElementTypeAtError ||
                                  configurationParseException.ConfigurationFileElement is IValueInitializerElementDecorator valueInitializerElementDecorator2 &&
                                  valueInitializerElementDecorator2.DecoratedValueInitializerElement.GetType() == expectedParentConfigurationFileElementTypeAtError))
                                Assert.IsInstanceOfType(configurationParseException.ParentConfigurationFileElement, expectedParentConfigurationFileElementTypeAtError);
                        }
                            
                    }

                    validateException(expectedException as ConfigurationParseException);

                    if (validateLoggedException)
                    {
                        var configurationParseException = Log4Tests.LoggedExceptions.First(x => x is ConfigurationParseException) as ConfigurationParseException;

                        // Lets set expectedException to the logged exception, so that it is passed to doAdditionalTests.
                        // If the exception is logged, it might be more specific, then the exception caught and re-thrown.
                        expectedException = configurationParseException as TExpectedException;
                        validateException(configurationParseException);
                    }
                }
                else
                {
                    Assert.IsNull(expectedConfigurationFileElementTypeAtError);
                    Assert.IsNull(expectedParentConfigurationFileElementTypeAtError);
                }

                doAdditionalTests?.Invoke(expectedException);
                LogHelper.Context.Log.Fatal(e);
            }
        }

        public static (IContainerInfo containerInfo, IConfiguration configuration) LoadConfigurationFile(DiImplementationType diImplementationType, 
                                             [NotNull] string configurationRelativePath,
                                             [CanBeNull] IDiModule[] additionalModulesToLoad = null,
                                             [CanBeNull] Action<XmlDocument> modifyConfigurationFileOnLoad = null)
        {
            TestsHelper.SetupLogger();
            var ioCConfigurator = new DiContainerBuilder.DiContainerBuilder()
                                  .StartFileBasedDi(
                                      new FileBasedConfigurationFileContentsProvider(Path.Combine(Helpers.TestsEntryAssemblyFolder, configurationRelativePath)),
                                      Helpers.TestsEntryAssemblyFolder,
                                      (sender, e) =>
                                      {
                                          // Lets explicitly set the DiManager to Autofac. Since we are going to test failure, the Di manager implementation does not matter.
                                          // However, this will give us predictability on what modules will be enabled.
                                          e.XmlDocument.SelectElement("/iocConfiguration/diManagers").SetAttributeValue(ConfigurationFileAttributeNames.ActiveDiManagerName, diImplementationType.ToString());
                                          modifyConfigurationFileOnLoad?.Invoke(e.XmlDocument);
                                      })
                                  .WithoutPresetDiContainer();

            if (additionalModulesToLoad?.Length > 0)
                ioCConfigurator.AddAdditionalDiModules(additionalModulesToLoad);

            var containerInfo = ioCConfigurator.RegisterModules().Start();

            return (containerInfo, containerInfo.DiContainer.Resolve<IConfiguration>());
        }

        public static void ValidateAreEqual(ITypeInfo typeInfo, Type type)
        {
            Assert.AreEqual(type.IsArray, typeInfo.Type.IsArray);

            if (!type.IsArray)
            {
                if (typeInfo.GenericTypeParameters.Count != (type.GenericTypeArguments?.Length ?? 0))
                    Assert.Fail();

                if (typeInfo.GenericTypeParameters.Count > 0)
                {
                    for (var i = 0; i < typeInfo.GenericTypeParameters.Count; ++i)
                        ValidateAreEqual(typeInfo.GenericTypeParameters[i], type.GenericTypeArguments[i]);
                }
            }

            Assert.AreSame(type, typeInfo.Type);
        }

        public static void ValidateTypeInfo(ITypeInfo typeInfo, Type expectedType, string expectedCSHarpTypeName)
        {
            Assert.IsNotNull(typeInfo);
            Assert.IsNotNull(typeInfo.Type);
            Assert.IsNotNull(expectedType);
            Assert.AreSame(expectedType, typeInfo.Type);
            Assert.AreEqual(expectedCSHarpTypeName, typeInfo.TypeCSharpFullName);

            Helpers.ValidateAreEqual(typeInfo, expectedType);
        }
        #endregion
    }
}