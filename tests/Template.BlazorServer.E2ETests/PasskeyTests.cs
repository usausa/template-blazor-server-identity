namespace Template.BlazorServer;

using System.Text.RegularExpressions;

using Microsoft.Playwright;
using Microsoft.Playwright.Xunit.v3;

// CDPのWebAuthn仮想認証器を使ってパスキーの登録〜ログインを検証する
public sealed class PasskeyTests : PageTest
{
    private async Task EnableVirtualAuthenticatorAsync()
    {
        var session = await Page.Context.NewCDPSessionAsync(Page);
        await session.SendAsync("WebAuthn.enable");
        await session.SendAsync("WebAuthn.addVirtualAuthenticator", new Dictionary<string, object>
        {
            ["options"] = new Dictionary<string, object>
            {
                ["protocol"] = "ctap2",
                ["transport"] = "internal",
                ["hasResidentKey"] = true,
                ["hasUserVerification"] = true,
                ["isUserVerified"] = true,
                ["automaticPresenceSimulation"] = true
            }
        });
    }

    [Fact]
    public async Task RegisterPasskeyAndLogin()
    {
        // Arrange
        await using var factory = new E2EApplicationFactory();
        factory.UseKestrel(0);
        factory.StartServer();

        // WebAuthnのRP IDにIPアドレスは使えないため、127.0.0.1ではなくlocalhostでアクセスする
        var address = factory.ServerAddress.Replace("127.0.0.1", "localhost", StringComparison.Ordinal);

        await EnableVirtualAuthenticatorAsync();

        // パスワードでログイン
        await Page.GotoAsync(address + "/Account/Login");
        await Page.FillAsync("#name", "admin");
        await Page.FillAsync("#password", "admin");
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "ログイン", Exact = true }).ClickAsync();
        await Expect(Page.GetByText("Template.BlazorServer")).ToBeVisibleAsync();

        // パスキーを登録(仮想認証器が自動応答)して名前を付ける
        await Page.GotoAsync(address + "/Account/Manage/Passkeys");
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "パスキーを追加" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*Account/Manage/RenamePasskey.*"));
        await Page.FillAsync("#passkey-name", "E2E Passkey");
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "保存" }).ClickAsync();
        await Expect(Page.GetByText("E2E Passkey")).ToBeVisibleAsync();

        // ログアウトしてログイン画面へ戻る
        await Page.Locator("form[action='Account/Logout'] button").ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*Account/Login.*"));

        // Assert (仮想認証器では条件付きUI(自動フィル)によりパスキーログインが自動実行される)
        await Expect(Page.GetByText("Template.BlazorServer")).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 30_000 });
    }
}
