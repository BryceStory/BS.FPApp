CREATE TABLE [dbo].[Setting] (
    [Id]               INT IDENTITY (1, 1) NOT NULL,
    [SalespersonIndex] INT CONSTRAINT [DF_Setting_SalespersonIndex] DEFAULT ((0)) NOT NULL,
    [AgentIndex]       INT CONSTRAINT [DF_Setting_AgentIndex] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED ([Id] ASC)
);

