CREATE TABLE [dbo].[ReadRecords] (
    [Id]        BIGINT           IDENTITY (1, 1) NOT NULL,
    [Type]      TINYINT          NOT NULL,
    [AccountId] UNIQUEIDENTIFIER NOT NULL,
    [TargetId]  VARCHAR (50)     NOT NULL,
    CONSTRAINT [PK_ReadRecord] PRIMARY KEY CLUSTERED ([Id] ASC)
);



