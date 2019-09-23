CREATE TABLE [dbo].[Orders] (
    [Id]                      UNIQUEIDENTIFIER NOT NULL,
    [OrderNo]                 VARCHAR (100)    NOT NULL,
    [MerchantAccountId]       UNIQUEIDENTIFIER NOT NULL,
    [UserAccountId]           UNIQUEIDENTIFIER NULL,
    [CryptoId]                INT              NOT NULL,
    [CryptoCode]              VARCHAR (10)     DEFAULT ('BTC') NOT NULL,
    [CryptoAmount]            DECIMAL (18, 8)  NOT NULL,
    [ActualCryptoAmount]      DECIMAL (18, 8)  NOT NULL,
    [FiatAmount]              DECIMAL (18, 4)  NULL,
    [FiatCurrency]            VARCHAR (50)     NOT NULL,
    [Markup]                  DECIMAL (10, 4)  NOT NULL,
    [ActualFiatAmount]        DECIMAL (18, 4)  NULL,
    [Status]                  TINYINT          NOT NULL,
    [Timestamp]               DATETIME         NOT NULL,
    [Remark]                  NVARCHAR (100)   NULL,
    [ExchangeRate]            DECIMAL (10, 2)  NULL,
    [ExpiredTime]             DATETIME         NOT NULL,
    [TransactionFee]          DECIMAL (18, 8)  NULL,
    [MerchantIP]              VARCHAR (18)     NULL,
    [MerchantToken]           VARCHAR (20)     NULL,
    [UserIP]                  VARCHAR (18)     NULL,
    [UserToken]               VARCHAR (20)     NULL,
    [PaymentType]             TINYINT          NOT NULL,
    [PaymentTime]             DATETIME         NULL,
    [UnifiedFiatAmount]       DECIMAL (18, 4)  NOT NULL,
    [UnifiedFiatCurrency]     VARCHAR (50)     NULL,
    [UnifiedExchangeRate]     DECIMAL (10, 2)  NOT NULL,
    [UnifiedActualFiatAmount] DECIMAL (18, 4)  NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'1 - User scan merchant QR code
2 - Merchant scan user QR code
3 - User scan merchant Bluetooth
4 - Merchant scan user Bluetooth
5 - NFC', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Orders', @level2type = N'COLUMN', @level2name = N'PaymentType';

