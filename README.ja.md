# Template project for Blazor Server (ASP.NET Core Identity)

[English](README.md) | 日本語

[Template](https://usausa.github.io/template/) project for Blazor Server (ASP.NET Core Identity).

# ✅ 実装

| 分類 | 機能 |
| --- | --- |
| 認証 | ASP.NET Core Identity(Cookie) / Identity ストア(EF Core・SQLite・マイグレーション) / Identity ロール / 独自クレーム(表示名) / セキュリティスタンプ再検証 |
| パスキー | パスキーログイン(WebAuthn) / パスキー管理(登録・名前変更・削除) |
| アカウント画面 | ログイン / 新規登録 / パスキー / アクセス拒否(MudBlazor・静的 SSR) |
| テスト | パスキー E2E(Playwright 仮想認証器) / クレーム結合テスト |

# 🔀 ベースとの差分

- 認証: ASP.NET Core Identity(Cookie)。ストアはIdentity専用のEF Core+SQLite(`identity.db`)。業務データは引き続きSmart.Data.Accessor(EFと併存)
- パスキー(WebAuthn)対応: ログイン画面のパスキーログイン、`/Account/Manage/Passkeys` での登録・名前変更・削除(.NET 10のIdentityパスキー機能+スキーマVersion3)
- アカウントUI: `/Account/Login`(パスワード+パスキー)/ `/Account/Register` / `/Account/Manage/Passkeys`(.NET 10 Blazor Identityテンプレートを範にMudBlazorへ移植、静的SSR)
- ロールはIdentityロールへ移行(Administrator。削除操作の限定は維持)。初期ユーザー(admin/admin、Administrator)はIdentityシードで作成
- Account系(テーブル/Accessor/Service/PBKDF2)と `/auth/login`・`/auth/logout` は撤去(ログアウトは `/Account/Logout`)
- 2FA(TOTP)はパスキー優先の方針により未実装(必要ならIdentity標準の2FAを追加可能)

# 📝 メモ

- パスワードポリシーは開発用初期パスワード(admin)を許容する緩和設定になっている。運用時は `ConfigureAuthentication` の `options.Password` を強化すること
- IdentityストアはEF Coreのマイグレーション(`Application/Identity/Migrations`)で作成し、起動時に `Database.MigrateAsync()` で適用している。スキーマを変えるときはマイグレーションを追加する
- パスキーはlocalhost以外ではHTTPSが必要(WebAuthnのRP IDにIPアドレスは使用不可)
- E2Eテスト(PasskeyTests)はPlaywrightのCDP WebAuthn仮想認証器でパスキーの登録〜ログインを検証している
- パスキーのテスト(`PasskeyTests`)はCDPの仮想認証器を使うので、Chromiumでしか通らない(firefox / webkitでは失敗する)

# 🔗 関連プロジェクト

| プロジェクト | 説明 |
| --- | --- |
| [template-blazor-server](https://github.com/usausa/template-blazor-server) | Blazor Server アプリのテンプレート |
