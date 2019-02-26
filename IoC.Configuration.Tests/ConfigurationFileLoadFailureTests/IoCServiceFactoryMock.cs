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
using IoC.Configuration.DynamicCode;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests
{
    public class IoCServiceFactoryMock : IIoCServiceFactory
    {
        #region Member Variables

        [CanBeNull]
        private AssemblyLocatorMock _assemblyLocatorMock;

        [NotNull]
        private readonly IIoCServiceFactory _ioCServiceFactory;

        /// <summary>
        ///     If the value is not set, default type generate will be used.
        /// </summary>

#pragma warning disable CS0612, CS0618
        [CanBeNull]
        private TypesListFactoryTypeGeneratorMock _typesListFactoryTypeGeneratorMock;

        [NotNull]
        private readonly TypesListFactoryTypeGeneratorMock.ValidationFailureMethod _typesListFactoryValidationFailureMethod;
#pragma warning restore CS0612, CS0618

        #endregion

        #region  Constructors

        public IoCServiceFactoryMock([NotNull] IIoCServiceFactory ioCServiceFactory,
#pragma warning disable CS0612, CS0618
                                     TypesListFactoryTypeGeneratorMock.ValidationFailureMethod typesListFactoryValidationFailureMethod
#pragma warning restore CS0612, CS0618
                                     )
        {
            _ioCServiceFactory = ioCServiceFactory;
            _typesListFactoryValidationFailureMethod = typesListFactoryValidationFailureMethod;
        }

        #endregion

        #region IIoCServiceFactory Interface Implementation

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
        /// <param name="assemblyLocator">The assembly locator.</param>
        /// <returns></returns>
        

        public IAssemblyLocator CreateAssemblyLocator(Func<IConfiguration> getConfugurationFunc, string entryAssemblyFolder)
        {
            if (_assemblyLocatorMock == null)
                _assemblyLocatorMock = new AssemblyLocatorMock(_ioCServiceFactory.CreateAssemblyLocator(getConfugurationFunc, entryAssemblyFolder));

            return _assemblyLocatorMock;
        }
      
        [Obsolete]
        ITypesListFactoryTypeGenerator IIoCServiceFactory.CreateTypesListFactoryTypeGenerator(ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            if (_typesListFactoryTypeGeneratorMock == null)
                _typesListFactoryTypeGeneratorMock = new TypesListFactoryTypeGeneratorMock(
                    _ioCServiceFactory.CreateTypesListFactoryTypeGenerator(typeBasedSimpleSerializerAggregator), _typesListFactoryValidationFailureMethod);

            return _typesListFactoryTypeGeneratorMock;
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

        #endregion

        #region Member Functions

        private static IoCServiceFactoryMock GetIoCServiceFactoryMock()
        {
            return (IoCServiceFactoryMock) IoCServiceFactoryAmbientContext.Context;
        }

        public static void SetFailedAssemblies(IEnumerable<string> assemblyNamesToFailWithoutExtensions)
        {
            GetIoCServiceFactoryMock()._assemblyLocatorMock.SetFailedToLoadAssemblies(assemblyNamesToFailWithoutExtensions);
        }

        #endregion
    }
}