CREATE TABLE [dbo].[BillerOrders]
(
	[Id] UNIQUEIDENTIFIER NOT NULL ,
	[AccountId] UNIQUEIDENTIFIER NOT NULL,
    [OrderNo] VARCHAR(100) NOT NULL, 
    [FiatAmount] DECIMAL(18, 4) NOT NULL, 
    [CryptoAmount] DECIMAL(18, 8) NOT NULL, 
    [BillerCode] VARCHAR(50) NOT NULL, 
    [ReferenceNumber] VARCHAR(50) NOT NULL, 
    [CryptoId] INT NOT NULL, 
    [CryptoCode] VARCHAR(10) NOT NULL,
    [ExchangeRate] DECIMAL(10, 2) NOT NULL, 
    [Timestamp] DATETIME NOT NULL, 
    [Discount] DECIMAL(10, 2) NOT NULL, 
    [Status] TINYINT NOT NULL, 
    [Tag] NVARCHAR(50) NULL, 
    [FiatCurrency] VARCHAR(10) NOT NULL, 
	[Remark] NVARCHAR(150) NULL,
	[CountryId] INT NOT NULL,
    [PayTime] DATETIME NOT NULL, 
	[FinishTime] DATETIME NULL, 
    CONSTRAINT [PK_BillerOrder] PRIMARY KEY ([Id])
)
