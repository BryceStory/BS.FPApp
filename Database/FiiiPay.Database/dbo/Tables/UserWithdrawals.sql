CREATE TABLE [dbo].[UserWithdrawals] (
    [Id]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL,
    [UserWalletId]  BIGINT           NOT NULL,
    [CryptoCode]    VARCHAR (10)     NULL,
    [Address]       VARCHAR (128)    NOT NULL,
    [Amount]        DECIMAL (18, 8)  NOT NULL,
    [Status]        TINYINT          NOT NULL,
    [Timestamp]     DATETIME         NOT NULL,
    [Remark]        NVARCHAR (100)   NULL,
    [OrderNo]       VARCHAR (50)     NOT NULL,
    [TransactionId] VARCHAR (255)    NULL,
    [SelfPlatform]  BIT              CONSTRAINT [DF_UserWithdrawals_SelfPlatform] DEFAULT ((0)) NOT NULL,
    [RequestId]     BIGINT           NULL,
    [Tag]           VARCHAR (100)    NULL,
    [CryptoId]      INT              NULL,
    CONSTRAINT [PK_UserWithdrawals] PRIMARY KEY CLUSTERED ([Id] ASC)
);



