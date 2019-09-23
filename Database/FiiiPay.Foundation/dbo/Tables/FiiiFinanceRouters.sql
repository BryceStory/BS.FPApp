CREATE TABLE [dbo].[FiiiFinanceRouters] (
    [CountryId]     INT           NOT NULL,
    [ServerAddress] VARCHAR (128) NOT NULL,
    [ClientKey]     VARCHAR (100) NOT NULL,
    [SecretKey]     VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_FiiiFinanceRouters] PRIMARY KEY CLUSTERED ([CountryId] ASC)
);

