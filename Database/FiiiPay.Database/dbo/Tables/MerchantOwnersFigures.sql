CREATE TABLE [dbo].[MerchantOwnersFigures]
(
    [MerchantInformationId] UNIQUEIDENTIFIER NOT NULL, 
    [FileId] UNIQUEIDENTIFIER NOT NULL, 
    [sort] INT NOT NULL, 
    [ThumbnailId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [PK_MerchantOwnersFigures] PRIMARY KEY ([MerchantInformationId], [FileId])
)
