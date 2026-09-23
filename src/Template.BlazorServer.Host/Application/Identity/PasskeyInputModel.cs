namespace Template.BlazorServer.Host.Application.Identity;

// パスキー操作の結果をフォームで受け取るモデル(.NET 10 Blazor Identityテンプレート由来)
public sealed class PasskeyInputModel
{
    public string? CredentialJson { get; set; }

    public string? Error { get; set; }
}
