using System;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DynamicCode;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration
{
    /// <summary>
    ///     This class is internal, and outside this project should be used only in test projects
    /// </summary>
    public interface IIoCServiceFactory
    {
        #region Current Type Interface

        /// <summary>
        ///     Creates an instance of <see cref="IAssemblyLocator" />.
        /// </summary>
        /// <param name="getConfugurationFunc">
        ///     A <see cref="System.Func{IConfiguration}" /> objects that returns an instance of
        ///     <see cref="IConfiguration" />
        /// </param>
        /// <param name="entryAssemblyFolder"></param>
        IAssemblyLocator CreateAssemblyLocator([NotNull] Func<IConfiguration> getConfugurationFunc, [NotNull] string entryAssemblyFolder);

        /// <summary>
        ///     Creates an instance of <see cref="ITypesListFactoryTypeGenerator" />
        /// </summary>
        ITypesListFactoryTypeGenerator CreateTypesListFactoryTypeGenerator([NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator);

        /// <summary>
        ///     Returns instance of <see cref="IProhibitedServiceTypesInServicesElementChecker" />.
        /// </summary>
        IProhibitedServiceTypesInServicesElementChecker GetProhibitedServiceTypesInServicesElementChecker();

        #endregion
    }
}