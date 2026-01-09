using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.DiContainerBuilder
{
    public interface IHostIntegratedContainerInfo<out THost> where THost: class, IHost
    {
        THost Host { get; }
        IContainerInfo ContainerInfo { get; }
    }
}
