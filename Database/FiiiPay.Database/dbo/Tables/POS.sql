CREATE TABLE [dbo].[POS] (
    [Id]              BIGINT       IDENTITY (1, 1) NOT NULL,
    [SN]              VARCHAR (50) NOT NULL,
    [Status]          BIT          NOT NULL,
    [Timestamp]       DATETIME     NOT NULL,
    [IsMiningEnabled] BIT          CONSTRAINT [DF_POS_IsMiningEnabled] DEFAULT ((0)) NOT NULL,
    [IsWhiteLabel]    BIT          CONSTRAINT [DF_POS_IsWhiteLabel] DEFAULT ((0)) NOT NULL,
    [WhiteLabel]      VARCHAR (20) NULL,
    [FirstCrypto]     VARCHAR (10) NULL,
    CONSTRAINT [PK_POS] PRIMARY KEY CLUSTERED ([Id] ASC)
);





