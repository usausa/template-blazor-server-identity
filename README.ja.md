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

- 認証: ASP.NET Core Identity(Cookie)。ストアは Identity 専用の EF Core+SQLite(`identity.db`)。業務データは引き続き Smart.Data.Accessor(EF と併存)
- パスキー(WebAuthn)対応: ログイン画面のパスキーログイン、`/Account/Manage/Passkeys` での登録・名前変更・削除(.NET 10 の Identity パスキー機能+スキーマ Version3)
- アカウント UI: `/Account/Login`(パスワード+パスキー)/ `/Account/Register` / `/Account/Manage/Passkeys`(.NET 10 Blazor Identity テンプレートを範に MudBlazor へ移植、静的 SSR)
- ロールは Identity ロールへ移行(Administrator。削除操作の限定は維持)。初期ユーザー(admin/admin、Administrator)は Identity シードで作成
- Account 系(テーブル/Accessor/Service/PBKDF2)と `/auth/login`・`/auth/logout` は撤去(ログアウトは `/Account/Logout`)
- 2FA(TOTP)はパスキー優先の方針により未実装(必要なら Identity 標準の 2FA を追加可能)

# 📝 メモ

- パスワードポリシーは開発用初期パスワード(admin)を許容する緩和設定になっている。運用時は `ConfigureAuthentication` の `options.Password` を強化すること
- Identity ストアは EF Core のマイグレーション(`Application/Identity/Migrations`)で作成し、起動時に `Database.MigrateAsync()` で適用している。スキーマを変えるときはマイグレーションを追加する
- パスキーは localhost 以外では HTTPS が必要(WebAuthn の RP ID に IP アドレスは使用不可)
- E2E テスト(PasskeyTests)は Playwright の CDP WebAuthn 仮想認証器でパスキーの登録〜ログインを検証している
- パスキーのテスト(`PasskeyTests`)は CDP の仮想認証器を使うので、Chromium でしか通らない(firefox / webkit では失敗する)

# 🔗 関連プロジェクト

| プロジェクト | 説明 |
| --- | --- |
| [template-blazor-server](https://github.com/usausa/template-blazor-server) | Blazor Server アプリのテンプレート |
