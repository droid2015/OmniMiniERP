using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omni.Core;
using Omni.Infrastructure;

namespace Omni.Modules.Base;

public class BaseStartup : IModuleStartup
{
    public void ConfigureServices(IServiceCollection s, IConfiguration cfg)
    {
        ModuleModelConfigurations.Register(new BaseModelConfig());
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/base/ping", () => "pong");
    }
}
