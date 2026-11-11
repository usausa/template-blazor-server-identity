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

# 📚 ドキュメント

| ドキュメント | 内容 |
| --- | --- |
| [identity.md](docs/identity.md) | ベースとの差分とメモ |
| [e2e.md](docs/e2e.md) | E2E テスト(Playwright) |

# 🔗 関連プロジェクト

| プロジェクト | 説明 |
| --- | --- |
| [template-blazor-server](https://github.com/usausa/template-blazor-server) | Blazor Server アプリのテンプレート |
