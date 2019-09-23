CREATE TABLE [dbo].[Feedback] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CreateTime]  DATETIME       NOT NULL,
    [ModifyTime]  DATETIME       NULL,
    [ModifyBy]    INT            NULL,
    [Status]      TINYINT        CONSTRAINT [DF_Feedback_Status] DEFAULT ((0)) NOT NULL,
    [SourceType]  TINYINT        NOT NULL,
    [AccountId]   BIGINT         NULL,
    [Username]    VARCHAR (50)   NULL,
    [FromCountry] NVARCHAR (50)  NULL,
    [Message]     NVARCHAR (256) NULL,
    [Result]      NVARCHAR (256) NULL,
    CONSTRAINT [PK_Feedback] PRIMARY KEY CLUSTERED ([Id] ASC)
);

