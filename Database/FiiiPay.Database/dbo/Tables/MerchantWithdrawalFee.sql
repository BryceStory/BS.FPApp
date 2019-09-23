CREATE TABLE [dbo].[MerchantWithdrawalFee] (
    [Id]           BIGINT          IDENTITY (1, 1) NOT NULL,
    [WithdrawalId] BIGINT          NOT NULL,
	[CryptoCode]   VARCHAR(10)     NULL, 
    [Amount]       DECIMAL (18, 8) NOT NULL,
    [Fee]          DECIMAL (18, 8) NOT NULL,
    [Timestamp]    DATETIME        NOT NULL,
    [Remark]       VARCHAR (100)   NULL,
    CONSTRAINT [PK_MerchantWithdrawalFee] PRIMARY KEY CLUSTERED ([Id] ASC)
);

