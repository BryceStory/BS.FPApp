CREATE TABLE [dbo].[Account] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [CreateTime]    DATETIME      NOT NULL,
    [CreateBy]      INT           NOT NULL,
    [ModifyTime]    DATETIME      NULL,
    [ModifyBy]      INT           NULL,
    [RoleId]        INT           NULL,
    [Username]      VARCHAR (50)  NOT NULL,
    [Password]      VARCHAR (200) NOT NULL,
    [Status]        TINYINT       CONSTRAINT [DF_Account_Status] DEFAULT ((0)) NOT NULL,
    [Cellphone]     VARCHAR (50)  NULL,
    [Email]         VARCHAR (200) NULL,
    [LastLoginTime] DATETIME      NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Account_AccountRole] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AccountRole] ([Id])
);

