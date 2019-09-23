CREATE TABLE [dbo].[Cities] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [StateId] INT            NOT NULL,
    [Name]    NVARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED ([Id] ASC)
);

