CREATE TABLE [dbo].[UserTransactions]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL,
    [Type] TINYINT NOT NULL, 
    [Status] TINYINT NOT NULL, 
    [Timestamp] DATETIME NOT NULL, 
    [Amount] DECIMAL(18, 8) NOT NULL, 
    [CryptoCode] VARCHAR(10) NOT NULL, 
    CONSTRAINT [FK_UserTransactions_UserAccounts] FOREIGN KEY ([UserAccountId]) REFERENCES [UserAccounts]([Id])
)
