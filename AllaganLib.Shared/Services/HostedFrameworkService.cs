using System.Threading;
using System.Threading.Tasks;

using Dalamud.Plugin.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AllaganLib.Shared.Services;

public abstract class HostedFrameworkService : IHostedService
{
    public ILogger<HostedFrameworkService> Logger { get; }

    public IFramework Framework { get; }

    public HostedFrameworkService(ILogger<HostedFrameworkService> logger, IFramework framework)
    {
        this.Logger = logger;
        this.Framework = framework;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.Framework.Update += this.FrameworkOnUpdate;
        return Task.CompletedTask;
    }

    public abstract void FrameworkOnUpdate(IFramework framework);

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.Framework.Update -= this.FrameworkOnUpdate;
        return Task.CompletedTask;
    }
}