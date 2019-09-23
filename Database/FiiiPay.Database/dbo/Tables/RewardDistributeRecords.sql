CREATE TABLE [dbo].[RewardDistributeRecords] (
    [Id]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [Timestamp]         DATETIME         NOT NULL,
    [OriginalReward]    BIGINT           NOT NULL,
    [Percentage]        DECIMAL (4, 2)   NOT NULL,
    [ProfitId]          BIGINT           NOT NULL,
    [UserAccountId]     UNIQUEIDENTIFIER NULL,
    [MerchantAccountId] UNIQUEIDENTIFIER NULL,
    [SN]                VARCHAR (50)     NULL,
    [ActualReward]      DECIMAL (18, 8)  CONSTRAINT [DF_RewardDistributeRecords_ActualReward] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_RewardDistributeRecords] PRIMARY KEY CLUSTERED ([Id] ASC)
);



