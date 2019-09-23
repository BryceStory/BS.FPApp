CREATE TABLE [dbo].[Currencies] (
    [ID]           SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Name_CN]      NVARCHAR (50) NOT NULL,
    [Name]         VARCHAR (100) NOT NULL,
    [Code]         VARCHAR (5)   NOT NULL,
    [IsFixedPrice] BIT           CONSTRAINT [DF_Currencies_IsFixedPrice] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([ID] ASC)
);

