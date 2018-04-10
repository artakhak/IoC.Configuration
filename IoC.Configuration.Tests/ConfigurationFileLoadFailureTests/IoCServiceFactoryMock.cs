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