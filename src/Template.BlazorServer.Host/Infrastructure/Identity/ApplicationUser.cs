namespace Template.BlazorServer.Host.Infrastructure.Identity;

using Microsoft.AspNetCore.Identity;

// プロフィール項目を追加する場合はここにプロパティを足す(列の追加は Migration を作る)
public sealed class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
