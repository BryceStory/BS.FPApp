CREATE TABLE [dbo].[Salesperson] (
    [Id]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreateTime] DATETIME       NOT NULL,
    [CreateBy]   INT            NOT NULL,
    [ModifyTime] DATETIME       NULL,
    [ModifyBy]   INT            NULL,
    [SaleCode]   VARCHAR (20)   NOT NULL,
    [SaleName]   NVARCHAR (100) NULL,
    [Gender]     TINYINT        CONSTRAINT [DF_Salesperson_Gender] DEFAULT ((0)) NOT NULL,
    [Position]   NVARCHAR (50)  NULL,
    [Mobile]     NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Salesperson] PRIMARY KEY CLUSTERED ([Id] ASC)
);

