CREATE TABLE [dbo].[GatewayRefundOrders] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [OrderId]       UNIQUEIDENTIFIER NOT NULL,
    [RefundTradeNo] VARCHAR (50)     NOT NULL,
    [Timestamp]     DATETIME         NOT NULL,
    [Status]        TINYINT          NOT NULL,
    [Remark]        NVARCHAR (100)   NULL,
    CONSTRAINT [PK_GatewayRefundOrders] PRIMARY KEY CLUSTERED ([Id] ASC)
);



