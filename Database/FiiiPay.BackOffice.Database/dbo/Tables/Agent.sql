CREATE TABLE [dbo].[Agent] (
    [Id]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreateTime]  DATETIME       NOT NULL,
    [CreateBy]    INT            NOT NULL,
    [ModifyTime]  DATETIME       NULL,
    [ModifyBy]    INT            NULL,
    [AgentCode]   VARCHAR (20)   NOT NULL,
    [SaleId]      BIGINT         NULL,
    [CompanyName] NVARCHAR (200) NULL,
    [CountryId]   INT            NOT NULL,
    [StateId]     INT            NOT NULL,
    [CityId]      INT            NOT NULL,
    [ContactName] NVARCHAR (100) NULL,
    [ContactWay]  NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Agent] PRIMARY KEY CLUSTERED ([Id] ASC)
);

