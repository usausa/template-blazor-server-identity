namespace Template.BlazorServer.Host.Endpoints;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;

using Template.BlazorServer.Host.Infrastructure.Identity;

// Identityの静的SSRページ(Components/Account)が使う補助エンドポイント
public static class AccountEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/Account");

        group.MapPost("/Logout", HandleLogoutAsync).RequireAuthorization();
        group.MapPost("/PasskeyCreationOptions", HandlePasskeyCreationOptionsAsync).RequireAuthorization();
        group.MapPost("/PasskeyRequestOptions", HandlePasskeyRequestOptionsAsync).AllowAnonymous();
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleLogoutAsync(
        SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();

        return TypedResults.LocalRedirect("~/Account/Login");
    }

    // パスキー登録用オプション(WebAuthn creation options)を発行する
    private static async ValueTask<IResult> HandlePasskeyCreationOptionsAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAntiforgery antiforgery)
    {
        await antiforgery.ValidateRequestAsync(context);

        var user = await userManager.GetUserAsync(context.User);
        if (user is null)
        {
            return TypedResults.NotFound();
        }

        var userId = await userManager.GetUserIdAsync(user);
        var userName = await userManager.GetUserNameAsync(user) ?? "User";
        var optionsJson = await signInManager.MakePasskeyCreationOptionsAsync(new()
        {
            Id = userId,
            Name = userName,
            DisplayName = userName
        });

        return TypedResults.Content(optionsJson, contentType: "application/json");
    }

    // パスキーログイン用オプション(WebAuthn request options)を発行する
    private static async ValueTask<IResult> HandlePasskeyRequestOptionsAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAntiforgery antiforgery,
        [FromQuery] string? username)
    {
        await antiforgery.ValidateRequestAsync(context);

        var user = String.IsNullOrEmpty(username) ? null : await userManager.FindByNameAsync(username);
        var optionsJson = await signInManager.MakePasskeyRequestOptionsAsync(user);

        return TypedResults.Content(optionsJson, contentType: "application/json");
    }
}
