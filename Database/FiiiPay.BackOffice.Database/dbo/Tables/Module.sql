CREATE TABLE [dbo].[Module] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [CreateTime]  DATETIME      NOT NULL,
    [CreateBy]    INT           NOT NULL,
    [ModifyTime]  DATETIME      NULL,
    [ModifyBy]    INT           NULL,
    [ParentId]    INT           NULL,
    [Name]        NVARCHAR (50) NOT NULL,
    [Code]        VARCHAR (50)  NOT NULL,
    [Icon]        VARCHAR (50)  NULL,
    [Sort]        INT           CONSTRAINT [DF_Module_Sort] DEFAULT ((0)) NOT NULL,
    [PathAddress] VARCHAR (100) NULL,
    CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED ([Id] ASC)
);

