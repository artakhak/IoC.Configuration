using System;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using IoC.Configuration.DynamicCode;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    public class IoCServiceFactory : IIoCServiceFactory
    {
        #region Member Variables

        [NotNull]
        private static readonly IProhibitedServiceTypesInServicesElementChecker _prohibitedServiceTypesInServicesElementChecker;

        #endregion

        #region  Constructors

        static IoCServiceFactory()
        {
            _prohibitedServiceTypesInServicesElementChecker = new ProhibitedServiceTypesInServicesElementChecker();
        }

        #endregion

        #region IIoCServiceFactory Interface Implementation

        public IAssemblyLocator CreateAssemblyLocator(Func<IConfiguration> getConfugurationFunc, [NotNull] string entryAssemblyFolder)
        {
            return new AssemblyLocator(getConfugurationFunc, entryAssemblyFolder);
        }

        public ITypesListFactoryTypeGenerator CreateTypesListFactoryTypeGenerator(ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            return new TypesListFactoryTypeGenerator(typeBasedSimpleSerializerAggregator);
        }

        public IProhibitedServiceTypesInServicesElementChecker GetProhibitedServiceTypesInServicesElementChecker()
        {
            return _prohibitedServiceTypesInServicesElementChecker;
        }

        #endregion
    }
}