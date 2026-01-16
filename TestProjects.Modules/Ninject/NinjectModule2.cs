using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using Ninject.Modules;
using OROptimizer.Diagnostics.Log;

namespace Modules.Ninject;

public class NinjectModule2 : NinjectModule
{
    private readonly ILog _logger;

    public NinjectModule2(ILog logger, int param2)
    {
        _logger = logger;
        Property2 = param2;
    }
    
    /// <inheritdoc />
    public override void Load()
    {
        Bind<ILog>().ToConstant(_logger).InSingletonScope();
    }

    /// <summary>
    ///     The value of parameter <paramref name="diContainer" /> will be injected by <see cref="DiContainerBuilder" />.
    /// </summary>
    /// <param name="diContainer"></param>
    public void OnDiContainerReady(IDiContainer diContainer)
    {
        
    }
    
    public int Property2 { get; }
}