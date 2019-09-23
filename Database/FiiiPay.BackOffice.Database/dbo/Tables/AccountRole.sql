CREATE TABLE [dbo].[AccountRole] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CreateTime]  DATETIME       NOT NULL,
    [CreateBy]    INT            NOT NULL,
    [ModifyTime]  DATETIME       NULL,
    [ModifyBy]    INT            NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (100) NULL,
    CONSTRAINT [PK_AccountRole] PRIMARY KEY CLUSTERED ([Id] ASC)
);

