-- SQLite
-- 認証系のテーブルはASP.NET Core Identity(identity.db、EF Coreが管理)へ移行済み
CREATE TABLE IF NOT EXISTS Data (
    Id         INTEGER  NOT NULL,
    Name       TEXT     NOT NULL,
    Value      INTEGER  NOT NULL,
    CreatedAt  TEXT     NOT NULL,
    PRIMARY KEY (Id AUTOINCREMENT),
    UNIQUE (Name)
);
