CREATE TABLE [dbo].[MasterSettings] (
    [Id]    INT          IDENTITY (1, 1) NOT NULL,
    [Group] VARCHAR (50) NOT NULL,
    [Name]  VARCHAR (50) NOT NULL,
    [Type]  VARCHAR (50) NOT NULL,
    [Value] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_MasterSettings] PRIMARY KEY CLUSTERED ([Id] ASC)
);

