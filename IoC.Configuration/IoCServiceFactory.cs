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
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    /// <summary>
    ///     A service factory.
    /// </summary>
    public class IoCServiceFactory : IIoCServiceFactory
    {
        #region Member Variables

        private ICreateInstanceFromTypeAndConstructorParameters _createInstanceFromTypeAndConstructorParameters;

        [NotNull]
        private readonly InjectedPropertiesValidator _injectedPropertiesValidator = new InjectedPropertiesValidator();

        [NotNull]
        private readonly IProhibitedServiceTypesInServicesElementChecker _prohibitedServiceTypesInServicesElementChecker =
            new ProhibitedServiceTypesInServicesElementChecker();

        #endregion

        #region IIoCServiceFactory Interface Implementation

        /// <summary>
        ///     Creates an instance of <see cref="IAssemblyLocator" />.
        /// </summary>
        /// <param name="getConfigurationFunc">
        ///     A <see cref="System.Func{IConfiguration}" /> objects that returns an instance of
        ///     <see cref="IConfiguration" />
        /// </param>
        /// <param name="entryAssemblyFolder"></param>
        /// <returns></returns>
        public IAssemblyLocator CreateAssemblyLocator(Func<IConfiguration> getConfigurationFunc, string entryAssemblyFolder)
        {
            return new AssemblyLocator(getConfigurationFunc, entryAssemblyFolder);
        }

        public IClassMemberValueInitializerHelper CreateClassMemberValueInitializerHelper(ITypeHelper typeHelper)
        {
            return new ClassMemberValueInitializerHelper(typeHelper, PluginAssemblyTypeUsageValidator, TypeMemberLookupHelper);
        }

        public IDeserializedFromStringValueInitializerHelper CreateDeserializedFromStringValueInitializerHelper(ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            return new DeserializedFromStringValueInitializerHelper(typeBasedSimpleSerializerAggregator);
        }

        public ICreateInstanceFromTypeAndConstructorParameters CreateInstanceFromTypeAndConstructorParameters
        {
            get
            {
                if (_createInstanceFromTypeAndConstructorParameters == null)
                    _createInstanceFromTypeAndConstructorParameters = new CreateInstanceFromTypeAndConstructorParameters(_injectedPropertiesValidator);

                return _createInstanceFromTypeAndConstructorParameters;
            }
        }

        /// <summary>
        ///     Creates the type helper.
        /// </summary>
        /// <param name="assemblyLocator">The assembly locator.</param>
        /// <returns></returns>
        public ITypeHelper CreateTypeHelper(IAssemblyLocator assemblyLocator)
        {
            return new TypeHelper(assemblyLocator, new TypeParser(), PluginAssemblyTypeUsageValidator);
        }

        /// <summary>
        ///     Returns instance of <see cref="IProhibitedServiceTypesInServicesElementChecker" />.
        /// </summary>
        /// <returns></returns>
        public IProhibitedServiceTypesInServicesElementChecker GetProhibitedServiceTypesInServicesElementChecker()
        {
            return _prohibitedServiceTypesInServicesElementChecker;
        }

        public IIdentifierValidator IdentifierValidator { get; } = new IdentifierValidator();

        public IImplementedTypeValidator ImplementedTypeValidator { get; } = new ImplementedTypeValidator();

        public IInjectedPropertiesValidator InjectedPropertiesValidator => _injectedPropertiesValidator;

        [NotNull]
        public IPluginAssemblyTypeUsageValidator PluginAssemblyTypeUsageValidator { get; } = new PluginAssemblyTypeUsageValidator();

        public ISettingValueInitializerHelper SettingValueInitializerHelper { get; } = new SettingValueInitializerHelper();

        public ITypeMemberLookupHelper TypeMemberLookupHelper { get; } = new TypeMemberLookupHelper();
        public IValidateServiceUsageInPlugin ValidateServiceUsageInPlugin { get; } = new ValidateServiceUsageInPlugin();

        #endregion
    }
}