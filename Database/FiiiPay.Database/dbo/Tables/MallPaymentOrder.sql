CREATE TABLE [dbo].[MallPaymentOrder] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [OrderId]         VARCHAR (50)     NOT NULL,
    [TradeNo]         VARCHAR (50)     NOT NULL,
    [UserAccountId]   UNIQUEIDENTIFIER NOT NULL,
    [CryptoAmount]    DECIMAL (18, 8)  NOT NULL,
    [Status]          TINYINT          NOT NULL,
    [RefundTradeNo]   VARCHAR (50)     NULL,
    [ExpiredTime]     DATETIME         NOT NULL,
    [Timestamp]       DATETIME         NOT NULL,
    [HasNotification] BIT              CONSTRAINT [DF_MallPaymentOrder_HasNotification] DEFAULT ((0)) NULL,
    [Remark]          NVARCHAR (50)    NULL,
    CONSTRAINT [PK_MallPaymentOrder] PRIMARY KEY CLUSTERED ([Id] ASC)
);

