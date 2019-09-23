CREATE TABLE [dbo].[ActionLog] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [CreateTime] DATETIME       NOT NULL,
    [AccountId]  BIGINT         NOT NULL,
    [Username]   VARCHAR (50)   NULL,
    [IPAddress]  VARCHAR (50)   NULL,
    [ModuleCode] VARCHAR (100)  NULL,
    [LogLevel]   INT            CONSTRAINT [DF_ActionLog_LogLevel] DEFAULT ((0)) NOT NULL,
    [LogContent] NVARCHAR (500) NULL,
    CONSTRAINT [PK_ActionLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

