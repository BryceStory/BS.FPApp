CREATE TABLE [dbo].[MerchantWallets] (
    [Id]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [MerchantAccountId] UNIQUEIDENTIFIER NOT NULL,
    [CryptoId]          INT              NOT NULL,
    [CryptoCode]        VARCHAR (10)     NULL,
    [Status]            TINYINT          CONSTRAINT [DF_MerchantWallets_Status] DEFAULT ((1)) NOT NULL,
    [SupportReceipt]    BIT              CONSTRAINT [DF_MerchantWallets_AllowReceipt] DEFAULT ((0)) NOT NULL,
    [Balance]           DECIMAL (18, 8)  NOT NULL,
    [Address]           VARCHAR (100)    NULL,
    [Sequence]          INT              CONSTRAINT [DF_MerchantWallets_Sequence] DEFAULT ((0)) NOT NULL,
    [FrozenBalance]     DECIMAL (18, 8)  NULL,
    [IsDefault]         BIT              NULL,
    [Tag]               VARCHAR (100)    NULL,
    CONSTRAINT [PK_MerchantWallets] PRIMARY KEY CLUSTERED ([Id] ASC)
);



