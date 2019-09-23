CREATE TABLE [dbo].[UserTransfers] (
    [Id]                BIGINT           IDENTITY (1, 1) NOT NULL,    
    [OrderNo]           VARCHAR (50)     NOT NULL,
    [FromUserAccountId] UNIQUEIDENTIFIER NOT NULL,
    [FromUserWalletId]  BIGINT           NOT NULL,
    [CoinId]            INT              NOT NULL,
    [CoinCode]          VARCHAR (10)     NULL,
    [ToUserAccountId]   UNIQUEIDENTIFIER NOT NULL,
    [ToUserWalletId]    BIGINT           NOT NULL,
    [Amount]            DECIMAL (18, 8)  NOT NULL,
    [Status]            TINYINT          NOT NULL,
	[Timestamp]         DATETIME         NOT NULL,
    [Remark]            NVARCHAR (100)   NULL,
    CONSTRAINT [PK_UserTransfers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

