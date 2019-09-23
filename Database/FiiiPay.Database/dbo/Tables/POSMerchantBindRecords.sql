CREATE TABLE [dbo].[POSMerchantBindRecords] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [POSId]            BIGINT           NOT NULL,
    [SN]               VARCHAR (50)     NOT NULL,
    [MerchantId]       UNIQUEIDENTIFIER NOT NULL,
    [MerchantUsername] VARCHAR (50)     NOT NULL,
    [BindTime]         DATETIME         NOT NULL,
    [UnbindTime]       DATETIME         NULL,
    [BindStatus]       TINYINT          CONSTRAINT [DF_POSMerchantBindRecords_BindStatus] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_POSMerchantBindRecords] PRIMARY KEY CLUSTERED ([Id] ASC)
);


