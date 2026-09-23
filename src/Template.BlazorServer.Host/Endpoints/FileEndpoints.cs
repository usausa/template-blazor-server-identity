namespace Template.BlazorServer.Host.Endpoints;

using Template.BlazorServer.Host.Application;
using Template.BlazorServer.Infrastructure.Storage;

//--------------------------------------------------------------------------------
// Models
//--------------------------------------------------------------------------------

public sealed class FileListResponse
{
    public IReadOnlyList<string> Entries { get; set; } = default!;
}

//--------------------------------------------------------------------------------
// Endpoints
//--------------------------------------------------------------------------------

public static class FileEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapFileEndpoints(this WebApplication app)
    {
        var group = app.MapApiGroup(ApiRoutes.Files)
            .RequireAuthorization()
            .AddEndpointFilter(static async (context, next) =>
            {
                try
                {
                    return await next(context);
                }
                catch (StorageException)
                {
                    return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid path.");
                }
            });

        group.MapGet("/list/{**path}", HandleListAsync);
        group.MapGet("/download/{**path}", HandleDownloadAsync);
        group.MapDelete("/{**path}", HandleDeleteAsync).RequireAuthorization(Policies.Administrator);
    }

    //--------------------------------------------------------------------------------
    // List
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleListAsync(
        IStorage storage,
        string? path,
        CancellationToken cancellationToken)
    {
        path ??= string.Empty;

        if (!await storage.DirectoryExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var entries = await storage.ListAsync(path, cancellationToken);
        return TypedResults.Ok(new FileListResponse { Entries = entries });
    }

    //--------------------------------------------------------------------------------
    // Download
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleDownloadAsync(
        IStorage storage,
        string path,
        CancellationToken cancellationToken)
    {
        if (!await storage.FileExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var stream = await storage.ReadAsync(path, cancellationToken);
        return TypedResults.Stream(stream, "application/octet-stream", Path.GetFileName(path));
    }

    //--------------------------------------------------------------------------------
    // Delete
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleDeleteAsync(
        IStorage storage,
        string path,
        CancellationToken cancellationToken)
    {
        if (!await storage.FileExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        await storage.DeleteAsync(path, cancellationToken);
        return TypedResults.NoContent();
    }
}
