CREATE TABLE [dbo].[Refunds] (
    [Id]        BIGINT           IDENTITY (1, 1) NOT NULL,
    [OrderId]   UNIQUEIDENTIFIER NOT NULL,
    [Timestamp] DATETIME         NOT NULL,
    [Status]    TINYINT          NOT NULL,
    [Remark]    NVARCHAR (100)   NULL,
    CONSTRAINT [PK_Chargebacks] PRIMARY KEY CLUSTERED ([Id] ASC)
);



