using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace Omni.Core;

public record ModuleManifest(
    string Name,
    string Title,
    string Version,
    string[] Depends,
    string MigrationsAssembly,
    bool Installable = true,
    bool AutoInstall = false,
    string? Description = null
);

public interface IModuleStartup
{
    void ConfigureServices(IServiceCollection services, IConfiguration cfg);
    void MapEndpoints(IEndpointRouteBuilder app);
}
