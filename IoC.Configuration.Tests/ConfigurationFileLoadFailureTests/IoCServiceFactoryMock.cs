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
        [CanBeNull]
        private TypesListFactoryTypeGeneratorMock _typesListFactoryTypeGeneratorMock;

        private readonly TypesListFactoryTypeGeneratorMock.ValidationFailureMethod _typesListFactoryValidationFailureMethod;

        #endregion

        #region  Constructors

        public IoCServiceFactoryMock([NotNull] IIoCServiceFactory ioCServiceFactory,
                                     TypesListFactoryTypeGeneratorMock.ValidationFailureMethod typesListFactoryValidationFailureMethod)
        {
            _ioCServiceFactory = ioCServiceFactory;
            _typesListFactoryValidationFailureMethod = typesListFactoryValidationFailureMethod;
        }

        #endregion

        #region IIoCServiceFactory Interface Implementation

        public IAssemblyLocator CreateAssemblyLocator(Func<IConfiguration> getConfugurationFunc, string entryAssemblyFolder)
        {
            if (_assemblyLocatorMock == null)
                _assemblyLocatorMock = new AssemblyLocatorMock(_ioCServiceFactory.CreateAssemblyLocator(getConfugurationFunc, entryAssemblyFolder));

            return _assemblyLocatorMock;
        }

        public ITypesListFactoryTypeGenerator CreateTypesListFactoryTypeGenerator(ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
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