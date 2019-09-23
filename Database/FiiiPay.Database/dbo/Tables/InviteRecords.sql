CREATE TABLE [dbo].[InviteRecords] (
    [Id]               INT              IDENTITY (1, 1) NOT NULL,
    [AccountId]        UNIQUEIDENTIFIER NOT NULL,
    [InviterCode]      VARCHAR (8)      NULL,
    [Type]             TINYINT          NOT NULL,
    [InviterAccountId] UNIQUEIDENTIFIER NOT NULL,
    [Timestamp]        DATETIME         NOT NULL,
    [SN] VARCHAR(50) NULL, 
    CONSTRAINT [PK_InviteRecords] PRIMARY KEY CLUSTERED ([Id] ASC)
);

