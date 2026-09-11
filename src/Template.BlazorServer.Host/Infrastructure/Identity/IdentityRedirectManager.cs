namespace Template.BlazorServer.Host.Infrastructure.Identity;

#pragma warning disable CA1054
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

// Identityの静的SSRページ用リダイレクトヘルパー(.NET 10 Blazor Identityテンプレート由来)
public sealed class IdentityRedirectManager
{
    public const string StatusCookieName = "Identity.StatusMessage";

    private static readonly CookieBuilder StatusCookieBuilder = new()
    {
        SameSite = SameSiteMode.Strict,
        HttpOnly = true,
        IsEssential = true,
        MaxAge = TimeSpan.FromSeconds(5)
    };

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

    public void RedirectToWithStatus(string uri, string message, HttpContext context)
    {
        context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
        RedirectTo(uri);
    }

    public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

    public void RedirectToCurrentPageWithStatus(string message, HttpContext context) =>
        RedirectToWithStatus(CurrentPath, message, context);

    public void RedirectToInvalidUser(UserManager<ApplicationUser> userManager, HttpContext context) =>
        RedirectToWithStatus("Account/Login", $"Error: ユーザー '{userManager.GetUserId(context.User)}' を読み込めませんでした。", context);
}
#pragma warning restore CA1054
