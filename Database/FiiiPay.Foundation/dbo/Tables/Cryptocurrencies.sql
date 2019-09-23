CREATE TABLE [dbo].[Cryptocurrencies] (
    [Id]              INT              NOT NULL,
    [Name]            VARCHAR (50)     NOT NULL,
    [Code]            VARCHAR (10)     NOT NULL,
    [Status]          TINYINT          NOT NULL,
    [DecimalPlace]    TINYINT          CONSTRAINT [DF_Cryptocurrencies_DecimalPlace] DEFAULT ((8)) NOT NULL,
    [IconURL]         UNIQUEIDENTIFIER NULL,
    [BGURL]           UNIQUEIDENTIFIER NULL,
    [Withdrawal_Tier] DECIMAL (10, 2)  NULL,
    [Withdrawal_Fee]  DECIMAL (18, 8)  NULL,
    [Sequence]        INT              CONSTRAINT [DF_Cryptocurrencies_Sequence] DEFAULT ((0)) NOT NULL,
    [NeedTag]         BIT              CONSTRAINT [DF_Cryptocurrencies_NeedTag] DEFAULT ((0)) NOT NULL,
    [IsFixedPrice]    BIT              CONSTRAINT [DF_Cryptocurrencies_IsFixedPrice] DEFAULT ((0)) NOT NULL,
    [Enable] BIT NOT NULL DEFAULT ((1)), 
    [IsWhiteLabel] BIT NOT NULL DEFAULT ((0)), 
    CONSTRAINT [PK_BCoin] PRIMARY KEY CLUSTERED ([Id] ASC)
);



