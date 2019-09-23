CREATE TABLE [dbo].[MerchantRecommends]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [MerchantInformationId] UNIQUEIDENTIFIER NOT NULL, 
    [RecommendContent] NVARCHAR(28) NOT NULL, 
    [RecommendPicture] UNIQUEIDENTIFIER NOT NULL, 
    [Sort] INT NOT NULL, 
    [ThumbnailId] UNIQUEIDENTIFIER NOT NULL
)
