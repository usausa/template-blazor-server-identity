namespace Template.BlazorServer.Host.Application.Identity;

using Microsoft.AspNetCore.Identity;

// ログイン時に Cookie へ載せるクレームを組み立てる。ロールや業務クレームをカスタムする入口はここ
public sealed class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        // 基底が Id / Name / SecurityStamp / Email / ロールを載せる
        var identity = await base.GenerateClaimsAsync(user);

        if (!String.IsNullOrEmpty(user.DisplayName))
        {
            identity.AddClaim(new Claim(ApplicationClaimTypes.DisplayName, user.DisplayName));
        }

        // [MEMO] 部署や権限フラグなど業務固有のクレームは、ApplicationUser の列や別テーブルから読んでここに足す

        return identity;
    }
}
