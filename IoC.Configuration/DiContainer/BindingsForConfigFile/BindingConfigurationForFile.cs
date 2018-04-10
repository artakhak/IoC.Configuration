using System;
using System.Collections.Generic;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public sealed class BindingConfigurationForFile : BindingConfiguration<BindingImplementationConfigurationForFile>
    {
        #region  Constructors

        private BindingConfigurationForFile([NotNull] Type serviceType, bool registerOnlyIfNotRegistered,
                                            [NotNull] [ItemNotNull] IEnumerable<IServiceImplementationElement> serviceImplementations)
            : base(serviceType)
        {
            RegisterIfNotRegistered = registerOnlyIfNotRegistered;

            foreach (var serviceImplementation in serviceImplementations)
            {
                if (!serviceImplementation.Enabled)
                    continue;

                AddImplementation(new BindingImplementationConfigurationForFile(serviceImplementation));
            }

            if (Implementations.Count == 0)
                LogHelper.Context.Log.WarnFormat("No implementation is provided for service '{0}' either because all the implementations are disabled or none exists.", ServiceType.FullName);
        }

        #endregion

        #region Member Functions

        public static BindingConfigurationForFile CreateBindingConfigurationForFile([NotNull] Type serviceType, bool registerOnlyIfNotRegistered,
                                                                                    [NotNull] [ItemNotNull] IEnumerable<IServiceImplementationElement> serviceImplementations)
        {
            var bindingConfigurationForFile = new BindingConfigurationForFile(serviceType, registerOnlyIfNotRegistered, serviceImplementations);
            bindingConfigurationForFile.Validate();
            return bindingConfigurationForFile;
        }

        #endregion
    }
}