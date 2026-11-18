namespace Template.BlazorServer.Host.Application.Identity;

#pragma warning disable CA1054
using Microsoft.AspNetCore.Components;

// Identityの静的SSRページ用リダイレクトヘルパー(.NET 11 Blazor Identityテンプレート由来)
public sealed class IdentityRedirectManager
{
    public const string StatusMessageKey = "Identity.StatusMessage";

    private readonly NavigationManager navigationManager;

    public IdentityRedirectManager(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
    }

    private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

    public void RedirectTo(string? uri)
    {
        uri ??= string.Empty;

        // オープンリダイレクト防止
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = navigationManager.ToBaseRelativePath(uri);
        }

        navigationManager.NavigateTo(uri);
    }

    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }

    public void RedirectToCurrentPage() => RedirectTo(CurrentPath);
}
#pragma warning restore CA1054
