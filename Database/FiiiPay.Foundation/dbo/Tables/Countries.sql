CREATE TABLE [dbo].[Countries] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (20) NOT NULL,
    [Name_CN]         NVARCHAR (20) NOT NULL,
    [PhoneCode]       VARCHAR (10)  NOT NULL,
    [CustomerService] VARCHAR (300) NOT NULL,
    [PinYin]          VARCHAR (50)  NOT NULL,
    [FiatCurrency]    VARCHAR (10)  NOT NULL,
    [Code]            VARCHAR (10)  NOT NULL,
    [IsHot]           BIT           CONSTRAINT [DF_Countries_IsHot] DEFAULT ((0)) NOT NULL,
    [Status]          BIT           CONSTRAINT [DF_Countries_Status] DEFAULT ((1)) NOT NULL,
    [IsSupportStore] BIT NOT NULL DEFAULT ((1)), 
    CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED ([Id] ASC)
);

