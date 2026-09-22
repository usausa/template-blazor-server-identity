namespace Template.BlazorServer;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Template.BlazorServer.Host.Application;
using Template.BlazorServer.Host.Infrastructure.Identity;

public sealed class IdentityTests : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory factory;

    public IdentityTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task PrincipalContainsDisplayNameClaim()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var principalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        var user = new ApplicationUser { UserName = "claim-user", DisplayName = "クレーム 太郎" };
        Assert.True((await userManager.CreateAsync(user, "claim-user")).Succeeded);
        await userManager.AddToRoleAsync(user, Roles.Administrator);

        // Act
        var principal = await principalFactory.CreateAsync(user);

        // Assert
        Assert.Equal("claim-user", principal.Identity?.Name);
        Assert.Equal("クレーム 太郎", principal.FindFirst(ApplicationClaimTypes.DisplayName)?.Value);
        Assert.True(principal.IsInRole(Roles.Administrator));
    }

    [Fact]
    public async Task PrincipalWithoutDisplayNameHasNoClaim()
    {
        // Arrange
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var principalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        var user = new ApplicationUser { UserName = "plain-user" };
        Assert.True((await userManager.CreateAsync(user, "plain-user")).Succeeded);

        // Act
        var principal = await principalFactory.CreateAsync(user);

        // Assert
        Assert.Null(principal.FindFirst(ApplicationClaimTypes.DisplayName));
    }
}
