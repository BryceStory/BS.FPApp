CREATE TABLE [dbo].[MerchantWalletStatements] (
    [Id]        BIGINT          IDENTITY (1, 1) NOT NULL,
    [WalletId]  BIGINT          NOT NULL,
    [Action]    VARCHAR (10)    NOT NULL,
    [Amount]    DECIMAL (18, 8) NOT NULL,
    [Balance]   DECIMAL (18, 8) NOT NULL,
    [Timestamp] DATETIME        NOT NULL,
    [Remark]    NVARCHAR (100)  NULL,
    CONSTRAINT [PK_Account_MerchantWalletStatements] PRIMARY KEY CLUSTERED ([Id] ASC)
);



