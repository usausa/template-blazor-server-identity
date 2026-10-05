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
            .ProducesProblem(StatusCodes.Status401Unauthorized)
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
            })
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/list/{**path}", HandleListAsync)
            .WithName("FileList")
            .Produces<FileListResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapGet("/download/{**path}", HandleDownloadAsync)
            .WithName("FileDownload")
            .Produces<Stream>(StatusCodes.Status200OK, "application/octet-stream")
            .ProducesProblem(StatusCodes.Status404NotFound);
        group.MapDelete("/{**path}", HandleDeleteAsync)
            .RequireAuthorization(Policies.Administrator)
            .WithName("FileDelete")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
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
