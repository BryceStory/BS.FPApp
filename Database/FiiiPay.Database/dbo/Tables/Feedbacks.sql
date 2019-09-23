CREATE TABLE [dbo].[Feedbacks] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [Type]         VARCHAR (50)     NOT NULL,
    [AccountId]    UNIQUEIDENTIFIER NOT NULL,
    [Context]      NVARCHAR (1000)  NOT NULL,
    [HasProcessor] BIT              NOT NULL,
    [Timestamp]    DATETIME         NOT NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY CLUSTERED ([Id] ASC)
);

