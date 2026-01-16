using Autofac;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using OROptimizer.Diagnostics.Log;
using AutofacModule = Autofac.Module;

namespace Modules.Autofac;

public class AutofacModule2 : AutofacModule
{
    private readonly ILog _logger;

    public AutofacModule2(ILog logger, int param2)
    {
        _logger = logger;
        Property2 = param2;
    }
    
    /// <summary>
    ///     The value of parameter <paramref name="diContainer" /> will be injected by <see cref="DiContainerBuilder" />.
    /// </summary>
    /// <param name="diContainer"></param>
    public void OnDiContainerReady(IDiContainer diContainer)
    {
    }

    /// <inheritdoc />
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.Register(c => _logger).As<ILog>().SingleInstance();
    }
    
    public int Property2 { get; }
}