using Microsoft.EntityFrameworkCore;

namespace Omni.Infrastructure;

public class OmniDbContext : DbContext
{
    public OmniDbContext(DbContextOptions<OmniDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        foreach (var cfg in ModuleModelConfigurations.All) cfg.Configure(mb);
        base.OnModelCreating(mb);
    }
}
