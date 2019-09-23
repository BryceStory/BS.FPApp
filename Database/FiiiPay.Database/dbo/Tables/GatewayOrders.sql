CREATE TABLE [dbo].[GatewayOrders] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [TradeNo]            VARCHAR (50)     NOT NULL,
    [OrderNo]            VARCHAR (100)    NOT NULL,
    [MerchantAccountId]  UNIQUEIDENTIFIER NOT NULL,
    [MerchantName]       NVARCHAR (50)    NOT NULL,
    [UserAccountId]      UNIQUEIDENTIFIER NULL,
    [CryptoId]           INT              NOT NULL,
    [CryptoCode]         VARCHAR (10)     NULL,
    [CryptoAmount]       DECIMAL (18, 8)  NOT NULL,
    [ActualCryptoAmount] DECIMAL (18, 8)  NOT NULL,
    [FiatAmount]         DECIMAL (18, 4)  NULL,
    [FiatCurrency]       VARCHAR (50)     NULL,
    [Markup]             DECIMAL (10, 2)  NULL,
    [ActualFiatAmount]   DECIMAL (18, 4)  NULL,
    [Status]             TINYINT          NOT NULL,
    [ExchangeRate]       DECIMAL (10, 2)  NULL,
    [ExpiredTime]        DATETIME         NOT NULL,
    [PaymentTime]        DATETIME         NULL,
    [TransactionFee]     DECIMAL (18, 8)  NULL,
    [Timestamp]          DATETIME         NOT NULL,
    [Remark]             NVARCHAR (100)   NULL,
    CONSTRAINT [PK_ThirdPartyOrders] PRIMARY KEY CLUSTERED ([Id] ASC)
);



