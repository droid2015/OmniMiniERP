using Microsoft.EntityFrameworkCore;
using Omni.Infrastructure;

namespace Omni.Modules.Base;

public class BaseModelConfig : IModelConfiguration
{
    public void Configure(ModelBuilder mb)
    {
        mb.Entity<User>(e => { e.HasKey(x => x.Id); e.HasIndex(x => x.Email).IsUnique(); });
        mb.Entity<ConfigParam>(e => { e.HasKey(x => x.Id); e.HasIndex(x => x.Key).IsUnique(); });
    }
}
