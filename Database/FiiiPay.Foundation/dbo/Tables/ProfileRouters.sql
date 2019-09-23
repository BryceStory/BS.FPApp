CREATE TABLE [dbo].[ProfileRouters] (
    [Country]       INT           NOT NULL,
    [ServerAddress] VARCHAR (128) NOT NULL,
    [ClientKey]     VARCHAR (100) NOT NULL,
    [SecretKey]     VARCHAR (256) NOT NULL,
    CONSTRAINT [PK_KYCRouter] PRIMARY KEY CLUSTERED ([Country] ASC)
);

