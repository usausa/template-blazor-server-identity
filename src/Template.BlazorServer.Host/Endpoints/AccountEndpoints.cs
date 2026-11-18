namespace Template.BlazorServer.Host.Endpoints;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;

using Template.BlazorServer.Host.Application.Identity;

// Identityの静的SSRページ(Components/Account)が使う補助エンドポイント
public static class AccountEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/Account");

        group.MapPost("/Logout", HandleLogoutAsync)
            .RequireAuthorization()
            .WithName("AccountLogout")
            .Produces(StatusCodes.Status302Found)
            .Produces(StatusCodes.Status400BadRequest);
        group.MapPost("/PasskeyCreationOptions", HandlePasskeyCreationOptionsAsync)
            .RequireAuthorization()
            .WithName("AccountPasskeyCreationOptions")
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
        group.MapPost("/PasskeyRequestOptions", HandlePasskeyRequestOptionsAsync)
            .AllowAnonymous()
            .WithName("AccountPasskeyRequestOptions")
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest);
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    [RequireAntiforgeryToken]
    private static async ValueTask<IResult> HandleLogoutAsync(
        HttpContext context,
        SignInManager<ApplicationUser> signInManager)
    {
        if (context.Features.Get<IAntiforgeryValidationFeature>() is not { IsValid: true })
        {
            return TypedResults.BadRequest();
        }

        await signInManager.SignOutAsync();

        return TypedResults.LocalRedirect("~/Account/Login");
    }

    // パスキー登録用オプション(WebAuthn creation options)を発行する
    [RequireAntiforgeryToken]
    private static async ValueTask<IResult> HandlePasskeyCreationOptionsAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        if (context.Features.Get<IAntiforgeryValidationFeature>() is not { IsValid: true })
        {
            return TypedResults.BadRequest();
        }

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
    [RequireAntiforgeryToken]
    private static async ValueTask<IResult> HandlePasskeyRequestOptionsAsync(
        HttpContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        [FromQuery] string? username)
    {
        if (context.Features.Get<IAntiforgeryValidationFeature>() is not { IsValid: true })
        {
            return TypedResults.BadRequest();
        }

        var user = String.IsNullOrEmpty(username) ? null : await userManager.FindByNameAsync(username);
        var optionsJson = await signInManager.MakePasskeyRequestOptionsAsync(user);

        return TypedResults.Content(optionsJson, contentType: "application/json");
    }
}
