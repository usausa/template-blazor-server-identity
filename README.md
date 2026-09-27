# Template project for Blazor Server (ASP.NET Core Identity)

template-blazor-server をベースに、自前Cookie認証をASP.NET Core Identity(パスキー対応)へ置き換えた変種。

## ベースとの差分

- 認証: ASP.NET Core Identity(Cookie)。ストアはIdentity専用のEF Core+SQLite(`identity.db`)。業務データは引き続きSmart.Data.Accessor(EFと併存)
- パスキー(WebAuthn)対応: ログイン画面のパスキーログイン、`/Account/Manage/Passkeys` での登録・名前変更・削除(.NET 10のIdentityパスキー機能+スキーマVersion3)
- アカウントUI: `/Account/Login`(パスワード+パスキー)/ `/Account/Register` / `/Account/Manage/Passkeys`(.NET 10 Blazor Identityテンプレートを範にMudBlazorへ移植、静的SSR)
- ロールはIdentityロールへ移行(Administrator。削除操作の限定は維持)。初期ユーザー(admin/admin、Administrator)はIdentityシードで作成
- Account系(テーブル/Accessor/Service/PBKDF2)と `/auth/login`・`/auth/logout` は撤去(ログアウトは `/Account/Logout`)
- 2FA(TOTP)はパスキー優先の方針により未実装(必要ならIdentity標準の2FAを追加可能)

## メモ

- パスワードポリシーは開発用初期パスワード(admin)を許容する緩和設定になっている。運用時は `ConfigureAuthentication` の `options.Password` を強化すること
- IdentityストアはEnsureCreatedで作成している。スキーマ移行が必要になったらEF Coreマイグレーション運用へ切替える
- パスキーはlocalhost以外ではHTTPSが必要(WebAuthnのRP IDにIPアドレスは使用不可)
- E2Eテスト(PasskeyTest)はPlaywrightのCDP WebAuthn仮想認証器でパスキーの登録〜ログインを検証している

## E2Eテスト

E2E(`tests/Template.BlazorServer.E2ETests`)はPlaywrightで動かす。日常の実行には含めず、必要なときとリリース前に流す。コマンドはリポジトリ直下のPowerShellで実行する。

```powershell
# ブラウザの入手(初回。ビルドで出力されるスクリプトを使う)
dotnet build tests/Template.BlazorServer.E2ETests
pwsh tests/Template.BlazorServer.E2ETests/bin/Debug/net10.0/playwright.ps1 install

# 実行(既定は Chromium・画面なし)
dotnet run --project tests/Template.BlazorServer.E2ETests

# 画面を出して実行
$env:HEADED = "1"; dotnet run --project tests/Template.BlazorServer.E2ETests; Remove-Item Env:HEADED

# ブラウザを切り替える(chromium / firefox / webkit)
$env:BROWSER = "firefox"; dotnet run --project tests/Template.BlazorServer.E2ETests; Remove-Item Env:BROWSER

# Playwright Inspector で 1 手ずつ実行
$env:PWDEBUG = "1"; dotnet run --project tests/Template.BlazorServer.E2ETests; Remove-Item Env:PWDEBUG
```

- 失敗したテストだけ、操作ごとの画面・DOM・通信・コンソールを記録したトレースを `tests/Template.BlazorServer.E2ETests/bin/Debug/net10.0/playwright-traces/<テストの表示名>.zip` に残す。`pwsh tests/Template.BlazorServer.E2ETests/bin/Debug/net10.0/playwright.ps1 show-trace <zip>` か https://trace.playwright.dev で開く
- パスキーのテスト(`PasskeyTests`)はCDPの仮想認証器を使うので、Chromiumでしか通らない(firefox / webkitでは失敗する)
