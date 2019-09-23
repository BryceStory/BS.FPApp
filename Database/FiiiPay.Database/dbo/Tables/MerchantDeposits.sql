CREATE TABLE [dbo].[MerchantDeposits] (
    [Id]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [MerchantAccountId] UNIQUEIDENTIFIER NOT NULL,
    [MerchantWalletId]  BIGINT           NOT NULL,
    [CryptoCode]        VARCHAR (10)     NULL,
    [FromAddress]       VARCHAR (100)    NULL,
    [ToAddress]         VARCHAR (100)    NULL,
    [Amount]            DECIMAL (18, 8)  NOT NULL,
    [Status]            TINYINT          NOT NULL,
    [Timestamp]         DATETIME         NOT NULL,
    [Remark]            NVARCHAR (100)   NULL,
    [OrderNo]           VARCHAR (50)     NOT NULL,
    [TransactionId]     VARCHAR (255)    NOT NULL,
    [SelfPlatform]      BIT              CONSTRAINT [DF_MerchantDeposits_SelfPlatform] DEFAULT ((0)) NOT NULL,
    [RequestId]         BIGINT           NULL,
    [FromTag]           VARCHAR (100)    NULL,
    [ToTag]             VARCHAR (100)    NULL,
    [FromType]          TINYINT          CONSTRAINT [DF_MerchantDeposits_FromType] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_MerchantDeposits] PRIMARY KEY CLUSTERED ([Id] ASC)
);



