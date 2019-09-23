CREATE TABLE [dbo].[MemberVerifyRecord] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ApproveId]   INT            NULL,
    [ProfileId]   BIGINT         NOT NULL,
    [ProfileType] TINYINT        NOT NULL,
    [Status]      TINYINT        CONSTRAINT [DF_MemberVerifyRecord_Status] DEFAULT ((0)) NOT NULL,
    [SubmitTime]  DATETIME       NOT NULL,
    [ApproveTime] DATETIME       NULL,
    [Remark]      NVARCHAR (200) NULL,
    CONSTRAINT [PK_MemberVerifyRecord] PRIMARY KEY CLUSTERED ([Id] ASC)
);

