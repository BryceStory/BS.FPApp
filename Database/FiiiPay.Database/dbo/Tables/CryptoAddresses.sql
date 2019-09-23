CREATE TABLE [dbo].[CryptoAddresses] (
    [Id]          BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountId]   UNIQUEIDENTIFIER NOT NULL,
    [AccountType] TINYINT          NOT NULL,
    [CryptoId]    INT              NOT NULL,
    [Alias]       NVARCHAR (50)    NOT NULL,
    [Address]     VARCHAR (300)    NOT NULL,
    [Tag]         VARCHAR (100)    NULL,
    CONSTRAINT [PK_Account_WithdrawalsAddress] PRIMARY KEY CLUSTERED ([Id] ASC)
);



