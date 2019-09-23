CREATE TABLE [dbo].[PriceInfo] (
    [ID]             SMALLINT        IDENTITY (1, 1) NOT NULL,
    [CryptoID]       INT             NOT NULL,
    [CurrencyID]     SMALLINT        NOT NULL,
    [Price]          DECIMAL (18, 2) NOT NULL,
    [LastUpdateDate] DATETIME        NOT NULL,
    CONSTRAINT [PK_PriceInfo] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PriceInfo_Cryptocurrencies] FOREIGN KEY ([CryptoID]) REFERENCES [dbo].[Cryptocurrencies] ([Id]),
    CONSTRAINT [FK_PriceInfo_Currencies] FOREIGN KEY ([CurrencyID]) REFERENCES [dbo].[Currencies] ([ID])
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description1', @value = N'Refer to dbo.currencies', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PriceInfo', @level2type = N'COLUMN', @level2name = N'CurrencyID';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Refer to dbo.cryptocurrencies', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'PriceInfo', @level2type = N'COLUMN', @level2name = N'CryptoID';

