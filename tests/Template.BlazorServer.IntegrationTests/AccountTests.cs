namespace Template.BlazorServer;

using Microsoft.AspNetCore.Mvc.Testing;

public sealed class AccountTests : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory factory;

    public AccountTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Theory]
    [InlineData("cross-site", HttpStatusCode.BadRequest)]
    [InlineData("same-site", HttpStatusCode.BadRequest)]
    [InlineData("same-origin", HttpStatusCode.Redirect)]
    public async Task LoginAllowsOnlySameOrigin(string site, HttpStatusCode expected)
    {
        // Arrange
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        using var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["_handler"] = "login",
            ["Input.Name"] = "admin",
            ["Input.Password"] = "admin"
        });
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/Account/Login", UriKind.Relative));
        request.Content = form;
        request.Headers.Add("Sec-Fetch-Site", site);

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("cross-site", HttpStatusCode.BadRequest)]
    [InlineData("same-site", HttpStatusCode.BadRequest)]
    [InlineData("same-origin", HttpStatusCode.Redirect)]
    public async Task LogoutAllowsOnlySameOrigin(string site, HttpStatusCode expected)
    {
        // Arrange
        var client = await factory.CreateAuthenticatedClientAsync(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/Account/Logout", UriKind.Relative));
        request.Headers.Add("Sec-Fetch-Site", site);

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("cross-site", HttpStatusCode.BadRequest)]
    [InlineData("same-origin", HttpStatusCode.OK)]
    public async Task PasskeyCreationOptionsAllowsOnlySameOrigin(string site, HttpStatusCode expected)
    {
        // Arrange
        var client = await factory.CreateAuthenticatedClientAsync();
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/Account/PasskeyCreationOptions", UriKind.Relative));
        request.Headers.Add("Sec-Fetch-Site", site);

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("cross-site", HttpStatusCode.BadRequest)]
    [InlineData("same-origin", HttpStatusCode.OK)]
    public async Task PasskeyRequestOptionsAllowsOnlySameOrigin(string site, HttpStatusCode expected)
    {
        // Arrange
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("/Account/PasskeyRequestOptions", UriKind.Relative));
        request.Headers.Add("Sec-Fetch-Site", site);

        // Act
        var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(expected, response.StatusCode);
    }
}
