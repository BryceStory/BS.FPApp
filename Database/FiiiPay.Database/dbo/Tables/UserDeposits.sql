CREATE TABLE [dbo].[UserDeposits] (
    [Id]            BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserAccountId] UNIQUEIDENTIFIER NOT NULL,
    [UserWalletId]  BIGINT           NOT NULL,
    [CryptoCode]    VARCHAR (10)     NULL,
    [FromAddress]   VARCHAR (100)    NULL,
    [ToAddress]     VARCHAR (100)    NULL,
    [Amount]        DECIMAL (18, 8)  NOT NULL,
    [Status]        TINYINT          NOT NULL,
    [Timestamp]     DATETIME         NOT NULL,
    [Remark]        NVARCHAR (100)   NULL,
    [OrderNo]       VARCHAR (50)     NOT NULL,
    [TransactionId] VARCHAR (255)    NOT NULL,
    [SelfPlatform]  BIT              CONSTRAINT [DF_UserDeposits_SelfPlatform] DEFAULT ((0)) NOT NULL,
    [RequestId]     BIGINT           NULL,
    [FromTag]       VARCHAR (100)    NULL,
    [ToTag]         VARCHAR (100)    NULL,
    [FromType]      TINYINT          CONSTRAINT [DF_UserDeposits_FromType] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserDeposits] PRIMARY KEY CLUSTERED ([Id] ASC)
);



