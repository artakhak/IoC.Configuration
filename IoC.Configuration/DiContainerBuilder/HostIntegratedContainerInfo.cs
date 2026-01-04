using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.DiContainerBuilder
{
    public class HostIntegratedContainerInfo<THost>: IHostIntegratedContainerInfo<THost> where THost: class, IHost
    {
        public HostIntegratedContainerInfo(THost host, IContainerInfo containerInfo)
        {
            Host = host;
            ContainerInfo = containerInfo;
        }

        public THost Host { get; }
        public IContainerInfo ContainerInfo { get; }
    }
}