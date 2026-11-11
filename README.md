# Template project for Blazor Server (ASP.NET Core Identity)

English | [日本語](README.ja.md)

[Template](https://usausa.github.io/template/) project for Blazor Server (ASP.NET Core Identity).

# ✅ Implement

| Category | Feature |
| --- | --- |
| Auth | ASP.NET Core Identity (Cookie) / Identity store (EF Core, SQLite, migrations) / Identity roles / Custom claims (display name) / Security stamp revalidation |
| Passkey | Passkey login (WebAuthn) / Passkey management (register, rename, delete) |
| Account UI | Login / Registration / Passkeys / Access denied (MudBlazor, static SSR) |
| Test | Passkey E2E (Playwright virtual authenticator) / Claims integration tests |

# 🔀 Differences from the base

- Authentication: ASP.NET Core Identity (Cookie). The store is EF Core + SQLite dedicated to Identity (`identity.db`). Business data still uses Smart.Data.Accessor (coexisting with EF)
- Passkey (WebAuthn) support: passkey login on the login screen, and registration, renaming, and deletion at `/Account/Manage/Passkeys` (the Identity passkey feature of .NET 10 + schema Version3)
- Account UI: `/Account/Login` (password + passkey) / `/Account/Register` / `/Account/Manage/Passkeys` (ported to MudBlazor based on the .NET 10 Blazor Identity template, static SSR)
- Roles moved to Identity roles (Administrator. Delete operations are still limited to it). The initial user (admin/admin, Administrator) is created by the Identity seed
- The Account parts (tables/Accessor/Service/PBKDF2) and `/auth/login` and `/auth/logout` were removed (logout is `/Account/Logout`)
- 2FA (TOTP) is not implemented because of the passkey-first policy (the standard 2FA of Identity can be added if needed)

# 📝 Notes

- The password policy is relaxed to allow the initial password for development (admin). In operation, strengthen `options.Password` in `ConfigureAuthentication`
- The Identity store is created with EF Core migrations (`Application/Identity/Migrations`), which are applied at startup with `Database.MigrateAsync()`. When you change the schema, add a migration
- Passkeys require HTTPS except on localhost (an IP address cannot be used as the RP ID of WebAuthn)
- The E2E test (PasskeyTests) verifies passkey registration through login with the CDP WebAuthn virtual authenticator of Playwright
- The passkey tests (`PasskeyTests`) use the CDP virtual authenticator, so they pass only on Chromium (they fail on firefox / webkit)

# 🔗 Related Projects

| Project | Description |
| --- | --- |
| [template-blazor-server](https://github.com/usausa/template-blazor-server) | Blazor Server application template |
