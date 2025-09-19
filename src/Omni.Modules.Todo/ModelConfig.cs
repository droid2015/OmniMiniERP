using Microsoft.EntityFrameworkCore;
using Omni.Infrastructure;

namespace Omni.Modules.Todo;

public class TodoModelConfig : IModelConfiguration
{
    public void Configure(ModelBuilder mb)
    {
        mb.Entity<TodoTask>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.IsDone);
        });
    }
}
