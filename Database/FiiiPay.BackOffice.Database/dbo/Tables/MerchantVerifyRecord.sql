CREATE TABLE [dbo].[MerchantVerifyRecord] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [ApproveId]   INT            NULL,
    [ProfileId]   BIGINT         NOT NULL,
    [Status]      TINYINT        CONSTRAINT [DF_MerchantVerifyRecord_Status] DEFAULT ((0)) NOT NULL,
    [SubmitTime]  DATETIME       NOT NULL,
    [ApproveTime] DATETIME       NULL,
    [Remark]      NVARCHAR (200) NULL,
    CONSTRAINT [PK_MerchantVerifyRecord] PRIMARY KEY CLUSTERED ([Id] ASC)
);

