# HANDOVER (template-blazor-server-identity)

作成日: 2026-08-23。template-blazor-server をベースに、自前Cookie認証をASP.NET Core Identity(パスキー対応)へ置き換えた変種の引継ぎ。

## 1. 状態

- ビルド0警告 / UnitTests 5件・IntegrationTests 3件・E2ETests 3件 全通過
- ReSharper(jb inspectcode 2026.2)対応済み: 指摘0件(フレームワーク注釈と実態が異なる箇所はReSharper disableコメントで抑止)
- 実機確認済み: adminログイン→データCRUD一巡→ログアウト / 新規登録→自動サインイン / 一般ユーザーで削除ボタン非表示(ロール制御)
- パスキーはE2E(PlaywrightのCDP WebAuthn仮想認証器)で登録〜パスキーログインまで自動検証済み
- ソリューション名・プロジェクト名・名前空間はベースのまま(リポジトリ名のみで識別)

## 2. 決定事項

- **2FA(TOTP)は未実装**(【要判断】の推奨どおりパスキー優先。必要になればIdentity標準の2FAを追加)
- アカウントは**ユーザー名ベース**(ベースのID/パスワード方式を踏襲。メールアドレス・確認メールは扱わない。`RequireConfirmedAccount=false`)
- 移植したUIは Login / Register / Manage/Passkeys / RenamePasskey / AccessDenied に限定(外部ログイン・メール系・個人データ系は対象外)
- パスワードポリシーは開発用初期パスワード(admin)を許容する緩和設定(運用時に強化する前提をコードコメントに明記)
- IdentityストアはEnsureCreatedで作成(テンプレート用途。マイグレーション運用への切替口をREADMEに明記)

## 3. 主な変更点

- 撤去: AccountAccessor / AccountService / AccountEntity / IPasswordProvider(PBKDF2)一式 / AuthEndpoints(/auth/login・/auth/logout)/ Login.razor(自前版)/ Database.sqlのAccountテーブル / DefaultPasswordProviderTest
- 追加(Host):
  - `Infrastructure/Identity/`: ApplicationUser / IdentityContext(Identity専用DbContext、接続文字列 `Identity`=identity.db)/ IdentityRedirectManager / IdentityRevalidatingAuthenticationStateProvider / PasskeyInputModel / PasskeyOperation
  - `Components/Account/`: Pages(Login/Register/AccessDenied/Manage/Passkeys/Manage/RenamePasskey)+ Shared(StatusMessage/PasskeySubmit+PasskeySubmit.razor.js)。いずれも静的SSR([ExcludeFromInteractiveRouting])で、.NET 10 Blazor Identityテンプレート由来のコードをMudBlazorの見た目に合わせて移植
  - `Endpoints/AccountEndpoints.cs`: /Account/Logout(POST)、/Account/PasskeyCreationOptions、/Account/PasskeyRequestOptions
- ConfigureAuthentication: `AddIdentityCookies` + `AddIdentityCore<ApplicationUser>`(SchemaVersion=Version3=パスキー対応)+ `AddRoles<IdentityRole>` + EFストア。ApplicationCookieへベースと同等の安全既定(HttpOnly/SameSite=Lax/Secure、APIパスは401/403を返す)を適用。LoginPath=/Account/Login、AccessDeniedPath=/Account/AccessDenied
- シード: Administratorロール+初期ユーザー(Auth:InitialId/InitialPassword)を InitializeApplicationAsync で作成
- MainLayout: ログアウトフォームを /Account/Logout へ変更、パスキー管理へのリンク(鍵アイコン)を追加
- テスト: TestApplicationFactory / E2EApplicationFactory にIdentity用接続文字列(一時ファイル)を追加。LoginTestのセレクタ調整(ボタン名の完全一致)。PasskeyTest 新設

## 4. 知見

1. **PlaywrightのCDP仮想認証器(WebAuthn.addVirtualAuthenticator)でパスキーのE2Eが完全自動化できる**。ctap2/internal/hasResidentKey/hasUserVerification/isUserVerified/automaticPresenceSimulation を有効にする
2. **WebAuthnのRP IDにIPアドレスは使えない**。E2Eサーバは127.0.0.1でlistenするため、テストは `localhost` へ読み替えてアクセスする(実運用でもlocalhost以外はHTTPS必須)
3. 仮想認証器+発見可能な資格情報がある場合、**ログイン画面の条件付きUI(autofill)でパスキーログインが自動実行される**(PasskeySubmit.razor.jsのtryAutofillPasskey)。E2Eではボタンクリックではなく自動ログインの結果を検証する(クリックはページ遷移と競合する)
4. パスキー対応にはIdentityストアの **SchemaVersion=Version3** 指定が必要(既定スキーマにはパスキーテーブルがない)
5. `<passkey-submit>` カスタム要素は blazor.web.js ではなく **コロケートJS(PasskeySubmit.razor.js)** が定義する。App.razorへの `<script type="module">` の追加を忘れると、ボタンが普通のsubmitになり「ブラウザからパスキーが提供されませんでした」系のエラーになる
6. MudBlazorの静的SSRページでは `MudButton` に `name`/`value` を渡せばフォームのsubmitter値として機能する(Passkeys一覧のrename/delete分岐で使用)
7. Blazorの `[SupplyParameterFromForm]` プロパティに初期化子を付けるとBL0008警告になる。`default!` + `OnInitialized`での `??=` が定石

## 5. 残作業・注意

- 実IdP(実ブラウザ+実認証器)でのパスキー動作確認は未実施(仮想認証器での自動検証のみ)。Windows Hello等での手動確認を推奨
- 一般ユーザー登録(/Account/Register)は誰でも可能な構成。閉域用途では登録ページの無効化やロール付与フローの追加を検討
