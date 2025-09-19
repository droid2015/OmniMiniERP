using System.Reflection;
using System.Runtime.Loader;
using Microsoft.EntityFrameworkCore;
using Omni.Core;
using Omni.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// DbContext (PostgreSQL ví dụ)
builder.Services.AddDbContext<OmniDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==== Module Loader (read manifests, load DLLs) ====
var modulesDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "modules"); // dev-friendly
modulesDir = Path.GetFullPath(modulesDir);

// Read manifests
List<(ModuleManifest manifest, IModuleStartup startup)> loaded = new();
var manifests = new List<ModuleManifest>();

if (Directory.Exists(modulesDir))
{
    foreach (var dir in Directory.GetDirectories(modulesDir))
    {
        var mf = Path.Combine(dir, "module.manifest.json");
        if (!File.Exists(mf)) continue;

        var json = await File.ReadAllTextAsync(mf);
        var m = System.Text.Json.JsonSerializer.Deserialize<ModuleManifest>(json,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (m is null || !m.Installable) continue;

        manifests.Add(m);

        var dll = Directory.GetFiles(dir, "*.dll").FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == Path.GetFileName(dir));
        dll ??= Directory.GetFiles(dir, "*.dll").FirstOrDefault(); // fallback: dll bất kỳ trong folder

        if (dll is null) continue;
        var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(dll));

        var stType = asm.GetTypes().FirstOrDefault(t => typeof(IModuleStartup).IsAssignableFrom(t) && !t.IsAbstract);
        if (stType is null) continue;

        var startup = (IModuleStartup)Activator.CreateInstance(stType)!;
        startup.ConfigureServices(builder.Services, builder.Configuration);
        loaded.Add((m, startup));
    }
}

// Build app
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// (Tuỳ chọn) Migrate hợp nhất:
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OmniDbContext>();
    await db.Database.MigrateAsync();
}

app.MapGet("/", () => Results.Redirect("/swagger"));

// Map endpoints từ các module
foreach (var (_, startup) in loaded) startup.MapEndpoints(app);

app.Run();
