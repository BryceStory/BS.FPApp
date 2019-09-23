CREATE TABLE [dbo].[Articles] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Type]            VARCHAR (50)   NULL,
    [Title]           NVARCHAR (512) NOT NULL,
    [Body]            NVARCHAR (MAX) NOT NULL,
    [CreateTime]      DATETIME       NOT NULL,
    [UpdateTime]      DATETIME       NULL,
    [ShouldPop]       BIT            NOT NULL,
    [AccountType]     TINYINT        CONSTRAINT [DF_Articles_AccountType] DEFAULT ((1)) NOT NULL,
    [HasPushed]       BIT            NULL,
    [Descdescription] NVARCHAR (100) NULL,
    CONSTRAINT [PK__Articles__3214EC076EEEE9D4] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO