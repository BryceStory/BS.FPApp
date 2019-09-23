CREATE TABLE [dbo].[VerifyRecords] (
    [Id]         BIGINT           IDENTITY (1, 1) NOT NULL,
    [AccountId]  UNIQUEIDENTIFIER NOT NULL,
    [Username]   NVARCHAR (50)    NULL,
    [Type]       TINYINT          NOT NULL,
    [Body]       NVARCHAR (512)   NULL,
    [CreateTime] DATETIME         NOT NULL,
    CONSTRAINT [PK_VerifyRecords] PRIMARY KEY CLUSTERED ([Id] ASC)
);



