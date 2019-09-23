CREATE TABLE [dbo].[UserWallets] (
    [Id]             BIGINT           IDENTITY (1, 1) NOT NULL,
    [UserAccountId]  UNIQUEIDENTIFIER NOT NULL,
    [CryptoId]       INT              NOT NULL,
    [CryptoCode]     VARCHAR (10)     NULL,
    [Balance]        DECIMAL (18, 8)  NOT NULL,
    [FrozenBalance]  DECIMAL (18, 8)  NOT NULL,
    [Address]        VARCHAR (100)    NULL,
    [ShowInHomePage] BIT              CONSTRAINT [DF_UserWallets_ShowInHomePage] DEFAULT ((1)) NOT NULL,
    [HomePageRank]   INT              CONSTRAINT [DF_UserWallets_HomePageRank] DEFAULT ((0)) NOT NULL,
    [PayRank]        INT              CONSTRAINT [DF_UserWallets_PayRank] DEFAULT ((0)) NOT NULL,
    [Tag]            VARCHAR (100)    NULL,
    CONSTRAINT [PK_Wallets] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CK_UserWallets_Balance] CHECK ([Balance]>=(0) AND [FrozenBalance]>=(0))
);



