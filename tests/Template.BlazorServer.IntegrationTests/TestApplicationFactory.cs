namespace Template.BlazorServer;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

public sealed class TestApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string databaseFile = $"test-{Guid.NewGuid():N}.db";

    private readonly string identityDatabaseFile = $"test-identity-{Guid.NewGuid():N}.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("http_ports", string.Empty);
        builder.UseSetting("ConnectionStrings:Default", $"Data Source={databaseFile};Cache=Shared;Pooling=False");
        builder.UseSetting("ConnectionStrings:Identity", $"Data Source={identityDatabaseFile};Cache=Shared;Pooling=False");
        builder.UseSetting("Prometheus:Uri", string.Empty);
        builder.UseSetting("Worker:Enable", "false");
        builder.UseSetting("Profiler:SqlLog:Enable", "false");
        builder.UseSetting("Profiler:SqlTelemetry:Enable", "false");
        builder.UseSetting("Log:HttpLog", "false");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            DeleteFile(databaseFile);
            DeleteFile(identityDatabaseFile);
        }
    }

    private static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
                // Ignore
            }
        }
    }
}
