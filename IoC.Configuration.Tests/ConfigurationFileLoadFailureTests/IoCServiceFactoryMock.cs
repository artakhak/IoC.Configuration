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
using System.Collections.Generic;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests
{
    public class IoCServiceFactoryMock : IIoCServiceFactory
    {
        [CanBeNull]
        private AssemblyLocatorMock _assemblyLocatorMock;

        [NotNull]
        private readonly IIoCServiceFactory _ioCServiceFactory;


        public IoCServiceFactoryMock([NotNull] IIoCServiceFactory ioCServiceFactory)
        {
            _ioCServiceFactory = ioCServiceFactory;
        }

        /// <summary>
        /// Creates the type helper.
        /// </summary>
        /// <param name="assemblyLocator">The assembly locator.</param>
        /// <returns></returns>
        public ITypeHelper CreateTypeHelper(IAssemblyLocator assemblyLocator) =>
            _ioCServiceFactory.CreateTypeHelper(assemblyLocator);

        /// <summary>
        /// Creates the type helper.
        /// </summary>
        /// <returns></returns>
        public IAssemblyLocator CreateAssemblyLocator(Func<IConfiguration> getConfigurationFunc, string entryAssemblyFolder)
        {
            if (_assemblyLocatorMock == null)
                _assemblyLocatorMock = new AssemblyLocatorMock(_ioCServiceFactory.CreateAssemblyLocator(getConfigurationFunc, entryAssemblyFolder));

            return _assemblyLocatorMock;
        }

        public IProhibitedServiceTypesInServicesElementChecker GetProhibitedServiceTypesInServicesElementChecker()
        {
            return _ioCServiceFactory.GetProhibitedServiceTypesInServicesElementChecker();
        }
       
        public IImplementedTypeValidator ImplementedTypeValidator => _ioCServiceFactory.ImplementedTypeValidator;

        public IInjectedPropertiesValidator InjectedPropertiesValidator => _ioCServiceFactory.InjectedPropertiesValidator;
        public ICreateInstanceFromTypeAndConstructorParameters CreateInstanceFromTypeAndConstructorParameters => _ioCServiceFactory.CreateInstanceFromTypeAndConstructorParameters;
        public IIdentifierValidator IdentifierValidator => _ioCServiceFactory.IdentifierValidator;
        public IPluginAssemblyTypeUsageValidator PluginAssemblyTypeUsageValidator => _ioCServiceFactory.PluginAssemblyTypeUsageValidator;
        public ISettingValueInitializerHelper SettingValueInitializerHelper => _ioCServiceFactory.SettingValueInitializerHelper;
        public IDeserializedFromStringValueInitializerHelper CreateDeserializedFromStringValueInitializerHelper(ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            return _ioCServiceFactory.CreateDeserializedFromStringValueInitializerHelper(typeBasedSimpleSerializerAggregator);
        }

        public IClassMemberValueInitializerHelper CreateClassMemberValueInitializerHelper(ITypeHelper typeHelper)
        {
            return _ioCServiceFactory.CreateClassMemberValueInitializerHelper(typeHelper);
        }

        public ITypeMemberLookupHelper TypeMemberLookupHelper => _ioCServiceFactory.TypeMemberLookupHelper;
        public IValidateServiceUsageInPlugin ValidateServiceUsageInPlugin => _ioCServiceFactory.ValidateServiceUsageInPlugin;

        private static IoCServiceFactoryMock GetIoCServiceFactoryMock()
        {
            return (IoCServiceFactoryMock) IoCServiceFactoryAmbientContext.Context;
        }

        public static void SetFailedAssemblies(IEnumerable<string> assemblyNamesToFailWithoutExtensions)
        {
            GetIoCServiceFactoryMock()._assemblyLocatorMock.SetFailedToLoadAssemblies(assemblyNamesToFailWithoutExtensions);
        }
    }
}