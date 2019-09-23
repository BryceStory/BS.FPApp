CREATE TABLE [dbo].[UserExTransferOrders] (
    [Id]         BIGINT           IDENTITY (1, 1) NOT NULL,    
	[AccountId]  UNIQUEIDENTIFIER NOT NULL,
    [OrderNo]    VARCHAR (50)     NOT NULL,
    [OrderType]  TINYINT          NOT NULL,    
    [WalletId]   BIGINT           NOT NULL,
    [CryptoId]   INT              NOT NULL,
    [CryptoCode] VARCHAR (10)     NULL,
    [Amount]     DECIMAL (18, 8)  NOT NULL,
    [Status]     TINYINT          NOT NULL,
	[Timestamp]  DATETIME         NOT NULL,
    [Remark]     NVARCHAR (100)   NULL,
    [ExId]       VARCHAR (50)     NULL,
    CONSTRAINT [PK_UserExTransferOrders] PRIMARY KEY CLUSTERED ([Id] ASC)
);

