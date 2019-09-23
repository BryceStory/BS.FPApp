CREATE TABLE [dbo].[MerchantCategorys]
(
    [MerchantInformationId] UNIQUEIDENTIFIER NOT NULL, 
    [Category] INT NOT NULL, 
    CONSTRAINT [PK_MerchantCategorys] PRIMARY KEY ([MerchantInformationId], [Category])
)
