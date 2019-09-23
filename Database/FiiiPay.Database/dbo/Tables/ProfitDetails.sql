CREATE TABLE [dbo].[ProfitDetails] (
    [Id]           INT              IDENTITY (1, 1) NOT NULL,
    [InvitationId] INT              NULL,
    [CryptoId]     INT              NOT NULL,
    [CryptoCode]   VARCHAR (10)     NULL,
    [CryptoAmount] DECIMAL (18, 8)  NOT NULL,
    [AccountId]    UNIQUEIDENTIFIER NOT NULL,
    [Type]         INT              NULL,
    [Status]       TINYINT          CONSTRAINT [DF_ProfitDetails_Status] DEFAULT ((1)) NOT NULL,
    [OrderNo]      VARCHAR (100)    NULL,
    [Timestamp]    DATETIME         NOT NULL,
    CONSTRAINT [PK_ProfitDetails] PRIMARY KEY CLUSTERED ([Id] ASC)
);



