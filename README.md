# Template project for Blazor Server (ASP.NET Core Identity)

English | [日本語](README.ja.md)

A variant based on template-blazor-server that replaces the custom cookie authentication with ASP.NET Core Identity (with passkey support).

## Differences from the base

- Authentication: ASP.NET Core Identity (Cookie). The store is EF Core + SQLite dedicated to Identity (`identity.db`). Business data still uses Smart.Data.Accessor (coexisting with EF)
- Passkey (WebAuthn) support: passkey login on the login screen, and registration, renaming, and deletion at `/Account/Manage/Passkeys` (the Identity passkey feature of .NET 10 + schema Version3)
- Account UI: `/Account/Login` (password + passkey) / `/Account/Register` / `/Account/Manage/Passkeys` (ported to MudBlazor based on the .NET 10 Blazor Identity template, static SSR)
- Roles moved to Identity roles (Administrator. Delete operations are still limited to it). The initial user (admin/admin, Administrator) is created by the Identity seed
- The Account parts (tables/Accessor/Service/PBKDF2) and `/auth/login` and `/auth/logout` were removed (logout is `/Account/Logout`)
- 2FA (TOTP) is not implemented because of the passkey-first policy (the standard 2FA of Identity can be added if needed)

## Notes

- The password policy is relaxed to allow the initial password for development (admin). In operation, strengthen `options.Password` in `ConfigureAuthentication`
- The Identity store is created with EnsureCreated. When schema migration becomes necessary, switch to EF Core migrations
- Passkeys require HTTPS except on localhost (an IP address cannot be used as the RP ID of WebAuthn)
- The E2E test (PasskeyTest) verifies passkey registration through login with the CDP WebAuthn virtual authenticator of Playwright

## E2E tests

The E2E tests (`tests/Template.BlazorServer.E2ETests`) run with Playwright. They are not part of the daily runs, and are run when needed and before releases. Run the commands in PowerShell at the repository root.

```powershell
# Get the browsers (first time. Uses the script output by the build)
dotnet build tests/Template.BlazorServer.E2ETests
pwsh tests/Template.BlazorServer.E2ETests/bin/Debug/net10.0/playwright.ps1 install

# Run (Chromium and headless by default)
dotnet run --project tests/Template.BlazorServer.E2ETests

# Run with the browser shown
$env:HEADED = "1"; dotnet run --project tests/Template.BlazorServer.E2ETests; Remove-Item Env:HEADED

# Switch the browser (chromium / firefox / webkit)
$env:BROWSER = "firefox"; dotnet run --project tests/Template.BlazorServer.E2ETests; Remove-Item Env:BROWSER

# Run step by step with Playwright Inspector
$env:PWDEBUG = "1"; dotnet run --project tests/Template.BlazorServer.E2ETests; Remove-Item Env:PWDEBUG
```

- Only for failed tests, a trace that records the screen, DOM, network, and console of each action is saved to `tests/Template.BlazorServer.E2ETests/bin/Debug/net10.0/playwright-traces/<test display name>.zip`. Open it with `pwsh tests/Template.BlazorServer.E2ETests/bin/Debug/net10.0/playwright.ps1 show-trace <zip>` or https://trace.playwright.dev
- The passkey tests (`PasskeyTests`) use the CDP virtual authenticator, so they pass only on Chromium (they fail on firefox / webkit)
