using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public class RegisterModulesWithHostBuilder<THost> : FileBasedConfiguratorAbstr, IRegisterModulesWithHostBuilder<THost> where THost: class, IHost
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FileBasedContainerStarter" /> class.
        /// </summary>
        /// <param name="fileBasedConfiguration">The file based configuration.</param>
        public RegisterModulesWithHostBuilder([NotNull] FileBasedConfiguration fileBasedConfiguration) : base(fileBasedConfiguration)
        {
        }

        /// <inheritdoc />
        public IHostIntegratedContainerInfo<THost> RegisterServiceProviderAndBuildApp(Action servicesWereRegistered)
        {
            FileBasedConfiguration.RegisterServiceProviderAndBuildApp();

            servicesWereRegistered?.Invoke();
            
            var hostBuilder = FileBasedConfiguration.HostBuilder;

            if (!(hostBuilder is IApplicationHostBuilder<THost> applicationHostBuilder))
                throw new InvalidOperationException($"The value of '{nameof(FileBasedConfiguration.HostBuilder)}' is expected to be an instance of '{typeof(IApplicationHostBuilder<THost>)}'");
            
            LogHelper.Context.Log.Info("Registered modules for application builder. Building the host...");
            var host = applicationHostBuilder.Build();

            LogHelper.Context.Log.Info("Host was built.");

            // StartContainer() should be executed only after _hostBuilder.Build() is executed.
            var containerInfo = FileBasedConfiguration.StartContainer();

            return new HostIntegratedContainerInfo<THost>(host, containerInfo);
        }
    }
}