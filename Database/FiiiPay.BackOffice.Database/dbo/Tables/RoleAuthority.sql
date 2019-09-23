CREATE TABLE [dbo].[RoleAuthority] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [CreateTime]   DATETIME NOT NULL,
    [CreateBy]     INT      NOT NULL,
    [ModifyTime]   DATETIME NULL,
    [ModifyBy]     INT      NULL,
    [RoleId]       INT      NOT NULL,
    [ModuleId]     INT      NOT NULL,
    [PermissionId] INT      NOT NULL,
    [Value]        INT      CONSTRAINT [DF_RoleAuthority_Value] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_RoleAuthority] PRIMARY KEY CLUSTERED ([Id] ASC)
);

