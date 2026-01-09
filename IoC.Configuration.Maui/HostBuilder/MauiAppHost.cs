using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Hosting;

namespace IoC.Configuration.Maui.HostBuilder;

/// <summary>
/// Maui implementation for <see cref="IHost"/>
/// </summary>
public class MauiAppHost : IHost
{
    public MauiAppHost(MauiApp mauiApp)
    {
        MauiApp = mauiApp;
    }

    /// <summary>
    /// Maui app.
    /// </summary>
    public MauiApp MauiApp { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        MauiApp.Dispose();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public IServiceProvider Services => MauiApp.Services;
}
