using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;
using OROptimizer;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainer
{
    // TODO: Either finish this and make public, or get rid off this class.
    // TODO: Get rid off this once we have typesScanner element
    /*
     TODO: Add a new configuration to allow specifying list of assemblies,
     as well as interfaces and type pickers.
     Look at controllerAssemblies element for example on picking assemblies
     */
    internal class TypeScannerModule : ModuleAbstr
    {
        [NotNull]
        [ItemNotNull]
        private readonly IEnumerable<System.Reflection.Assembly> _assembliesToScan;

        [NotNull]
        [ItemNotNull]
        private readonly IEnumerable<IScannedTypeRegistrationInfo> _scannedTypeRegistrations;

        [NotNull]
        private readonly static HashSet<string> _scannedAssemblyNames = new HashSet<string>(StringComparer.Ordinal);

        [NotNull]
        private readonly static HashSet<string> _scannedTypeToServiceAssociations = new HashSet<string>(StringComparer.Ordinal);

        public TypeScannerModule(
        [NotNull, ItemNotNull] IEnumerable<System.Reflection.Assembly> assembliesToScan,
        [NotNull, ItemNotNull] IEnumerable<IScannedTypeRegistrationInfo> scannedTypeRegistrations)
        {
            _assembliesToScan = assembliesToScan;
            _scannedTypeRegistrations = scannedTypeRegistrations;
        }

        /// <inheritdoc />
        protected sealed override void AddServiceRegistrations()
        {
            Dictionary<Type, IScannedTypeRegistrationInfo> serviceTypesToRegister = new Dictionary<Type, IScannedTypeRegistrationInfo>();

            foreach (var scannedTypeRegistrationInfo in _scannedTypeRegistrations)
            {
                if (!serviceTypesToRegister.ContainsKey(scannedTypeRegistrationInfo.ServiceType))
                {
                    serviceTypesToRegister[scannedTypeRegistrationInfo.ServiceType] = scannedTypeRegistrationInfo;
                }
                else
                {
                    GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"Interface type '{scannedTypeRegistrationInfo.ServiceType.GetTypeNameInCSharpClass()}' occurs multiple times.");
                }
            }

            int scannedAssembliesCount = 0;
            foreach (var assembly in _assembliesToScan)
            {
                LogHelper.Context.Log.InfoFormat("Scanning assembly '{0}'.", assembly.GetName().FullName);

                if (assembly == null)
                    GlobalsCoreAmbientContext.Context.LogAnErrorAndThrowException($"An assembly at index {scannedAssembliesCount} is null.");

                var assemblyName = assembly.GetName().Name;

                if (_scannedAssemblyNames.Contains(assemblyName))
                {
                    LogHelper.Context.Log.ErrorFormat("Assembly '{0}' is scanned multiple times.");
                    continue;
                }

                _scannedAssemblyNames.Add(assemblyName);

                foreach (var type in assembly.GetExportedTypes())
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    var publicConstructors = type.GetConstructors().FirstOrDefault(x => x.IsPublic);

                    if (publicConstructors == null)
                        continue;

                    foreach (var scannedTypeRegistrationInfo in serviceTypesToRegister.Values)
                    {
                        if (!scannedTypeRegistrationInfo.ServiceType.IsAssignableFrom(type))
                            continue;

                        if (scannedTypeRegistrationInfo.AdditionalImplementationTypeFilters != null)
                        {
                            bool implementationShouldBeIgnored = false;

                            foreach (var additionalImplementationFilter in scannedTypeRegistrationInfo.AdditionalImplementationTypeFilters)
                            {
                                if (!additionalImplementationFilter.Predicate(type))
                                {
                                    implementationShouldBeIgnored = true;
                                    break;
                                }
                            }

                            if (implementationShouldBeIgnored)
                                continue;
                        }

                        var scannedTypeToServiceAssociationName = $"{type.GetTypeNameInCSharpClass()}_{scannedTypeRegistrationInfo.ServiceType.GetTypeNameInCSharpClass()}";

                        if (_scannedTypeToServiceAssociations.Contains(scannedTypeToServiceAssociationName))
                        {
                            LogHelper.Context.Log.ErrorFormat("Service '{0}' is bound to implementation '{1}' multiple times.",
                                scannedTypeRegistrationInfo.ServiceType.GetTypeNameInCSharpClass(),
                                type.GetTypeNameInCSharpClass());
                            continue;
                        }

                        _scannedTypeToServiceAssociations.Add(scannedTypeToServiceAssociationName);
                        
                        LogHelper.Context.Log.InfoFormat("Bound type '{0}' to itself.", type.GetTypeNameInCSharpClass());

                        if (type != scannedTypeRegistrationInfo.ServiceType)
                        {
                            Bind(scannedTypeRegistrationInfo.ServiceType).To(type).SetResolutionScope(DiResolutionScope.Singleton);
                            LogHelper.Context.Log.InfoFormat("Bound type '{0}' to '{1}'.",
                                scannedTypeRegistrationInfo.ServiceType.GetTypeNameInCSharpClass(),
                                type.GetTypeNameInCSharpClass());
                        }
                        else
                        {
                            Bind(type).ToSelf();
                        }
                    }
                }

                LogHelper.Context.Log.InfoFormat("Scanned assembly '{0}'.", assembly.GetName().FullName);
                ++scannedAssembliesCount;
            }
        }
    }
}