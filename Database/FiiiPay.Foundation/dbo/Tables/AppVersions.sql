CREATE TABLE [dbo].[AppVersions] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Platform]      TINYINT        NOT NULL,
    [Version]       VARCHAR (50)   NOT NULL,
    [ForceToUpdate] BIT            NOT NULL,
    [Description]   NVARCHAR (255) NULL,
    [Url]           VARCHAR (255)  NULL,
    [App]           TINYINT        NULL,
    [Version1] VARCHAR(50) NOT NULL DEFAULT '', 
    [ForceToUpdate1] BIT NOT NULL DEFAULT 0, 
    [Url1] VARCHAR(255) NOT NULL DEFAULT '', 
    CONSTRAINT [PK_AppVersions] PRIMARY KEY CLUSTERED ([Id] ASC)
);



