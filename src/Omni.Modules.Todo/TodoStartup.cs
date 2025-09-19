using Microsoft.AspNetCore.Builder;      // MapGroup/MapGet/MapPost...
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;      // IEndpointRouteBuilder
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omni.Core;
using Omni.Infrastructure;
using Omni.Modules.Todo;                 // << để thấy TodoTask, TodoModelConfig

namespace Omni.Modules.Todo;

public class TodoStartup : IModuleStartup
{
    public void ConfigureServices(IServiceCollection s, IConfiguration cfg)
    {
        ModuleModelConfigurations.Register(new TodoModelConfig());
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/todo");

        g.MapGet("/", async (OmniDbContext db) =>
            await db.Set<TodoTask>().OrderByDescending(x => x.CreatedAt).ToListAsync());

        g.MapPost("/", async (OmniDbContext db, TodoTask dto) =>
        {
            dto.Id = Guid.NewGuid();
            db.Add(dto);
            await db.SaveChangesAsync();
            return Results.Created($"/api/todo/{dto.Id}", dto);
        });

        g.MapPatch("/{id:guid}/toggle", async (OmniDbContext db, Guid id) =>
        {
            var t = await db.Set<TodoTask>().FindAsync(id);
            if (t is null) return Results.NotFound();
            t.IsDone = !t.IsDone;
            await db.SaveChangesAsync();
            return Results.Ok(t);
        });

        g.MapDelete("/{id:guid}", async (OmniDbContext db, Guid id) =>
        {
            var t = await db.Set<TodoTask>().FindAsync(id);
            if (t is null) return Results.NotFound();
            db.Remove(t);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
