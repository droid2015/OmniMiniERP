using Microsoft.EntityFrameworkCore;

namespace Omni.Infrastructure;

public interface IModelConfiguration { void Configure(ModelBuilder mb); }

public static class ModuleModelConfigurations
{
    public static readonly IList<IModelConfiguration> All = new List<IModelConfiguration>();
    public static void Register(IModelConfiguration cfg) => All.Add(cfg);
}
