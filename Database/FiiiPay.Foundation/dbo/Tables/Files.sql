CREATE TABLE [dbo].[Files] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [AccountId] UNIQUEIDENTIFIER NULL,
    [FileType]  VARCHAR (10)     NOT NULL,
    [FileName]  NVARCHAR (200)   NOT NULL,
    [MimeType]  VARCHAR (100)    NULL,
    [FilePath]  VARCHAR (200)    NOT NULL,
    [Timestamp] DATETIME         NOT NULL,
    [GroupName] VARCHAR(20) NULL, 
    CONSTRAINT [PK__Files__3214EC07D9475137] PRIMARY KEY CLUSTERED ([Id] ASC)
);

