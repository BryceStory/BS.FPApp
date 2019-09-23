CREATE TABLE [dbo].[ModulePermission] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CreateTime]  DATETIME       NOT NULL,
    [CreateBy]    INT            NOT NULL,
    [ModifyTime]  DATETIME       NULL,
    [ModifyBy]    INT            NULL,
    [ModuleId]    INT            NOT NULL,
    [IsDefault]   BIT            CONSTRAINT [DF_ModulePermission_IsDefault] DEFAULT ((0)) NOT NULL,
    [Code]        VARCHAR (50)   NOT NULL,
    [Description] NVARCHAR (50)  NOT NULL,
    [Remark]      NVARCHAR (256) NULL,
    CONSTRAINT [PK_ModulePermission] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ModulePermission_Module] FOREIGN KEY ([ModuleId]) REFERENCES [dbo].[Module] ([Id])
);

